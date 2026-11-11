"""Numerical checks for source-pixel RGB profiles and software white balance."""

import unittest
import numpy as np
from PyQt6.QtGui import QColor, QImage
from microscope_imaging import image_rgb8, sample_rgb_path, white_balance_gains, balance_rgb8


class ImagingTests(unittest.TestCase):
    def test_rgb_copy_respects_padding_and_channel_order(self):
        image = QImage(3, 2, QImage.Format.Format_RGB888)
        image.fill(QColor(10, 20, 30))
        image.setPixelColor(2, 1, QColor(40, 50, 60))
        rgb = image_rgb8(image)
        self.assertEqual(rgb.shape, (2, 3, 3))
        np.testing.assert_array_equal(rgb[0, 0], [10, 20, 30])
        np.testing.assert_array_equal(rgb[1, 2], [40, 50, 60])
        image.fill(QColor("black"))
        np.testing.assert_array_equal(rgb[1, 2], [40, 50, 60])

    def test_polyline_arc_length_and_bilinear_rgb_on_known_gradient(self):
        y, x = np.mgrid[:8, :9]
        rgb = np.stack([x * 10, y * 20, x * 5 + y * 7], axis=-1).astype(np.uint8)
        distance, coordinates, values = sample_rgb_path(rgb, [(1, 1), (4, 1), (4, 5)])
        np.testing.assert_allclose(distance, np.arange(8))
        np.testing.assert_allclose(coordinates[3], [4, 1])
        np.testing.assert_allclose(values[-1], [40, 100, 55])
        distance, coordinates, values = sample_rgb_path(rgb, [(0.5, 0.5), (2, 0.5)])
        np.testing.assert_allclose(distance, [0, 1, 1.5])
        np.testing.assert_allclose(values, [[5, 10, 6], [15, 10, 11], [20, 10, 13.5]])

    def test_reversed_duplicate_vertices_and_invalid_paths(self):
        rgb = np.zeros((10, 10, 3), dtype=np.uint8)
        distance, coordinates, _ = sample_rgb_path(rgb, [(5, 0), (5, 0), (1, 0)])
        np.testing.assert_array_equal(distance, [0, 1, 2, 3, 4])
        np.testing.assert_array_equal(coordinates[:, 0], [5, 4, 3, 2, 1])
        for points in [[(1, 1), (1, 1)], [(-1, 0), (2, 2)], [(1, 1), (11, 2)], [(1, 1), (float("nan"), 2)]]:
            with self.assertRaises(ValueError):
                sample_rgb_path(rgb, points)

    def test_gray_world_and_clipping_preserve_source(self):
        rgb = np.full((4, 5, 3), [50, 100, 150], dtype=np.uint8)
        gains = white_balance_gains(rgb)
        np.testing.assert_allclose(gains, [2, 1, 2 / 3])
        np.testing.assert_array_equal(balance_rgb8(rgb, gains), np.full_like(rgb, 100))
        np.testing.assert_array_equal(balance_rgb8(rgb, [1, 1, 4])[0, 0], [50, 100, 255])
        np.testing.assert_array_equal(rgb[0, 0], [50, 100, 150])
        np.testing.assert_array_equal(balance_rgb8(rgb, [1, 1, 1]), rgb)

    def test_unusable_reference_and_invalid_gains_fail_explicitly(self):
        with self.assertRaises(ValueError):
            white_balance_gains(np.zeros((2, 2, 3), dtype=np.uint8))
        rgb = np.full((1, 1, 3), 30, dtype=np.uint8)
        for gains in [[0, 1, 1], [1, 1], [1, float("nan"), 1], [17, 1, 1]]:
            with self.assertRaises(ValueError):
                balance_rgb8(rgb, gains)


if __name__ == "__main__":
    unittest.main()
