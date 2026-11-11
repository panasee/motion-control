"""Microscope workspace with an optional camera backend and image annotations.

Camera and Onway connections are explicit and independent. Image geometry uses
source pixels; physical measurements use the saved microscope calibration.
"""

import csv
import json
import os
from math import hypot, isfinite
from pathlib import Path
from queue import Empty
import numpy as np

from PyQt6.QtCore import QByteArray, QIODevice, QPointF, QRectF, QSaveFile, QSize, Qt, QTimer, pyqtSignal
from PyQt6.QtGui import (
    QAction, QActionGroup, QBrush, QColor, QIcon, QImage, QImageReader,
    QPainter, QPainterPath, QPainterPathStroker, QPalette, QPen, QPixmap, QPolygonF,
)
from PyQt6.QtWidgets import (
    QApplication, QCheckBox, QColorDialog, QComboBox, QDockWidget,
    QDoubleSpinBox, QFileDialog, QFormLayout, QGraphicsItem, QGraphicsPathItem,
    QGraphicsScene, QGraphicsSimpleTextItem, QGroupBox, QHBoxLayout,
    QHeaderView, QInputDialog, QLabel, QLineEdit, QMainWindow, QMessageBox,
    QPushButton, QScrollArea, QSlider, QTabWidget, QTableWidget,
    QTableWidgetItem, QToolBar, QVBoxLayout, QWidget, QWidgetAction,
)

from imaging_canvas import CanvasView, EditablePolygonItem, LINE_STYLES
from microscope_imaging import image_rgb8, balance_rgb8, white_balance_gains, sample_rgb_path, RGBProfilePlot
from microscope_camera import CameraSession
from microscope_motion import MotionConsole
from app_version import APP_VERSION
from microscope_backends import default_camera_backend
from camera_image import CameraImageState
from microscope_settings import (
    application_settings_path, settings_read_path, EDGE_PROCESSING_SIZES,
    DEFAULT_EDGE_DETECTION_PARAMETERS, validate_edge_detection_parameters,
)


TOOL_NAMES = {
    "select": "选择", "pan": "平移", "line": "长度",
    "polygon": "轮廓", "rectangle": "矩形", "profile": "RGB 路径",
}
TOOL_HINTS = {
    "select": "拖动标记移动；Shift＋拖动旋转；勾选编辑节点可修整轮廓。",
    "pan": "拖动画布平移；滚轮缩放；中键在任意工具下均可平移。",
    "line": "在图像上拖出测量线；结果按原图像素计算。",
    "polygon": "依次点击轮廓顶点；Enter 或双击完成；Backspace 撤回；Esc 取消。",
    "rectangle": "在图像上拖出矩形；完成后可像轮廓一样编辑四个节点。",
    "profile": "依次点击路径节点；Enter 或双击完成；沿开放折线测量 RGB 强度。",
    "white_balance": "点击图像中的中性灰 / 白色区域，以周围 9×9 像素校正白平衡；Esc 取消。",
}


def polygon_area(points):
    """Area of a simple polygon, or None for crossing/touching nonadjacent edges."""
    def cross(a, b, c):
        return (b.x() - a.x()) * (c.y() - a.y()) - (b.y() - a.y()) * (c.x() - a.x())

    def on_segment(a, b, p):
        return (abs(cross(a, b, p)) < 1e-9 and
                min(a.x(), b.x()) <= p.x() <= max(a.x(), b.x()) and
                min(a.y(), b.y()) <= p.y() <= max(a.y(), b.y()))

    count = len(points)
    for i in range(count):
        a, b = points[i], points[(i + 1) % count]
        for j in range(i + 1, count):
            if j == i + 1 or (i == 0 and j == count - 1):
                continue
            c, d = points[j], points[(j + 1) % count]
            if ((cross(a, b, c) * cross(a, b, d) < 0 and
                 cross(c, d, a) * cross(c, d, b) < 0) or
                    on_segment(a, b, c) or on_segment(a, b, d) or
                    on_segment(c, d, a) or on_segment(c, d, b)):
                return None
    return abs(sum(points[i].x() * points[(i + 1) % count].y() -
                   points[(i + 1) % count].x() * points[i].y()
                   for i in range(count))) / 2


class AnnotationItem(EditablePolygonItem):
    """Reuse the stack designer's drag, rotation and editable-vertex behavior."""

    def __init__(self, points, kind, name, changed):
        self.changed = None
        self.kind = kind
        self.caption = None
        super().__init__(points, name)
        self.outline_color = QColor("#ffcd59")
        self.fill_color = QColor("#ffcd59")
        self.fill_alpha = 24 if kind == "polygon" else 0
        self.edit_vertices = False
        self.apply_style()
        self.caption = QGraphicsSimpleTextItem(self)
        self.caption.setFlag(QGraphicsItem.GraphicsItemFlag.ItemIgnoresTransformations)
        self.caption.setAcceptedMouseButtons(Qt.MouseButton.NoButton)
        self.changed = changed

    def notify(self):
        if self.changed is not None:
            self.changed()

    def itemChange(self, change, value):
        result = super().itemChange(change, value)
        if change in (QGraphicsItem.GraphicsItemChange.ItemPositionHasChanged,
                      QGraphicsItem.GraphicsItemChange.ItemRotationHasChanged):
            self.notify()
        return result

    def move_vertex(self, index, point):
        super().move_vertex(index, point)
        self.notify()

    def remove_vertex(self, index):
        if self.kind == "polygon":
            super().remove_vertex(index)
            self.notify()
        elif self.kind == "profile" and len(self.polygon()) > 2:
            polygon = self.polygon()
            polygon.remove(index)
            self.setPolygon(polygon)
            self.rebuild_handles()
            self.notify()

    def insert_vertex(self, point):
        if self.kind == "polygon":
            super().insert_vertex(point)
            self.notify()
        elif self.kind == "profile":
            points = list(self.polygon())
            distances = []
            for a, b in zip(points, points[1:]):
                delta = b - a
                length2 = delta.x() ** 2 + delta.y() ** 2
                t = 0 if length2 == 0 else max(0, min(1, (
                    (point.x() - a.x()) * delta.x() + (point.y() - a.y()) * delta.y()) / length2))
                nearest = a + t * delta
                distances.append(hypot(point.x() - nearest.x(), point.y() - nearest.y()))
            points.insert(int(np.argmin(distances)) + 1, point)
            self.setPolygon(QPolygonF(points))
            self.rebuild_handles()
            self.notify()

    def open_path(self):
        points = list(self.polygon())
        path = QPainterPath(points[0])
        for point in points[1:]:
            path.lineTo(point)
        return path

    def shape(self):
        if self.kind != "profile":
            return super().shape()
        stroker = QPainterPathStroker()
        stroker.setWidth(max(8, self.line_width))
        return stroker.createStroke(self.open_path())

    def paint(self, painter, option, widget=None):
        if self.kind != "profile":
            super().paint(painter, option, widget)
            return
        painter.setPen(self.pen())
        painter.setBrush(Qt.BrushStyle.NoBrush)
        painter.drawPath(self.open_path())

    def measurements(self, scale=1.0):
        points = list(self.mapToScene(self.polygon()))
        edges = list(zip(points, points[1:]))
        if self.kind == "polygon":
            edges.append((points[-1], points[0]))
        length = sum(hypot(b.x() - a.x(), b.y() - a.y()) for a, b in edges)
        area = polygon_area(points) if self.kind == "polygon" else None
        return length * scale, None if area is None else area * scale * scale

    def update_caption(self, scale, unit):
        length, area = self.measurements(scale)
        value = f"{length:.2f} {unit}" if self.kind != "polygon" else (
            "自交轮廓" if area is None else f"{area:.2f} {unit}²")
        self.caption.setText(f"{self.name}  ·  {value}")
        self.caption.setBrush(QBrush(self.outline_color))
        self.caption.setPos(self.polygon().boundingRect().bottomLeft() + QPointF(0, 6))


class MicroscopeView(CanvasView):
    annotation_drawn = pyqtSignal(str, object)
    zoom_changed = pyqtSignal(float)
    cursor_moved = pyqtSignal(QPointF)
    neutral_picked = pyqtSignal(QPointF)
    tool_cancelled = pyqtSignal()

    def __init__(self, scene):
        super().__init__(scene)
        self.setBackgroundBrush(QBrush(QColor("#20252b")))
        self.setFrameShape(self.Shape.NoFrame)
        self.setMouseTracking(True)
        self.tool = "select"
        self.image_rect = QRectF()
        self.points = []
        self.start_point = None
        self.preview = None
        self.setMinimumSize(360, 300)

    def set_tool(self, tool):
        self.cancel_drawing()
        self.tool = tool
        self.setCursor(Qt.CursorShape.OpenHandCursor if tool == "pan" else
                       Qt.CursorShape.ArrowCursor if tool == "select" else Qt.CursorShape.CrossCursor)

    def cancel_drawing(self):
        if self.preview is not None:
            self.scene().removeItem(self.preview)
            self.preview = None
        self.points = []
        self.start_point = None

    def bounded_point(self, position):
        p = self.mapToScene(position)
        return QPointF(max(0, min(self.image_rect.width(), p.x())),
                       max(0, min(self.image_rect.height(), p.y())))

    def draw_preview(self, points):
        if not points:
            return
        path = QPainterPath(points[0])
        for point in points[1:]:
            path.lineTo(point)
        if self.tool in ("polygon", "rectangle"):
            path.closeSubpath()
        if self.preview is None:
            self.preview = QGraphicsPathItem()
            pen = QPen(QColor("#ffcd59"), 1.5, Qt.PenStyle.DashLine)
            pen.setCosmetic(True)
            self.preview.setPen(pen)
            self.preview.setZValue(10000)
            self.preview.setAcceptedMouseButtons(Qt.MouseButton.NoButton)
            self.scene().addItem(self.preview)
        self.preview.setPath(path)

    def finish_polygon(self):
        points = list(self.points)
        if len(points) > 1 and points[-1] == points[-2]:
            points.pop()
        if len(points) >= (2 if self.tool == "profile" else 3):
            kind = self.tool
            self.cancel_drawing()
            self.annotation_drawn.emit(kind, points)

    def mousePressEvent(self, event):
        if event.button() == Qt.MouseButton.LeftButton and self.tool == "pan":
            self._pan_pos = event.position()
            self.setCursor(Qt.CursorShape.ClosedHandCursor)
            event.accept()
            return
        if event.button() == Qt.MouseButton.LeftButton and self.tool not in ("select", "pan"):
            if not self.image_rect.contains(self.mapToScene(event.position().toPoint())):
                return
            point = self.bounded_point(event.position().toPoint())
            if self.tool == "white_balance":
                self.neutral_picked.emit(point)
            elif self.tool in ("polygon", "profile"):
                if not self.points or self.points[-1] != point:
                    self.points.append(point)
                self.draw_preview(self.points)
            else:
                self.start_point = point
            event.accept()
            return
        super().mousePressEvent(event)

    def mouseMoveEvent(self, event):
        point = self.bounded_point(event.position().toPoint())
        self.cursor_moved.emit(self.mapToScene(event.position().toPoint()))
        if self._pan_pos is not None:
            super().mouseMoveEvent(event)
            return
        if self.tool in ("polygon", "profile") and self.points:
            self.draw_preview(self.points + [point])
        elif self.start_point is not None:
            self.draw_preview(self.shape_points(point))
        else:
            super().mouseMoveEvent(event)

    def shape_points(self, end):
        start = self.start_point
        if self.tool == "rectangle":
            return [start, QPointF(end.x(), start.y()), end, QPointF(start.x(), end.y())]
        return [start, end]

    def mouseReleaseEvent(self, event):
        if event.button() == Qt.MouseButton.LeftButton and self.tool == "pan":
            self._pan_pos = None
            self.setCursor(Qt.CursorShape.OpenHandCursor)
            event.accept()
            return
        if event.button() == Qt.MouseButton.LeftButton and self.start_point is not None:
            end = self.bounded_point(event.position().toPoint())
            points = self.shape_points(end)
            valid = hypot(end.x() - self.start_point.x(), end.y() - self.start_point.y()) > 0.5
            if self.tool == "rectangle":
                valid = valid and abs(end.x() - self.start_point.x()) > 0.5 and abs(end.y() - self.start_point.y()) > 0.5
            kind = "line" if self.tool == "line" else "polygon"
            self.cancel_drawing()
            if valid:
                self.annotation_drawn.emit(kind, points)
            event.accept()
            return
        super().mouseReleaseEvent(event)

    def mouseDoubleClickEvent(self, event):
        if self.tool in ("polygon", "profile") and event.button() == Qt.MouseButton.LeftButton:
            self.finish_polygon()
            event.accept()
            return
        super().mouseDoubleClickEvent(event)

    def keyPressEvent(self, event):
        if event.key() == Qt.Key.Key_Escape:
            self.cancel_drawing()
            if self.tool == "white_balance":
                self.tool_cancelled.emit()
        elif event.key() in (Qt.Key.Key_Return, Qt.Key.Key_Enter) and self.tool in ("polygon", "profile"):
            self.finish_polygon()
        elif event.key() == Qt.Key.Key_Backspace and self.points:
            self.points.pop()
            if self.preview is not None:
                self.preview.setPath(QPainterPath())
            self.draw_preview(self.points)
        else:
            super().keyPressEvent(event)

    def wheelEvent(self, event):
        super().wheelEvent(event)
        self.zoom_changed.emit(self.transform().m11())

    def fit_content(self):
        if not self.image_rect.isEmpty():
            self.fitInView(self.image_rect.adjusted(-15, -15, 15, 15), Qt.AspectRatioMode.KeepAspectRatio)
            self.zoom_changed.emit(self.transform().m11())

    def drawForeground(self, painter, rect):
        super().drawForeground(painter, rect)
        if self.image_rect.isEmpty():
            painter.save()
            painter.resetTransform()
            painter.setPen(QColor("#dce3eb"))
            font = painter.font()
            font.setPointSize(19)
            painter.setFont(font)
            bounds = QRectF(self.viewport().rect())
            painter.drawText(bounds.adjusted(0, -60, 0, -60), Qt.AlignmentFlag.AlignCenter, "显微镜图像工作区")
            font.setPointSize(10)
            painter.setFont(font)
            painter.setPen(QColor("#9aa9b9"))
            painter.drawText(bounds.adjusted(0, 30, 0, 30), Qt.AlignmentFlag.AlignCenter,
                             "打开本地图像，开始测量与轮廓描绘\n\n滚轮缩放  ·  中键平移  ·  原图像素坐标\n\n相机尚未接入")
            painter.restore()


def tool_icon(tool):
    pixmap = QPixmap(24, 24)
    pixmap.fill(Qt.GlobalColor.transparent)
    painter = QPainter(pixmap)
    painter.setRenderHint(QPainter.RenderHint.Antialiasing)
    painter.setPen(QPen(QColor("#34495e"), 1.8))
    if tool == "line":
        painter.drawLine(5, 18, 19, 6)
        painter.drawLine(3, 15, 7, 20)
        painter.drawLine(17, 4, 21, 9)
    elif tool == "rectangle":
        painter.drawRect(4, 5, 16, 14)
    elif tool == "profile":
        for offset, color in enumerate(("#d94a46", "#258844", "#307cd4")):
            painter.setPen(QPen(QColor(color), 1.8))
            path = QPainterPath(QPointF(3, 18 - offset * 3))
            path.lineTo(9, 10 - offset * 2)
            path.lineTo(15, 14 - offset * 2)
            path.lineTo(21, 5 + offset * 2)
            painter.drawPath(path)
    elif tool == "polygon":
        points = QPolygonF([QPointF(4, 7), QPointF(16, 3), QPointF(21, 14), QPointF(12, 20), QPointF(3, 16)])
        painter.drawPolygon(points)
        for point in points:
            painter.fillRect(QRectF(point.x() - 2, point.y() - 2, 4, 4), QColor("#1874bd"))
    elif tool == "pan":
        painter.drawLine(12, 3, 12, 21)
        painter.drawLine(3, 12, 21, 12)
        for a, b, c, d in [(12, 3, 9, 6), (12, 3, 15, 6), (21, 12, 18, 9),
                           (21, 12, 18, 15), (12, 21, 9, 18), (12, 21, 15, 18),
                           (3, 12, 6, 9), (3, 12, 6, 15)]:
            painter.drawLine(a, b, c, d)
    else:
        painter.drawPolygon(QPolygonF([QPointF(5, 3), QPointF(19, 13), QPointF(12, 14), QPointF(9, 21)]))
    painter.end()
    return QIcon(pixmap)


class MicroscopeWindow(QMainWindow):
    """Microscope-owned top-level window; SDK loads only on explicit connection."""

    @staticmethod
    def default_settings_path():
        return application_settings_path()

    def __init__(self, *, settings_path=None, camera_factory=default_camera_backend,
                 image_state=None):
        super().__init__(None, Qt.WindowType.Window)
        self.settings_path = Path(settings_path) if settings_path is not None else self.default_settings_path()
        self._settings_read_path = settings_read_path(
            self.settings_path, migrate_legacy=settings_path is None and
            self.settings_path == application_settings_path())
        self._settings_load_failed = False
        self.stack_designer = None
        self.image_state = image_state if image_state is not None else CameraImageState()
        self.camera_session = CameraSession(camera_factory)
        self.camera_state = "disconnected"
        self.camera_settings = None
        self.camera_error = ""
        self.camera_closing = False
        self.camera_frame_metadata = None
        self._camera_source_key = None
        self.annotation_defaults = {"color": "#ffcd59", "width": 2.0,
                                    "style": "SolidLine", "fill": 24}
        # Keep this workspace's light instrument panels independent of the
        # existing controller application's dark palette.
        palette = QPalette()
        for role, color in {
            QPalette.ColorRole.Window: "#edf1f5", QPalette.ColorRole.WindowText: "#283746",
            QPalette.ColorRole.Base: "#ffffff", QPalette.ColorRole.AlternateBase: "#f4f7fa",
            QPalette.ColorRole.Text: "#283746", QPalette.ColorRole.Button: "#edf1f5",
            QPalette.ColorRole.ButtonText: "#283746", QPalette.ColorRole.Highlight: "#dbeafa",
            QPalette.ColorRole.HighlightedText: "#233e54", QPalette.ColorRole.Light: "#ffffff",
            QPalette.ColorRole.Mid: "#b7c2ce", QPalette.ColorRole.Dark: "#98a7b5",
            QPalette.ColorRole.ToolTipBase: "#ffffec", QPalette.ColorRole.ToolTipText: "#283746",
        }.items():
            palette.setColor(role, QColor(color))
        self.setPalette(palette)
        self.setWindowTitle(f"显微镜工作台 v{APP_VERSION}")
        self.resize(1380, 900)
        self.setMinimumSize(1040, 720)
        self.setCorner(Qt.Corner.BottomRightCorner, Qt.DockWidgetArea.RightDockWidgetArea)
        self.image = QImage()
        self.display_image = QImage()
        self.source_rgb = None
        self.balanced_rgb = None
        self.wb_gains = np.ones(3)
        self.profile_data = None
        self.annotations = []
        self.microns_per_pixel = None
        self._next_id = 1
        self._syncing = False
        self.scene = QGraphicsScene(self)
        self.view = MicroscopeView(self.scene)
        self.view.annotation_drawn.connect(self.add_annotation)
        self.view.neutral_picked.connect(self.pick_white_balance)
        self.view.tool_cancelled.connect(lambda: self.set_tool("select"))
        self.scene.selectionChanged.connect(self.sync_selection)
        self.setStyleSheet("""
            QMainWindow, QWidget { color: #283746; font-family: 'Microsoft YaHei UI'; font-size: 12px; }
            QMainWindow { background: #e7ebef; }
            QMenuBar, QToolBar, QStatusBar { background: #f3f5f7; }
            QMenuBar::item:selected, QMenu::item:selected { background: #dceaf7; }
            QMenu { background: #fafbfc; border: 1px solid #b7c2ce; }
            QToolBar { border: 0; border-bottom: 1px solid #c8d1dc; spacing: 5px; padding: 5px; }
            QToolButton { padding: 5px; border: 1px solid transparent; border-radius: 3px; }
            QToolButton:hover, QPushButton:hover { background: #e1edf7; border-color: #91b4d6; }
            QToolButton:checked { background: #d2e7f9; border: 1px solid #8cb5db; }
            QDockWidget { font-weight: 600; }
            QDockWidget::title { background: #dfe5ec; padding: 7px; }
            QGroupBox { background: #f7f9fb; border: 1px solid #d3dce5; border-radius: 3px;
                        margin-top: 17px; padding: 12px 9px 8px; font-weight: 600; }
            QGroupBox::title { subcontrol-origin: margin; left: 10px; top: 1px; }
            QPushButton { background: #f8fafc; border: 1px solid #bcc9d6; border-radius: 3px; padding: 6px 10px; }
            QPushButton:disabled { color: #909eab; background: #edf1f5; border-color: #d3dce5; }
            QLineEdit, QDoubleSpinBox, QComboBox { background: white; color: #283746;
                border: 1px solid #bdc9d6; border-radius: 2px; padding: 4px; min-height: 19px; }
            QComboBox QAbstractItemView { background: white; color: #283746; selection-background-color: #d2e7f9; }
            QTabWidget::pane { border: 1px solid #ccd5df; background: #f7f9fb; }
            QTabBar::tab { background: #e5ebf1; border: 1px solid #ccd5df; padding: 8px 20px; }
            QTabBar::tab:selected { background: #f7f9fb; color: #1166a6; border-bottom: 2px solid #287dba; }
            QTableWidget { background: white; alternate-background-color: #f4f7fa; gridline-color: #e2e7ed;
                border: 0; selection-background-color: #dbeafa; selection-color: #233e54; }
            QHeaderView::section { background: #eef2f6; padding: 5px; border: 0; border-bottom: 1px solid #ccd5df; }
            QScrollArea { border: 0; background: #f7f9fb; }
            QLabel#muted { color: #697d90; font-size: 11px; }
        """)
        self.build_canvas()
        self.build_controls()
        self.build_results()
        self.build_actions()
        # Dock/scroll widgets with style sheets may resolve their palette from
        # QApplication rather than the parent. Apply the theme locally only.
        for widget in self.findChildren(QWidget):
            widget.setPalette(palette)
        self.cursor_label = QLabel("X —   Y —")
        self.zoom_label = QLabel("100%")
        self.statusBar().addPermanentWidget(self.cursor_label)
        self.statusBar().addPermanentWidget(self.zoom_label)
        self.hardware_status = QLabel("相机未连接")
        self.statusBar().addPermanentWidget(self.hardware_status)
        self.motion_status = QLabel("Onway 未连接")
        self.statusBar().addPermanentWidget(self.motion_status)
        self.motion.status_changed.connect(lambda state: self.motion_status.setText(f"Onway {state}"))
        self.motion.ready_to_close.connect(self.close)
        self.camera_timer = QTimer(self)
        self.camera_timer.setInterval(67)
        self.camera_timer.timeout.connect(self.poll_camera)
        self.camera_timer.start()
        self.view.cursor_moved.connect(self.update_cursor)
        self.view.zoom_changed.connect(lambda zoom: self.zoom_label.setText(f"{zoom * 100:.0f}%"))
        self.set_tool("select")
        self.sync_selection()
        self.resizeDocks([self.controls_dock], [330], Qt.Orientation.Horizontal)
        self.resizeDocks([self.results_dock], [155], Qt.Orientation.Vertical)
        try:
            self.restore_settings()
        except (OSError, ValueError, TypeError, KeyError) as exc:
            # Never overwrite an unreadable configuration with defaults.
            self._settings_load_failed = True
            QMessageBox.critical(self, "显微镜设置读取失败",
                                 f"{self._settings_read_path}\n{exc}\n本次不会覆盖设置文件。请修复或移走该文件后重新启动。")

    def settings_snapshot(self):
        return {
            "version": 1,
            "exposure_ms": self.exposure_box.value(), "gain": self.gain_box.value(),
            "motion_speed": self.motion_speed_box.value(),
            "white_balance": self.wb_gains.tolist(), "microns_per_pixel": self.microns_per_pixel,
            "device_tab": self.device_tabs.currentIndex(), "result_tab": self.result_tabs.currentIndex(),
            "profile_source": self.profile_source.currentIndex(),
            "rgb_channels": [box.isChecked() for box in self.channel_checks],
            "annotation_style": dict(self.annotation_defaults), "edit_nodes": self.edit_nodes.isChecked(),
            "edge_detection": self.edge_detection_parameters(),
            "geometry": bytes(self.saveGeometry()).hex(), "layout": bytes(self.saveState(1)).hex(),
        }

    def restore_settings(self):
        if not self._settings_read_path.exists():
            return
        data = json.loads(self._settings_read_path.read_text(encoding="utf-8"))
        if not isinstance(data, dict) or type(data.get("version")) is not int or data["version"] != 1:
            raise ValueError("不支持的显微镜设置格式。")

        def number(value, low, high):
            if type(value) not in (int, float) or not isfinite(value) or not low <= value <= high:
                raise ValueError(f"设置数值超出范围 [{low}, {high}]：{value!r}")

        # Validate the whole document before applying any parameter.
        for key, box in [("exposure_ms", self.exposure_box), ("gain", self.gain_box),
                         ("motion_speed", self.motion_speed_box)]:
            number(data[key], box.minimum(), box.maximum())
        gains = data["white_balance"]
        if not isinstance(gains, list) or len(gains) != 3:
            raise ValueError("白平衡必须包含 R/G/B 三个增益。")
        for gain in gains:
            number(gain, 0.01, 16)
        calibration = data["microns_per_pixel"]
        if calibration is not None:
            number(calibration, 1e-300, float("inf"))
        for key in ("device_tab", "result_tab", "profile_source"):
            if type(data[key]) is not int or data[key] not in (0, 1):
                raise ValueError(f"无效的选项卡 / 数据来源：{key}")
        channels = data["rgb_channels"]
        if not isinstance(channels, list) or len(channels) != 3 or any(type(x) is not bool for x in channels):
            raise ValueError("RGB 通道显隐设置无效。")
        if type(data["edit_nodes"]) is not bool:
            raise ValueError("节点编辑设置无效。")
        style = data["annotation_style"]
        if not isinstance(style, dict) or not isinstance(style["color"], str) or not QColor(style["color"]).isValid():
            raise ValueError("标记颜色无效。")
        number(style["width"], 0.5, 8)
        number(style["fill"], 0, 255)
        if style["style"] not in [value.name for value in LINE_STYLES.values()]:
            raise ValueError("标记线形无效。")
        # Older microscope preference files have no edge-detection section.
        edge_parameters = validate_edge_detection_parameters(
            data.get("edge_detection", DEFAULT_EDGE_DETECTION_PARAMETERS))
        geometry = QByteArray(bytes.fromhex(data["geometry"]))
        layout = QByteArray(bytes.fromhex(data["layout"]))
        if not self.restoreGeometry(geometry) or not self.restoreState(layout, 1):
            raise ValueError("窗口布局数据无效。")
        self.exposure_box.setValue(data["exposure_ms"])
        self.gain_box.setValue(data["gain"])
        self.motion_speed_box.setValue(data["motion_speed"])
        self.wb_gains = np.asarray(gains, dtype=float)
        for box, gain in zip(self.wb_boxes, gains):
            box.blockSignals(True)
            box.setValue(gain)
            box.blockSignals(False)
        self.set_calibration(calibration)
        self.device_tabs.setCurrentIndex(data["device_tab"])
        self.result_tabs.setCurrentIndex(data["result_tab"])
        self.profile_source.setCurrentIndex(data["profile_source"])
        for box, checked in zip(self.channel_checks, channels):
            box.setChecked(checked)
        self.annotation_defaults = dict(style)
        self.edit_nodes.setChecked(data["edit_nodes"])
        self.set_edge_detection_parameters(edge_parameters)

    def save_settings(self):
        if self._settings_load_failed:
            return
        self.settings_path.parent.mkdir(parents=True, exist_ok=True)
        payload = json.dumps(self.settings_snapshot(), ensure_ascii=False, indent=2, allow_nan=False).encode("utf-8")
        target = QSaveFile(str(self.settings_path))
        # QSaveFile commits by atomic replacement; failed writes preserve the old file.
        if not target.open(QIODevice.OpenModeFlag.WriteOnly):
            raise OSError(target.errorString())
        if target.write(payload) != len(payload):
            message = target.errorString()
            target.cancelWriting()
            raise OSError(message)
        if not target.commit():
            raise OSError(target.errorString())

    def closeEvent(self, event):
        if not self.motion.prepare_close():
            event.ignore()
            return
        self.image_state.clear()
        if self.camera_session.alive:
            self.camera_closing = True
            self.camera_session.request("disconnect")
            self.set_camera_state("disconnecting")
            event.ignore()
            return
        # Commit text still being edited before reading the spin-box values.
        for box in self.findChildren(QDoubleSpinBox):
            box.interpretText()
        try:
            self.save_settings()
        except (OSError, ValueError) as exc:
            QMessageBox.warning(self, "显微镜设置保存失败", f"{self.settings_path}\n{exc}\n窗口暂不关闭，请修复后重试。")
            event.ignore()
            return
        if self.stack_designer is not None and not self.stack_designer.close():
            event.ignore()
            return
        super().closeEvent(event)

    def open_stack_designer(self):
        if self.stack_designer is None:
            try:
                from stack_designer import StackDesignerWindow
            except ModuleNotFoundError as exc:
                if exc.name not in {"cv2", "numpy"}:
                    raise
                QMessageBox.warning(self, "缺少图像处理依赖",
                                    "叠层设计需要 numpy 和 opencv-python，请安装后重试。")
                return
            self.stack_designer = StackDesignerWindow(self)
        if self.stack_designer.isMinimized():
            self.stack_designer.showNormal()
        else:
            self.stack_designer.show()
        self.stack_designer.raise_()
        self.stack_designer.activateWindow()

    def build_canvas(self):
        central = QWidget()
        layout = QVBoxLayout(central)
        layout.setContentsMargins(0, 0, 0, 0)
        layout.setSpacing(0)
        self.image_title = QLabel("  图像  /  尚未打开")
        self.image_title.setStyleSheet("background: #343e49; color: #e6edf5; padding: 9px; font-weight: 600;")
        layout.addWidget(self.image_title)
        layout.addWidget(self.view, 1)
        self.setCentralWidget(central)

    @staticmethod
    def muted(text):
        label = QLabel(text)
        label.setObjectName("muted")
        label.setWordWrap(True)
        return label

    @staticmethod
    def spin(minimum, maximum, value, suffix="", decimals=3):
        box = QDoubleSpinBox()
        box.setRange(minimum, maximum)
        box.setDecimals(decimals)
        box.setValue(value)
        box.setSuffix(suffix)
        return box

    def hardware_button(self, text):
        button = QPushButton(text)
        button.setProperty("hardware_command", True)
        button.setEnabled(False)
        button.setToolTip("硬件尚未接入；当前仅提供界面布局。")
        return button

    def camera_page(self):
        page = QWidget()
        layout = QVBoxLayout(page)
        self.camera_status = self.muted("未连接 · 点击连接后读取设备参数")
        layout.addWidget(self.camera_status)
        self.camera_connect = QPushButton("连接相机")
        self.camera_connect.clicked.connect(self.toggle_camera_connection)
        layout.addWidget(self.camera_connect)
        form = QFormLayout()
        model = QComboBox()
        model.addItem("Nikon DS-Fi3（USB 3.0）")
        model.setEnabled(False)
        form.addRow("设备", model)
        self.exposure_box = self.spin(0.01, 60000, 50, " ms", 2)
        self.gain_box = self.spin(0, 100, 1, " ×", 2)
        form.addRow("曝光", self.exposure_box)
        form.addRow("增益", self.gain_box)
        self.camera_mode = QComboBox()
        self.camera_mode.addItem("全幅 2880 × 2048", 1)
        self.camera_mode.addItem("平均采样 1440 × 1024", 3)
        form.addRow("采样", self.camera_mode)
        layout.addLayout(form)
        self.camera_apply = QPushButton("应用参数（手动曝光）")
        self.camera_apply.setEnabled(False)
        self.camera_apply.clicked.connect(self.apply_camera_settings)
        layout.addWidget(self.camera_apply)
        self.camera_readback = self.muted("读回 —；编辑参数后请点击应用")
        layout.addWidget(self.camera_readback)
        row = QHBoxLayout()
        self.camera_preview = self.hardware_button("实时预览")
        self.camera_capture = self.hardware_button("拍摄单帧")
        self.camera_preview.setToolTip("使用已应用参数；停止后保留最后一帧。")
        self.camera_capture.setToolTip("使用已应用参数采集一张新图像。")
        self.camera_preview.clicked.connect(self.toggle_camera_preview)
        self.camera_capture.clicked.connect(lambda: self.start_camera_acquisition("capture"))
        row.addWidget(self.camera_preview)
        row.addWidget(self.camera_capture)
        layout.addLayout(row)
        self.camera_save = QPushButton("保存相机原图与参数…")
        self.camera_save.setEnabled(False)
        self.camera_save.clicked.connect(self.save_camera_frame)
        layout.addWidget(self.camera_save)
        self.wb_group = QGroupBox("白平衡 · 软件校色")
        wb_layout = QVBoxLayout(self.wb_group)
        row = QHBoxLayout()
        automatic = QPushButton("自动（灰世界）")
        automatic.clicked.connect(self.auto_white_balance)
        neutral = QPushButton("选取中性区域")
        neutral.clicked.connect(lambda: self.set_tool("white_balance"))
        row.addWidget(automatic)
        row.addWidget(neutral)
        wb_layout.addLayout(row)
        row = QHBoxLayout()
        self.wb_boxes = []
        for channel in "RGB":
            row.addWidget(QLabel(channel))
            box = self.spin(0.01, 16, 1, "", 3)
            box.setButtonSymbols(QDoubleSpinBox.ButtonSymbols.NoButtons)
            box.setFixedWidth(62)
            box.setSingleStep(0.05)
            box.valueChanged.connect(self.manual_white_balance)
            row.addWidget(box)
            self.wb_boxes.append(box)
        wb_layout.addLayout(row)
        reset = QPushButton("恢复原图白平衡")
        reset.clicked.connect(lambda: self.set_white_balance([1, 1, 1]))
        wb_layout.addWidget(reset)
        wb_layout.addWidget(self.muted("软件校色；原图保留。增益会记忆并沿用到新图像。"))
        self.wb_group.setEnabled(False)
        layout.addWidget(self.wb_group)
        display = QGroupBox("图像信息")
        details = QVBoxLayout(display)
        self.image_info = self.muted("尺寸 —\n来源 —\n帧率 —")
        details.addWidget(self.image_info)
        layout.addWidget(display)
        layout.addStretch()
        return page

    def set_camera_state(self, state):
        if self.camera_closing and state != "disconnected":
            state = "disconnecting"
        self.camera_state = state
        if state in ("connecting", "disconnecting", "disconnected"):
            self.image_state.clear()
        idle = state == "idle"
        live = state == "streaming"
        disconnected = state == "disconnected"
        names = {"disconnected": "未连接", "connecting": "连接中…", "idle": "已连接 · 空闲",
                 "configuring": "应用参数中…", "streaming": "实时预览 · 手动曝光",
                 "capturing": "单帧拍摄中…", "stopping": "停止中…", "disconnecting": "释放相机中…"}
        label = names.get(state, state)
        self.camera_status.setText(self.camera_error or label)
        self.hardware_status.setText(f"相机 {label}")
        self.camera_connect.setText("连接相机" if disconnected else "断开相机")
        self.camera_connect.setEnabled(not self.camera_closing and
                                       state not in ("connecting", "disconnecting"))
        self.camera_preview.setEnabled(idle or live)
        self.camera_preview.setText("停止预览" if live else "实时预览")
        self.camera_capture.setEnabled(idle)
        self.camera_apply.setEnabled(idle)
        for control in (self.exposure_box, self.gain_box, self.camera_mode):
            control.setEnabled(idle or disconnected)
        self.camera_save.setEnabled(self.camera_frame_metadata is not None and (idle or disconnected))

    def toggle_camera_connection(self):
        if self.camera_session.alive:
            self.camera_session.request("disconnect")
            self.set_camera_state("disconnecting")
        else:
            self.camera_error = ""
            self.set_camera_state("connecting")
            self.camera_session.connect()

    def apply_camera_settings(self):
        mode = self.camera_mode.currentData()
        if (self.camera_settings and mode != self.camera_settings["mode"] and self.annotations and
                QMessageBox.question(self, "更换采样模式", "新图像将清除标记并使旧标定失效，是否继续？",
                                     QMessageBox.StandardButton.Yes | QMessageBox.StandardButton.No,
                                     QMessageBox.StandardButton.No) != QMessageBox.StandardButton.Yes):
            return
        self.set_camera_state("configuring")
        self.camera_session.request("configure", exposure_ms=self.exposure_box.value(),
                                    gain=self.gain_box.value(), mode=mode)

    def toggle_camera_preview(self):
        if self.camera_state == "streaming":
            self.camera_session.request("stop")
            self.set_camera_state("stopping")
        else:
            self.start_camera_acquisition("preview")

    def start_camera_acquisition(self, command):
        if not self.camera_settings or self.camera_state != "idle":
            return
        settings = self.camera_settings
        key = (settings["device"]["serial"], settings["mode"])
        if self._camera_source_key != key and self.annotations:
            if QMessageBox.question(self, "切换到相机图像", "新图像将清除标记并使旧标定失效，是否继续？",
                                    QMessageBox.StandardButton.Yes | QMessageBox.StandardButton.No,
                                    QMessageBox.StandardButton.No) != QMessageBox.StandardButton.Yes:
                return
        self.set_camera_state("capturing" if command == "capture" else "configuring")
        self.camera_session.request(command)

    def poll_camera(self):
        while True:
            try:
                event, data = self.camera_session.events.get_nowait()
            except Empty:
                break
            if event == "error":
                self.image_state.clear()
                self.camera_error = data
                self.statusBar().showMessage(data)
            elif event == "ready":
                self.camera_settings = data
                # Readback, not the old UI preset, is authoritative on connection.
                for name, box in (("exposure_ms", self.exposure_box), ("gain", self.gain_box)):
                    low, high, step = data["ranges"][name]
                    box.setRange(low, high)
                    box.setSingleStep(step)
                    box.setValue(data[name])
                self.camera_mode.clear()
                for mode in data["modes"]:
                    label = "全幅" if mode["id"] == 1 else "平均采样"
                    self.camera_mode.addItem(f"{label} {mode['width']} × {mode['height']}", mode["id"])
                self.camera_mode.setCurrentIndex(self.camera_mode.findData(data["mode"]))
                exposure_mode = "自动" if data["exposure_auto"] else "手动"
                self.camera_readback.setText(f"设备 {data['device']['serial']} · {exposure_mode}\n"
                                            f"读回 {data['exposure_ms']:g} ms / {data['gain']:g} ×")
                self.set_camera_state("idle")
            elif event in ("idle", "captured"):
                self.set_camera_state("idle")
            elif event in ("streaming", "capturing", "disconnected"):
                self.set_camera_state(event)
        frame = self.camera_session.take_frame()
        if frame is not None:
            self.show_camera_frame(frame)
        if self.camera_closing and not self.camera_session.alive:
            self.camera_closing = False
            self.set_camera_state("disconnected")
            if self.camera_error:
                QMessageBox.warning(self, "相机关闭状态", self.camera_error)
            self.close()

    def show_camera_frame(self, frame):
        rgb, meta = frame.rgb, frame.metadata
        height, width, _ = rgb.shape
        image = QImage(rgb.data, width, height, rgb.strides[0], QImage.Format.Format_RGB888).copy()
        key = (meta["device_serial"], meta["mode"])
        if self._camera_source_key != key or self.image.size() != image.size():
            self.set_calibration(None)
            self.load_image(image, "Nikon DS-Fi3")
            self._camera_source_key = key
        else:
            # Keep graphics item, view transform, selection and annotation nodes.
            self.image = image
            self.source_rgb = rgb
            self.set_white_balance(self.wb_gains)
        self.camera_frame_metadata = meta.copy()
        if self.camera_state not in ("connecting", "disconnecting", "disconnected") and not self.camera_error:
            self.image_state.publish(image, meta)
        self.image_title.setText(f"  Nikon DS-Fi3    |    {width} × {height} px    |    帧 {meta['frame_no']}")
        self.image_info.setText(f"尺寸  {width} × {height} · RGB8\n来源  DS-Fi3 / {meta['device_serial']}\n"
                                f"采集  {meta['fps']:.1f} fps · 帧 {meta['frame_no']}\n"
                                f"帧号缺口  {meta['missing_frame_ids']}（位置未定位）\n"
                                f"显示跳过  {self.camera_session.display_skipped} 帧")
        self.camera_readback.setText(f"帧读回 {meta['exposure_ms']:g} ms / {meta['gain']:g} × · 手动")
        self.camera_save.setEnabled(self.camera_state in ("idle", "disconnected"))

    def save_camera_frame(self):
        if self.camera_frame_metadata is None:
            return
        # Freeze the exact image and metadata together before the modal dialog.
        image, metadata = self.image.copy(), self.camera_frame_metadata.copy()
        metadata.update(software_white_balance=self.wb_gains.tolist(),
                        saved_pixels="original_rgb8_without_software_white_balance_or_annotations",
                        microns_per_pixel=self.microns_per_pixel)
        path, _ = QFileDialog.getSaveFileName(self, "保存相机原图", "DS-Fi3.png", "PNG (*.png)")
        if not path:
            return
        path = Path(path).with_suffix(".png")
        try:
            target = QSaveFile(str(path))
            if not target.open(QIODevice.OpenModeFlag.WriteOnly):
                raise OSError(target.errorString())
            if not image.save(target, "PNG") or not target.commit():
                raise OSError(target.errorString())
            target = QSaveFile(str(path.with_suffix(".json")))
            payload = json.dumps(metadata, ensure_ascii=False, indent=2).encode("utf-8")
            if not target.open(QIODevice.OpenModeFlag.WriteOnly):
                raise OSError(target.errorString())
            if target.write(payload) != len(payload) or not target.commit():
                raise OSError(target.errorString())
        except OSError as exc:
            QMessageBox.warning(self, "保存失败", f"图像与参数未全部保存：{exc}")

    def motion_page(self):
        self.motion = MotionConsole(self.settings_path.parent)
        self.motion_speed_box = self.motion.speed
        return self.motion

    def build_controls(self):
        self.controls_dock = QDockWidget("设备与标记", self)
        self.controls_dock.setObjectName("microscope-controls")
        self.controls_dock.setFeatures(QDockWidget.DockWidgetFeature.DockWidgetMovable |
                                       QDockWidget.DockWidgetFeature.DockWidgetFloatable)
        container = QWidget()
        layout = QVBoxLayout(container)
        layout.setContentsMargins(8, 8, 8, 8)
        self.device_tabs = QTabWidget()
        self.device_tabs.addTab(self.camera_page(), "相机")
        self.device_tabs.addTab(self.motion_page(), "控制台")
        layout.addWidget(self.device_tabs)
        self.inspector = QGroupBox("标记与测量")
        form = QFormLayout(self.inspector)
        self.name_edit = QLineEdit()
        self.name_edit.setPlaceholderText("选择一个标记")
        self.name_edit.editingFinished.connect(self.apply_properties)
        form.addRow("名称", self.name_edit)
        self.color_button = QPushButton("标记颜色")
        self.color_button.clicked.connect(self.choose_color)
        form.addRow("颜色", self.color_button)
        self.width_box = self.spin(0.5, 8, 2, " px", 1)
        self.width_box.valueChanged.connect(self.apply_properties)
        form.addRow("线宽", self.width_box)
        self.style_box = QComboBox()
        self.style_box.addItems(LINE_STYLES)
        self.style_box.currentTextChanged.connect(self.apply_properties)
        form.addRow("线形", self.style_box)
        self.rotation_box = self.spin(-180, 180, 0, " °", 1)
        self.rotation_box.valueChanged.connect(self.apply_properties)
        form.addRow("角度", self.rotation_box)
        self.fill_slider = QSlider(Qt.Orientation.Horizontal)
        self.fill_slider.setRange(0, 100)
        self.fill_slider.setValue(10)
        self.fill_slider.valueChanged.connect(self.apply_properties)
        form.addRow("填充", self.fill_slider)
        self.edit_nodes = QCheckBox("编辑节点")
        self.edit_nodes.toggled.connect(self.toggle_nodes)
        form.addRow(self.edit_nodes)
        form.addRow(self.muted("拖动移动 · Shift＋拖动旋转\n编辑节点时：双击加点，右键删点"))
        remove = QPushButton("删除选中标记")
        remove.clicked.connect(self.delete_selected)
        form.addRow(remove)
        layout.addWidget(self.inspector)
        layout.addStretch()
        scroll = QScrollArea()
        scroll.setWidgetResizable(True)
        scroll.setWidget(container)
        scroll.setMinimumWidth(308)
        self.controls_dock.setWidget(scroll)
        self.addDockWidget(Qt.DockWidgetArea.RightDockWidgetArea, self.controls_dock)

    def build_results(self):
        self.results_dock = QDockWidget("测量结果", self)
        self.results_dock.setObjectName("microscope-results")
        self.table = QTableWidget(0, 4)
        self.table.setHorizontalHeaderLabels(["标记名称", "类型", "长度 / 周长", "面积"])
        self.table.setSelectionBehavior(QTableWidget.SelectionBehavior.SelectRows)
        self.table.setSelectionMode(QTableWidget.SelectionMode.SingleSelection)
        self.table.setEditTriggers(QTableWidget.EditTrigger.NoEditTriggers)
        self.table.setAlternatingRowColors(True)
        self.table.verticalHeader().hide()
        self.table.horizontalHeader().setSectionResizeMode(QHeaderView.ResizeMode.Stretch)
        self.table.itemSelectionChanged.connect(self.select_table_item)
        self.result_tabs = QTabWidget()
        self.result_tabs.addTab(self.table, "尺寸测量")
        profile_page = QWidget()
        layout = QVBoxLayout(profile_page)
        layout.setContentsMargins(5, 3, 5, 3)
        row = QHBoxLayout()
        self.profile_label = QLabel("未选择路径")
        row.addWidget(self.profile_label)
        row.addStretch()
        self.profile_source = QComboBox()
        self.profile_source.addItems(["原始图像 · RGB8", "白平衡后 · RGB8"])
        self.profile_source.currentIndexChanged.connect(self.update_rgb_profile)
        row.addWidget(self.profile_source)
        self.profile_plot = RGBProfilePlot()
        self.channel_checks = []
        for index, channel in enumerate("RGB"):
            check = QCheckBox(channel)
            check.setChecked(True)
            check.setStyleSheet(f"color: {RGBProfilePlot.COLORS[index]};")
            check.toggled.connect(lambda checked, c=index: self.show_rgb_channel(c, checked))
            row.addWidget(check)
            self.channel_checks.append(check)
        self.profile_export = QPushButton("导出曲线…")
        self.profile_export.setEnabled(False)
        self.profile_export.clicked.connect(self.export_rgb_profile)
        row.addWidget(self.profile_export)
        layout.addLayout(row)
        layout.addWidget(self.profile_plot, 1)
        layout.addWidget(self.muted("沿原图路径每 1 px 采样，双线性插值；RGB8 像素值，不代表绝对光功率。"))
        self.result_tabs.addTab(profile_page, "RGB 路径强度")
        self.results_dock.setWidget(self.result_tabs)
        self.addDockWidget(Qt.DockWidgetArea.BottomDockWidgetArea, self.results_dock)

    def build_actions(self):
        file_menu = self.menuBar().addMenu("文件(&F)")
        edit_menu = self.menuBar().addMenu("编辑(&E)")
        self.build_edge_detection_menu(edit_menu)
        view_menu = self.menuBar().addMenu("视图(&V)")
        measure_menu = self.menuBar().addMenu("测量(&M)")
        tools_menu = self.menuBar().addMenu("工具(&T)")
        stack_action = tools_menu.addAction("异质结叠层设计…")
        stack_action.triggered.connect(self.open_stack_designer)
        toolbar = QToolBar("图像工具", self)
        toolbar.setObjectName("microscope-image-tools")
        toolbar.setMovable(False)
        self.addToolBar(toolbar)
        for text, callback, shortcut in [
            ("打开图像…", self.open_image, "Ctrl+O"),
            ("导出标记图…", self.export_image, "Ctrl+Shift+S"),
            ("导出测量表…", self.export_measurements, ""),
        ]:
            action = QAction(text, self)
            action.triggered.connect(callback)
            if shortcut:
                action.setShortcut(shortcut)
            file_menu.addAction(action)
            toolbar.addAction(action)
        toolbar.addSeparator()
        for text, callback in [("适合窗口", self.view.fit_content), ("1:1", self.actual_size)]:
            action = QAction(text, self)
            action.triggered.connect(callback)
            toolbar.addAction(action)
            view_menu.addAction(action)
        view_menu.addAction(self.results_dock.toggleViewAction())
        toolbar.addSeparator()
        calibration = QAction("设置标定…", self)
        calibration.triggered.connect(self.calibrate)
        toolbar.addAction(calibration)
        measure_menu.addAction(calibration)
        reset = measure_menu.addAction("清除标定（恢复像素）")
        reset.triggered.connect(lambda: self.set_calibration(None))
        self.calibration_label = QLabel("  未标定 · px  ")
        self.calibration_label.setToolTip("标定比例会记忆并沿用到新图像；更换物镜、倍率或像素合并设置后请重新标定。")
        self.calibration_label.setStyleSheet("color: #9b641d; font-weight: 600;")
        toolbar.addWidget(self.calibration_label)
        tools = QToolBar("标记工具", self)
        tools.setObjectName("microscope-annotation-tools")
        tools.setMovable(False)
        tools.setToolButtonStyle(Qt.ToolButtonStyle.ToolButtonTextUnderIcon)
        tools.setIconSize(QSize(24, 24))
        self.addToolBar(Qt.ToolBarArea.LeftToolBarArea, tools)
        self.tool_group = QActionGroup(self)
        self.tool_actions = {}
        for tool, name in TOOL_NAMES.items():
            action = QAction(tool_icon(tool), name, self)
            action.setCheckable(True)
            action.setToolTip(TOOL_HINTS[tool])
            action.triggered.connect(lambda checked, t=tool: self.set_tool(t))
            self.tool_group.addAction(action)
            tools.addAction(action)
            measure_menu.addAction(action)
            self.tool_actions[tool] = action
        measure_menu.addSeparator()
        delete = measure_menu.addAction("删除选中标记")
        delete.triggered.connect(self.delete_selected)
        # Scope Delete to the canvas so editing a name never deletes the object.
        delete_key = QAction(self.view)
        delete_key.setShortcut("Delete")
        delete_key.setShortcutContext(Qt.ShortcutContext.WidgetWithChildrenShortcut)
        delete_key.triggered.connect(self.delete_selected)
        self.view.addAction(delete_key)
        measure_menu.addAction("清空标记…").triggered.connect(self.clear_annotations)

    def build_edge_detection_menu(self, edit_menu):
        self.edge_parameters_menu = edit_menu.addMenu("边缘识别参数")
        self.edge_parameters_menu.setObjectName("edge-detection-parameters")
        panel = QWidget(self.edge_parameters_menu)
        panel.setMinimumWidth(370)
        layout = QVBoxLayout(panel)
        layout.setContentsMargins(14, 12, 14, 10)
        form = QFormLayout()
        form.setHorizontalSpacing(16)
        form.setVerticalSpacing(10)
        self.edge_resolution_box = QComboBox(panel)
        for size in EDGE_PROCESSING_SIZES:
            self.edge_resolution_box.addItem(f"最长边 {size} px", size)
        self.edge_resolution_box.setToolTip("保持原图比例，仅缩小用于识别的图像；相机采集尺寸不变。")
        form.addRow("处理分辨率", self.edge_resolution_box)

        self.edge_threshold_box = QDoubleSpinBox(panel)
        self.edge_threshold_box.setRange(0, 255)
        self.edge_threshold_box.setDecimals(0)
        self.edge_threshold_box.setSpecialValueText("自动（Otsu）")
        self.edge_threshold_box.setToolTip("0 为自动；1–255 为与基底的色差阈值。降低数值可保留弱对比区域，也可能增加粘连。")
        form.addRow("色差阈值", self.edge_threshold_box)

        self.edge_min_area_box = QDoubleSpinBox(panel)
        self.edge_min_area_box.setRange(0.001, 50)
        self.edge_min_area_box.setDecimals(3)
        self.edge_min_area_box.setSingleStep(0.001)
        self.edge_min_area_box.setSuffix(" %")
        self.edge_min_area_box.setToolTip("占整幅图像面积的百分比；降低数值可保留更小的候选区域。")
        form.addRow("最小面积（画面占比）", self.edge_min_area_box)

        self.edge_simplification_box = QDoubleSpinBox(panel)
        self.edge_simplification_box.setRange(0.01, 5)
        self.edge_simplification_box.setDecimals(2)
        self.edge_simplification_box.setSingleStep(0.1)
        self.edge_simplification_box.setSuffix(" %")
        self.edge_simplification_box.setToolTip("按轮廓周长计算简化容差；越小保留越多边界细节。")
        form.addRow("轮廓简化（周长占比）", self.edge_simplification_box)
        self._edge_parameter_boxes = (self.edge_threshold_box, self.edge_min_area_box,
                                     self.edge_simplification_box)
        for box in self._edge_parameter_boxes:
            box.setKeyboardTracking(False)
            box.setMinimumWidth(155)
        layout.addLayout(form)
        note = QLabel("处理分辨率不改变原图及相机采集尺寸。", panel)
        note.setObjectName("muted")
        layout.addWidget(note)
        action = QWidgetAction(self.edge_parameters_menu)
        action.setDefaultWidget(panel)
        self.edge_parameters_menu.addAction(action)
        self.edge_parameters_menu.addSeparator()
        self.edge_reset_action = self.edge_parameters_menu.addAction("恢复默认值")
        self.edge_reset_action.triggered.connect(
            lambda: self.set_edge_detection_parameters(DEFAULT_EDGE_DETECTION_PARAMETERS))
        self.edge_parameters_menu.aboutToHide.connect(self.commit_edge_parameter_edits)
        self.set_edge_detection_parameters(DEFAULT_EDGE_DETECTION_PARAMETERS)

    def commit_edge_parameter_edits(self):
        for box in self._edge_parameter_boxes:
            box.interpretText()

    def edge_detection_parameters(self):
        """Detached configuration snapshot for a future processing worker.

        Read on the GUI thread and pass the snapshot to a worker; never let a
        worker read Qt widgets. Changing preferences does not start processing.
        """
        return validate_edge_detection_parameters({
            "max_dimension": self.edge_resolution_box.currentData(),
            "threshold": int(self.edge_threshold_box.value()),
            "min_area_percent": self.edge_min_area_box.value(),
            "simplification": self.edge_simplification_box.value(),
        })

    def set_edge_detection_parameters(self, parameters):
        parameters = validate_edge_detection_parameters(parameters)
        self.edge_resolution_box.setCurrentIndex(
            self.edge_resolution_box.findData(parameters["max_dimension"]))
        self.edge_threshold_box.setValue(parameters["threshold"])
        self.edge_min_area_box.setValue(parameters["min_area_percent"])
        self.edge_simplification_box.setValue(parameters["simplification"])

    def set_tool(self, tool):
        self.view.set_tool(tool)
        if tool in self.tool_actions:
            self.tool_actions[tool].setChecked(True)
        else:
            self.tool_group.setExclusive(False)
            for action in self.tool_actions.values():
                action.setChecked(False)
            self.tool_group.setExclusive(True)
        if tool == "profile":
            self.results_dock.show()
            self.result_tabs.setCurrentIndex(1)
            self.resizeDocks([self.results_dock], [260], Qt.Orientation.Vertical)
        self.view.setFocus()
        self.statusBar().showMessage(TOOL_HINTS[tool])

    def actual_size(self):
        self.view.resetTransform()
        self.view.zoom_changed.emit(1.0)

    def update_cursor(self, point):
        if self.view.image_rect.contains(point):
            self.cursor_label.setText(f"X {point.x():.1f}   Y {point.y():.1f} px")
        else:
            self.cursor_label.setText("X —   Y —")

    def open_image(self):
        if self.camera_state not in ("idle", "disconnected"):
            QMessageBox.information(self, "相机正在工作", "请先停止预览 / 拍摄，再打开本地图像。")
            return
        path, _ = QFileDialog.getOpenFileName(self, "打开显微图像", "", "图像 (*.png *.jpg *.jpeg *.bmp *.tif *.tiff)")
        if not path:
            return
        reader = QImageReader(path)
        reader.setAutoTransform(True)
        image = reader.read()
        if image.isNull():
            QMessageBox.warning(self, "打开失败", reader.errorString())
            return
        if self.annotations and QMessageBox.question(
                self, "更换图像", "更换图像将清除当前标记，沿用标定与白平衡。是否继续？",
                QMessageBox.StandardButton.Yes | QMessageBox.StandardButton.No,
                QMessageBox.StandardButton.No) != QMessageBox.StandardButton.Yes:
            return
        self.load_image(image, Path(path).name)

    def load_image(self, image, name):
        if image.isNull():
            raise ValueError("图像为空。")
        self.image_state.clear()
        self._camera_source_key = None
        self.camera_frame_metadata = None
        self.camera_save.setEnabled(False)
        self.view.cancel_drawing()
        self.scene.blockSignals(True)
        self.annotations.clear()
        self.scene.clear()
        self.scene.blockSignals(False)
        self.image = image.copy()
        self.image.setDevicePixelRatio(1)
        self.source_rgb = image_rgb8(self.image)
        self.balanced_rgb = self.source_rgb.copy()
        self.display_image = self.image.copy()
        self.wb_group.setEnabled(True)
        self.image_item = self.scene.addPixmap(QPixmap.fromImage(self.image))
        self.image_item.setZValue(-100)
        self.image_item.setAcceptedMouseButtons(Qt.MouseButton.NoButton)
        self.set_white_balance(self.wb_gains)
        self.view.image_rect = QRectF(0, 0, image.width(), image.height())
        self.scene.setSceneRect(self.view.image_rect)
        self.image_title.setText(f"  {name}    |    {image.width()} × {image.height()} px")
        self.image_info.setText(f"尺寸  {image.width()} × {image.height()} px\n来源  本地图像\n帧率  —")
        self._next_id = 1
        self.refresh_measurements()
        self.sync_selection()
        self.view.fit_content()

    def add_annotation(self, kind, points):
        if self.image.isNull():
            raise ValueError("请先打开图像。")
        if kind not in ("line", "polygon", "profile") or len(points) < (3 if kind == "polygon" else 2):
            raise ValueError("标记类型或节点数量无效。")
        name = f"{ {'line': '长度', 'polygon': '轮廓', 'profile': 'RGB 路径'}[kind]} {self._next_id:02d}"
        item = AnnotationItem([(p.x(), p.y()) for p in points], kind, name, self.refresh_measurements)
        style = self.annotation_defaults
        item.outline_color = QColor(style["color"])
        item.fill_color = QColor(style["color"])
        item.line_width = style["width"]
        item.line_style = Qt.PenStyle[style["style"]]
        item.fill_alpha = int(style["fill"]) if kind == "polygon" else 0
        item.apply_style()
        item.setZValue(self._next_id)
        self._next_id += 1
        self.annotations.append(item)
        self.scene.addItem(item)
        self.scene.clearSelection()
        item.setSelected(True)
        self.refresh_measurements()
        if kind == "profile":
            self.results_dock.show()
            self.result_tabs.setCurrentIndex(1)
        return item

    def selected_annotation(self):
        return next((item for item in self.annotations if item.isSelected()), None)

    def sync_selection(self):
        if not hasattr(self, "inspector"):
            return
        item = self.selected_annotation()
        self._syncing = True
        self.inspector.setEnabled(item is not None)
        self.name_edit.setText(item.name if item else "")
        if item:
            self.width_box.setValue(item.line_width)
            self.rotation_box.setValue(item.rotation())
            self.style_box.setCurrentText(next(name for name, value in LINE_STYLES.items() if value == item.line_style))
            self.fill_slider.setValue(round(item.fill_alpha / 255 * 100))
            self.fill_slider.setEnabled(item.kind == "polygon")
            self.color_button.setStyleSheet(f"border-left: 14px solid {item.outline_color.name()};")
        self._syncing = False
        self.toggle_nodes()
        self.table.blockSignals(True)
        self.table.clearSelection()
        if item:
            self.table.selectRow(self.annotations.index(item))
        self.table.blockSignals(False)
        self.update_rgb_profile()

    def toggle_nodes(self):
        for item in self.annotations:
            item.edit_vertices = self.edit_nodes.isChecked()
            item.update_handles()

    def apply_properties(self, *_):
        item = self.selected_annotation()
        if self._syncing or item is None:
            return
        item.name = self.name_edit.text().strip() or item.name
        item.line_width = self.width_box.value()
        item.line_style = LINE_STYLES[self.style_box.currentText()]
        item.fill_alpha = round(self.fill_slider.value() / 100 * 255) if item.kind == "polygon" else 0
        item.setRotation(self.rotation_box.value())
        item.apply_style()
        self.annotation_defaults.update(width=item.line_width, style=item.line_style.name)
        if item.kind == "polygon":
            self.annotation_defaults["fill"] = item.fill_alpha
        self.refresh_measurements()

    def choose_color(self):
        item = self.selected_annotation()
        if item is None:
            return
        color = QColorDialog.getColor(item.outline_color, self, "标记颜色")
        if color.isValid():
            item.outline_color = color
            item.fill_color = color
            self.annotation_defaults["color"] = color.name()
            item.apply_style()
            self.refresh_measurements()
            self.sync_selection()

    def set_calibration(self, microns_per_pixel):
        if microns_per_pixel is not None and (not isfinite(microns_per_pixel) or microns_per_pixel <= 0):
            raise ValueError("标定比例必须为有限正数。")
        self.microns_per_pixel = microns_per_pixel
        self.calibration_label.setText("  未标定 · px  " if microns_per_pixel is None else
                                       f"  {microns_per_pixel:g} µm / px  ")
        self.refresh_measurements()

    def set_white_balance(self, gains):
        if self.source_rgb is None:
            raise ValueError("请先打开图像。")
        # Identity white balance needs no per-pixel allocation on every live frame.
        balanced = (self.source_rgb if np.array_equal(np.asarray(gains), np.ones(3))
                    else balance_rgb8(self.source_rgb, gains))
        self.wb_gains = np.asarray(gains, dtype=float).copy()
        self.balanced_rgb = balanced
        if np.array_equal(self.wb_gains, np.ones(3)):
            self.display_image = self.image.copy()
        else:
            height, width, _ = balanced.shape
            self.display_image = QImage(balanced.data, width, height, balanced.strides[0],
                                        QImage.Format.Format_RGB888).copy()
        self.image_item.setPixmap(QPixmap.fromImage(self.display_image))
        for box, gain in zip(self.wb_boxes, self.wb_gains):
            box.blockSignals(True)
            box.setValue(float(gain))
            box.blockSignals(False)
        self.update_rgb_profile()

    def manual_white_balance(self, *_):
        self.set_white_balance([box.value() for box in self.wb_boxes])

    def auto_white_balance(self):
        if self.source_rgb is None:
            return
        try:
            self.set_white_balance(white_balance_gains(self.source_rgb))
        except ValueError as exc:
            QMessageBox.warning(self, "无法自动白平衡", str(exc))

    def pick_white_balance(self, point):
        if self.source_rgb is None:
            return
        height, width, _ = self.source_rgb.shape
        x, y = min(width - 1, max(0, round(point.x()))), min(height - 1, max(0, round(point.y())))
        patch = self.source_rgb[max(0, y - 4):min(height, y + 5), max(0, x - 4):min(width, x + 5)]
        try:
            self.set_white_balance(white_balance_gains(patch))
        except ValueError as exc:
            QMessageBox.warning(self, "无法校正白平衡", str(exc))
        self.set_tool("select")

    def show_rgb_channel(self, channel, visible):
        self.profile_plot.channels[channel] = visible
        self.profile_plot.update()

    def update_rgb_profile(self, *_):
        if not hasattr(self, "profile_plot"):
            return
        item = self.selected_annotation()
        self.profile_data = None
        self.profile_export.setEnabled(False)
        self.profile_label.setText(item.name if item is not None else "未选择路径")
        message = "选择测量线，或用 RGB 路径工具绘制折线。"
        if self.source_rgb is not None and item is not None and item.kind in ("line", "profile"):
            points = [(point.x(), point.y()) for point in item.mapToScene(item.polygon())]
            rgb = self.source_rgb if self.profile_source.currentIndex() == 0 else self.balanced_rgb
            try:
                distance, coordinates, values = sample_rgb_path(rgb, points)
            except ValueError as exc:
                message = str(exc)
            else:
                self.profile_data = (distance, coordinates, values)
                scale = self.microns_per_pixel or 1.0
                unit = "px" if self.microns_per_pixel is None else "µm"
                self.profile_plot.set_profile(distance * scale, values, unit)
                self.profile_export.setEnabled(True)
                return
        self.profile_plot.set_profile(np.empty(0), np.empty((0, 3)), "px", message)

    def export_rgb_profile(self):
        if self.profile_data is None:
            QMessageBox.information(self, "导出曲线", "请先选择图像范围内的有效测量线或 RGB 路径。")
            return
        path, _ = QFileDialog.getSaveFileName(self, "导出 RGB 曲线", "RGB路径.csv", "CSV 表格 (*.csv)")
        if not path:
            return
        distance, coordinates, values = self.profile_data
        corrected = self.profile_source.currentIndex() == 1
        gains = self.wb_gains if corrected else np.ones(3)
        try:
            with open(path, "w", encoding="utf-8-sig", newline="") as stream:
                writer = csv.writer(stream)
                writer.writerow(["distance_px", "distance_um", "x_px", "y_px", "R", "G", "B",
                                 "source", "gain_R", "gain_G", "gain_B"])
                for d, (x, y), (r, g, b) in zip(distance, coordinates, values):
                    writer.writerow([d, "" if self.microns_per_pixel is None else d * self.microns_per_pixel,
                                     x, y, r, g, b, "white_balanced_rgb8" if corrected else "original_rgb8", *gains])
        except OSError as exc:
            QMessageBox.warning(self, "导出失败", str(exc))

    def calibrate(self):
        if self.image.isNull():
            QMessageBox.information(self, "标定", "请先打开图像，再设置该图像的标定比例。")
            return
        value, accepted = QInputDialog.getDouble(
            self, "设置图像标定", "每个原图像素对应的长度（µm/px）：\n请使用该倍率下实测的标定值。",
            self.microns_per_pixel or 1.0, 0.000001, 100000, 6)
        if accepted:
            self.set_calibration(value)

    def measurement_rows(self):
        unit = "px" if self.microns_per_pixel is None else "µm"
        scale = 1.0 if self.microns_per_pixel is None else self.microns_per_pixel
        rows = []
        for item in self.annotations:
            length, area = item.measurements(scale)
            rows.append([item.name, {"line": "长度", "polygon": "轮廓", "profile": "RGB 路径"}[item.kind],
                         f"{length:.3f} {unit}", "—" if item.kind != "polygon" else
                         "自交，无法计算" if area is None else f"{area:.3f} {unit}²"])
        return rows

    def refresh_measurements(self):
        if not hasattr(self, "table"):
            return
        self.table.blockSignals(True)
        self.table.setRowCount(len(self.annotations))
        for row, values in enumerate(self.measurement_rows()):
            for column, value in enumerate(values):
                self.table.setItem(row, column, QTableWidgetItem(value))
        selected = self.selected_annotation()
        if selected:
            self.table.selectRow(self.annotations.index(selected))
            self.rotation_box.blockSignals(True)
            self.rotation_box.setValue(selected.rotation())
            self.rotation_box.blockSignals(False)
        self.table.blockSignals(False)
        scale = self.microns_per_pixel or 1.0
        unit = "px" if self.microns_per_pixel is None else "µm"
        for item in self.annotations:
            item.update_caption(scale, unit)
        self.results_dock.setWindowTitle(f"测量结果  ·  {len(self.annotations)} 个标记")
        self.update_rgb_profile()

    def select_table_item(self):
        row = self.table.currentRow()
        if 0 <= row < len(self.annotations):
            self.scene.clearSelection()
            self.annotations[row].setSelected(True)

    def delete_selected(self):
        for item in list(self.annotations):
            if item.isSelected():
                self.annotations.remove(item)
                self.scene.removeItem(item)
                item.changed = None
        self.refresh_measurements()
        self.sync_selection()

    def clear_annotations(self):
        if self.annotations and QMessageBox.question(
                self, "清空标记", "清除本图像的全部标记？", QMessageBox.StandardButton.Yes |
                QMessageBox.StandardButton.No, QMessageBox.StandardButton.No) == QMessageBox.StandardButton.Yes:
            for item in list(self.annotations):
                self.scene.removeItem(item)
                item.changed = None
            self.annotations.clear()
            self.refresh_measurements()
            self.sync_selection()

    def render_annotated_image(self):
        if self.image.isNull():
            raise ValueError("请先打开图像。")
        # Render separately from the viewport: zoom and selection handles never
        # affect the exported dimensions, geometry or labels.
        output = self.display_image.convertToFormat(QImage.Format.Format_RGB32)
        painter = QPainter(output)
        painter.setRenderHint(QPainter.RenderHint.Antialiasing)
        for item in self.annotations:
            painter.setPen(item.pen())
            painter.setBrush(item.brush())
            polygon = item.mapToScene(item.polygon())
            if item.kind != "polygon":
                painter.drawPolyline(polygon)
            else:
                painter.drawPolygon(polygon)
            painter.drawText(item.mapToScene(item.caption.pos()), item.caption.text())
        painter.end()
        return output

    def export_image(self):
        if self.image.isNull():
            QMessageBox.information(self, "导出标记图", "请先打开图像。")
            return
        path, _ = QFileDialog.getSaveFileName(self, "导出标记图", "标记图.png", "PNG 图像 (*.png)")
        if path and not self.render_annotated_image().save(path, "PNG"):
            QMessageBox.warning(self, "导出失败", "无法写入图像，请检查路径与权限。")

    def export_measurements(self):
        if not self.annotations:
            QMessageBox.information(self, "导出测量表", "当前没有测量标记。")
            return
        path, _ = QFileDialog.getSaveFileName(self, "导出测量表", "测量.csv", "CSV 表格 (*.csv)")
        if not path:
            return
        try:
            with open(path, "w", encoding="utf-8-sig", newline="") as stream:
                writer = csv.writer(stream)
                writer.writerow(["标记名称", "类型", "长度 / 周长", "面积"])
                writer.writerows(self.measurement_rows())
        except OSError as exc:
            QMessageBox.warning(self, "导出失败", str(exc))


if __name__ == "__main__":
    # Compatibility command; application composition belongs to microscope_app.
    from microscope_app import main
    raise SystemExit(main())
