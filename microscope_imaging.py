"""RGB8 image profiles and reversible software white balance for the frontend."""

import numpy as np
from PyQt6.QtCore import QPointF, QRectF, Qt
from PyQt6.QtGui import QColor, QImage, QPainter, QPainterPath, QPen
from PyQt6.QtWidgets import QWidget


def image_rgb8(image):
    """Copy unpremultiplied RGB display values, respecting QImage row padding."""
    if image.isNull():
        raise ValueError("请先打开图像。")
    converted = image.convertToFormat(QImage.Format.Format_RGB888)
    bits = converted.constBits()
    bits.setsize(converted.sizeInBytes())
    rows = np.frombuffer(bits, np.uint8).reshape(converted.height(), converted.bytesPerLine())
    return rows[:, :converted.width() * 3].reshape(converted.height(), converted.width(), 3).copy()


def white_balance_gains(reference):
    """Gray-world gains; a neutral reference patch uses the same calculation."""
    means = np.asarray(reference, dtype=np.float64).reshape(-1, 3).mean(axis=0)
    if not np.all(np.isfinite(means)) or np.any(means <= 0):
        raise ValueError("参考区域含零值通道，无法计算白平衡；请选择有亮度的中性区域。")
    gains = means.mean() / means
    if np.any(gains < 0.01) or np.any(gains > 16):
        raise ValueError("所需增益超出 0.01–16 倍范围，请重新选择参考区域。")
    return gains


def balance_rgb8(rgb, gains):
    gains = np.asarray(gains, dtype=np.float64)
    if gains.shape != (3,) or not np.all(np.isfinite(gains)) or np.any(gains < 0.01) or np.any(gains > 16):
        raise ValueError("R/G/B 增益必须为 0.01–16 之间的有限数值。")
    return np.clip(np.rint(rgb.astype(np.float32) * gains.astype(np.float32)), 0, 255).astype(np.uint8)


def sample_rgb_path(rgb, points):
    """Sample an open polyline every source pixel of arc length plus its endpoint.

    RGB uses bilinear interpolation. Coordinates on the image's outer right/
    bottom edge use the last pixel; coordinates outside the image are rejected.
    """
    points = np.asarray(points, dtype=np.float64)
    if points.ndim != 2 or points.shape[1] != 2 or len(points) < 2 or not np.all(np.isfinite(points)):
        raise ValueError("路径需要至少两个有效节点。")
    height, width, _ = rgb.shape
    if np.any(points < 0) or np.any(points[:, 0] > width) or np.any(points[:, 1] > height):
        raise ValueError("路径超出图像范围，请移回图像后测量。")
    lengths = np.linalg.norm(np.diff(points, axis=0), axis=1)
    # Repeated vertices have no arc length and must not create zero divisors.
    points = points[np.r_[True, lengths > 1e-9]]
    if len(points) < 2:
        raise ValueError("路径长度为零，请拉开节点。")
    cumulative = np.r_[0.0, np.cumsum(np.linalg.norm(np.diff(points, axis=0), axis=1))]
    total = cumulative[-1]
    if total > 1_000_000:
        raise ValueError("路径超过一百万像素，请缩短路径后测量。")
    distance = np.r_[np.arange(0, total, 1.0), total]
    segment = np.minimum(np.searchsorted(cumulative, distance, side="right") - 1, len(points) - 2)
    fraction = (distance - cumulative[segment]) / (cumulative[segment + 1] - cumulative[segment])
    coordinates = points[segment] + fraction[:, None] * (points[segment + 1] - points[segment])
    x = np.clip(coordinates[:, 0], 0, width - 1)
    y = np.clip(coordinates[:, 1], 0, height - 1)
    x0, y0 = x.astype(int), y.astype(int)
    x1, y1 = np.minimum(x0 + 1, width - 1), np.minimum(y0 + 1, height - 1)
    fx, fy = (x - x0)[:, None], (y - y0)[:, None]
    values = ((1 - fy) * ((1 - fx) * rgb[y0, x0] + fx * rgb[y0, x1]) +
              fy * ((1 - fx) * rgb[y1, x0] + fx * rgb[y1, x1]))
    return distance, coordinates, values


class RGBProfilePlot(QWidget):
    """Small Qt plot with fixed RGB8 range and a nearest-sample cursor."""

    COLORS = ("#d94a46", "#258844", "#307cd4")

    def __init__(self, parent=None):
        super().__init__(parent)
        self.distance = np.empty(0)
        self.values = np.empty((0, 3))
        self.channels = [True, True, True]
        self.unit = "px"
        self.message = "选择测量线，或用 RGB 路径工具绘制折线。"
        self.hover_index = None
        self.setMinimumSize(320, 155)
        self.setMouseTracking(True)

    def set_profile(self, distance, values, unit, message=""):
        self.distance, self.values, self.unit, self.message = distance, values, unit, message
        self.hover_index = None
        self.update()

    def plot_rect(self):
        return QRectF(self.rect()).adjusted(48, 30, -18, -32)

    def paintEvent(self, event):
        painter = QPainter(self)
        painter.fillRect(self.rect(), QColor("#ffffff"))
        painter.setRenderHint(QPainter.RenderHint.Antialiasing)
        rect = self.plot_rect()
        painter.setPen(QColor("#65788b"))
        painter.drawText(QPointF(8, 17), "RGB 强度（0–255）")
        if not len(self.distance):
            painter.drawText(rect, Qt.AlignmentFlag.AlignCenter, self.message)
            return
        for value in (0, 64, 128, 192, 255):
            y = rect.bottom() - value / 255 * rect.height()
            painter.setPen(QPen(QColor("#e2e8ef"), 1))
            painter.drawLine(QPointF(rect.left(), y), QPointF(rect.right(), y))
            painter.setPen(QColor("#65788b"))
            painter.drawText(QRectF(0, y - 9, 40, 18), Qt.AlignmentFlag.AlignRight, str(value))
        total = self.distance[-1]
        for fraction in (0, 0.25, 0.5, 0.75, 1):
            x = rect.left() + fraction * rect.width()
            painter.drawText(QRectF(x - 30, rect.bottom() + 3, 60, 17), Qt.AlignmentFlag.AlignCenter,
                             f"{total * fraction:.1f}")
        painter.drawText(QRectF(rect.left(), self.height() - 15, rect.width(), 15),
                         Qt.AlignmentFlag.AlignCenter, f"沿路径距离（{self.unit}）")
        # Keep rendering bounded without changing the measurements/export.
        # Each screen column retains both extrema, including narrow peaks.
        bin_ids = np.minimum((self.distance / total * rect.width()).astype(int), int(rect.width()))
        starts = np.r_[0, np.flatnonzero(np.diff(bin_ids)) + 1]
        ends = np.r_[starts[1:], len(self.distance)]
        for channel, color in enumerate(self.COLORS):
            if not self.channels[channel]:
                continue
            indices = {0, len(self.distance) - 1}
            for start, end in zip(starts, ends):
                part = self.values[start:end, channel]
                indices.add(int(start + np.argmin(part)))
                indices.add(int(start + np.argmax(part)))
            path = QPainterPath()
            for n, index in enumerate(sorted(indices)):
                point = QPointF(rect.left() + self.distance[index] / total * rect.width(),
                                rect.bottom() - self.values[index, channel] / 255 * rect.height())
                if n == 0:
                    path.moveTo(point)
                else:
                    path.lineTo(point)
            painter.setPen(QPen(QColor(color), 1.5))
            painter.drawPath(path)
        if self.hover_index is not None:
            index = self.hover_index
            x = rect.left() + self.distance[index] / total * rect.width()
            painter.setPen(QPen(QColor("#728397"), 1, Qt.PenStyle.DashLine))
            painter.drawLine(QPointF(x, rect.top()), QPointF(x, rect.bottom()))
            r, g, b = self.values[index]
            painter.drawText(QRectF(165, 0, self.width() - 175, 24), Qt.AlignmentFlag.AlignRight,
                             f"{self.distance[index]:.2f} {self.unit}   R {r:.1f}   G {g:.1f}   B {b:.1f}")

    def mouseMoveEvent(self, event):
        rect = self.plot_rect()
        if len(self.distance) and rect.contains(event.position()):
            value = (event.position().x() - rect.left()) / rect.width() * self.distance[-1]
            self.hover_index = int(np.argmin(np.abs(self.distance - value)))
        else:
            self.hover_index = None
        self.update()

    def leaveEvent(self, event):
        self.hover_index = None
        self.update()
        super().leaveEvent(event)
