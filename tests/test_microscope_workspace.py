"""Real Qt interaction tests for the independent, hardware-free workspace."""

import os
os.environ.setdefault("QT_QPA_PLATFORM", "offscreen")

import csv
import json
import subprocess
import sys
import numpy as np
import tempfile
import unittest
from pathlib import Path
from unittest.mock import patch

from PyQt6.QtCore import QPointF, Qt
from PyQt6.QtGui import QColor, QImage, QFontDatabase, QPalette
from PyQt6.QtTest import QTest
from PyQt6.QtWidgets import QApplication, QMessageBox, QPushButton, QScrollArea

from microscope_workspace import MicroscopeWindow, polygon_area


class MicroscopeWorkspaceTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.app = QApplication.instance() or QApplication([])
        # Windows offscreen Qt has no system font database by default.
        font = Path("C:/Windows/Fonts/msyh.ttc")
        if font.exists():
            QFontDatabase.addApplicationFont(str(font))

    def setUp(self):
        folder = tempfile.TemporaryDirectory()
        self.addCleanup(folder.cleanup)
        self.settings_path = Path(folder.name) / "microscope-settings.json"
        settings = patch.object(MicroscopeWindow, "default_settings_path", return_value=self.settings_path)
        settings.start()
        self.addCleanup(settings.stop)
        self.window = MicroscopeWindow()
        self.window.show()
        self.app.processEvents()
        self.image = QImage(640, 480, QImage.Format.Format_RGB32)
        self.image.fill(QColor("#687381"))
        self.window.load_image(self.image, "测试图像")
        self.app.processEvents()

    def tearDown(self):
        self.window.close()
        self.window.deleteLater()
        self.app.processEvents()

    def points(self, *coordinates):
        return [QPointF(x, y) for x, y in coordinates]

    def drag(self, start, end):
        view = self.window.view
        QTest.mousePress(view.viewport(), Qt.MouseButton.LeftButton, pos=view.mapFromScene(start))
        QTest.mouseMove(view.viewport(), view.mapFromScene(end))
        QTest.mouseRelease(view.viewport(), Qt.MouseButton.LeftButton, pos=view.mapFromScene(end))
        self.app.processEvents()

    def test_camera_and_motion_share_exclusive_tabs_and_never_enable_hardware_commands(self):
        tabs = self.window.device_tabs
        self.assertEqual([tabs.tabText(i) for i in range(tabs.count())], ["相机", "控制台"])
        for index in range(2):
            tabs.setCurrentIndex(index)
            self.app.processEvents()
            self.assertTrue(tabs.widget(index).isVisible())
            self.assertFalse(tabs.widget(1 - index).isVisible())
            buttons = [button for button in tabs.widget(index).findChildren(QPushButton)
                       if button.property("hardware_command")]
            self.assertTrue(buttons)
            self.assertTrue(all(not button.isEnabled() for button in buttons))

    def test_workspace_panels_remain_readable_under_host_dark_theme_without_changing_host(self):
        original = self.app.palette()
        dark = QPalette(original)
        dark.setColor(QPalette.ColorRole.Window, QColor("#353535"))
        dark.setColor(QPalette.ColorRole.Text, QColor("white"))
        self.app.setPalette(dark)
        workspace = MicroscopeWindow()
        try:
            workspace.show()
            self.app.processEvents()
            surface = workspace.findChild(QScrollArea).widget()
            self.assertGreater(surface.palette().color(QPalette.ColorRole.Window).lightness(), 200)
            self.assertLess(surface.palette().color(QPalette.ColorRole.Text).lightness(), 100)
            self.assertEqual(self.app.palette().color(QPalette.ColorRole.Window), QColor("#353535"))
        finally:
            workspace.close()
            workspace.deleteLater()
            self.app.setPalette(original)

    def test_line_mouse_draw_uses_source_pixels_and_calibration_not_view_scale(self):
        self.window.set_tool("line")
        self.drag(QPointF(60, 80), QPointF(360, 80))
        self.assertEqual(len(self.window.annotations), 1)
        item = self.window.annotations[0]
        length = item.measurements()[0]
        self.assertAlmostEqual(length, 300, delta=1)
        self.assertIn("px", self.window.table.item(0, 2).text())
        self.window.view.scale(2, 2)
        self.window.set_calibration(0.25)
        self.assertAlmostEqual(item.measurements(0.25)[0], length / 4)
        self.assertIn("µm", self.window.table.item(0, 2).text())
        for invalid in [0, -1, float("nan"), float("inf")]:
            with self.assertRaises(ValueError):
                self.window.set_calibration(invalid)

    def test_rectangle_polygon_area_and_transform_invariance(self):
        self.window.set_tool("rectangle")
        self.drag(QPointF(80, 80), QPointF(280, 180))
        item = self.window.annotations[0]
        length, area = item.measurements()
        self.assertAlmostEqual(area, 20000, delta=250)
        item.setRotation(47)
        item.setPos(33, -8)
        self.assertAlmostEqual(item.measurements()[0], length)
        self.assertAlmostEqual(item.measurements()[1], area)
        self.window.set_calibration(0.5)
        self.assertAlmostEqual(item.measurements(0.5)[1], area / 4)

    def test_polygon_enter_backspace_escape_and_minimum_vertices(self):
        view = self.window.view
        self.window.set_tool("polygon")
        for point in self.points((100, 100), (300, 100), (300, 250), (100, 250)):
            QTest.mouseClick(view.viewport(), Qt.MouseButton.LeftButton, pos=view.mapFromScene(point))
        QTest.keyClick(view, Qt.Key.Key_Backspace)
        self.assertEqual(len(view.points), 3)
        QTest.keyClick(view, Qt.Key.Key_Return)
        self.assertEqual(len(self.window.annotations), 1)
        self.assertEqual(len(self.window.annotations[0].polygon()), 3)
        for point in self.points((20, 20), (50, 50)):
            QTest.mouseClick(view.viewport(), Qt.MouseButton.LeftButton, pos=view.mapFromScene(point))
        QTest.keyClick(view, Qt.Key.Key_Return)
        self.assertEqual(len(self.window.annotations), 1)
        QTest.keyClick(view, Qt.Key.Key_Escape)
        self.assertIsNone(view.preview)
        self.assertEqual(view.points, [])

    def test_node_drag_updates_geometry_and_measurement_table_without_moving_parent(self):
        item = self.window.add_annotation("polygon", self.points((100, 100), (300, 100), (300, 250), (100, 250)))
        self.window.set_tool("select")
        self.window.edit_nodes.setChecked(True)
        self.app.processEvents()
        before = self.window.table.item(0, 3).text()
        self.drag(item.handles[0].scenePos(), QPointF(150, 140))
        self.assertEqual(item.pos(), QPointF())
        self.assertAlmostEqual(item.polygon()[0].x(), 150, delta=1)
        self.assertNotEqual(self.window.table.item(0, 3).text(), before)
        item.insert_vertex(QPointF(220, 100))
        self.assertEqual(len(item.polygon()), 5)
        item.remove_vertex(1)
        self.assertEqual(len(item.polygon()), 4)

    def test_crossing_polygon_is_marked_invalid_instead_of_reporting_false_area(self):
        points = self.points((100, 100), (300, 250), (300, 100), (100, 250))
        self.assertIsNone(polygon_area(points))
        self.window.add_annotation("polygon", points)
        self.assertIn("自交", self.window.table.item(0, 3).text())

    def test_selection_properties_and_delete_shortcut_scope(self):
        first = self.window.add_annotation("line", self.points((20, 20), (100, 20)))
        second = self.window.add_annotation("polygon", self.points((150, 100), (250, 100), (200, 180)))
        self.window.table.selectRow(0)
        self.app.processEvents()
        self.assertTrue(first.isSelected())
        self.assertFalse(second.isSelected())
        self.window.name_edit.setText("边缘长度")
        self.window.name_edit.editingFinished.emit()
        self.window.width_box.setValue(3)
        self.assertEqual(first.name, "边缘长度")
        self.assertEqual(first.line_width, 3)
        self.window.name_edit.setFocus()
        QTest.keyClick(self.window.name_edit, Qt.Key.Key_Delete)
        self.assertEqual(len(self.window.annotations), 2)
        self.window.view.setFocus()
        QTest.keyClick(self.window.view, Qt.Key.Key_Delete)
        self.assertEqual(self.window.annotations, [second])

    def test_new_image_resets_markers_but_keeps_calibration_and_invalid_image_preserves_work(self):
        item = self.window.add_annotation("line", self.points((20, 20), (100, 20)))
        self.window.set_calibration(0.12)
        with self.assertRaises(ValueError):
            self.window.load_image(QImage(), "无效")
        self.assertEqual(self.window.annotations, [item])
        self.assertEqual(self.window.microns_per_pixel, 0.12)
        self.window.load_image(self.image, "另一张")
        self.assertEqual(self.window.annotations, [])
        self.assertEqual(self.window.microns_per_pixel, 0.12)
        self.assertEqual(self.window.table.rowCount(), 0)

    def test_image_replacement_cancel_preserves_work(self):
        item = self.window.add_annotation("line", self.points((20, 20), (100, 20)))
        with tempfile.TemporaryDirectory() as folder:
            path = str(Path(folder) / "图像.png")
            self.assertTrue(self.image.save(path))
            with patch("microscope_workspace.QFileDialog.getOpenFileName", return_value=(path, "")), \
                 patch("microscope_workspace.QMessageBox.question", return_value=QMessageBox.StandardButton.No):
                self.window.open_image()
        self.assertEqual(self.window.annotations, [item])

    def test_export_is_original_resolution_independent_of_zoom_selection_and_has_csv_units(self):
        self.window.add_annotation("line", self.points((20, 20), (100, 20)))
        self.window.set_calibration(0.5)
        self.window.edit_nodes.setChecked(True)
        before = self.window.render_annotated_image()
        self.window.view.scale(3, 3)
        self.window.scene.clearSelection()
        self.assertEqual(before, self.window.render_annotated_image())
        self.assertEqual(before.size(), self.image.size())
        self.assertNotEqual(before.pixelColor(60, 20), self.image.pixelColor(60, 20))
        with tempfile.TemporaryDirectory() as folder:
            path = Path(folder) / "测量.csv"
            with patch("microscope_workspace.QFileDialog.getSaveFileName", return_value=(str(path), "")), \
                 patch("microscope_workspace.QMessageBox.warning") as warning:
                self.window.export_measurements()
                warning.assert_not_called()
            with path.open(encoding="utf-8-sig", newline="") as stream:
                rows = list(csv.reader(stream))
            self.assertEqual(rows[1][2], "40.000 µm")

    def test_tools_menu_launches_peer_without_owning_its_window(self):
        from controller_gui import ControllerMainWindow
        with patch("controller_gui.PositionHttpServer.start"), patch("controller_gui.PositionHttpServer.stop"), \
                patch("controller_gui.launch_microscope", return_value=123) as launch:
            shell = ControllerMainWindow()
            try:
                tools_menu = next(action.menu() for action in shell.menuBar().actions() if "工具" in action.text())
                self.assertFalse(any("叠层" in action.text() or "堆叠" in action.text()
                                     for action in tools_menu.actions()))
                self.assertFalse(hasattr(shell, "stack_designer"))
                action = next(action for action in tools_menu.actions() if "显微镜" in action.text())
                action.trigger()
                action.trigger()
                self.assertEqual(launch.call_count, 2)
                self.assertFalse(hasattr(shell, "microscope_window"))
                self.assertFalse(hasattr(shell, "camera_image_state"))
                self.assertIsNone(self.window.parent())
                self.assertEqual([shell.controller_tabs.tabText(i) for i in range(2)], ["Onway", "Nexcope"])
                shell.close()
                self.assertTrue(self.window.isVisible())
            finally:
                shell.close()
                shell.deleteLater()
                self.app.processEvents()

    def test_rgb_polyline_mouse_creation_open_geometry_and_node_editing(self):
        self.window.set_tool("profile")
        view = self.window.view
        for point in self.points((100, 100), (200, 100), (200, 200)):
            QTest.mouseClick(view.viewport(), Qt.MouseButton.LeftButton, pos=view.mapFromScene(point))
        QTest.keyClick(view, Qt.Key.Key_Return)
        item = self.window.annotations[0]
        self.assertEqual(item.kind, "profile")
        self.assertAlmostEqual(item.measurements()[0], 200, delta=2)
        self.assertIsNone(item.measurements()[1])
        self.assertFalse(item.shape().contains(QPointF(150, 150)))
        self.assertEqual(self.window.result_tabs.currentIndex(), 1)
        self.assertIsNotNone(self.window.profile_data)
        item.insert_vertex(QPointF(150, 110))
        self.assertEqual(len(item.polygon()), 4)
        item.remove_vertex(1)
        item.remove_vertex(1)
        self.assertEqual(len(item.polygon()), 2)
        item.remove_vertex(1)
        self.assertEqual(len(item.polygon()), 2)
        before = self.window.profile_data[0][-1]
        item.move_vertex(1, QPointF(250, 250))
        self.assertNotEqual(self.window.profile_data[0][-1], before)

    def test_rgb_source_is_unannotated_original_by_default_and_wb_is_not_cumulative(self):
        image = QImage(30, 20, QImage.Format.Format_RGB32)
        image.fill(QColor(40, 80, 120))
        self.window.load_image(image, "RGB")
        self.window.add_annotation("profile", self.points((2, 5), (20, 5)))
        self.window.set_white_balance([2, 1, 0.5])
        np.testing.assert_allclose(self.window.profile_data[2], np.tile([40, 80, 120], (19, 1)))
        self.assertEqual(self.window.image.pixelColor(5, 5), QColor(40, 80, 120))
        self.assertEqual(self.window.display_image.pixelColor(5, 5), QColor(80, 80, 60))
        self.window.profile_source.setCurrentIndex(1)
        np.testing.assert_allclose(self.window.profile_data[2], np.tile([80, 80, 60], (19, 1)))
        self.window.set_white_balance([2, 1, 0.5])
        self.assertEqual(self.window.display_image.pixelColor(5, 5), QColor(80, 80, 60))
        self.window.set_white_balance([1, 1, 1])
        self.assertEqual(self.window.display_image, self.window.image)
        self.window.set_calibration(0.25)
        self.assertAlmostEqual(self.window.profile_plot.distance[-1], 4.5)
        self.assertEqual(self.window.profile_plot.unit, "µm")
        self.window.view.scale(3, 3)
        self.window.update_rgb_profile()
        self.assertAlmostEqual(self.window.profile_plot.distance[-1], 4.5)

    def test_white_balance_neutral_pick_error_and_escape(self):
        image = QImage(40, 40, QImage.Format.Format_RGB32)
        image.fill(QColor(40, 80, 120))
        self.window.load_image(image, "neutral")
        self.window.set_tool("white_balance")
        point = self.window.view.mapFromScene(QPointF(20, 20))
        QTest.mouseClick(self.window.view.viewport(), Qt.MouseButton.LeftButton, pos=point)
        self.assertEqual(self.window.view.tool, "select")
        self.assertEqual(self.window.display_image.pixelColor(20, 20), QColor(80, 80, 80))
        self.window.auto_white_balance()
        self.assertEqual(self.window.display_image.pixelColor(20, 20), QColor(80, 80, 80))
        self.window.set_tool("white_balance")
        QTest.keyClick(self.window.view, Qt.Key.Key_Escape)
        self.assertEqual(self.window.view.tool, "select")
        image.fill(QColor("black"))
        self.window.load_image(image, "black")
        np.testing.assert_allclose(self.window.wb_gains, [2, 1, 2 / 3])
        with patch("microscope_workspace.QMessageBox.warning") as warning:
            self.window.pick_white_balance(QPointF(0, 0))
        warning.assert_called_once()
        self.assertEqual(self.window.display_image.pixelColor(20, 20), QColor("black"))

    def test_settings_survive_new_window_and_apply_to_new_images(self):
        window = self.window
        window.exposure_box.setValue(123.45)
        window.gain_box.setValue(2.3)
        window.motion_speed_box.setValue(0.432)
        window.set_edge_detection_parameters({"max_dimension": 1440, "threshold": 18,
                                              "min_area_percent": 0.003, "simplification": 0.25})
        window.set_white_balance([1.123456, 0.8, 1.4])
        window.set_calibration(0.25)
        window.device_tabs.setCurrentIndex(1)
        window.result_tabs.setCurrentIndex(1)
        window.profile_source.setCurrentIndex(1)
        window.channel_checks[1].setChecked(False)
        window.edit_nodes.setChecked(True)
        window.add_annotation("polygon", self.points((20, 20), (50, 20), (50, 50)))
        window.width_box.setValue(3.5)
        window.fill_slider.setValue(30)
        window.style_box.setCurrentIndex(1)
        with patch("microscope_workspace.QColorDialog.getColor", return_value=QColor("#12ab34")):
            window.choose_color()
        window.resize(1200, 800)
        window.results_dock.hide()
        self.assertTrue(window.close())
        expected = window.settings_snapshot()
        # A fresh interpreter must recover settings without any in-memory window.
        process = subprocess.run([sys.executable, "-c", """
import sys
from PyQt6.QtWidgets import QApplication
from PyQt6.QtGui import QImage, QColor
from microscope_workspace import MicroscopeWindow
app = QApplication([])
window = MicroscopeWindow(settings_path=sys.argv[1])
assert not window._settings_load_failed
assert window.exposure_box.value() == 123.45
assert window.gain_box.value() == 2.3
assert window.motion_speed_box.value() == 0.432
assert window.edge_detection_parameters() == {"max_dimension": 1440, "threshold": 18,
                                               "min_area_percent": 0.003, "simplification": 0.25}
assert window.wb_gains.tolist() == [1.123456, 0.8, 1.4]
assert window.microns_per_pixel == 0.25
assert window.device_tabs.currentIndex() == 1
assert window.profile_source.currentIndex() == 1
assert window.profile_plot.channels == [True, False, True]
image = QImage(10, 10, QImage.Format.Format_RGB32)
image.fill(QColor(100, 100, 100))
window.load_image(image, 'restart')
assert window.display_image.pixelColor(0, 0) == QColor(112, 80, 140)
assert window.microns_per_pixel == 0.25
""", str(self.settings_path)], cwd=Path(__file__).resolve().parents[1],
                                 capture_output=True, text=True, timeout=30)
        self.assertEqual(process.returncode, 0, process.stdout + process.stderr)
        restored = MicroscopeWindow()
        try:
            self.assertTrue(restored.image.isNull())
            self.assertEqual(restored.annotations, [])
            actual = restored.settings_snapshot()
            for key in expected.keys() - {"geometry", "layout"}:
                self.assertEqual(actual[key], expected[key], key)
            # Qt clamps restored geometry to the available screen (the offscreen
            # platform can be smaller than this application's minimum size).
            screen = restored.screen().availableGeometry()
            self.assertLessEqual(restored.width(), max(restored.minimumWidth(), screen.width()))
            self.assertLessEqual(restored.height(), max(restored.minimumHeight(), screen.height()))
            self.assertLessEqual(restored.width(), window.width())
            self.assertLessEqual(restored.height(), window.height())
            self.assertTrue(restored.results_dock.isHidden())
            restored.load_image(self.image, "restored")
            self.assertEqual(restored.microns_per_pixel, 0.25)
            np.testing.assert_allclose(restored.wb_gains, [1.123456, 0.8, 1.4])
            np.testing.assert_array_equal(restored.balanced_rgb[0, 0],
                                          np.rint(np.array([104, 115, 129]) * restored.wb_gains))
            item = restored.add_annotation("polygon", self.points((20, 20), (50, 20), (50, 50)))
            self.assertEqual(item.outline_color.name(), "#12ab34")
            self.assertEqual(item.line_width, 3.5)
            self.assertEqual(item.fill_alpha, round(0.3 * 255))
            self.assertEqual(item.line_style.name, expected["annotation_style"]["style"])
            self.assertTrue(item.edit_vertices)
            restored.set_calibration(None)
            restored.set_white_balance([1, 1, 1])
            restored.close()
            again = MicroscopeWindow()
            try:
                self.assertIsNone(again.microns_per_pixel)
                np.testing.assert_equal(again.wb_gains, [1, 1, 1])
            finally:
                again.close()
                again.deleteLater()
        finally:
            restored.close()
            restored.deleteLater()

    def test_corrupt_or_invalid_settings_report_error_without_overwriting_file(self):
        valid = self.window.settings_snapshot()
        bad_gain = dict(valid, gain=-1)
        bad_bool = dict(valid, edit_nodes="false")
        bad_wb = dict(valid, white_balance=[1, 0, 1])
        bad_edge = [dict(valid, edge_detection=value) for value in (
            None, dict(valid["edge_detection"], max_dimension=999),
            dict(valid["edge_detection"], min_area_percent=0),
            dict(valid["edge_detection"], threshold=True),
        )]
        for content in ["{bad json", json.dumps(bad_gain), json.dumps(bad_bool), json.dumps(bad_wb),
                        *(json.dumps(value) for value in bad_edge)]:
            with self.subTest(content=content):
                self.settings_path.write_text(content, encoding="utf-8")
                with patch("microscope_workspace.QMessageBox.critical") as warning:
                    restored = MicroscopeWindow()
                try:
                    warning.assert_called_once()
                    self.assertTrue(restored._settings_load_failed)
                    restored.close()
                    self.assertEqual(self.settings_path.read_text(encoding="utf-8"), content)
                finally:
                    restored.deleteLater()

    def test_save_failure_keeps_window_open_and_previous_settings_intact(self):
        self.window.save_settings()
        previous = self.settings_path.read_bytes()
        with patch("microscope_workspace.QSaveFile.commit", return_value=False), \
             patch("microscope_workspace.QMessageBox.warning") as warning:
            self.assertFalse(self.window.close())
        warning.assert_called_once()
        self.assertTrue(self.window.isVisible())
        self.assertEqual(self.settings_path.read_bytes(), previous)

    def test_profile_out_of_bounds_and_delete_clear_stale_data(self):
        item = self.window.add_annotation("profile", self.points((10, 10), (20, 20)))
        item.setPos(-30, 0)
        self.assertIsNone(self.window.profile_data)
        self.assertFalse(self.window.profile_export.isEnabled())
        self.assertIn("超出", self.window.profile_plot.message)
        item.setPos(0, 0)
        self.assertIsNotNone(self.window.profile_data)
        self.window.delete_selected()
        self.assertIsNone(self.window.profile_data)
        self.assertEqual(len(self.window.profile_plot.distance), 0)

    def test_rgb_export_reports_source_gains_coordinates_and_both_distance_units(self):
        self.window.add_annotation("profile", self.points((10, 10), (13, 10), (13, 14)))
        self.window.set_white_balance([1.2, 1, 0.8])
        self.window.profile_source.setCurrentIndex(1)
        self.window.set_calibration(0.5)
        with tempfile.TemporaryDirectory() as folder:
            path = Path(folder) / "RGB.csv"
            with patch("microscope_workspace.QFileDialog.getSaveFileName", return_value=(str(path), "")), \
                 patch("microscope_workspace.QMessageBox.warning") as warning:
                self.window.export_rgb_profile()
                warning.assert_not_called()
            with path.open(encoding="utf-8-sig", newline="") as stream:
                rows = list(csv.DictReader(stream))
            self.assertEqual(len(rows), 8)
            self.assertEqual(float(rows[-1]["distance_px"]), 7)
            self.assertEqual(float(rows[-1]["distance_um"]), 3.5)
            self.assertEqual(float(rows[-1]["x_px"]), 13)
            self.assertEqual(float(rows[-1]["y_px"]), 14)
            self.assertEqual(rows[-1]["source"], "white_balanced_rgb8")
            self.assertEqual(float(rows[-1]["gain_R"]), 1.2)


if __name__ == "__main__":
    unittest.main()
