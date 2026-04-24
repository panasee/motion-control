"""Structural and behavioral tests for the top-level controller shell."""

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


def _iter_calls(node: ast.AST):
    for child in ast.walk(node):
        if isinstance(child, ast.Call):
            yield child


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
        "QSlider",
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

    dummy_driver_module = types.ModuleType("onway_driver")
    dummy_driver_module.MCCDriver = type("DummyDriver", (), {"list_serial_ports": staticmethod(lambda: [])})
    dummy_driver_module.ParaIndex = object()
    dummy_driver_module.SERIAL_AVAILABLE = False

    dummy_nexcope_driver_module = types.ModuleType("nexcope_driver")
    dummy_nexcope_driver_module.NexcopeDriver = type("DummyNexcopeDriver", (), {})
    dummy_nexcope_driver_module.SERIAL_AVAILABLE = False

    stub_modules = {
        "PyQt6": pyqt6,
        "PyQt6.QtWidgets": dummy_widgets,
        "PyQt6.QtCore": dummy_core,
        "PyQt6.QtGui": dummy_gui,
        "onway_driver": dummy_driver_module,
        "nexcope_driver": dummy_nexcope_driver_module,
    }

    spec = importlib.util.spec_from_file_location("controller_gui_under_test", SOURCE_PATH)
    module = importlib.util.module_from_spec(spec)
    with patch.dict(sys.modules, stub_modules):
        assert spec.loader is not None
        spec.loader.exec_module(module)
    return module


class ControllerShellStructureTest(unittest.TestCase):
    def test_controller_main_window_class_exists(self) -> None:
        module = _parse_source()

        shell_class = _find_class(module, "ControllerMainWindow")

        base_names = [base.id for base in shell_class.bases if isinstance(base, ast.Name)]
        self.assertIn("QMainWindow", base_names)

    def test_controller_tabs_are_labeled_onway_and_nexcope(self) -> None:
        module = _parse_source()
        shell_class = _find_class(module, "ControllerMainWindow")

        labels = []
        for call in _iter_calls(shell_class):
            if isinstance(call.func, ast.Attribute) and call.func.attr == "addTab" and len(call.args) >= 2:
                owner = call.func.value
                self.assertIsInstance(owner, ast.Attribute)
                self.assertEqual(owner.attr, "controller_tabs")
                label = call.args[1]
                if isinstance(label, ast.Constant) and isinstance(label.value, str):
                    labels.append(label.value)

        self.assertEqual(labels, ["Onway", "Nexcope"])

    def test_controller_wraps_existing_onway_main_window(self) -> None:
        module = _parse_source()
        shell_class = _find_class(module, "ControllerMainWindow")

        assignments = []
        for node in ast.walk(shell_class):
            if isinstance(node, ast.Assign) and len(node.targets) == 1:
                target = node.targets[0]
                if (
                    isinstance(target, ast.Attribute)
                    and isinstance(target.value, ast.Name)
                    and target.value.id == "self"
                    and target.attr == "onway_window"
                    and isinstance(node.value, ast.Call)
                    and isinstance(node.value.func, ast.Name)
                ):
                    assignments.append(node.value.func.id)

        self.assertIn("MCCMainWindow", assignments)

    def test_main_launches_controller_main_window(self) -> None:
        module = _parse_source()

        main_func = next(
            node for node in module.body if isinstance(node, ast.FunctionDef) and node.name == "main"
        )

        constructed_classes = []
        for call in _iter_calls(main_func):
            if isinstance(call.func, ast.Name):
                constructed_classes.append(call.func.id)

        self.assertIn("ControllerMainWindow", constructed_classes)
        self.assertNotIn("MCCMainWindow", constructed_classes)

    def test_switching_tabs_installs_window_mode_handler(self) -> None:
        module = _parse_source()
        shell_class = _find_class(module, "ControllerMainWindow")

        current_changed_connects = []
        for call in _iter_calls(shell_class):
            if (
                isinstance(call.func, ast.Attribute)
                and call.func.attr == "connect"
                and isinstance(call.func.value, ast.Attribute)
                and call.func.value.attr == "currentChanged"
            ):
                current_changed_connects.append(call)

        self.assertEqual(len(current_changed_connects), 1)

    def test_controller_declares_distinct_window_sizes_for_onway_and_nexcope(self) -> None:
        source = SOURCE_PATH.read_text(encoding="utf-8")

        self.assertIn("ONWAY_WINDOW_WIDTH", source)
        self.assertIn("ONWAY_WINDOW_HEIGHT", source)
        self.assertIn("NEXCOPE_WINDOW_WIDTH", source)
        self.assertIn("NEXCOPE_WINDOW_HEIGHT", source)
        self.assertIn("apply_window_mode", source)


class ControllerShellConnectionHandoffTest(unittest.TestCase):
    class FakeDriver:
        def __init__(self, events, name):
            self.events = events
            self.name = name
            self.connected = False

        def connect(self):
            self.events.append(f"{self.name}.connect")
            self.connected = True
            return True

        def connect_serial(self):
            self.events.append(f"{self.name}.connect_serial")
            self.connected = True
            return True

        def disconnect(self):
            self.events.append(f"{self.name}.disconnect")
            self.connected = False

    class FakePage:
        def __init__(self, events, name):
            self.events = events
            self.name = name
            self.driver = ControllerShellConnectionHandoffTest.FakeDriver(events, name)
            self._active_connection_method = None

        def on_disconnect(self):
            self.events.append(f"{self.name}.on_disconnect")
            self.driver.disconnect()

        def on_serial_disconnect(self):
            self.events.append(f"{self.name}.on_serial_disconnect")
            self.driver.disconnect()

    def setUp(self) -> None:
        module = _load_controller_module()
        self.ControllerMainWindow = module.ControllerMainWindow

    def _make_shell(self):
        shell = object.__new__(self.ControllerMainWindow)
        shell._active_connection_owner = None
        return shell

    def test_new_connection_disconnects_previous_connected_page_first(self) -> None:
        shell = self._make_shell()
        events = []
        onway_page = self.FakePage(events, "onway")
        nexcope_page = self.FakePage(events, "nexcope")

        self.ControllerMainWindow._install_connection_hooks(shell, onway_page)
        self.ControllerMainWindow._install_connection_hooks(shell, nexcope_page)

        onway_page.driver.connect()
        nexcope_page.driver.connect()

        self.assertEqual(
            events,
            ["onway.connect", "onway.on_disconnect", "onway.disconnect", "nexcope.connect"],
        )
        self.assertFalse(onway_page.driver.connected)
        self.assertTrue(nexcope_page.driver.connected)
        self.assertIs(shell._active_connection_owner, nexcope_page)

    def test_serial_handoff_uses_page_serial_disconnect_flow(self) -> None:
        shell = self._make_shell()
        events = []
        onway_page = self.FakePage(events, "onway")
        nexcope_page = self.FakePage(events, "nexcope")

        self.ControllerMainWindow._install_connection_hooks(shell, onway_page)
        self.ControllerMainWindow._install_connection_hooks(shell, nexcope_page)

        onway_page.driver.connect_serial()
        nexcope_page.driver.connect()

        self.assertEqual(
            events,
            ["onway.connect_serial", "onway.on_serial_disconnect", "onway.disconnect", "nexcope.connect"],
        )
        self.assertIsNone(onway_page._active_connection_method)
        self.assertIs(shell._active_connection_owner, nexcope_page)

    def test_same_page_reconnect_does_not_disconnect_itself_first(self) -> None:
        shell = self._make_shell()
        events = []
        onway_page = self.FakePage(events, "onway")

        self.ControllerMainWindow._install_connection_hooks(shell, onway_page)

        onway_page.driver.connect()
        onway_page.driver.connect_serial()

        self.assertEqual(
            events,
            ["onway.connect", "onway.connect_serial"],
        )
        self.assertTrue(onway_page.driver.connected)
        self.assertIs(shell._active_connection_owner, onway_page)

    def test_apply_window_mode_uses_nexcope_size_for_nexcope_tab(self) -> None:
        shell = self._make_shell()
        shell.ONWAY_WINDOW_WIDTH = 980
        shell.ONWAY_WINDOW_HEIGHT = 760
        shell.NEXCOPE_WINDOW_WIDTH = 780
        shell.NEXCOPE_WINDOW_HEIGHT = 620
        events = []
        shell.resize = lambda w, h: events.append(("resize", w, h))
        shell.setMinimumSize = lambda w, h: events.append(("minimum", w, h))

        self.ControllerMainWindow.apply_window_mode(shell, 1)

        self.assertEqual(events, [("minimum", 780, 620), ("resize", 780, 620)])

    def test_apply_window_mode_restores_onway_size_for_onway_tab(self) -> None:
        shell = self._make_shell()
        shell.ONWAY_WINDOW_WIDTH = 980
        shell.ONWAY_WINDOW_HEIGHT = 760
        shell.NEXCOPE_WINDOW_WIDTH = 780
        shell.NEXCOPE_WINDOW_HEIGHT = 620
        shell.resize = lambda w, h: setattr(shell, "_last_resize", (w, h))
        shell.setMinimumSize = lambda w, h: setattr(shell, "_last_min_size", (w, h))

        self.ControllerMainWindow.apply_window_mode(shell, 0)

        self.assertEqual(shell._last_resize, (980, 760))
        self.assertEqual(shell._last_min_size, (980, 760))


if __name__ == "__main__":
    unittest.main()
