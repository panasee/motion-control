"""Smoke test for the module rename."""

import sys
import types
import unittest
from pathlib import Path


PROJECT_ROOT = Path(__file__).resolve().parents[1]


def _install_pyqt6_stubs() -> None:
    def _stub_type(name: str) -> type:
        return type(name, (), {})

    qtwidgets = types.ModuleType("PyQt6.QtWidgets")
    for name in (
        "QApplication",
        "QMainWindow",
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
    ):
        setattr(qtwidgets, name, _stub_type(name))

    qtcore = types.ModuleType("PyQt6.QtCore")
    qtcore.Qt = _stub_type("Qt")
    qtcore.QTimer = _stub_type("QTimer")
    qtcore.QRectF = _stub_type("QRectF")

    qtgui = types.ModuleType("PyQt6.QtGui")
    for name in ("QFont", "QColor", "QPalette", "QPainter", "QPen", "QBrush"):
        setattr(qtgui, name, _stub_type(name))

    pyqt6 = types.ModuleType("PyQt6")
    pyqt6.QtWidgets = qtwidgets
    pyqt6.QtCore = qtcore
    pyqt6.QtGui = qtgui

    sys.modules.update(
        {
            "PyQt6": pyqt6,
            "PyQt6.QtWidgets": qtwidgets,
            "PyQt6.QtCore": qtcore,
            "PyQt6.QtGui": qtgui,
        }
    )


class ModuleRenameSmokeTest(unittest.TestCase):
    def test_renamed_modules_import(self) -> None:
        _install_pyqt6_stubs()

        sys.path.insert(0, str(PROJECT_ROOT))
        try:
            from onway_driver import MCCDriver
            from controller_gui import main
        finally:
            sys.path.remove(str(PROJECT_ROOT))

        self.assertIsNotNone(MCCDriver)
        self.assertTrue(callable(main))

    def test_readme_mentions_current_entrypoint_and_drivers(self) -> None:
        text = (PROJECT_ROOT / "README.md").read_text(encoding="utf-8")

        self.assertIn("controller_gui.py", text)
        self.assertIn("onway_driver.py", text)
        self.assertIn("nexcope_driver.py", text)


if __name__ == "__main__":
    unittest.main()
