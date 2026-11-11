"""Real Qt scene interactions and synthetic-photo contour regression tests."""

import os
import importlib.util
import json
import base64
from copy import deepcopy
import sys
from pathlib import Path
import tempfile
import time
import unittest
from unittest.mock import patch

os.environ.setdefault("QT_QPA_PLATFORM", "offscreen")

import cv2
import numpy as np
from PyQt6.QtCore import QPoint, QPointF, QRect, Qt, QSaveFile
from PyQt6.QtGui import QColor, QImage, QPixmap
from PyQt6.QtTest import QTest
from PyQt6.QtWidgets import QApplication, QMainWindow, QWidget, QMessageBox

from stack_designer import FlakeItem, StackDesignerWindow, ScreenCaptureOverlay, extract_contours, LINE_STYLES

REAL_QT_MODULES = {name: sys.modules[name] for name in (
    "PyQt6", "PyQt6.QtCore", "PyQt6.QtGui", "PyQt6.QtWidgets")}


def sample_photo():
    rgb = np.full((360, 480, 3), (170, 160, 190), dtype=np.uint8)
    points = np.array([[50, 45], [230, 65], [220, 130], [130, 130], [125, 260], [55, 235]], np.int32)
    cv2.fillPoly(rgb, [points], (80, 120, 155))
    cv2.rectangle(rgb, (310, 180), (425, 290), (210, 200, 120), -1)
    return rgb, points


class ContourTests(unittest.TestCase):
    def test_detects_multiple_colored_flakes_and_preserves_concavity(self):
        rgb, expected = sample_photo()
        polygons = extract_contours(rgb)
        self.assertEqual(len(polygons), 2)
        target = np.zeros(rgb.shape[:2], np.uint8)
        actual = target.copy()
        cv2.fillPoly(target, [expected], 1)
        cv2.fillPoly(actual, [np.array(polygons[0], np.int32)], 1)
        self.assertGreater(np.sum(actual & target) / np.sum(actual | target), 0.95)
        self.assertFalse(cv2.isContourConvex(np.array(polygons[0], np.int32)))

    def test_uniform_image_fails_explicitly(self):
        with self.assertRaisesRegex(ValueError, "未识别"):
            extract_contours(np.full((200, 200, 3), 128, np.uint8))

    def test_area_filter_and_manual_threshold(self):
        rgb, _ = sample_photo()
        self.assertEqual(len(extract_contours(rgb, threshold=20, min_area_percent=8)), 1)

    def test_large_photo_returns_original_pixel_coordinates(self):
        rgb = np.full((2000, 2400, 3), 180, np.uint8)
        cv2.rectangle(rgb, (300, 400), (1500, 1700), (40, 90, 130), -1)
        points = np.array(extract_contours(rgb)[0])
        np.testing.assert_allclose(points.min(axis=0), [300, 400], atol=5)
        np.testing.assert_allclose(points.max(axis=0), [1500, 1700], atol=5)

    def test_invalid_parameters_rejected(self):
        rgb, _ = sample_photo()
        with self.assertRaises(ValueError):
            extract_contours(rgb, min_area_percent=0)


class DesignerTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.app = QApplication.instance() or QApplication([])

    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.photo = Path(self.temp.name) / "薄片照片.png"
        rgb, _ = sample_photo()
        image = QImage(rgb.data, rgb.shape[1], rgb.shape[0], rgb.strides[0], QImage.Format.Format_RGB888)
        self.assertTrue(image.save(str(self.photo)))
        self.window = StackDesignerWindow()
        self.window.show()
        self.app.processEvents()

    def tearDown(self):
        self.window.wait_for_extraction()
        self.app.processEvents()
        self.window.close()
        self.window.deleteLater()
        self.app.processEvents()
        self.temp.cleanup()

    def load_photo(self):
        self.window.load_photo(self.photo)
        self.finish_extraction()
        self.assertEqual(len(self.window.candidates), 2)

    def finish_extraction(self):
        deadline = time.monotonic() + 10
        while self.window.worker is not None and time.monotonic() < deadline:
            self.app.processEvents()
            QTest.qWait(10)
        self.assertIsNone(self.window.worker, "Contour worker did not finish")

    def test_import_edit_style_copy_and_reimport_preserves_board(self):
        self.load_photo()
        source = self.window.candidates[0]
        source.handles[0].setPos(QPointF(40, 40))
        self.assertEqual(source.polygon()[0], QPointF(40, 40))
        source.insert_vertex(QPointF(80, 40))
        count = len(source.polygon())
        source.remove_vertex(1)
        self.assertEqual(len(source.polygon()), count - 1)
        self.window.line_style_combo.setCurrentText("虚线")
        self.window.line_width.setValue(4)
        self.window.fill_opacity.setValue(40)
        with patch("stack_designer.QColorDialog.getColor", return_value=QColor("#e53935")):
            self.window.choose_color("outline_color")
        self.window.add_to_board()
        item = self.window.layers[0]
        self.assertEqual(item.pen().style(), Qt.PenStyle.DashLine)
        self.assertEqual(item.pen().widthF(), 4)
        self.assertEqual(item.pen().color().name(), "#e53935")
        self.assertEqual(item.brush().color().alpha(), 102)
        before = item.polygon()
        source.handles[0].setPos(QPointF(70, 70))
        self.assertEqual(item.polygon(), before)
        self.window.rotation.setValue(30)
        self.window.object_scale.setValue(1.5)
        self.assertEqual(item.rotation(), 30)
        self.assertEqual(item.scale(), 1.5)
        self.load_photo()
        self.assertEqual(self.window.layers, [item])
        self.assertEqual(item.polygon(), before)
        self.window.add_to_board()
        self.assertEqual(len(self.window.layers), 2)

    def manual_photo(self, tool="wand"):
        self.window.photo_tabs.setCurrentIndex(1)
        self.window.selection_tool.setCurrentIndex(self.window.selection_tool.findData(tool))
        self.window.load_photo(self.photo)
        self.app.processEvents()
        self.assertIsNone(self.window.worker)
        return self.window.photo_selection.model

    def photo_click(self, x, y, modifiers=Qt.KeyboardModifier.NoModifier):
        view = self.window.photo_view
        QTest.mouseClick(view.viewport(), Qt.MouseButton.LeftButton, modifiers, view.mapFromScene(QPointF(x, y)))

    def test_manual_wand_modifiers_undo_and_generate_append(self):
        model = self.manual_photo()
        self.photo_click(90, 90)
        first = model.mask.copy()
        self.assertGreater(np.count_nonzero(first), 1000)
        self.photo_click(350, 220, Qt.KeyboardModifier.ShiftModifier)
        both = model.mask.copy()
        self.assertGreater(np.count_nonzero(both), np.count_nonzero(first))
        self.photo_click(350, 220, Qt.KeyboardModifier.AltModifier)
        np.testing.assert_array_equal(model.mask, first)
        QTest.keyClick(self.window.photo_view, Qt.Key.Key_Z, Qt.KeyboardModifier.ControlModifier)
        np.testing.assert_array_equal(model.mask, both)
        self.window.generate_selection_button.click()
        self.assertEqual(len(self.window.candidates), 2)
        self.assertEqual(self.window.photo_selection.tool, "edit")
        self.assertTrue(self.window.add_button.isEnabled())
        existing = self.window.candidates[0]
        existing.setRotation(17)
        self.window.add_button.click()
        self.assertEqual(self.window.layers[0].rotation(), 17)
        self.window.selection_tool.setCurrentIndex(0)
        self.assertFalse(existing.isVisible())
        self.assertFalse(self.window.add_button.isEnabled())
        self.photo_click(350, 220)
        self.window.generate_selection_button.click()
        self.assertEqual(len(self.window.candidates), 3)
        self.assertIs(self.window.candidates[0], existing)
        self.assertEqual(existing.rotation(), 17)
        self.assertEqual(self.window.candidate_combo.currentIndex(), 2)

    def test_lasso_keyboard_completion_cancel_and_reimport(self):
        model = self.manual_photo("lasso")
        for point in ((50, 50), (250, 50), (250, 200)):
            self.photo_click(*point)
        self.assertFalse(self.window.generate_selection_button.isEnabled())
        QTest.keyClick(self.window.photo_view, Qt.Key.Key_Backspace)
        self.assertEqual(len(self.window.photo_selection.lasso), 2)
        self.photo_click(150, 200)
        QTest.keyClick(self.window.photo_view, Qt.Key.Key_Return)
        self.assertTrue(self.window.generate_selection_button.isEnabled())
        self.assertGreater(np.count_nonzero(model.mask), 1000)
        before = model.mask.copy()
        self.photo_click(300, 300)
        QTest.keyClick(self.window.photo_view, Qt.Key.Key_Escape)
        np.testing.assert_array_equal(model.mask, before)
        self.window.load_photo(self.photo)
        self.assertFalse(self.window.photo_selection.model.mask.any())
        self.assertFalse(self.window.photo_selection.model.undo_stack)
        self.assertFalse(self.window.photo_selection.has_draft)

    def test_rectangle_and_brush_are_one_undoable_gesture(self):
        model = self.manual_photo("rectangle")
        view = self.window.photo_view
        start = view.mapFromScene(QPointF(60, 60))
        end = view.mapFromScene(QPointF(180, 180))
        QTest.mousePress(view.viewport(), Qt.MouseButton.LeftButton, pos=start)
        QTest.mouseMove(view.viewport(), end)
        QTest.mouseRelease(view.viewport(), Qt.MouseButton.LeftButton, pos=end)
        rectangle = model.mask.copy()
        self.assertTrue(rectangle[100, 100])
        self.assertEqual(len(model.undo_stack), 1)
        self.window.selection_tool.setCurrentIndex(self.window.selection_tool.findData("brush"))
        self.window.selection_operation.setCurrentIndex(self.window.selection_operation.findData("subtract"))
        self.photo_click(100, 100)
        self.assertFalse(model.mask[100, 100])
        self.assertEqual(len(model.undo_stack), 2)
        with patch("stack_designer.QMessageBox.warning") as warning:
            self.window.generate_selection_button.click()
        self.assertIn("孔洞", warning.call_args.args[2])
        self.assertFalse(self.window.candidates)
        self.window.selection_actions["undo"].click()
        np.testing.assert_array_equal(model.mask, rectangle)
        self.window.selection_actions["redo"].click()
        self.assertFalse(model.mask[100, 100])
        self.window.selection_actions["fill"].click()
        np.testing.assert_array_equal(model.mask, rectangle)
        self.window.generate_selection_button.click()
        self.window.add_button.click()
        self.assertEqual(len(self.window.layers), 1)

    def test_layer_order_list_selection_and_deletion(self):
        self.load_photo()
        self.window.add_to_board()
        first = self.window.layers[0]
        self.window.add_to_board()
        second = self.window.layers[1]
        self.window.reorder_layer(-1)
        self.assertEqual(self.window.layers, [second, first])
        self.assertGreater(first.zValue(), second.zValue())
        self.window.reorder_layer(100)
        self.assertEqual(self.window.layers, [first, second])
        self.window.layer_list.setCurrentRow(1)
        self.assertIs(self.window.selected_layer(), first)
        self.assertIs(self.window.active_item, first)
        self.window.reorder_layer(1)
        self.assertEqual(self.window.layers, [second, first])
        self.window.delete_layer()
        self.assertEqual(self.window.layers, [second])
        self.assertIsNone(first.scene())
        self.assertEqual(second.zValue(), 0)

    def test_mouse_drags_board_polygon_and_photo_vertex(self):
        self.load_photo()
        source = self.window.candidates[0]
        handle = source.handles[0]
        old_vertex = QPointF(handle.pos())
        view = self.window.photo_view
        start = view.mapFromScene(handle.scenePos())
        QTest.mousePress(view.viewport(), Qt.MouseButton.LeftButton, pos=start)
        QTest.mouseMove(view.viewport(), start + QPoint(15, 10), delay=30)
        QTest.mouseRelease(view.viewport(), Qt.MouseButton.LeftButton, pos=start + QPoint(15, 10))
        self.assertNotEqual(source.polygon()[0], old_vertex)
        self.window.add_to_board()
        item = self.window.layers[0]
        old_position = QPointF(item.pos())
        view = self.window.board_view
        start = view.mapFromScene(item.mapToScene(QPointF(-40, -20)))
        QTest.mousePress(view.viewport(), Qt.MouseButton.LeftButton, pos=start)
        QTest.mouseMove(view.viewport(), start + QPoint(40, 30), delay=30)
        QTest.mouseRelease(view.viewport(), Qt.MouseButton.LeftButton, pos=start + QPoint(40, 30))
        self.assertNotEqual(item.pos(), old_position)
        self.window.edit_nodes.setChecked(True)
        self.assertTrue(all(handle.isVisible() for handle in item.handles))

    def test_photo_outline_translates_and_rotates_before_copying_to_board(self):
        self.load_photo()
        source = self.window.candidates[0]
        self.window.edit_photo_nodes.setChecked(False)
        self.assertTrue(all(not handle.isVisible() for handle in source.handles))
        vertices = source.polygon()
        image = self.window.image.copy()
        view = self.window.photo_view
        start = view.mapFromScene(source.mapToScene(QPointF(80, 90)))
        end = start + QPoint(30, 20)
        expected_position = source.pos() + view.mapToScene(end) - view.mapToScene(start)
        QTest.mousePress(view.viewport(), Qt.MouseButton.LeftButton, pos=start)
        QTest.mouseMove(view.viewport(), end, delay=30)
        QTest.mouseRelease(view.viewport(), Qt.MouseButton.LeftButton, pos=end)
        self.assertAlmostEqual(source.pos().x(), expected_position.x(), places=6)
        self.assertAlmostEqual(source.pos().y(), expected_position.y(), places=6)
        self.assertEqual(source.polygon(), vertices)
        self.assertEqual(self.window.image, image)
        self.assertTrue(self.window.rotation.isEnabled())
        self.window.rotation.setValue(33.33)
        self.assertEqual(source.rotation(), 33.33)
        source.move_vertex(0, QPointF(-15.25, -35.75))
        source_points = source.mapToScene(source.polygon())
        self.window.add_to_board()
        copied = self.window.layers[-1]
        self.assertEqual(copied.rotation(), source.rotation())
        copied_points = copied.mapToScene(copied.polygon())
        np.testing.assert_allclose(
            [[p.x() - copied_points[0].x(), p.y() - copied_points[0].y()] for p in copied_points],
            [[p.x() - source_points[0].x(), p.y() - source_points[0].y()] for p in source_points], atol=1e-10,
        )

    def rotate_with_mouse(self, item, view):
        polygon = item.polygon()
        interior = (polygon[0] + polygon[1] + polygon[2]) / 3
        pivot = item.mapToScene(item.transformOriginPoint())
        start_point = item.mapToScene(interior)
        delta = start_point - pivot
        end_point = pivot + QPointF(-delta.y(), delta.x())
        start, end = view.mapFromScene(start_point), view.mapFromScene(end_point)
        before_angle, before_pos = item.rotation(), QPointF(item.pos())
        QTest.mousePress(view.viewport(), Qt.MouseButton.LeftButton,
                         Qt.KeyboardModifier.ShiftModifier, pos=start)
        QTest.mouseMove(view.viewport(), end, delay=30)
        QTest.mouseRelease(view.viewport(), Qt.MouseButton.LeftButton,
                           Qt.KeyboardModifier.ShiftModifier, pos=end)
        change = (item.rotation() - before_angle + 180) % 360 - 180
        self.assertAlmostEqual(change, 90, delta=1.5)
        self.assertEqual(item.pos(), before_pos)
        self.assertEqual(item.polygon(), polygon)
        self.assertEqual(self.window.rotation.value(), round(item.rotation(), 2))
        self.assertIsNone(item._rotation_drag)

    def test_shift_drag_rotates_photo_and_board_without_moving_vertices(self):
        self.load_photo()
        self.window.edit_photo_nodes.setChecked(False)
        self.rotate_with_mouse(self.window.candidates[0], self.window.photo_view)
        self.window.add_to_board()
        self.rotate_with_mouse(self.window.layers[0], self.window.board_view)

    def test_dragging_vertex_on_rotated_board_changes_only_that_vertex(self):
        self.load_photo()
        self.window.add_to_board()
        item = self.window.layers[0]
        self.window.rotation.setValue(35.5)
        self.window.object_scale.setValue(1.3)
        self.window.edit_nodes.setChecked(True)
        before_pos, before_polygon = QPointF(item.pos()), item.polygon()
        view = self.window.board_view
        start = view.mapFromScene(item.handles[0].scenePos())
        end = start + QPoint(25, -15)
        QTest.mousePress(view.viewport(), Qt.MouseButton.LeftButton, pos=start)
        QTest.mouseMove(view.viewport(), end, delay=30)
        QTest.mouseRelease(view.viewport(), Qt.MouseButton.LeftButton, pos=end)
        self.assertNotEqual(item.polygon()[0], before_polygon[0])
        self.assertEqual(list(item.polygon())[1:], list(before_polygon)[1:])
        self.assertEqual(item.pos(), before_pos)
        self.assertEqual(item.rotation(), 35.5)
        self.assertEqual(item.scale(), 1.3)

    def test_reloaded_rotation_updates_controls_and_style_does_not_round_geometry(self):
        self.load_photo()
        self.window.add_to_board()
        path = Path(self.temp.name) / "rotated.hstack.json"
        self.window.save_project_to(path)
        self.window.install_project(self.window.read_project(path), path)
        item = self.window.layers[0]
        item.setRotation(22.123456)
        self.assertEqual(self.window.rotation.value(), 22.12)
        self.window.line_width.setValue(3)
        self.assertEqual(item.rotation(), 22.123456)

    def test_no_fewer_than_three_vertices_and_switch_preserves_edits(self):
        self.load_photo()
        source = self.window.candidates[0]
        while len(source.polygon()) > 3:
            source.remove_vertex(0)
        source.remove_vertex(0)
        self.assertEqual(len(source.polygon()), 3)
        self.window.candidate_combo.setCurrentIndex(1)
        self.window.candidate_combo.setCurrentIndex(0)
        self.assertEqual(len(source.polygon()), 3)
        self.assertTrue(source.isSelected())

    def test_mouse_adds_and_deletes_vertex_and_switches_property_target(self):
        self.load_photo()
        source = self.window.candidates[0]
        self.window.add_to_board()
        board_item = self.window.layers[0]
        self.assertFalse(source.isSelected())
        view = self.window.photo_view
        inside = view.mapFromScene(QPointF(85, 90))
        QTest.mouseClick(view.viewport(), Qt.MouseButton.LeftButton, pos=inside)
        self.assertIs(self.window.active_item, source)
        self.assertFalse(board_item.isSelected())
        self.window.line_width.setValue(6)
        self.assertEqual(source.line_width, 6)
        self.assertEqual(board_item.line_width, 2)
        count = len(source.polygon())
        QTest.mouseDClick(view.viewport(), Qt.MouseButton.LeftButton, pos=inside)
        QTest.mouseRelease(view.viewport(), Qt.MouseButton.LeftButton, pos=inside)
        self.assertEqual(len(source.polygon()), count + 1)
        handle = source.handles[0]
        QTest.mouseClick(view.viewport(), Qt.MouseButton.RightButton, pos=view.mapFromScene(handle.scenePos()))
        self.assertEqual(len(source.polygon()), count)

    def test_worker_failure_is_reported_and_import_reenabled(self):
        image = QImage(200, 200, QImage.Format.Format_RGB888)
        image.fill(QColor("gray"))
        self.assertTrue(image.save(str(self.photo)))
        with patch("stack_designer.QMessageBox.warning") as warning:
            self.window.load_photo(self.photo)
            deadline = time.monotonic() + 10
            while self.window.worker is not None and time.monotonic() < deadline:
                self.app.processEvents()
                QTest.qWait(10)
            self.assertIsNone(self.window.worker)
            warning.assert_called_once()
        self.assertFalse(self.window.candidates)
        self.assertFalse(self.window.add_button.isEnabled())
        self.assertTrue(self.window.import_button.isEnabled())

    def test_bad_photo_keeps_current_work(self):
        self.load_photo()
        source = self.window.candidates[0]
        with self.assertRaisesRegex(ValueError, "无法读取"):
            self.window.load_photo(Path(self.temp.name) / "missing.png")
        self.assertIs(self.window.candidates[0], source)

    def test_close_reopen_retains_layers(self):
        self.load_photo()
        self.window.add_to_board()
        item = self.window.layers[0]
        self.window.close()
        self.window.show()
        self.assertEqual(self.window.layers, [item])

    def add_export_rectangle(self, color="red"):
        self.window.board_scene.clearSelection()
        item = FlakeItem([(0, 0), (100, 0), (100, 80), (0, 80)], "export")
        item.outline_color = QColor(color)
        item.line_width = 4
        item.fill_alpha = 255
        item.apply_style()
        self.window.board_scene.addItem(item)
        self.window.layers.append(item)
        self.window.refresh_layers(item)
        item.setSelected(True)
        self.window.edit_nodes.setChecked(True)
        return item

    def test_export_png_contains_only_outlines_with_transparent_interior(self):
        item = self.add_export_rectangle()
        before_brush = item.brush()
        image = self.window.render_outline_image()
        self.assertEqual((image.width(), image.height()), (112, 92))
        self.assertEqual(image.pixelColor(56, 46).alpha(), 0)
        self.assertEqual(image.pixelColor(0, 0).alpha(), 0)
        self.assertEqual(image.pixelColor(6, 46), QColor("red"))
        self.assertTrue(all(image.pixelColor(x, y).alpha() == 0
                            for x in range(10, 102) for y in range(10, 82)))
        self.assertEqual(item.brush(), before_brush)
        self.assertTrue(item.isSelected())
        self.assertTrue(all(handle.isVisible() for handle in item.handles))
        self.window.board_view.scale(3, 3)
        self.assertEqual(image, self.window.render_outline_image())
        path = str(Path(self.temp.name) / "透明线框.png")
        with patch("stack_designer.QFileDialog") as dialog:
            dialog.return_value.exec.return_value = 1
            dialog.return_value.selectedFiles.return_value = [path]
            self.window.export_png()
        restored = QImage(path)
        self.assertFalse(restored.isNull())
        self.assertTrue(restored.hasAlphaChannel())
        self.assertEqual(restored.pixelColor(56, 46).alpha(), 0)
        self.assertEqual(restored.pixelColor(6, 46), QColor("red"))

    def test_export_preserves_transforms_line_style_and_layer_order(self):
        first = self.add_export_rectangle()
        second = self.add_export_rectangle("blue")
        self.assertEqual(self.window.render_outline_image().pixelColor(6, 46), QColor("blue"))
        self.window.reorder_layer(-1)
        self.assertEqual(self.window.render_outline_image().pixelColor(6, 46), QColor("red"))
        self.window.delete_layer()
        self.assertEqual(self.window.layers, [first])
        first.setPos(-1200, 900)
        first.setScale(2)
        first.setRotation(90)
        solid = self.window.render_outline_image()
        self.assertEqual((solid.width(), solid.height()), (172, 212))
        self.assertEqual(solid.pixelColor(86, 106).alpha(), 0)
        self.assertEqual(solid.pixelColor(6, 106), QColor("red"))
        first.line_style = Qt.PenStyle.DashLine
        first.apply_style()
        dashed = self.window.render_outline_image()
        self.assertLess(sum(dashed.pixelColor(6, y).alpha() for y in range(212)),
                        sum(solid.pixelColor(6, y).alpha() for y in range(212)))

    def test_export_reports_empty_board_oversize_and_write_failure(self):
        with self.assertRaisesRegex(ValueError, "白板为空"):
            self.window.render_outline_image()
        item = self.add_export_rectangle()
        item.setScale(1000)
        with self.assertRaisesRegex(ValueError, "尺寸过大"):
            self.window.render_outline_image()
        item.setScale(1)
        with patch("stack_designer.QFileDialog") as dialog, \
             patch("stack_designer.QImage.save", return_value=False), \
             patch("stack_designer.QMessageBox.warning") as warning:
            dialog.return_value.exec.return_value = 1
            dialog.return_value.selectedFiles.return_value = [str(Path(self.temp.name) / "fail.png")]
            self.window.export_png()
            warning.assert_called_once()
            self.assertIn("写入失败", warning.call_args.args[2])

    def test_project_roundtrip_preserves_geometry_styles_order_and_editability_without_photos(self):
        self.load_photo()
        for index, style in enumerate(LINE_STYLES.values()):
            self.window.add_to_board()
            item = self.window.layers[-1]
            item.name = f"薄片 {index}"
            item.setPos(-120.123456789 + index * 30, 75.987654321 - index * 20)
            item.setRotation(-35.25 + index * 10)
            item.setScale(0.725 + index * 0.3)
            item.outline_color = QColor(30 + index * 30, 70, 180, 180 + index * 20)
            item.fill_color = QColor(70, 120 + index * 20, 160)
            item.fill_alpha = 65 + index * 30
            item.line_width = 1.5 + index
            item.line_style = style
            item.apply_style()
            item.move_vertex(0, QPointF(-250.123456789, -200.987654321))
            self.assertNotEqual(item.transformOriginPoint(), item.polygon().boundingRect().center())
        self.window.reorder_layer(-2)
        before = self.window.project_data()
        before_image = self.window.render_outline_image()
        before_geometry = [item.mapToScene(item.polygon()) for item in self.window.layers]
        path = Path(self.temp.name) / "多层工程.hstack.json"
        self.window.save_project_to(path)
        saved = json.loads(path.read_text(encoding="utf-8"))
        self.assertEqual(saved, before)
        self.assertEqual(set(saved), {"format", "version", "layers", "background"})
        self.assertIsNone(saved["background"])
        self.assertNotIn(str(self.photo), path.read_text(encoding="utf-8"))
        self.photo.unlink()

        restored = StackDesignerWindow()
        try:
            restored.install_project(restored.read_project(path), path)
            self.assertIsNone(restored.image)
            self.assertEqual(restored.project_data(), before)
            self.assertEqual(restored.render_outline_image(), before_image)
            self.assertEqual([item.mapToScene(item.polygon()) for item in restored.layers], before_geometry)
            self.assertEqual([item.zValue() for item in restored.layers], [0, 1, 2, 3])
            self.assertEqual(restored.layer_list.item(0).text(), before["layers"][-1]["name"])
            restored.edit_nodes.setChecked(True)
            top = restored.layers[-1]
            top.handles[0].setPos(QPointF(13.125, -27.75))
            self.assertEqual(top.polygon()[0], QPointF(13.125, -27.75))
            top.moveBy(10, -20)
            restored.reorder_layer(-1)
            edited = restored.project_data()
            restored.save_project_to(path)
            restored.install_project(restored.read_project(path), path)
            self.assertEqual(restored.project_data(), edited)
        finally:
            restored.close()
            restored.deleteLater()
            self.app.processEvents()

    def test_open_project_cancel_then_replace_and_preserve_photo_panel(self):
        self.load_photo()
        self.window.add_to_board()
        path = Path(self.temp.name) / "saved.hstack.json"
        self.window.save_project_to(path)
        saved = self.window.project_data()
        self.window.layers[0].moveBy(200, 100)
        unsaved = self.window.project_data()
        candidate = self.window.candidates[0]
        with patch("stack_designer.QFileDialog.getOpenFileName", return_value=(str(path), "")), \
             patch("stack_designer.QMessageBox.question", return_value=QMessageBox.StandardButton.Cancel):
            self.window.open_project()
        self.assertEqual(self.window.project_data(), unsaved)
        with patch("stack_designer.QFileDialog.getOpenFileName", return_value=(str(path), "")), \
             patch("stack_designer.QMessageBox.question", return_value=QMessageBox.StandardButton.Yes):
            self.window.open_project()
        self.assertEqual(self.window.project_data(), saved)
        self.assertIs(self.window.candidates[0], candidate)
        self.assertIsNotNone(self.window.image)

    def test_invalid_project_never_replaces_existing_board(self):
        self.add_export_rectangle()
        original = self.window.project_data()
        path = Path(self.temp.name) / "invalid.hstack.json"
        documents = [None, [], {}, {**original, "version": 3}, {**original, "version": True},
                     {**original, "layers": {}}, {**original, "layers": [{}]}]
        invalid_fields = {
            "vertices": [[], [[0, 0], [1, 1]], [[0, 0], [1], [2, 2]], [[0, 0], [1, 1], [float('nan'), 2]]],
            "position": [[True, 0], [float('inf'), 0], [10 ** 400, 0]],
            "transform_origin": [None],
            "rotation": ["30", 361],
            "scale": [0, -1, 1001],
            "line_width": [0],
            "fill_alpha": [1.5, 256, True],
            "outline_color": ["not a color", None],
            "line_style": ["unknown", []],
            "name": ["", 123],
        }
        for field, values in invalid_fields.items():
            for value in values:
                invalid = deepcopy(original)
                invalid["layers"][0][field] = value
                documents.append(invalid)
        texts = [json.dumps(document) for document in documents] + ['{"format":', '{"layers": []}']
        for text in texts:
            with self.subTest(document=text):
                path.write_text(text, encoding="utf-8")
                with patch("stack_designer.QFileDialog.getOpenFileName", return_value=(str(path), "")), \
                     patch("stack_designer.QMessageBox.warning") as warning, \
                     patch("stack_designer.QMessageBox.question") as question:
                    self.window.open_project()
                    warning.assert_called_once()
                    question.assert_not_called()
                self.assertEqual(self.window.project_data(), original)
                self.assertIsNone(self.window.project_path)

    def test_project_save_dialog_and_empty_board_roundtrip(self):
        path = Path(self.temp.name) / "空白板.hstack.json"
        with patch("stack_designer.QFileDialog") as dialog:
            dialog.return_value.exec.return_value = 1
            dialog.return_value.selectedFiles.return_value = [str(path)]
            self.window.save_project()
        self.assertEqual(json.loads(path.read_text(encoding="utf-8"))["layers"], [])
        self.add_export_rectangle()
        self.window.install_project(self.window.read_project(path), path)
        self.assertEqual(self.window.layers, [])
        self.assertEqual(self.window.layer_list.count(), 0)
        self.assertIsNone(self.window.active_item)
        self.assertFalse(self.window.properties.isEnabled())

    def test_failed_atomic_save_preserves_existing_file(self):
        self.add_export_rectangle()
        path = Path(self.temp.name) / "protected.hstack.json"
        self.window.save_project_to(path)
        before = path.read_bytes()
        self.window.layers[0].moveBy(30, 40)

        class InterruptedSave(QSaveFile):
            def write(self, payload):
                super().write(payload[:12])
                return -1

        with patch("stack_designer.QSaveFile", InterruptedSave):
            with self.assertRaisesRegex(OSError, "写入失败"):
                self.window.save_project_to(path)
        self.assertEqual(path.read_bytes(), before)
        self.assertEqual(list(Path(self.temp.name).glob("protected*")), [path])

    def test_project_file_access_failures_are_reported(self):
        self.add_export_rectangle()
        before = self.window.project_data()
        missing = Path(self.temp.name) / "missing" / "project.hstack.json"
        with patch("stack_designer.QFileDialog.getOpenFileName", return_value=(str(missing), "")), \
             patch("stack_designer.QMessageBox.warning") as warning:
            self.window.open_project()
            warning.assert_called_once()
        self.assertEqual(self.window.project_data(), before)
        with patch("stack_designer.QFileDialog") as dialog, \
             patch("stack_designer.QMessageBox.warning") as warning:
            dialog.return_value.exec.return_value = 1
            dialog.return_value.selectedFiles.return_value = [str(missing)]
            self.window.save_project()
            warning.assert_called_once()
        self.assertIsNone(self.window.project_path)

    def test_capture_crop_maps_logical_coordinates_to_native_pixels(self):
        image = QImage(200, 160, QImage.Format.Format_RGB32)
        image.fill(QColor("blue"))
        for x in range(20, 100):
            for y in range(30, 90):
                image.setPixelColor(x, y, QColor("red"))
        pixmap = QPixmap.fromImage(image)
        pixmap.setDevicePixelRatio(2)
        overlay = ScreenCaptureOverlay(pixmap, QRect(-100, 25, 100, 80))
        cropped = overlay.crop_selection(QRect(10, 15, 40, 30))
        self.assertEqual(cropped.size(), image.copy(20, 30, 80, 60).size())
        self.assertEqual(cropped, image.copy(20, 30, 80, 60))
        self.assertEqual(cropped.devicePixelRatio(), 1)
        overlay.deleteLater()

    def test_capture_overlay_mouse_selection_and_escape(self):
        image = QImage(240, 180, QImage.Format.Format_RGB32)
        image.fill(QColor("#2468ac"))
        overlay = ScreenCaptureOverlay(QPixmap.fromImage(image), QRect(0, 0, 240, 180))
        captures, cancellations = [], []
        overlay.captured.connect(captures.append)
        overlay.cancelled.connect(lambda: cancellations.append(True))
        overlay.show()
        self.app.processEvents()
        QTest.mousePress(overlay, Qt.MouseButton.LeftButton, pos=QPoint(80, 70))
        QTest.mouseMove(overlay, QPoint(20, 25))
        QTest.mouseRelease(overlay, Qt.MouseButton.LeftButton, pos=QPoint(20, 25))
        self.assertEqual(len(captures), 1)
        self.assertEqual(captures[0], image.copy(20, 25, 60, 45))
        QTest.keyClick(overlay, Qt.Key.Key_Escape)
        self.assertEqual(cancellations, [True])
        overlay.hide()
        overlay.deleteLater()

    def test_capture_hides_window_and_imports_selected_image_then_cancel_preserves_work(self):
        screen = QApplication.primaryScreen()
        image = QImage(str(self.photo))
        self.window.start_screen_capture()
        self.window.capture_timer.stop()
        self.assertFalse(self.window.isVisible())
        with patch.object(type(screen), "grabWindow", return_value=QPixmap.fromImage(image)):
            self.window.capture_screens()
        self.assertTrue(self.window.capture_windows)
        self.window.capture_windows[0].captured.emit(image)
        self.assertTrue(self.window.isVisible())
        self.assertFalse(self.window.capture_windows)
        self.finish_extraction()
        self.assertEqual(len(self.window.candidates), 2)
        self.assertEqual(self.window.photo_path.stem, "屏幕截图")
        self.window.add_to_board()
        before = self.window.project_data()
        photo_before = self.window.image.copy()
        self.window.start_screen_capture()
        self.window.finish_screen_capture()
        self.assertTrue(self.window.isVisible())
        self.assertFalse(self.window.capture_timer.isActive())
        self.assertEqual(self.window.image, photo_before)
        self.assertEqual(self.window.project_data(), before)

    def test_failed_capture_restores_window_and_reports_error(self):
        self.window.start_screen_capture()
        self.window.capture_timer.stop()
        with patch("stack_designer.QApplication.screens", return_value=[]), \
             patch("stack_designer.QMessageBox.warning") as warning:
            self.window.capture_screens()
            warning.assert_called_once()
        self.assertFalse(self.window.capture_active)
        self.assertTrue(self.window.isVisible())
        self.assertTrue(self.window.capture_button.isEnabled())

    def test_paste_image_extracts_without_a_file_and_invalid_clipboard_keeps_photo(self):
        image = QImage(str(self.photo))
        with patch("stack_designer.QApplication.clipboard") as clipboard:
            clipboard.return_value.image.return_value = image
            self.window.paste_photo()
        self.finish_extraction()
        self.assertEqual(len(self.window.candidates), 2)
        self.assertEqual(self.window.photo_path.stem, "剪贴板图片")
        self.assertTrue(self.window.current_background_button.isEnabled())
        self.window.current_background_button.click()
        self.assertEqual(self.window.background.image, self.window.image)
        candidate = self.window.candidates[0]
        with patch("stack_designer.QApplication.clipboard") as clipboard, \
             patch("stack_designer.QMessageBox.warning") as warning:
            clipboard.return_value.image.return_value = QImage()
            self.window.paste_photo()
            warning.assert_called_once()
        self.assertIs(self.window.candidates[0], candidate)

    def test_background_is_locked_behind_layers_and_does_not_change_png_export(self):
        item = self.add_export_rectangle()
        before = self.window.render_outline_image()
        with patch("stack_designer.QFileDialog.getOpenFileName", return_value=(str(self.photo), "")):
            self.window.import_background()
        background = self.window.background
        self.assertLess(background.zValue(), item.zValue())
        self.assertEqual(background.acceptedMouseButtons(), Qt.MouseButton.NoButton)
        self.window.background_scale.setValue(1.25)
        self.window.background_rotation.setValue(23.5)
        self.window.background_opacity.setValue(60)
        self.assertEqual(background.scale(), 1.25)
        self.assertEqual(background.rotation(), 23.5)
        self.assertEqual(background.opacity(), 0.6)
        self.assertEqual(self.window.render_outline_image(), before)
        self.window.remove_background()
        self.assertIsNone(self.window.background)
        self.assertEqual(self.window.layers, [item])
        self.assertEqual(self.window.render_outline_image(), before)

    def test_background_drag_then_lock(self):
        self.window.set_background_image(QImage(str(self.photo)))
        background = self.window.background
        self.window.move_background.setChecked(True)
        view = self.window.board_view
        old = QPointF(background.pos())
        start = view.mapFromScene(background.mapToScene(QPointF(0, 0)))
        QTest.mousePress(view.viewport(), Qt.MouseButton.LeftButton, pos=start)
        QTest.mouseMove(view.viewport(), start + QPoint(40, 30), delay=30)
        QTest.mouseRelease(view.viewport(), Qt.MouseButton.LeftButton, pos=start + QPoint(40, 30))
        self.assertNotEqual(background.pos(), old)
        self.window.move_background.setChecked(False)
        old = QPointF(background.pos())
        start = view.mapFromScene(background.mapToScene(QPointF(0, 0)))
        QTest.mousePress(view.viewport(), Qt.MouseButton.LeftButton, pos=start)
        QTest.mouseMove(view.viewport(), start + QPoint(40, 30), delay=30)
        QTest.mouseRelease(view.viewport(), Qt.MouseButton.LeftButton, pos=start + QPoint(40, 30))
        self.assertEqual(background.pos(), old)

    def test_background_project_roundtrip_is_self_contained_and_v1_remains_supported(self):
        self.add_export_rectangle()
        self.window.set_background_image(QImage(str(self.photo)))
        self.window.background.setPos(-345.25, 123.75)
        self.window.background.setScale(1.125)
        self.window.background.setRotation(-32.5)
        self.window.background.setOpacity(0.65)
        before = self.window.project_data()
        before_image = self.window.background.image.convertToFormat(QImage.Format.Format_RGBA8888)
        before_polygon = self.window.layers[0].mapToScene(self.window.layers[0].polygon())
        path = Path(self.temp.name) / "背景工程.hstack.json"
        self.window.save_project_to(path)
        self.photo.unlink()
        self.window.install_project(self.window.read_project(path), path)
        self.assertEqual(self.window.project_data(), before)
        self.assertEqual(self.window.background.image.convertToFormat(QImage.Format.Format_RGBA8888), before_image)
        self.assertEqual(self.window.layers[0].mapToScene(self.window.layers[0].polygon()), before_polygon)
        self.assertFalse(self.window.move_background.isChecked())
        self.assertEqual(self.window.background.acceptedMouseButtons(), Qt.MouseButton.NoButton)
        legacy = {"format": "hstack", "version": 1, "layers": before["layers"]}
        path.write_text(json.dumps(legacy), encoding="utf-8")
        self.window.install_project(self.window.read_project(path), path)
        self.assertIsNone(self.window.background)
        self.assertEqual(self.window.project_data()["layers"], legacy["layers"])

    def test_background_only_project_prompts_before_replacement(self):
        self.window.set_background_image(QImage(str(self.photo)))
        path = Path(self.temp.name) / "background_only.hstack.json"
        self.window.save_project_to(path)
        background = self.window.background
        with patch("stack_designer.QFileDialog.getOpenFileName", return_value=(str(path), "")), \
             patch("stack_designer.QMessageBox.question", return_value=QMessageBox.StandardButton.Cancel) as question:
            self.window.open_project()
            question.assert_called_once()
        self.assertIs(self.window.background, background)
        self.window.install_project(self.window.read_project(path), path)
        self.assertFalse(self.window.layers)
        self.assertIsNotNone(self.window.background)

    def test_corrupt_background_project_preserves_entire_board(self):
        self.add_export_rectangle()
        self.window.set_background_image(QImage(str(self.photo)))
        before = self.window.project_data()
        background = self.window.background
        path = Path(self.temp.name) / "corrupt_background.hstack.json"
        for field, value in [("image_png_base64", "###"),
                             ("image_png_base64", base64.b64encode(b"not PNG").decode()),
                             ("position", [float("inf"), 0]), ("scale", 0), ("opacity", 2)]:
            with self.subTest(field=field):
                data = deepcopy(before)
                data["background"][field] = value
                path.write_text(json.dumps(data), encoding="utf-8")
                with patch("stack_designer.QFileDialog.getOpenFileName", return_value=(str(path), "")), \
                     patch("stack_designer.QMessageBox.warning") as warning:
                    self.window.open_project()
                    warning.assert_called_once()
                self.assertIs(self.window.background, background)
                self.assertEqual(self.window.project_data(), before)


class MicroscopeIntegrationTests(unittest.TestCase):
    def test_menu_owns_reuses_and_closes_designer_and_preserves_capture_behavior(self):
        with patch.dict(sys.modules, REAL_QT_MODULES):
            from microscope_workspace import MicroscopeWindow
        app = QApplication.instance() or QApplication([])
        with tempfile.TemporaryDirectory() as folder:
            microscope = MicroscopeWindow(settings_path=Path(folder) / "settings.json")
            microscope.show()
            try:
                self.assertIsNone(microscope.stack_designer)
                menu = next(action.menu() for action in microscope.menuBar().actions() if "工具" in action.text())
                action = next(action for action in menu.actions() if "叠层" in action.text())
                action.trigger()
                designer = microscope.stack_designer
                self.assertIs(designer.parentWidget(), microscope)
                self.assertTrue(designer.isWindow())
                self.assertTrue(designer.isVisible())
                layer = FlakeItem([(0, 0), (100, 0), (100, 80)], "retained")
                designer.layers.append(layer)
                designer.board_scene.addItem(layer)
                designer.refresh_layers(layer)
                designer.start_screen_capture()
                self.assertFalse(microscope.isVisible())
                self.assertFalse(designer.isVisible())
                designer.finish_screen_capture()
                self.assertTrue(microscope.isVisible())
                self.assertTrue(designer.isVisible())
                designer.close()
                microscope.device_tabs.setCurrentIndex(1)
                action.trigger()
                self.assertIs(microscope.stack_designer, designer)
                self.assertEqual(designer.layers, [layer])
                self.assertTrue(designer.isVisible())
                self.assertFalse(microscope.camera_session.alive)
                microscope.close()
                self.assertFalse(designer.isVisible())
            finally:
                microscope.close()
                microscope.deleteLater()
                app.processEvents()


if __name__ == "__main__":
    unittest.main()
