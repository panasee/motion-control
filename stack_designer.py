"""Photo contour editing and a hardware-independent heterostructure whiteboard."""

from pathlib import Path
from math import ceil, isfinite
from sys import float_info
import json
import base64
import binascii

import cv2
import numpy as np
from PyQt6.QtCore import QPointF, QRectF, QRect, Qt, QThread, QTimer, QBuffer, QSaveFile, QIODevice, pyqtSignal, pyqtSlot
from PyQt6.QtGui import QColor, QBrush, QImage, QImageReader, QPainter, QPen, QPixmap, QPolygonF
from PyQt6.QtWidgets import (
    QApplication, QCheckBox, QColorDialog, QComboBox, QDoubleSpinBox, QFileDialog,
    QFormLayout, QGraphicsEllipseItem, QGraphicsItem, QGraphicsPolygonItem,
    QGraphicsScene, QGraphicsView, QGraphicsPixmapItem, QGroupBox, QHBoxLayout, QLabel, QLineEdit,
    QListWidget, QListWidgetItem, QMainWindow, QMessageBox, QPushButton,
    QSplitter, QTabWidget, QVBoxLayout, QWidget,
)
from photo_selection import PhotoSelectionController


from imaging_canvas import CanvasView, EditablePolygonItem as FlakeItem, LINE_STYLES


def validate_project(data):
    """Validate the complete versioned document before changing the whiteboard."""
    if not isinstance(data, dict) or data.get("format") != "hstack":
        raise ValueError("不是有效的白板工程文件。")
    if type(data.get("version")) is not int or data["version"] not in (1, 2):
        raise ValueError("不支持此工程文件的格式版本。")
    if not isinstance(data.get("layers"), list):
        raise ValueError("工程文件缺少有效的图层列表。")

    def number(value, label, minimum=None, maximum=None):
        if type(value) not in (int, float) or not -float_info.max <= value <= float_info.max:
            raise ValueError(f"{label} 必须是有限数值。")
        if (minimum is not None and value < minimum) or (maximum is not None and value > maximum):
            raise ValueError(f"{label} 超出支持的范围。")

    def point(value, label):
        if not isinstance(value, list) or len(value) != 2:
            raise ValueError(f"{label} 必须包含两个坐标。")
        for component in value:
            number(component, label)

    required = {"name", "vertices", "position", "transform_origin", "rotation", "scale",
                "outline_color", "fill_color", "fill_alpha", "line_width", "line_style"}
    for index, layer in enumerate(data["layers"], 1):
        label = f"图层 {index}"
        if not isinstance(layer, dict) or not required.issubset(layer):
            raise ValueError(f"{label} 缺少必要的多边形或样式信息。")
        if not isinstance(layer["name"], str) or not layer["name"].strip():
            raise ValueError(f"{label} 名称不能为空。")
        vertices = layer["vertices"]
        if not isinstance(vertices, list) or len(vertices) < 3:
            raise ValueError(f"{label} 至少需要三个节点。")
        for vertex in vertices:
            point(vertex, f"{label} 节点")
        point(layer["position"], f"{label} 位置")
        point(layer["transform_origin"], f"{label} 变换中心")
        number(layer["rotation"], f"{label} 旋转角度", -360, 360)
        number(layer["scale"], f"{label} 尺寸倍率", 0.001, 1000)
        number(layer["line_width"], f"{label} 线宽", 0.5, 20)
        if type(layer["fill_alpha"]) is not int or not 0 <= layer["fill_alpha"] <= 255:
            raise ValueError(f"{label} 填充透明度必须是 0–255 的整数。")
        for field in ("outline_color", "fill_color"):
            color = layer[field]
            if not isinstance(color, str) or not QColor(color).isValid():
                raise ValueError(f"{label} 的 {field} 颜色无效。")
        if layer["line_style"] not in [style.name for style in LINE_STYLES.values()]:
            raise ValueError(f"{label} 线形无效。")

    # Version 1 contains polygon layers only; version 2 explicitly includes a
    # nullable background. This is a format migration, not a decode fallback.
    if data["version"] == 2:
        if "background" not in data:
            raise ValueError("工程文件缺少背景信息。")
        background = data["background"]
        if background is not None:
            fields = {"image_png_base64", "position", "rotation", "scale", "opacity"}
            if not isinstance(background, dict) or not fields.issubset(background):
                raise ValueError("背景照片信息不完整。")
            if not isinstance(background["image_png_base64"], str):
                raise ValueError("背景照片编码无效。")
            point(background["position"], "背景位置")
            number(background["rotation"], "背景旋转角度", -360, 360)
            number(background["scale"], "背景尺寸倍率", 0.001, 1000)
            number(background["opacity"], "背景不透明度", 0, 1)


class ScreenCaptureOverlay(QWidget):
    """Select a region on one frozen screen; crop using native pixel ratios."""

    captured = pyqtSignal(QImage)
    cancelled = pyqtSignal()

    def __init__(self, pixmap, geometry):
        super().__init__(None, Qt.WindowType.Tool | Qt.WindowType.FramelessWindowHint |
                         Qt.WindowType.WindowStaysOnTopHint)
        self.pixmap = pixmap
        self.start = None
        self.selection = QRect()
        self.setGeometry(geometry)
        self.setCursor(Qt.CursorShape.CrossCursor)
        self.setMouseTracking(True)
        self.setFocusPolicy(Qt.FocusPolicy.StrongFocus)

    def crop_selection(self, rectangle):
        rectangle = rectangle.normalized().intersected(self.rect())
        # QScreen geometry uses logical pixels, grabWindow returns native pixels.
        sx, sy = self.pixmap.width() / self.width(), self.pixmap.height() / self.height()
        left, top = round(rectangle.x() * sx), round(rectangle.y() * sy)
        right = round((rectangle.x() + rectangle.width()) * sx)
        bottom = round((rectangle.y() + rectangle.height()) * sy)
        image = self.pixmap.toImage().copy(QRect(left, top, right - left, bottom - top))
        image.setDevicePixelRatio(1)
        return image

    def paintEvent(self, event):
        painter = QPainter(self)
        painter.drawPixmap(self.rect(), self.pixmap)
        region = self.selection.normalized().intersected(self.rect())
        shade = QColor(0, 0, 0, 100)
        if region.isEmpty():
            painter.fillRect(self.rect(), shade)
        else:
            painter.fillRect(QRect(0, 0, self.width(), region.top()), shade)
            painter.fillRect(QRect(0, region.y() + region.height(), self.width(), self.height() - region.y() - region.height()), shade)
            painter.fillRect(QRect(0, region.top(), region.left(), region.height()), shade)
            painter.fillRect(QRect(region.x() + region.width(), region.top(), self.width() - region.x() - region.width(), region.height()), shade)
            painter.setPen(QPen(QColor("#ffcc33"), 2))
            painter.drawRect(region)
        painter.setPen(QColor("white"))
        painter.drawText(QRect(20, 20, self.width() - 40, 45), Qt.AlignmentFlag.AlignLeft,
                         "拖动框选本屏幕区域并松开鼠标导入；Esc 取消")

    def mousePressEvent(self, event):
        if event.button() == Qt.MouseButton.LeftButton:
            self.start = event.position().toPoint()
            self.selection = QRect(self.start, self.start)
            self.update()

    def mouseMoveEvent(self, event):
        if self.start is not None:
            self.selection = QRectF(QPointF(self.start), event.position()).normalized().toAlignedRect().intersected(self.rect())
            self.update()

    def mouseReleaseEvent(self, event):
        if event.button() == Qt.MouseButton.LeftButton and self.start is not None:
            self.selection = QRectF(QPointF(self.start), event.position()).normalized().toAlignedRect().intersected(self.rect())
            self.start = None
            if self.selection.width() >= 4 and self.selection.height() >= 4:
                self.captured.emit(self.crop_selection(self.selection))
            self.update()

    def keyPressEvent(self, event):
        if event.key() == Qt.Key.Key_Escape:
            self.cancelled.emit()
        else:
            super().keyPressEvent(event)

    def closeEvent(self, event):
        self.cancelled.emit()
        event.accept()


class BackgroundItem(QGraphicsPixmapItem):
    """One reference photo, always behind polygons and locked by default."""

    def __init__(self, image):
        self.image = image.copy()
        self.image.setDevicePixelRatio(1)
        super().__init__(QPixmap.fromImage(self.image))
        self.setOffset(-self.image.width() / 2, -self.image.height() / 2)
        self.setZValue(-1)
        self.setTransformationMode(Qt.TransformationMode.SmoothTransformation)
        self.setShapeMode(QGraphicsPixmapItem.ShapeMode.BoundingRectShape)
        self.setFlag(QGraphicsItem.GraphicsItemFlag.ItemSendsGeometryChanges)
        self.set_editable(False)

    def set_editable(self, enabled):
        self.setFlag(QGraphicsItem.GraphicsItemFlag.ItemIsMovable, enabled)
        self.setAcceptedMouseButtons(Qt.MouseButton.LeftButton if enabled else Qt.MouseButton.NoButton)
        self.setCursor(Qt.CursorShape.SizeAllCursor if enabled else Qt.CursorShape.ArrowCursor)

    def itemChange(self, change, value):
        if change in (QGraphicsItem.GraphicsItemChange.ItemPositionHasChanged,
                      QGraphicsItem.GraphicsItemChange.ItemScaleHasChanged,
                      QGraphicsItem.GraphicsItemChange.ItemRotationHasChanged) and self.scene() is not None:
            self.scene().setSceneRect(self.scene().sceneRect().united(
                self.sceneBoundingRect().adjusted(-500, -500, 500, 500)))
        return super().itemChange(change, value)


def extract_contours(rgb, threshold=0, min_area_percent=0.1, simplification=0.4):
    """Return outer polygons in input pixel coordinates, largest first.

    The median image-border Lab color estimates the substrate. Otsu thresholds
    color distance (not brightness), or the caller supplies a distance threshold.
    This assumes reasonably uniform substrate visible at the photo border.
    Holes are intentionally excluded: each flake is one editable outer polygon.
    """
    if rgb.ndim != 3 or rgb.shape[2] != 3 or rgb.dtype != np.uint8:
        raise ValueError("需要 uint8 RGB 照片。")
    height, width = rgb.shape[:2]
    if min(height, width) < 8:
        raise ValueError("照片尺寸过小，无法识别轮廓。")
    if not (0 <= threshold <= 255 and 0 < min_area_percent <= 100 and 0 < simplification <= 5):
        raise ValueError("轮廓提取参数超出范围。")
    ratio = min(1.0, 1600 / max(height, width))
    small = cv2.resize(rgb, (round(width * ratio), round(height * ratio)), interpolation=cv2.INTER_AREA)
    lab = cv2.cvtColor(small, cv2.COLOR_RGB2LAB).astype(np.float32)
    border = np.concatenate((lab[0], lab[-1], lab[:, 0], lab[:, -1]))
    background = np.median(border, axis=0)
    distance = np.clip(np.linalg.norm(lab - background, axis=2), 0, 255).astype(np.uint8)
    distance = cv2.GaussianBlur(distance, (5, 5), 0)
    mode = cv2.THRESH_BINARY | (cv2.THRESH_OTSU if threshold == 0 else 0)
    _, mask = cv2.threshold(distance, threshold, 255, mode)
    contours, _ = cv2.findContours(mask, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
    image_area = small.shape[0] * small.shape[1]
    polygons = []
    for contour in sorted(contours, key=cv2.contourArea, reverse=True):
        area = cv2.contourArea(contour)
        if area < image_area * min_area_percent / 100 or area > image_area * 0.95:
            continue
        polygon = cv2.approxPolyDP(contour, simplification / 100 * cv2.arcLength(contour, True), True)
        if len(polygon) >= 3:
            points = polygon.reshape(-1, 2).astype(float)
            points *= [width / small.shape[1], height / small.shape[0]]
            polygons.append(points.tolist())
    if not polygons:
        raise ValueError("未识别到有效 flake。请调整色差阈值、最小面积，或裁剪照片使基底在边缘可见。")
    return polygons


class ContourWorker(QThread):
    extracted = pyqtSignal(object)
    failed = pyqtSignal(str)

    def __init__(self, image, parameters, parent=None):
        super().__init__(parent)
        self.image = image
        self.parameters = parameters

    def run(self):
        # Bound processing memory, then restore original photo coordinates.
        image = self.image
        if max(image.width(), image.height()) > 1600:
            image = image.scaled(1600, 1600, Qt.AspectRatioMode.KeepAspectRatio,
                                 Qt.TransformationMode.SmoothTransformation)
        image = image.convertToFormat(QImage.Format.Format_RGB888)
        ptr = image.constBits()
        ptr.setsize(image.sizeInBytes())
        rgb = np.frombuffer(ptr, np.uint8).reshape(image.height(), image.bytesPerLine())
        rgb = rgb[:, :image.width() * 3].reshape(image.height(), image.width(), 3)
        try:
            polygons = extract_contours(rgb, **self.parameters)
        except (ValueError, cv2.error) as exc:
            self.failed.emit(str(exc))
            return
        scale = np.array([self.image.width() / image.width(), self.image.height() / image.height()])
        self.extracted.emit([(np.asarray(points) * scale).tolist() for points in polygons])



def number_box(minimum, maximum, value, step=1, decimals=2):
    box = QDoubleSpinBox()
    box.setRange(minimum, maximum)
    box.setDecimals(decimals)
    box.setSingleStep(step)
    box.setValue(value)
    return box


def button(text, callback, layout):
    widget = QPushButton(text)
    widget.clicked.connect(callback)
    layout.addWidget(widget)
    return widget


class StackDesignerWindow(QMainWindow):
    def __init__(self, parent=None):
        super().__init__(parent, Qt.WindowType.Window)
        self.setWindowTitle("异质结堆叠设计")
        self.resize(1380, 850)
        self.image = None
        self.photo_path = None
        self.worker = None
        self.capture_windows = []
        self.capture_active = False
        self.capture_hidden_windows = []
        self.capture_timer = QTimer(self)
        self.capture_timer.setSingleShot(True)
        self.capture_timer.timeout.connect(self.capture_screens)
        self.candidates = []
        self.layers = []  # Bottom to top; z-values always match this order.
        self.background = None
        self.project_path = None
        self.active_item = None
        self._syncing = False
        self.photo_scene = QGraphicsScene(self)
        self.board_scene = QGraphicsScene(self)
        self.board_scene.setSceneRect(-5000, -5000, 10000, 10000)
        self.photo_view = CanvasView(self.photo_scene)
        self.photo_selection = PhotoSelectionController(self.photo_view, self)
        self.board_view = CanvasView(self.board_scene)
        self.board_view.centerOn(0, 0)

        splitter = QSplitter()
        photo_panel = QWidget()
        photo_layout = QVBoxLayout(photo_panel)
        photo_layout.addWidget(QLabel("1 · 照片与轮廓"))
        photo_buttons = QHBoxLayout()
        self.import_button = button("导入 flake 照片…", self.import_photo, photo_buttons)
        button("适合照片", self.photo_view.fit_content, photo_buttons)
        photo_layout.addLayout(photo_buttons)
        capture_buttons = QHBoxLayout()
        self.capture_button = button("截图导入…", self.start_screen_capture, capture_buttons)
        self.paste_button = button("粘贴图片", self.paste_photo, capture_buttons)
        photo_layout.addLayout(capture_buttons)
        self.photo_tabs = QTabWidget()
        auto_panel = QWidget()
        auto_layout = QVBoxLayout(auto_panel)
        form = QFormLayout()
        self.threshold = number_box(0, 255, 0, decimals=0)
        self.threshold.setSpecialValueText("自动")
        self.threshold.setToolTip("与照片边缘基底颜色的差异；0 为自动，较小值可保留弱对比区域。")
        self.min_area = number_box(0.001, 50, 0.1, 0.05, 3)
        self.simplification = number_box(0.01, 5, 0.4, 0.1)
        form.addRow("色差阈值", self.threshold)
        form.addRow("最小面积（照片 %）", self.min_area)
        form.addRow("简化程度（周长 %）", self.simplification)
        auto_layout.addLayout(form)
        self.extract_button = button("重新识别（替换照片中的轮廓编辑）", self.start_extraction, auto_layout)
        self.extract_button.setEnabled(False)
        self.photo_tabs.addTab(auto_panel, "自动识别")
        self.manual_panel = self.create_selection_controls()
        self.photo_tabs.addTab(self.manual_panel, "手动选区")
        photo_layout.addWidget(self.photo_tabs)
        self.candidate_combo = QComboBox()
        self.candidate_combo.currentIndexChanged.connect(self.select_candidate)
        photo_layout.addWidget(self.candidate_combo)
        self.edit_photo_nodes = QCheckBox("编辑照片轮廓节点")
        self.edit_photo_nodes.setChecked(True)
        self.edit_photo_nodes.toggled.connect(self.set_photo_edit_mode)
        photo_layout.addWidget(self.edit_photo_nodes)
        photo_layout.addWidget(self.photo_view, 1)
        self.add_button = button("将当前轮廓加入白板 →", self.add_to_board, photo_layout)
        self.add_button.setEnabled(False)
        photo_hint = QLabel("拖动图形内部平移；Shift＋拖动旋转，或在右侧输入角度。\n节点：拖动调整，双击图形添加，右键节点删除。")
        photo_hint.setWordWrap(True)
        photo_layout.addWidget(photo_hint)
        self.photo_tabs.currentChanged.connect(self.update_selection_tool)
        self.photo_selection.changed.connect(self.update_selection_controls)
        self.photo_selection.failed.connect(lambda message: QMessageBox.warning(self, "选区操作失败", message))
        self.update_selection_controls()
        splitter.addWidget(photo_panel)

        board_panel = QWidget()
        board_layout = QVBoxLayout(board_panel)
        board_layout.addWidget(QLabel("2 · 堆叠白板"))
        project_buttons = QHBoxLayout()
        button("打开工程…", self.open_project, project_buttons)
        button("保存工程…", self.save_project, project_buttons)
        board_layout.addLayout(project_buttons)
        board_buttons = QHBoxLayout()
        button("显示全部", self.board_view.fit_content, board_buttons)
        button("导出透明 PNG…", self.export_png, board_buttons)
        self.edit_nodes = QCheckBox("编辑白板节点")
        self.edit_nodes.toggled.connect(self.set_board_edit_mode)
        board_buttons.addWidget(self.edit_nodes)
        board_layout.addLayout(board_buttons)
        board_layout.addWidget(self.board_view, 1)
        hint = QLabel("拖动图形平移；Shift＋拖动旋转；滚轮缩放视图；中键平移视图。\n初始尺寸按照片像素保持；不同倍率照片请先统一比例。")
        hint.setWordWrap(True)
        board_layout.addWidget(hint)
        splitter.addWidget(board_panel)

        sidebar = QWidget()
        sidebar.setMinimumWidth(220)
        sidebar_layout = QVBoxLayout(sidebar)
        self.properties = QGroupBox("当前对象：未选择")
        properties_layout = QFormLayout(self.properties)
        self.name_edit = QLineEdit()
        self.name_edit.editingFinished.connect(self.rename_active)
        self.line_style_combo = QComboBox()
        self.line_style_combo.addItems(LINE_STYLES)
        self.line_width = number_box(0.5, 20, 2, 0.5)
        self.fill_opacity = number_box(0, 100, 25, 5, 0)
        self.rotation = number_box(-360, 360, 0)
        self.object_scale = number_box(0.001, 1000, 1, 0.1, 3)
        self.outline_button = QPushButton("选择…")
        self.fill_button = QPushButton("选择…")
        self.outline_button.clicked.connect(lambda: self.choose_color("outline_color"))
        self.fill_button.clicked.connect(lambda: self.choose_color("fill_color"))
        properties_layout.addRow("名称", self.name_edit)
        properties_layout.addRow("轮廓颜色", self.outline_button)
        properties_layout.addRow("线形", self.line_style_combo)
        properties_layout.addRow("线宽", self.line_width)
        properties_layout.addRow("填充颜色", self.fill_button)
        properties_layout.addRow("填充不透明度 %", self.fill_opacity)
        properties_layout.addRow("旋转角度 °", self.rotation)
        properties_layout.addRow("尺寸倍率", self.object_scale)
        self.line_style_combo.currentIndexChanged.connect(self.apply_properties)
        for widget in (self.line_width, self.fill_opacity):
            widget.valueChanged.connect(self.apply_properties)
        self.rotation.valueChanged.connect(self.apply_rotation)
        self.object_scale.valueChanged.connect(self.apply_scale)
        sidebar_layout.addWidget(self.properties)
        self.properties.setEnabled(False)
        background_group = QGroupBox("白板背景照片")
        background_layout = QVBoxLayout(background_group)
        background_buttons = QHBoxLayout()
        button("导入…", self.import_background, background_buttons)
        self.current_background_button = button("当前照片", self.use_current_photo_as_background, background_buttons)
        self.current_background_button.setEnabled(False)
        self.remove_background_button = button("移除", self.remove_background, background_buttons)
        background_layout.addLayout(background_buttons)
        self.move_background = QCheckBox("移动背景（默认锁定）")
        self.move_background.toggled.connect(self.set_background_edit_mode)
        background_layout.addWidget(self.move_background)
        background_form = QFormLayout()
        self.background_scale = number_box(0.001, 1000, 1, 0.1, 3)
        self.background_rotation = number_box(-360, 360, 0)
        self.background_opacity = number_box(0, 100, 100, 5, 0)
        background_form.addRow("尺寸倍率", self.background_scale)
        background_form.addRow("旋转角度 °", self.background_rotation)
        background_form.addRow("不透明度 %", self.background_opacity)
        for widget in (self.background_scale, self.background_rotation, self.background_opacity):
            widget.valueChanged.connect(self.apply_background_properties)
        background_layout.addLayout(background_form)
        sidebar_layout.addWidget(background_group)
        self.sync_background_controls()
        sidebar_layout.addWidget(QLabel("白板叠层（最上方 = 最前层）"))
        self.layer_list = QListWidget()
        self.layer_list.currentItemChanged.connect(self.select_layer)
        sidebar_layout.addWidget(self.layer_list, 1)
        order_buttons = QHBoxLayout()
        button("上移一层", lambda: self.reorder_layer(1), order_buttons)
        button("下移一层", lambda: self.reorder_layer(-1), order_buttons)
        sidebar_layout.addLayout(order_buttons)
        ends = QHBoxLayout()
        button("置顶", lambda: self.reorder_layer(len(self.layers)), ends)
        button("置底", lambda: self.reorder_layer(-len(self.layers)), ends)
        sidebar_layout.addLayout(ends)
        button("删除选中白板图形", self.delete_layer, sidebar_layout)
        splitter.addWidget(sidebar)
        splitter.setSizes([450, 650, 260])
        self.setCentralWidget(splitter)
        self.photo_scene.selectionChanged.connect(self.photo_selection_changed)
        self.board_scene.selectionChanged.connect(self.board_selection_changed)
        self.statusBar().showMessage("导入照片后自动识别轮廓。退出主程序前请保存白板工程，以便下次继续编辑。")

    def create_selection_controls(self):
        panel = QWidget()
        layout = QVBoxLayout(panel)
        form = QFormLayout()
        self.selection_tool = QComboBox()
        for label, tool in (("魔棒 · 点击相近颜色", "wand"), ("多边形套索", "lasso"),
                            ("画笔修补", "brush"), ("矩形选区", "rectangle"), ("查看 / 编辑轮廓", "edit")):
            self.selection_tool.addItem(label, tool)
        self.selection_tool.currentIndexChanged.connect(self.update_selection_tool)
        self.selection_operation = QComboBox()
        for label, operation in (("新选区", "new"), ("增加", "add"), ("减去", "subtract"), ("交集", "intersect")):
            self.selection_operation.addItem(label, operation)
        self.selection_operation.currentIndexChanged.connect(
            lambda: setattr(self.photo_selection, "operation", self.selection_operation.currentData()))
        tool_row = QHBoxLayout()
        tool_row.addWidget(self.selection_tool, 2)
        tool_row.addWidget(self.selection_operation, 1)
        form.addRow("工具 / 模式", tool_row)
        self.wand_tolerance = number_box(0, 255, 20, 1, 0)
        self.wand_tolerance.valueChanged.connect(lambda value: setattr(self.photo_selection, "tolerance", int(value)))
        self.wand_contiguous = QCheckBox("仅连续区域")
        self.wand_contiguous.setChecked(True)
        self.wand_contiguous.toggled.connect(lambda value: setattr(self.photo_selection, "contiguous", value))
        wand_row = QHBoxLayout()
        wand_row.addWidget(self.wand_tolerance)
        wand_row.addWidget(self.wand_contiguous)
        form.addRow("魔棒容差", wand_row)
        self.selection_brush_size = number_box(1, 500, 20, 1, 0)
        self.selection_brush_size.valueChanged.connect(lambda value: setattr(self.photo_selection, "brush_size", int(value)))
        self.selection_epsilon = number_box(0, 20, 1, 0.1, 2)
        self.selection_epsilon.setToolTip("轮廓简化允许的偏差，单位为原图像素；越小保留的节点越多，0 不做额外简化。")
        precision_row = QHBoxLayout()
        precision_row.addWidget(self.selection_brush_size)
        precision_row.addWidget(QLabel("轮廓误差"))
        precision_row.addWidget(self.selection_epsilon)
        form.addRow("画笔直径 / 像素", precision_row)
        layout.addLayout(form)
        actions = QHBoxLayout()
        self.selection_actions = {}
        for label, action in (("撤销", "undo"), ("重做", "redo"), ("清除", "clear"), ("反选", "invert"), ("填补孔洞", "fill")):
            self.selection_actions[action] = button(label, lambda checked=False, name=action: self.photo_selection.action(name), actions)
        layout.addLayout(actions)
        self.generate_selection_button = button("从选区生成可编辑轮廓", self.generate_selection_contours, layout)
        self.selection_info = QLabel("请导入照片。")
        layout.addWidget(self.selection_info)
        hint = QLabel("Shift 增加；Alt 减去；Shift＋Alt 交集。套索：点击顶点，Enter / 双击完成，退格撤回顶点，Esc 取消。\nCtrl＋Z 撤销；Ctrl＋Y 重做。先生成轮廓，再加入白板。")
        hint.setWordWrap(True)
        layout.addWidget(hint)
        return panel

    def update_selection_tool(self):
        tool = self.selection_tool.currentData() if self.photo_tabs.currentIndex() == 1 else "edit"
        self.photo_selection.set_tool(tool)
        self.wand_tolerance.setEnabled(tool == "wand")
        self.wand_contiguous.setEnabled(tool == "wand")
        self.selection_brush_size.setEnabled(tool == "brush")
        self.select_candidate(self.candidate_combo.currentIndex())

    def update_selection_controls(self):
        model = self.photo_selection.model
        ready = model is not None
        selected = int(np.count_nonzero(model.mask)) if ready else 0
        self.generate_selection_button.setEnabled(bool(selected) and not self.photo_selection.has_draft)
        for action, control in self.selection_actions.items():
            enabled = ready
            if action in ("undo", "redo"):
                enabled = ready and bool(getattr(model, action + "_stack"))
            elif action in ("clear", "fill"):
                enabled = bool(selected)
            control.setEnabled(enabled)
        message = f"已选 {selected:,} 个原图像素" if ready else "请导入照片。"
        if self.photo_selection.has_draft:
            message += " · 正在绘制，请先完成或按 Esc 取消"
        self.selection_info.setText(message)

    def generate_selection_contours(self):
        if self.photo_selection.model is None or self.photo_selection.has_draft:
            return
        try:
            polygons = self.photo_selection.model.polygons(self.selection_epsilon.value())
        except ValueError as exc:
            QMessageBox.warning(self, "无法生成轮廓", str(exc))
            return
        self.selection_tool.setCurrentIndex(self.selection_tool.findData("edit"))
        self.install_candidates(polygons)

    def import_photo(self):
        path, _ = QFileDialog.getOpenFileName(self, "导入 flake 照片", "", "照片 (*.png *.jpg *.jpeg *.bmp *.tif *.tiff);;所有文件 (*)")
        if not path:
            return
        try:
            self.load_photo(path)
        except ValueError as exc:
            QMessageBox.warning(self, "照片导入失败", str(exc))

    def load_photo(self, path):
        reader = QImageReader(str(path))
        reader.setAutoTransform(True)
        image = reader.read()
        if image.isNull():
            raise ValueError(f"无法读取照片：{reader.errorString()}")
        self.load_image(image, str(path))

    def load_image(self, image, name):
        if self.worker is not None and self.worker.isRunning():
            raise ValueError("正在识别，请等待完成后再导入照片。")
        if image.isNull():
            raise ValueError("没有可导入的图片。")
        self.clear_candidates()
        self.photo_selection.clear_image()
        self.photo_scene.clear()
        self.image = image.copy()
        self.image.setDevicePixelRatio(1)
        self.current_background_button.setEnabled(True)
        self.photo_path = Path(name)
        photo = self.photo_scene.addPixmap(QPixmap.fromImage(self.image))
        photo.setZValue(-1)
        self.photo_selection.set_image(self.image)
        self.photo_scene.setSceneRect(QRectF(0, 0, image.width(), image.height()))
        self.photo_view.fit_content()
        if self.photo_tabs.currentIndex() == 0:
            self.start_extraction()
        else:
            self.extract_button.setEnabled(True)
            self.statusBar().showMessage("请选择工具，在照片上创建选区。")

    def paste_photo(self):
        try:
            self.load_image(QApplication.clipboard().image(), "剪贴板图片")
        except ValueError as exc:
            QMessageBox.warning(self, "图片粘贴失败", str(exc))

    def start_screen_capture(self):
        if self.capture_active or (self.worker is not None and self.worker.isRunning()):
            return
        self.capture_active = True
        self.capture_button.setEnabled(False)
        self.capture_hidden_windows = []
        # Hide only our windows, leaving other apps available as capture targets.
        for window in (self, self.parentWidget()):
            if window is not None and window.isVisible():
                self.capture_hidden_windows.append((window, window.windowState()))
                window.hide()
        self.capture_timer.start(300)

    def capture_screens(self):
        if not self.capture_active:
            return
        screens = QApplication.screens()
        snapshots = [(screen, screen.grabWindow(0)) for screen in screens]
        if not snapshots or any(pixmap.isNull() for _, pixmap in snapshots):
            self.finish_screen_capture()
            QMessageBox.warning(self, "截图失败", "无法获取屏幕图像，请检查屏幕录制权限或使用“粘贴图片”。")
            return
        for screen, pixmap in snapshots:
            overlay = ScreenCaptureOverlay(pixmap, screen.geometry())
            overlay.winId()
            overlay.windowHandle().setScreen(screen)
            overlay.setGeometry(screen.geometry())
            overlay.captured.connect(self.finish_screen_capture)
            overlay.cancelled.connect(self.finish_screen_capture)
            self.capture_windows.append(overlay)
        for overlay in self.capture_windows:
            overlay.show()
        self.capture_windows[0].activateWindow()
        self.capture_windows[0].setFocus()

    def finish_screen_capture(self, image=None):
        if not self.capture_active:
            return
        self.capture_active = False
        self.capture_timer.stop()
        for overlay in self.capture_windows:
            overlay.hide()
            overlay.deleteLater()
        self.capture_windows.clear()
        for window, state in reversed(self.capture_hidden_windows):
            window.setWindowState(state)
            window.show()
        self.capture_hidden_windows.clear()
        self.capture_button.setEnabled(True)
        self.raise_()
        self.activateWindow()
        if image is not None:
            try:
                self.load_image(image, "屏幕截图")
            except ValueError as exc:
                QMessageBox.warning(self, "截图导入失败", str(exc))

    def import_background(self):
        path, _ = QFileDialog.getOpenFileName(self, "导入白板背景照片", "", "照片 (*.png *.jpg *.jpeg *.bmp *.tif *.tiff);;所有文件 (*)")
        if not path:
            return
        reader = QImageReader(path)
        reader.setAutoTransform(True)
        image = reader.read()
        if image.isNull():
            QMessageBox.warning(self, "背景导入失败", f"无法读取照片：{reader.errorString()}")
            return
        self.set_background_image(image)

    def set_background_image(self, image):
        if image.isNull():
            raise ValueError("背景照片无效。")
        background = BackgroundItem(image)
        background.setPos(self.board_view.mapToScene(self.board_view.viewport().rect().center()))
        self.remove_background()
        self.background = background
        self.board_scene.addItem(background)
        self.board_scene.setSceneRect(self.board_scene.sceneRect().united(
            background.sceneBoundingRect().adjusted(-500, -500, 500, 500)))
        self.sync_background_controls()
        self.board_view.fit_content()

    def use_current_photo_as_background(self):
        if self.image is not None:
            self.set_background_image(self.image)

    def remove_background(self):
        if self.background is not None:
            self.board_scene.removeItem(self.background)
            self.background = None
        self.sync_background_controls()

    def sync_background_controls(self):
        enabled = self.background is not None
        self.remove_background_button.setEnabled(enabled)
        self.move_background.setEnabled(enabled)
        self.move_background.setChecked(False)
        for widget, value in (
            (self.background_scale, self.background.scale() if enabled else 1),
            (self.background_rotation, self.background.rotation() if enabled else 0),
            (self.background_opacity, self.background.opacity() * 100 if enabled else 100),
        ):
            widget.blockSignals(True)
            widget.setValue(value)
            widget.blockSignals(False)
            widget.setEnabled(enabled)

    def set_background_edit_mode(self, enabled):
        if self.background is not None:
            self.background.set_editable(enabled)

    def apply_background_properties(self):
        if self.background is not None:
            self.background.setScale(self.background_scale.value())
            self.background.setRotation(self.background_rotation.value())
            self.background.setOpacity(self.background_opacity.value() / 100)

    def clear_candidates(self):
        if self.active_item in self.candidates:
            self.set_active_item(None)
        self.candidate_combo.blockSignals(True)
        self.candidate_combo.clear()
        self.candidate_combo.blockSignals(False)
        for item in self.candidates:
            self.photo_scene.removeItem(item)
        self.candidates.clear()
        self.add_button.setEnabled(False)

    def start_extraction(self):
        if self.image is None or (self.worker is not None and self.worker.isRunning()):
            return
        self.clear_candidates()
        self.photo_selection.cancel_draft()
        self.photo_selection.enabled = False
        self.photo_tabs.setEnabled(False)
        self.import_button.setEnabled(False)
        self.capture_button.setEnabled(False)
        self.paste_button.setEnabled(False)
        self.extract_button.setEnabled(False)
        self.statusBar().showMessage("正在识别轮廓…")
        self.worker = ContourWorker(self.image, {
            "threshold": self.threshold.value(),
            "min_area_percent": self.min_area.value(),
            "simplification": self.simplification.value(),
        }, self)
        self.worker.extracted.connect(self.install_candidates)
        self.worker.failed.connect(self.extraction_failed)
        self.worker.finished.connect(self.extraction_finished)
        self.worker.finished.connect(self.worker.deleteLater)
        self.worker.start()

    def install_candidates(self, polygons):
        first = len(self.candidates)
        self.candidate_combo.blockSignals(True)
        for i, points in enumerate(polygons, first):
            name = f"{self.photo_path.stem} · {i + 1}"
            item = FlakeItem(points, name)
            item.edit_vertices = self.edit_photo_nodes.isChecked()
            item.rotation_changed = self.sync_item_rotation
            self.photo_scene.addItem(item)
            item.setVisible(False)
            self.candidates.append(item)
            self.candidate_combo.addItem(f"轮廓 {i + 1}（{len(points)} 个节点）")
        self.candidate_combo.blockSignals(False)
        self.candidate_combo.setCurrentIndex(first)
        self.select_candidate(first)
        self.statusBar().showMessage(f"识别出 {len(polygons)} 个候选轮廓。请选择、检查并修整后加入白板。")

    def extraction_failed(self, message):
        self.statusBar().showMessage(message)
        QMessageBox.warning(self, "轮廓识别失败", message)

    def extraction_finished(self):
        self.worker = None
        self.photo_selection.enabled = True
        self.photo_tabs.setEnabled(True)
        self.import_button.setEnabled(True)
        self.capture_button.setEnabled(True)
        self.paste_button.setEnabled(True)
        self.extract_button.setEnabled(True)

    def wait_for_extraction(self):
        if self.worker is not None:
            self.worker.wait()

    def closeEvent(self, event):
        if self.capture_active:
            self.finish_screen_capture()
        self.wait_for_extraction()
        super().closeEvent(event)

    def select_candidate(self, index):
        editing = self.photo_selection.tool == "edit"
        for i, item in enumerate(self.candidates):
            item.setVisible(editing and i == index)
            item.setSelected(editing and i == index)
        valid = editing and 0 <= index < len(self.candidates)
        self.add_button.setEnabled(valid)
        self.edit_photo_nodes.setEnabled(editing)
        if valid:
            self.set_active_item(self.candidates[index])
        elif self.active_item in self.candidates:
            self.set_active_item(None)

    def add_to_board(self):
        index = self.candidate_combo.currentIndex()
        if not 0 <= index < len(self.candidates):
            return
        item = self.candidates[index].copy_to_board()
        item.rotation_changed = self.sync_item_rotation
        item.edit_vertices = self.edit_nodes.isChecked()
        self.board_scene.clearSelection()
        self.board_scene.addItem(item)
        item.setPos(self.board_view.mapToScene(self.board_view.viewport().rect().center()))
        self.layers.append(item)
        self.refresh_layers(item)
        item.setSelected(True)
        self.set_active_item(item)
        if len(self.layers) == 1:
            self.board_view.fit_content()

    @pyqtSlot()
    def photo_selection_changed(self):
        self.selection_changed(self.photo_scene)

    @pyqtSlot()
    def board_selection_changed(self):
        self.selection_changed(self.board_scene)

    def selection_changed(self, scene):
        selected = [item for item in scene.selectedItems() if isinstance(item, FlakeItem)]
        if selected:
            self.set_active_item(selected[0])
        elif self.active_item is not None and self.active_item.scene() is scene:
            self.set_active_item(None)
        if scene is self.board_scene:
            self.refresh_layers(selected[0] if selected else None)

    def set_active_item(self, item):
        if item is not None:
            other_scene = self.photo_scene if item in self.layers else self.board_scene
            other_scene.blockSignals(True)
            other_scene.clearSelection()
            other_scene.blockSignals(False)
            if other_scene is self.board_scene:
                self.refresh_layers(None)
        self.active_item = item
        self.properties.setEnabled(item is not None)
        if item is None:
            self.properties.setTitle("当前对象：未选择")
            return
        self._syncing = True
        self.properties.setTitle("当前对象：" + ("白板图形" if item in self.layers else "照片轮廓"))
        self.name_edit.setText(item.name)
        self.line_style_combo.setCurrentIndex(list(LINE_STYLES.values()).index(item.line_style))
        self.line_width.setValue(item.line_width)
        self.fill_opacity.setValue(item.fill_alpha / 255 * 100)
        self.rotation.setValue(item.rotation())
        self.object_scale.setValue(item.scale())
        self.rotation.setEnabled(True)
        self.object_scale.setEnabled(item in self.layers)
        self.outline_button.setText(item.outline_color.name())
        self.fill_button.setText(item.fill_color.name())
        self._syncing = False

    def apply_properties(self):
        item = self.active_item
        if self._syncing or item is None:
            return
        item.line_style = LINE_STYLES[self.line_style_combo.currentText()]
        item.line_width = self.line_width.value()
        item.fill_alpha = round(self.fill_opacity.value() * 255 / 100)
        item.apply_style()

    def apply_rotation(self, value):
        if not self._syncing and self.active_item is not None:
            self.active_item.setRotation(value)

    def apply_scale(self, value):
        if not self._syncing and self.active_item in self.layers:
            self.active_item.setScale(value)

    def sync_item_rotation(self, item):
        if self.active_item is item:
            self.rotation.blockSignals(True)
            self.rotation.setValue(item.rotation())
            self.rotation.blockSignals(False)

    def choose_color(self, attribute):
        item = self.active_item
        if item is None:
            return
        color = QColorDialog.getColor(getattr(item, attribute), self, "选择颜色")
        if color.isValid():
            setattr(item, attribute, color)
            item.apply_style()
            self.set_active_item(item)

    def rename_active(self):
        if self.active_item is None:
            return
        name = self.name_edit.text().strip()
        if not name:
            self.name_edit.setText(self.active_item.name)
            return
        self.active_item.name = name
        self.refresh_layers(self.selected_layer())

    def set_board_edit_mode(self, enabled):
        for item in self.layers:
            item.edit_vertices = enabled
            item.update_handles()

    def set_photo_edit_mode(self, enabled):
        for item in self.candidates:
            item.edit_vertices = enabled
            item.update_handles()

    def refresh_layers(self, selected):
        self.layer_list.blockSignals(True)
        self.layer_list.clear()
        for z, item in enumerate(self.layers):
            item.setZValue(z)
        for item in reversed(self.layers):
            row = QListWidgetItem(item.name)
            row.setData(Qt.ItemDataRole.UserRole, item)
            self.layer_list.addItem(row)
            if item is selected:
                self.layer_list.setCurrentItem(row)
        self.layer_list.blockSignals(False)

    def select_layer(self, current, previous):
        if current is None:
            return
        item = current.data(Qt.ItemDataRole.UserRole)
        self.board_scene.blockSignals(True)
        self.board_scene.clearSelection()
        item.setSelected(True)
        self.board_scene.blockSignals(False)
        self.set_active_item(item)

    def selected_layer(self):
        selected = self.board_scene.selectedItems()
        return next((item for item in self.layers if item in selected), None)

    def project_data(self):
        # Array order is bottom to top. Keep the original pivot: editing vertices
        # can change the polygon bounds without changing its rotation/scale pivot.
        background_data = None
        if self.background is not None:
            buffer = QBuffer()
            if not buffer.open(QIODevice.OpenModeFlag.WriteOnly) or not self.background.image.save(buffer, "PNG"):
                raise OSError("背景照片编码失败。")
            background_data = {
                "image_png_base64": base64.b64encode(bytes(buffer.data())).decode("ascii"),
                "position": [self.background.pos().x(), self.background.pos().y()],
                "rotation": self.background.rotation(),
                "scale": self.background.scale(),
                "opacity": self.background.opacity(),
            }
            buffer.close()
        return {
            "format": "hstack",
            "version": 2,
            "background": background_data,
            "layers": [{
                "name": item.name,
                "vertices": [[p.x(), p.y()] for p in item.polygon()],
                "position": [item.pos().x(), item.pos().y()],
                "transform_origin": [item.transformOriginPoint().x(), item.transformOriginPoint().y()],
                "rotation": item.rotation(),
                "scale": item.scale(),
                "outline_color": item.outline_color.name(QColor.NameFormat.HexArgb),
                "fill_color": item.fill_color.name(QColor.NameFormat.HexArgb),
                "fill_alpha": item.fill_alpha,
                "line_width": item.line_width,
                "line_style": item.line_style.name,
            } for item in self.layers],
        }

    def save_project_to(self, path):
        data = self.project_data()
        validate_project(data)
        payload = json.dumps(data, ensure_ascii=False, indent=2, allow_nan=False).encode("utf-8")
        # QSaveFile commits by replacement only after the complete write succeeds.
        output = QSaveFile(str(path))
        if not output.open(QIODevice.OpenModeFlag.WriteOnly):
            raise OSError(f"无法保存工程：{output.errorString()}")
        if output.write(payload) != len(payload):
            message = output.errorString()
            output.cancelWriting()
            raise OSError(f"工程写入失败：{message}")
        if not output.commit():
            raise OSError(f"工程保存失败：{output.errorString()}")
        self.project_path = Path(path)

    def save_project(self):
        dialog = QFileDialog(self, "保存白板工程")
        dialog.setAcceptMode(QFileDialog.AcceptMode.AcceptSave)
        dialog.setNameFilter("白板工程 (*.hstack.json)")
        dialog.setDefaultSuffix("hstack.json")
        dialog.selectFile(str(self.project_path) if self.project_path else "堆叠设计.hstack.json")
        if not dialog.exec():
            return
        path = dialog.selectedFiles()[0]
        try:
            self.save_project_to(path)
        except (OSError, ValueError) as exc:
            QMessageBox.warning(self, "工程保存失败", str(exc))
            return
        self.statusBar().showMessage(f"已保存白板工程：{path}")

    @staticmethod
    def read_project(path):
        data = json.loads(Path(path).read_text(encoding="utf-8-sig"))
        validate_project(data)
        items = []
        for layer in data["layers"]:
            item = FlakeItem(layer["vertices"], layer["name"])
            item.setTransformOriginPoint(QPointF(*layer["transform_origin"]))
            item.setPos(QPointF(*layer["position"]))
            item.setRotation(layer["rotation"])
            item.setScale(layer["scale"])
            item.outline_color = QColor(layer["outline_color"])
            item.fill_color = QColor(layer["fill_color"])
            item.fill_alpha = layer["fill_alpha"]
            item.line_width = layer["line_width"]
            item.line_style = next(style for style in LINE_STYLES.values() if style.name == layer["line_style"])
            item.apply_style()
            bounds = item.sceneBoundingRect()
            if not all(isfinite(value) for value in (bounds.x(), bounds.y(), bounds.width(), bounds.height(),
                                                     bounds.right(), bounds.bottom())):
                raise ValueError(f"图层 {item.name} 的变换结果超出有效坐标范围。")
            items.append(item)
        background = None
        if data["version"] == 2 and data["background"] is not None:
            record = data["background"]
            try:
                image_bytes = base64.b64decode(record["image_png_base64"], validate=True)
            except (binascii.Error, ValueError) as exc:
                raise ValueError("背景照片的 Base64 编码无效。") from exc
            image = QImage.fromData(image_bytes, "PNG")
            if image.isNull():
                raise ValueError("工程中的背景 PNG 照片损坏。")
            background = BackgroundItem(image)
            background.setPos(QPointF(*record["position"]))
            background.setRotation(record["rotation"])
            background.setScale(record["scale"])
            background.setOpacity(record["opacity"])
            bounds = background.sceneBoundingRect()
            if not all(isfinite(value) for value in (bounds.x(), bounds.y(), bounds.width(), bounds.height(),
                                                     bounds.right(), bounds.bottom())):
                raise ValueError("背景照片的变换结果超出有效坐标范围。")
        return items, background

    def install_project(self, project, path):
        items, background = project
        if self.active_item in self.layers:
            self.set_active_item(None)
        self.board_scene.blockSignals(True)
        self.board_scene.clear()
        self.layers = items
        self.background = background
        bounds = QRectF(-5000, -5000, 10000, 10000)
        if background is not None:
            self.board_scene.addItem(background)
            bounds = bounds.united(background.sceneBoundingRect().adjusted(-500, -500, 500, 500))
        for item in self.layers:
            item.rotation_changed = self.sync_item_rotation
            item.edit_vertices = self.edit_nodes.isChecked()
            self.board_scene.addItem(item)
            bounds = bounds.united(item.sceneBoundingRect().adjusted(-500, -500, 500, 500))
        self.board_scene.setSceneRect(bounds)
        self.board_scene.blockSignals(False)
        self.sync_background_controls()
        selected = self.layers[-1] if self.layers else None
        self.refresh_layers(selected)
        if selected is not None:
            selected.setSelected(True)
        if self.layers or background is not None:
            self.board_view.fit_content()
        else:
            self.board_view.resetTransform()
            self.board_view.centerOn(0, 0)
        self.project_path = Path(path)
        self.statusBar().showMessage(f"已打开白板工程：{path}（{len(self.layers)} 个图层）")

    def open_project(self):
        path, _ = QFileDialog.getOpenFileName(self, "打开白板工程", "", "白板工程 (*.hstack.json *.json)")
        if not path:
            return
        try:
            project = self.read_project(path)
        except (OSError, ValueError, RecursionError) as exc:
            QMessageBox.warning(self, "工程打开失败", str(exc))
            return
        if self.layers or self.background is not None:
            answer = QMessageBox.question(
                self, "替换当前白板", "打开工程将替换当前白板。未保存的白板修改会丢失，是否继续？",
                QMessageBox.StandardButton.Yes | QMessageBox.StandardButton.Cancel,
                QMessageBox.StandardButton.Cancel,
            )
            if answer != QMessageBox.StandardButton.Yes:
                return
        self.install_project(project, path)

    def render_outline_image(self):
        """One board unit per pixel, independent of view zoom or selection.

        Draw only the transformed polygons, without mutating the live scene.
        Fills, photo backgrounds, selection boxes and vertex handles are omitted.
        """
        if not self.layers:
            raise ValueError("白板为空，请先加入 flake 轮廓。")
        polygons = [item.mapToScene(item.polygon()) for item in self.layers]
        bounds = polygons[0].boundingRect()
        for polygon in polygons[1:]:
            bounds = bounds.united(polygon.boundingRect())
        padding = ceil(max(item.pen().widthF() for item in self.layers)) + 2
        bounds = bounds.adjusted(-padding, -padding, padding, padding)
        width, height = ceil(bounds.width()), ceil(bounds.height())
        # Bound raster memory explicitly rather than silently reducing resolution.
        if width > 32767 or height > 32767 or width * height > 64_000_000:
            raise ValueError("导出尺寸过大（最长边须不超过 32767 像素，总像素须不超过 6400 万）。请缩小图形尺寸或减小图形间距。")
        image = QImage(width, height, QImage.Format.Format_ARGB32_Premultiplied)
        if image.isNull():
            raise ValueError("无法分配导出图像所需的内存。请缩小图形尺寸后重试。")
        image.fill(Qt.GlobalColor.transparent)
        painter = QPainter(image)
        try:
            painter.setRenderHint(QPainter.RenderHint.Antialiasing)
            painter.setBrush(Qt.BrushStyle.NoBrush)
            painter.translate(-bounds.left(), -bounds.top())
            for item, polygon in zip(self.layers, polygons):
                painter.setPen(item.pen())
                painter.drawPolygon(polygon)
        finally:
            painter.end()
        return image

    def export_png(self):
        if not self.layers:
            QMessageBox.warning(self, "无法导出", "白板为空，请先加入 flake 轮廓。")
            return
        dialog = QFileDialog(self, "导出透明线框 PNG")
        dialog.setAcceptMode(QFileDialog.AcceptMode.AcceptSave)
        dialog.setNameFilter("PNG 图像 (*.png)")
        dialog.setDefaultSuffix("png")
        dialog.selectFile("堆叠设计.png")
        if not dialog.exec():
            return
        path = dialog.selectedFiles()[0]
        try:
            image = self.render_outline_image()
            if not image.save(path, "PNG"):
                raise OSError("PNG 写入失败，请检查目标目录权限和可用磁盘空间。")
        except (ValueError, OSError) as exc:
            QMessageBox.warning(self, "导出失败", str(exc))
            return
        self.statusBar().showMessage(f"已导出透明线框 PNG：{path}（{image.width()} × {image.height()}）")

    def reorder_layer(self, offset):
        item = self.selected_layer()
        if item is None:
            return
        old_index = self.layers.index(item)
        new_index = max(0, min(len(self.layers) - 1, old_index + offset))
        self.layers.insert(new_index, self.layers.pop(old_index))
        self.refresh_layers(item)

    def delete_layer(self):
        item = self.selected_layer()
        if item is None:
            return
        if self.active_item is item:
            self.set_active_item(None)
        self.layers.remove(item)
        self.board_scene.removeItem(item)
        self.refresh_layers(None)


if __name__ == "__main__":
    import sys
    from PyQt6.QtWidgets import QApplication

    app = QApplication(sys.argv)
    window = StackDesignerWindow()
    window.show()
    sys.exit(app.exec())
