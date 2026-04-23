"""Structural and storage tests for the Nexcope control page."""

import ast
import importlib.util
import sys
import types
import unittest
from pathlib import Path
from unittest.mock import patch


PROJECT_ROOT = Path(__file__).resolve().parents[1]
SOURCE_PATH = PROJECT_ROOT / "controller_gui.py"


def _parse_source() -> ast.Module:
    return ast.parse(SOURCE_PATH.read_text(encoding="utf-8"))


def _find_class(module: ast.Module, class_name: str) -> ast.ClassDef:
    for node in module.body:
        if isinstance(node, ast.ClassDef) and node.name == class_name:
            return node
    raise AssertionError(f"class {class_name!r} not found")


def _load_controller_module():
    dummy_widgets = types.ModuleType("PyQt6.QtWidgets")
    dummy_core = types.ModuleType("PyQt6.QtCore")
    dummy_gui = types.ModuleType("PyQt6.QtGui")

    class _Dummy:
        def __init__(self, *args, **kwargs):
            pass

    class _DummyQMainWindow(_Dummy):
        pass

    class _DummyQt:
        class AlignmentFlag:
            AlignCenter = object()

        class CheckState:
            Checked = types.SimpleNamespace(value=2)

        class ItemDataRole:
            UserRole = 32

    class _DummyQRectF(_Dummy):
        pass

    widget_names = [
        "QApplication",
        "QWidget",
        "QVBoxLayout",
        "QHBoxLayout",
        "QGridLayout",
        "QGroupBox",
        "QPushButton",
        "QLabel",
        "QLineEdit",
        "QComboBox",
        "QCheckBox",
        "QTextEdit",
        "QTabWidget",
        "QStatusBar",
        "QDoubleSpinBox",
        "QFrame",
        "QMessageBox",
        "QMenuBar",
        "QListWidget",
        "QListWidgetItem",
        "QInputDialog",
    ]
    for name in widget_names:
        setattr(dummy_widgets, name, _Dummy)
    dummy_widgets.QMainWindow = _DummyQMainWindow

    dummy_core.Qt = _DummyQt
    dummy_core.QTimer = _Dummy
    dummy_core.QRectF = _DummyQRectF

    for name in ["QFont", "QColor", "QPalette", "QPainter", "QPen", "QBrush"]:
        setattr(dummy_gui, name, _Dummy)

    pyqt6 = types.ModuleType("PyQt6")
    pyqt6.QtWidgets = dummy_widgets
    pyqt6.QtCore = dummy_core
    pyqt6.QtGui = dummy_gui

    dummy_onway_module = types.ModuleType("onway_driver")
    dummy_onway_module.MCCDriver = type("DummyDriver", (), {})
    dummy_onway_module.ParaIndex = object()
    dummy_onway_module.SERIAL_AVAILABLE = False

    dummy_nexcope_module = types.ModuleType("nexcope_driver")
    dummy_nexcope_module.NexcopeDriver = type("DummyNexcopeDriver", (), {})
    dummy_nexcope_module.SERIAL_AVAILABLE = False

    stub_modules = {
        "PyQt6": pyqt6,
        "PyQt6.QtWidgets": dummy_widgets,
        "PyQt6.QtCore": dummy_core,
        "PyQt6.QtGui": dummy_gui,
        "onway_driver": dummy_onway_module,
        "nexcope_driver": dummy_nexcope_module,
    }

    spec = importlib.util.spec_from_file_location("controller_gui_under_test", SOURCE_PATH)
    module = importlib.util.module_from_spec(spec)
    with patch.dict(sys.modules, stub_modules):
        assert spec.loader is not None
        spec.loader.exec_module(module)
    return module


class NexcopePageStructureTests(unittest.TestCase):
    def test_nexcope_page_class_exists_with_xyz_and_xy_constants(self) -> None:
        module = _load_controller_module()

        self.assertEqual(module.NexcopeControlPage.AXIS_NAMES, ["X", "Y", "Z"])
        self.assertEqual(module.NexcopeControlPage.XY_AXIS_INDICES, (0, 1))

    def test_controller_shell_uses_nexcope_page_instance_in_tab(self) -> None:
        module = _parse_source()
        shell_class = _find_class(module, "ControllerMainWindow")

        init_assignments = []
        for node in ast.walk(shell_class):
            if isinstance(node, ast.Assign) and len(node.targets) == 1:
                target = node.targets[0]
                if (
                    isinstance(target, ast.Attribute)
                    and isinstance(target.value, ast.Name)
                    and target.value.id == "self"
                    and target.attr == "nexcope_page"
                    and isinstance(node.value, ast.Call)
                    and isinstance(node.value.func, ast.Name)
                ):
                    init_assignments.append(node.value.func.id)

        self.assertIn("NexcopeControlPage", init_assignments)
        self.assertIn("return self.nexcope_page", SOURCE_PATH.read_text(encoding="utf-8"))
        self.assertIn("_install_connection_hooks(self.nexcope_page)", SOURCE_PATH.read_text(encoding="utf-8"))

    def test_nexcope_page_declares_ne950_group_titles(self) -> None:
        source = SOURCE_PATH.read_text(encoding="utf-8")

        for title in ["照明控制", "荧光转盘", "聚光镜", "分光镜", "物镜转盘"]:
            self.assertIn(title, source)
        self.assertIn("self.optical_groups = [", source)


class NexcopePageRecordDbTests(unittest.TestCase):
    def setUp(self) -> None:
        self.module = _load_controller_module()
        self.page = object.__new__(self.module.NexcopeControlPage)
        self.page.record_db_path = None
        self.page.record_db_conn = None
        self.module.NexcopeControlPage.init_record_db(self.page)
        self.addCleanup(self.module.NexcopeControlPage.cleanup_record_db, self.page)

    def test_init_record_db_creates_xy_position_records_table(self) -> None:
        row = self.page.record_db_conn.execute(
            "SELECT name FROM sqlite_master WHERE type='table' AND name='xy_position_records'"
        ).fetchone()

        self.assertEqual(row, ("xy_position_records",))

    def test_save_and_fetch_record_use_xy_only(self) -> None:
        record_id = self.module.NexcopeControlPage.save_xy_record(self.page, [12.5, 34.5])
        fetched = self.module.NexcopeControlPage.fetch_record_targets(self.page, record_id)

        self.assertEqual(fetched, [12.5, 34.5])
        columns = [
            row[1]
            for row in self.page.record_db_conn.execute("PRAGMA table_info(xy_position_records)")
        ]
        self.assertNotIn("axis_z", columns)

    def test_on_record_position_rejects_disconnected_state(self) -> None:
        page = object.__new__(self.module.NexcopeControlPage)
        page.driver = types.SimpleNamespace(connected=False)
        messages = []
        page.log_info = messages.append
        page.save_xy_record = lambda positions: self.fail("save_xy_record should not run while disconnected")
        page.refresh_record_list = lambda: self.fail("refresh_record_list should not run while disconnected")

        self.module.NexcopeControlPage.on_record_position(page)

        self.assertEqual(messages, ["未连接或无法读取 XY 位置，记录失败"])

    def test_mark_group_unsupported_disables_group_and_sets_tooltip(self) -> None:
        page = object.__new__(self.module.NexcopeControlPage)

        class FakeGroup:
            def __init__(self):
                self.enabled = True
                self.tooltip = ""

            def setEnabled(self, value):
                self.enabled = value

            def setToolTip(self, value):
                self.tooltip = value

        group = FakeGroup()
        self.module.NexcopeControlPage.mark_group_unsupported(page, group, "不支持测试")

        self.assertFalse(group.enabled)
        self.assertEqual(group.tooltip, "不支持测试")

    def test_load_optional_value_marks_group_unsupported_on_not_implemented(self) -> None:
        page = object.__new__(self.module.NexcopeControlPage)
        messages = []
        page.log_info = messages.append

        class FakeGroup:
            def __init__(self):
                self.enabled = True
                self.tooltip = ""

            def setEnabled(self, value):
                self.enabled = value

            def setToolTip(self, value):
                self.tooltip = value

        class FakeLabel:
            def __init__(self):
                self.text = ""

            def setText(self, value):
                self.text = value

        group = FakeGroup()
        label = FakeLabel()

        self.module.NexcopeControlPage.load_optional_value(
            page,
            group,
            label,
            lambda: (_ for _ in ()).throw(NotImplementedError("ERR-3")),
            "分光镜",
        )

        self.assertFalse(group.enabled)
        self.assertEqual(group.tooltip, "当前设备不支持该功能")
        self.assertEqual(label.text, "不支持")
        self.assertEqual(messages, ["分光镜 不支持: ERR-3"])


if __name__ == "__main__":
    unittest.main()
