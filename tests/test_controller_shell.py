"""Structural tests for the top-level controller shell."""

import ast
import unittest
from pathlib import Path


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


if __name__ == "__main__":
    unittest.main()
