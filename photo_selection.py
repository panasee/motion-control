"""Full-resolution photo selections, independent of whiteboard geometry."""

import cv2
import numpy as np
from PyQt6.QtCore import QEvent, QObject, QPointF, Qt, pyqtSignal
from PyQt6.QtGui import QColor, QImage, QPainterPath, QPen, QPixmap


class SelectionModel:
    HISTORY_LIMIT = 20

    def __init__(self, rgb):
        if rgb.dtype != np.uint8 or rgb.ndim != 3 or rgb.shape[2] != 3:
            raise ValueError("选区需要 uint8 RGB 图片。")
        self.rgb = np.ascontiguousarray(rgb.copy())
        self.mask = np.zeros(rgb.shape[:2], np.uint8)
        self.undo_stack = []
        self.redo_stack = []

    def wand(self, x, y, tolerance=20, contiguous=True):
        height, width = self.mask.shape
        if not (0 <= x < width and 0 <= y < height and 0 <= tolerance <= 255):
            raise ValueError("魔棒取样点或容差超出范围。")
        tolerance = int(tolerance)
        if contiguous:
            flood_mask = np.zeros((height + 2, width + 2), np.uint8)
            # Compare every candidate to the clicked color (fixed range), rather
            # than accumulating small color differences across a gradient.
            cv2.floodFill(self.rgb, flood_mask, (x, y), (0, 0, 0),
                          (tolerance,) * 3, (tolerance,) * 3,
                          4 | cv2.FLOODFILL_MASK_ONLY | cv2.FLOODFILL_FIXED_RANGE | (255 << 8))
            return flood_mask[1:-1, 1:-1].copy()
        color = self.rgb[y, x].astype(np.int16)
        lower = np.maximum(0, color - tolerance).astype(np.uint8)
        upper = np.minimum(255, color + tolerance).astype(np.uint8)
        return cv2.inRange(self.rgb, lower, upper)

    def combine(self, candidate, operation):
        if candidate.shape != self.mask.shape or candidate.dtype != np.uint8:
            raise ValueError("选区尺寸或数据类型不匹配。")
        if operation == "new":
            return candidate.copy()
        if operation == "add":
            return cv2.bitwise_or(self.mask, candidate)
        if operation == "subtract":
            return cv2.bitwise_and(self.mask, cv2.bitwise_not(candidate))
        if operation == "intersect":
            return cv2.bitwise_and(self.mask, candidate)
        raise ValueError("未知的选区操作。")

    def commit(self, mask):
        if mask.shape != self.mask.shape or mask.dtype != np.uint8:
            raise ValueError("选区尺寸或数据类型不匹配。")
        if np.array_equal(mask, self.mask):
            return False
        # Binary snapshots cost one bit per pixel, with at most 20 undo steps.
        self.undo_stack.append(np.packbits(self.mask != 0))
        self.undo_stack = self.undo_stack[-self.HISTORY_LIMIT:]
        self.redo_stack.clear()
        self.mask = mask.copy()
        return True

    def apply(self, candidate, operation):
        return self.commit(self.combine(candidate, operation))

    def undo(self):
        if not self.undo_stack:
            return False
        self.redo_stack.append(np.packbits(self.mask != 0))
        self.mask = np.unpackbits(self.undo_stack.pop(), count=self.mask.size).reshape(self.mask.shape) * 255
        return True

    def redo(self):
        if not self.redo_stack:
            return False
        self.undo_stack.append(np.packbits(self.mask != 0))
        self.mask = np.unpackbits(self.redo_stack.pop(), count=self.mask.size).reshape(self.mask.shape) * 255
        return True

    def fill_holes(self):
        padded = np.pad(self.mask, 1, constant_values=0)
        outside = padded.copy()
        cv2.floodFill(outside, None, (0, 0), 255)
        return self.commit(cv2.bitwise_or(padded, cv2.bitwise_not(outside))[1:-1, 1:-1])

    def polygons(self, epsilon=1):
        if not 0 <= epsilon <= 20:
            raise ValueError("轮廓误差应为 0–20 个原图像素。")
        contours, hierarchy = cv2.findContours(self.mask, cv2.RETR_CCOMP, cv2.CHAIN_APPROX_SIMPLE)
        if not contours:
            raise ValueError("请先建立选区。")
        if np.any(hierarchy[0, :, 3] >= 0):
            raise ValueError("选区含内部孔洞，当前图形只支持无孔多边形。可点击“填补孔洞”，或用减选将孔洞连到外边缘，再生成轮廓。")
        polygons = []
        for contour in sorted(contours, key=cv2.contourArea, reverse=True):
            if cv2.contourArea(contour) == 0:
                raise ValueError("选区包含零面积的孤立点或细线，请先用减选清理，或扩大该区域。")
            polygon = cv2.approxPolyDP(contour, epsilon, True)
            if len(polygon) < 3 or cv2.contourArea(polygon) == 0:
                raise ValueError("轮廓误差过大，部分区域无法形成多边形。请降低“轮廓误差”。")
            polygons.append(polygon.reshape(-1, 2).astype(float).tolist())
        return polygons


class PhotoSelectionController(QObject):
    """Intercept selection gestures; leave existing pan/zoom and edit modes alone."""

    changed = pyqtSignal()
    failed = pyqtSignal(str)

    def __init__(self, view, parent=None):
        super().__init__(parent)
        self.view = view
        self.model = None
        self.tool = "edit"
        self.operation = "new"
        self.tolerance = 20
        self.contiguous = True
        self.brush_size = 20
        self.enabled = True
        self.overlay = self.border = self.border_base = self.draft_item = None
        self.lasso = []
        self.stroke = None
        self.anchor = None
        self.last_point = None
        self.gesture_operation = "new"
        self.view.viewport().setMouseTracking(True)
        self.view.viewport().installEventFilter(self)
        self.view.installEventFilter(self)

    @property
    def has_draft(self):
        return bool(self.lasso) or self.stroke is not None

    def clear_image(self):
        self.lasso.clear()
        self.stroke = None
        self.anchor = self.last_point = None
        for item in (self.overlay, self.border_base, self.border, self.draft_item):
            if item is not None:
                self.view.scene().removeItem(item)
        self.overlay = self.border = self.border_base = self.draft_item = None
        self.model = None
        self.changed.emit()

    def set_image(self, image):
        self.clear_image()
        rgb_image = image.convertToFormat(QImage.Format.Format_RGB888)
        ptr = rgb_image.constBits()
        ptr.setsize(rgb_image.sizeInBytes())
        rgb = np.frombuffer(ptr, np.uint8).reshape(rgb_image.height(), rgb_image.bytesPerLine())
        rgb = rgb[:, :rgb_image.width() * 3].reshape(rgb_image.height(), rgb_image.width(), 3)
        self.model = SelectionModel(rgb)
        scene = self.view.scene()
        self.overlay = scene.addPixmap(QPixmap())
        self.border_base = scene.addPath(QPainterPath(), QPen(QColor("black"), 2))
        self.border = scene.addPath(QPainterPath(), QPen(QColor("white"), 1, Qt.PenStyle.DashLine))
        self.draft_item = scene.addPath(QPainterPath(), QPen(QColor("#ffcc33"), 2, Qt.PenStyle.DashLine))
        for z, item in enumerate((self.overlay, self.border_base, self.border, self.draft_item), 1000):
            item.setZValue(z)
            item.setAcceptedMouseButtons(Qt.MouseButton.NoButton)
            if item is not self.overlay:
                pen = item.pen()
                pen.setCosmetic(True)
                item.setPen(pen)
        self.refresh()

    def set_tool(self, tool):
        if tool not in ("edit", "wand", "lasso", "brush", "rectangle"):
            raise ValueError("未知的选区工具。")
        self.tool = tool
        self.cancel_draft()
        self.view.viewport().setCursor(Qt.CursorShape.ArrowCursor if tool == "edit" else Qt.CursorShape.CrossCursor)

    def operation_for(self, modifiers):
        shift = bool(modifiers & Qt.KeyboardModifier.ShiftModifier)
        alt = bool(modifiers & Qt.KeyboardModifier.AltModifier)
        if shift and alt:
            return "intersect"
        if alt:
            return "subtract"
        if shift:
            return "add"
        return self.operation

    def cancel_draft(self):
        self.lasso.clear()
        self.stroke = None
        self.anchor = self.last_point = None
        if self.draft_item is not None:
            self.draft_item.setPath(QPainterPath())
        self.refresh()

    def refresh(self, preview=None):
        if self.model is not None:
            mask = self.model.mask if preview is None else preview
            rgba = np.zeros((*mask.shape, 4), np.uint8)
            rgba[:, :, :3] = (30, 144, 255)
            rgba[:, :, 3] = np.where(mask != 0, 85, 0)
            image = QImage(rgba.data, mask.shape[1], mask.shape[0], rgba.strides[0], QImage.Format.Format_RGBA8888).copy()
            self.overlay.setPixmap(QPixmap.fromImage(image))
            contours, _ = cv2.findContours(mask, cv2.RETR_LIST, cv2.CHAIN_APPROX_SIMPLE)
            path = QPainterPath()
            for contour in contours:
                points = contour.reshape(-1, 2)
                path.moveTo(float(points[0, 0]), float(points[0, 1]))
                for x, y in points[1:]:
                    path.lineTo(float(x), float(y))
                path.closeSubpath()
            self.border.setPath(path)
            self.border_base.setPath(path)
            for item in (self.overlay, self.border, self.border_base, self.draft_item):
                item.setVisible(self.tool != "edit")
        self.changed.emit()

    def action(self, name):
        if self.model is None:
            return
        self.cancel_draft()
        if name == "undo":
            self.model.undo()
        elif name == "redo":
            self.model.redo()
        elif name == "clear":
            self.model.commit(np.zeros_like(self.model.mask))
        elif name == "invert":
            self.model.commit(cv2.bitwise_not(self.model.mask))
        elif name == "fill":
            self.model.fill_holes()
        else:
            raise ValueError("未知的选区操作。")
        self.refresh()

    def draw_lasso(self, cursor=None):
        path = QPainterPath()
        if self.lasso:
            path.moveTo(QPointF(*self.lasso[0]))
            for point in self.lasso[1:]:
                path.lineTo(QPointF(*point))
            if cursor is not None:
                path.lineTo(QPointF(*cursor))
        self.draft_item.setPath(path)
        self.changed.emit()

    def finish_lasso(self):
        if len(self.lasso) < 3:
            self.failed.emit("多边形套索至少需要三个点；可继续点击，Esc 取消。")
            return
        candidate = np.zeros_like(self.model.mask)
        cv2.fillPoly(candidate, [np.array(self.lasso, np.int32)], 255)
        self.model.apply(candidate, self.gesture_operation)
        self.cancel_draft()

    def eventFilter(self, watched, event):
        if not self.enabled or self.model is None or self.tool == "edit":
            return False
        kind = event.type()
        if kind == QEvent.Type.KeyPress:
            key, modifiers = event.key(), event.modifiers()
            if key == Qt.Key.Key_Escape:
                self.cancel_draft()
            elif key in (Qt.Key.Key_Return, Qt.Key.Key_Enter) and self.lasso:
                self.finish_lasso()
            elif key == Qt.Key.Key_Backspace and self.lasso:
                self.lasso.pop()
                self.draw_lasso()
            elif modifiers & Qt.KeyboardModifier.ControlModifier and key == Qt.Key.Key_Z:
                self.action("redo" if modifiers & Qt.KeyboardModifier.ShiftModifier else "undo")
            elif modifiers & Qt.KeyboardModifier.ControlModifier and key == Qt.Key.Key_Y:
                self.action("redo")
            elif modifiers & Qt.KeyboardModifier.ControlModifier and key == Qt.Key.Key_D:
                self.action("clear")
            else:
                return False
            return True
        if watched is not self.view.viewport() or kind not in (
            QEvent.Type.MouseButtonPress, QEvent.Type.MouseMove,
            QEvent.Type.MouseButtonRelease, QEvent.Type.MouseButtonDblClick,
        ):
            return False
        if event.buttons() & Qt.MouseButton.MiddleButton or event.button() == Qt.MouseButton.MiddleButton:
            return False
        point = self.view.mapToScene(event.position().toPoint())
        height, width = self.model.mask.shape
        inside = 0 <= point.x() < width and 0 <= point.y() < height
        xy = (max(0, min(width - 1, int(point.x()))), max(0, min(height - 1, int(point.y()))))
        try:
            return self.handle_mouse(kind, event, xy, inside)
        except (ValueError, cv2.error) as exc:
            self.cancel_draft()
            self.failed.emit(str(exc))
            return True

    def handle_mouse(self, kind, event, xy, inside):
        if kind == QEvent.Type.MouseButtonPress:
            if event.button() != Qt.MouseButton.LeftButton or not inside:
                return True
            self.view.setFocus()
            if not self.lasso:
                self.gesture_operation = self.operation_for(event.modifiers())
            if self.tool == "wand":
                candidate = self.model.wand(*xy, self.tolerance, self.contiguous)
                self.model.apply(candidate, self.gesture_operation)
                self.refresh()
            elif self.tool == "lasso":
                if not self.lasso or self.lasso[-1] != xy:
                    self.lasso.append(xy)
                self.draw_lasso()
            else:
                self.anchor = self.last_point = xy
                self.stroke = np.zeros_like(self.model.mask)
                self.update_stroke(xy)
            return True
        if kind == QEvent.Type.MouseButtonDblClick and event.button() == Qt.MouseButton.LeftButton:
            if self.tool == "lasso" and inside:
                if not self.lasso or self.lasso[-1] != xy:
                    self.lasso.append(xy)
                self.finish_lasso()
            return True
        if kind == QEvent.Type.MouseMove:
            if self.stroke is not None:
                self.update_stroke(xy)
            elif self.lasso:
                self.draw_lasso(xy)
            return True
        if kind == QEvent.Type.MouseButtonRelease and event.button() == Qt.MouseButton.LeftButton:
            if self.stroke is not None:
                self.update_stroke(xy)
                self.model.apply(self.stroke, self.gesture_operation)
                self.cancel_draft()
            return True
        return True

    def update_stroke(self, xy):
        if self.tool == "rectangle":
            self.stroke.fill(0)
            cv2.rectangle(self.stroke, self.anchor, xy, 255, -1)
        elif self.tool == "brush":
            radius = max(0, int(self.brush_size) // 2)
            cv2.line(self.stroke, self.last_point, xy, 255, max(1, 2 * radius))
            cv2.circle(self.stroke, xy, radius, 255, -1)
            self.last_point = xy
        self.refresh(self.model.combine(self.stroke, self.gesture_operation))
