"""Application-neutral Qt canvas and editable polygons.

No application, camera SDK, OpenCV, motion driver, settings or server imports.
Both microscope and stack designer depend on this leaf module.
"""
from math import atan2, degrees
from PyQt6.QtCore import QPointF, Qt
from PyQt6.QtGui import QBrush, QColor, QPainter, QPen, QPolygonF
from PyQt6.QtWidgets import QGraphicsEllipseItem, QGraphicsItem, QGraphicsPolygonItem, QGraphicsView

LINE_STYLES = {
    "实线": Qt.PenStyle.SolidLine,
    "虚线": Qt.PenStyle.DashLine,
    "点线": Qt.PenStyle.DotLine,
    "点划线": Qt.PenStyle.DashDotLine,
}


class VertexHandle(QGraphicsEllipseItem):
    def __init__(self, owner, index, point):
        super().__init__(-4, -4, 8, 8, owner)
        self.index = index
        self._drag_offset = None
        self.setBrush(QBrush(QColor("white")))
        self.setPen(QPen(QColor("#1677ff"), 1))
        self.setFlag(QGraphicsItem.GraphicsItemFlag.ItemIgnoresTransformations)
        self.setFlag(QGraphicsItem.GraphicsItemFlag.ItemIsMovable)
        self.setPos(point)
        self.setFlag(QGraphicsItem.GraphicsItemFlag.ItemSendsGeometryChanges)
        self.setZValue(1)
        self.setCursor(Qt.CursorShape.CrossCursor)
        self.setToolTip("拖动节点；右键删除（至少保留三个节点）")

    def itemChange(self, change, value):
        if change == QGraphicsItem.GraphicsItemChange.ItemPositionHasChanged:
            self.parentItem().move_vertex(self.index, value)
        return super().itemChange(change, value)

    def mousePressEvent(self, event):
        if event.button() == Qt.MouseButton.RightButton:
            event.accept()
            self.parentItem().remove_vertex(self.index)
            return
        if event.button() == Qt.MouseButton.LeftButton:
            self._drag_offset = self.parentItem().mapFromScene(event.scenePos()) - self.pos()
            event.accept()
            return
        super().mousePressEvent(event)

    def mouseMoveEvent(self, event):
        if self._drag_offset is not None:
            # Move this vertex in polygon-local coordinates, never its selected
            # movable parent (Qt's default graphics-item drag can move both).
            self.setPos(self.parentItem().mapFromScene(event.scenePos()) - self._drag_offset)
            event.accept()
            return
        super().mouseMoveEvent(event)

    def mouseReleaseEvent(self, event):
        if event.button() == Qt.MouseButton.LeftButton and self._drag_offset is not None:
            self._drag_offset = None
            event.accept()
            return
        super().mouseReleaseEvent(event)


class EditablePolygonItem(QGraphicsPolygonItem):
    def __init__(self, points, name, movable=True):
        super().__init__(QPolygonF([QPointF(float(x), float(y)) for x, y in points]))
        self.name = name
        self.outline_color = QColor("#1565c0")
        self.fill_color = QColor("#64b5f6")
        self.fill_alpha = 65
        self.line_width = 2.0
        self.line_style = Qt.PenStyle.SolidLine
        self.edit_vertices = not movable
        self.handles = []
        self.rotation_changed = None
        self._rotation_drag = None
        self.setFlag(QGraphicsItem.GraphicsItemFlag.ItemIsSelectable)
        self.setFlag(QGraphicsItem.GraphicsItemFlag.ItemIsMovable, movable)
        self.setFlag(QGraphicsItem.GraphicsItemFlag.ItemSendsGeometryChanges, movable)
        self.setTransformOriginPoint(self.polygon().boundingRect().center())
        self.apply_style()
        self.rebuild_handles()

    def apply_style(self):
        pen = QPen(self.outline_color, self.line_width, self.line_style)
        pen.setCosmetic(True)
        self.setPen(pen)
        fill = QColor(self.fill_color)
        fill.setAlpha(self.fill_alpha)
        self.setBrush(QBrush(fill))

    def rebuild_handles(self):
        for handle in self.handles:
            scene = handle.scene()
            handle.setParentItem(None)
            if scene is not None:
                scene.removeItem(handle)
        self.handles = [VertexHandle(self, i, point) for i, point in enumerate(self.polygon())]
        self.update_handles()

    def update_handles(self):
        for handle in self.handles:
            handle.setVisible(self.edit_vertices and self.isSelected())

    def itemChange(self, change, value):
        if change == QGraphicsItem.GraphicsItemChange.ItemSelectedHasChanged:
            self.update_handles()
        if change == QGraphicsItem.GraphicsItemChange.ItemRotationHasChanged and self.rotation_changed is not None:
            self.rotation_changed(self)
        if change in (QGraphicsItem.GraphicsItemChange.ItemPositionHasChanged,
                      QGraphicsItem.GraphicsItemChange.ItemScaleHasChanged,
                      QGraphicsItem.GraphicsItemChange.ItemRotationHasChanged) and self.scene() is not None:
            self.scene().setSceneRect(self.scene().sceneRect().united(
                self.sceneBoundingRect().adjusted(-500, -500, 500, 500)))
        return super().itemChange(change, value)

    def move_vertex(self, index, point):
        polygon = self.polygon()
        polygon[index] = point
        self.setPolygon(polygon)

    def remove_vertex(self, index):
        if len(self.polygon()) <= 3:
            return
        polygon = self.polygon()
        polygon.remove(index)
        self.setPolygon(polygon)
        self.rebuild_handles()

    def insert_vertex(self, point):
        polygon = list(self.polygon())
        distances = []
        for i, start in enumerate(polygon):
            end = polygon[(i + 1) % len(polygon)]
            dx, dy = end.x() - start.x(), end.y() - start.y()
            length_squared = dx * dx + dy * dy
            t = 0 if length_squared == 0 else max(0, min(1, (
                (point.x() - start.x()) * dx + (point.y() - start.y()) * dy) / length_squared))
            distances.append((point.x() - start.x() - t * dx) ** 2 +
                             (point.y() - start.y() - t * dy) ** 2)
        polygon.insert(min(range(len(distances)), key=distances.__getitem__) + 1, point)
        self.setPolygon(QPolygonF(polygon))
        self.rebuild_handles()

    def mouseDoubleClickEvent(self, event):
        if self.edit_vertices and event.button() == Qt.MouseButton.LeftButton:
            self.insert_vertex(event.pos())
            event.accept()
            return
        super().mouseDoubleClickEvent(event)

    def mousePressEvent(self, event):
        if event.button() == Qt.MouseButton.LeftButton and event.modifiers() & Qt.KeyboardModifier.ShiftModifier:
            if self.scene() is not None:
                self.scene().clearSelection()
            self.setSelected(True)
            pivot = self.mapToScene(self.transformOriginPoint())
            delta = event.scenePos() - pivot
            angle = degrees(atan2(delta.y(), delta.x())) if delta.x() ** 2 + delta.y() ** 2 > 1e-12 else None
            self._rotation_drag = (pivot, angle, self.rotation())
            event.accept()
            return
        super().mousePressEvent(event)

    def mouseMoveEvent(self, event):
        if self._rotation_drag is not None:
            pivot, initial_angle, initial_rotation = self._rotation_drag
            delta = event.scenePos() - pivot
            if delta.x() ** 2 + delta.y() ** 2 > 1e-12:
                angle = degrees(atan2(delta.y(), delta.x()))
                if initial_angle is None:
                    self._rotation_drag = (pivot, angle, initial_rotation)
                else:
                    self.setRotation((initial_rotation + angle - initial_angle + 180) % 360 - 180)
            event.accept()
            return
        super().mouseMoveEvent(event)

    def mouseReleaseEvent(self, event):
        if event.button() == Qt.MouseButton.LeftButton and self._rotation_drag is not None:
            self._rotation_drag = None
            event.accept()
            return
        super().mouseReleaseEvent(event)

    def copy_to_board(self):
        center = self.polygon().boundingRect().center()
        points = [(p.x() - center.x(), p.y() - center.y()) for p in self.polygon()]
        item = EditablePolygonItem(points, self.name)
        item.outline_color = QColor(self.outline_color)
        item.fill_color = QColor(self.fill_color)
        item.fill_alpha = self.fill_alpha
        item.line_width = self.line_width
        item.line_style = self.line_style
        item.setTransformOriginPoint(self.transformOriginPoint() - center)
        item.setRotation(self.rotation())
        item.setScale(self.scale())
        item.apply_style()
        return item


class CanvasView(QGraphicsView):
    """Wheel zoom and middle-button panning, without changing object scale."""

    def __init__(self, scene):
        super().__init__(scene)
        self.setRenderHint(QPainter.RenderHint.Antialiasing)
        self.setTransformationAnchor(QGraphicsView.ViewportAnchor.AnchorUnderMouse)
        self.setBackgroundBrush(QBrush(QColor("white")))
        self.setMinimumSize(280, 280)
        self._pan_pos = None

    def wheelEvent(self, event):
        factor = 1.2 if event.angleDelta().y() > 0 else 1 / 1.2
        if 0.005 <= self.transform().m11() * factor <= 100:
            self.scale(factor, factor)
        event.accept()

    def mousePressEvent(self, event):
        if event.button() == Qt.MouseButton.MiddleButton:
            self._pan_pos = event.position()
            self.setCursor(Qt.CursorShape.ClosedHandCursor)
            event.accept()
            return
        super().mousePressEvent(event)

    def mouseMoveEvent(self, event):
        if self._pan_pos is not None:
            delta = event.position() - self._pan_pos
            self._pan_pos = event.position()
            self.horizontalScrollBar().setValue(self.horizontalScrollBar().value() - round(delta.x()))
            self.verticalScrollBar().setValue(self.verticalScrollBar().value() - round(delta.y()))
            event.accept()
            return
        super().mouseMoveEvent(event)

    def mouseReleaseEvent(self, event):
        if event.button() == Qt.MouseButton.MiddleButton:
            self._pan_pos = None
            self.unsetCursor()
            event.accept()
            return
        super().mouseReleaseEvent(event)

    def fit_content(self):
        bounds = self.scene().itemsBoundingRect()
        if not bounds.isEmpty():
            self.fitInView(bounds.adjusted(-30, -30, 30, 30), Qt.AspectRatioMode.KeepAspectRatio)
