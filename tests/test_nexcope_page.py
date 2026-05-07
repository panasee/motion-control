"""Structural and storage tests for the Nexcope control page."""

import ast
import importlib.util
import tempfile
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
        "QAbstractSpinBox",
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

    for name in ["QFont", "QColor", "QPalette", "QPainter", "QPen", "QBrush", "QIcon"]:
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

    def test_nexcope_page_declares_only_illumination_extra_group(self) -> None:
        source = SOURCE_PATH.read_text(encoding="utf-8")

        self.assertIn("照明控制", source)
        for title in ["荧光转盘", "聚光镜", "分光镜", "物镜转盘"]:
            self.assertNotIn(title, source)
        self.assertIn("self.optical_groups = [", source)

    def test_nexcope_manual_control_uses_speed_based_jog_ui(self) -> None:
        source = SOURCE_PATH.read_text(encoding="utf-8")

        self.assertIn("self.manual_speed = QDoubleSpinBox()", source)
        self.assertIn('QLabel("速度(mm/s):")', source)
        self.assertIn("configure_speed_spinbox(self.manual_speed, minimum=0.001, maximum=10, default=1.0)", source)
        self.assertIn("self.manual_speed_preset = create_speed_preset_combo(self.manual_speed)", source)
        self.assertIn("speed_label.setMinimumWidth(100)", source)
        self.assertIn("layout.setColumnStretch(0, 2)", source)
        self.assertIn("for column in range(1, 5):", source)
        self.assertIn("layout.setColumnStretch(column, 9)", source)
        self.assertIn("layout.setColumnStretch(5, 2)", source)
        self.assertIn("JOG_INTERVAL_MS = 15", source)
        self.assertNotIn("self.manual_step = QDoubleSpinBox()", source)
        self.assertIn("btn_neg.setMinimumSize(100, 44)", source)
        self.assertIn("btn_pos.setMinimumSize(100, 44)", source)
        self.assertIn("layout.addWidget(btn_neg, row, 1, 1, 2)", source)
        self.assertIn("layout.addWidget(btn_pos, row, 3, 1, 2)", source)
        self.assertIn(".pressed.connect(", source)
        self.assertIn(".released.connect(", source)

    def test_nexcope_illumination_uses_percent_sliders_and_10s_polling(self) -> None:
        source = SOURCE_PATH.read_text(encoding="utf-8")

        self.assertIn("ILLUMINATION_POLL_INTERVAL_MS = 10000", source)
        self.assertIn("self.upper_light_slider = QSlider(", source)
        self.assertIn("self.lower_light_slider = QSlider(", source)
        self.assertIn('QLabel("上光路亮度")', source)
        self.assertIn('QLabel("下光路亮度")', source)
        self.assertIn('return f"{percent:.1f}%"', source)
        self.assertIn("self.illumination_poll_timer = QTimer()", source)

    def test_nexcope_motion_group_uses_home_button_without_extension_group(self) -> None:
        source = SOURCE_PATH.read_text(encoding="utf-8")

        self.assertNotIn("运动扩展", source)
        self.assertIn('QPushButton("回零")', source)
        self.assertIn("clicked.connect(self.on_move_to_center)", source)

    def test_nexcope_manual_control_declares_autofocus_controls(self) -> None:
        source = SOURCE_PATH.read_text(encoding="utf-8")

        self.assertIn('QLabel("对焦")', source)
        self.assertIn("self.autofocus_combo = QComboBox()", source)
        self.assertIn('self.autofocus_combo.addItems(["x10", "x50", "x100"])', source)
        self.assertIn('QPushButton("校准")', source)
        self.assertIn('QPushButton("前往")', source)
        self.assertIn("clicked.connect(self.on_autofocus_calibrate)", source)
        self.assertIn("clicked.connect(self.on_autofocus_go)", source)


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

    def test_on_record_position_schedules_stage_read_without_blocking_gui(self) -> None:
        scheduled = []
        calls = []
        page = object.__new__(self.module.NexcopeControlPage)
        page.driver = types.SimpleNamespace(connected=True)
        page.get_xy_positions = lambda: (calls.append("read_xy") or [12.0, 34.0])
        page.save_xy_record = lambda positions: (calls.append(("save", positions)) or 9)
        page.refresh_record_list = lambda: calls.append("refresh")
        page.log_info = lambda msg: calls.append(("log", msg))
        page.run_nexcope_serial_task = (
            lambda worker, on_success=None, on_error=None, pause_polling=True:
            scheduled.append((worker, on_success, on_error, pause_polling))
        )

        self.module.NexcopeControlPage.on_record_position(page)

        self.assertEqual(calls, [])
        self.assertEqual(len(scheduled), 1)
        worker, on_success, _on_error, pause_polling = scheduled[0]
        self.assertTrue(pause_polling)
        positions = worker()
        on_success(positions)
        self.assertEqual(
            calls,
            ["read_xy", ("save", [12.0, 34.0]), "refresh", ("log", "已记录 XY 位置 #9: X=12, Y=34")],
        )

    def test_move_to_recorded_xy_position_uses_relative_delta_commands(self) -> None:
        calls = []
        page = object.__new__(self.module.NexcopeControlPage)
        page.driver = types.SimpleNamespace(
            connected=True,
            get_stage_xyz=lambda: {"X": 900, "Y": 2300, "Z": 10},
            move_left=lambda payload: calls.append(("L", payload)),
            move_right=lambda payload: calls.append(("R", payload)),
            move_forward=lambda payload: calls.append(("F", payload)),
            move_back=lambda payload: calls.append(("B", payload)),
            move_gpx=lambda payload: self.fail("absolute GPX command should not be used"),
            move_gpy=lambda payload: self.fail("absolute GPY command should not be used"),
        )

        moved = self.module.NexcopeControlPage.move_to_recorded_xy_position(page, [1000.0, 2000.0])

        self.assertTrue(moved)
        self.assertEqual(calls, [("R", "100"), ("B", "300")])

    def test_record_double_click_schedules_xy_jump_without_blocking_gui(self) -> None:
        scheduled = []
        calls = []
        logs = []
        page = object.__new__(self.module.NexcopeControlPage)
        page.get_record_item_id = lambda item: 7
        page.fetch_record_targets = lambda record_id: [1000.0, 2000.0]
        page.move_to_recorded_xy_position = lambda targets: (calls.append(("move", targets)) or True)
        page.run_nexcope_serial_task = (
            lambda worker, on_success=None, on_error=None, pause_polling=True:
            scheduled.append((worker, on_success, on_error, pause_polling))
        )
        page.log_info = logs.append

        self.module.NexcopeControlPage.on_record_item_double_clicked(page, object())

        self.assertEqual(calls, [])
        self.assertEqual(len(scheduled), 1)
        worker, on_success, _on_error, pause_polling = scheduled[0]
        self.assertTrue(pause_polling)
        result = worker()
        on_success(result)
        self.assertEqual(calls, [("move", [1000.0, 2000.0])])
        self.assertEqual(logs, ["跳转到 XY 记录 #7: X=1000, Y=2000"])

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


class NexcopeJogTests(unittest.TestCase):
    def setUp(self) -> None:
        self.module = _load_controller_module()

    def test_compute_jog_step_counts_converts_mm_per_s_to_counts_per_tick(self) -> None:
        page = object.__new__(self.module.NexcopeControlPage)
        page.JOG_INTERVAL_MS = 100
        page.COUNTS_PER_UM = 100
        page.manual_speed = types.SimpleNamespace(value=lambda: 0.005)

        counts = self.module.NexcopeControlPage.compute_jog_step_counts(page)

        self.assertEqual(counts, 50)

    def test_on_axis_jog_start_sets_active_axis_and_starts_timer(self) -> None:
        page = object.__new__(self.module.NexcopeControlPage)
        page.driver = types.SimpleNamespace(connected=True)
        page.log_info = lambda msg: None
        page.manual_speed = types.SimpleNamespace(value=lambda: 0.005)
        page.compute_jog_step_counts = lambda: 50
        page._jog_error_count = 2
        events = []
        page._send_jog_step = lambda: (events.append("tick") or True)
        page.jog_timer = types.SimpleNamespace(start=lambda: events.append("start"))
        page.update_timer = types.SimpleNamespace(stop=lambda: events.append("pause_status"))

        self.module.NexcopeControlPage.on_axis_jog_start(page, "X", -1)

        self.assertEqual(page.active_jog_axis, "X")
        self.assertEqual(page.active_jog_direction, -1)
        self.assertEqual(page._jog_error_count, 0)
        self.assertEqual(events, ["pause_status", "tick", "start"])

    def test_send_jog_step_writes_nonblocking_command_without_ack_read(self) -> None:
        calls = []
        page = object.__new__(self.module.NexcopeControlPage)
        page.driver = types.SimpleNamespace(
            connected=True,
            send_command=lambda command: calls.append(command),
        )
        page.log_info = lambda msg: None
        page.compute_jog_step_counts = lambda: 25
        page.active_jog_axis = "Y"
        page.active_jog_direction = 1

        sent = self.module.NexcopeControlPage._send_jog_step(page)

        self.assertTrue(sent)
        self.assertEqual(calls, ["F 25"])

    def test_on_axis_jog_stop_clears_active_jog_and_stops_timer(self) -> None:
        events = []
        page = object.__new__(self.module.NexcopeControlPage)
        page.jog_timer = types.SimpleNamespace(stop=lambda: events.append("stop"))
        page.update_timer = types.SimpleNamespace(start=lambda: events.append("resume_status"))
        page.driver = types.SimpleNamespace(connected=True)
        page.active_jog_axis = "Z"
        page.active_jog_direction = -1
        page._jog_error_count = 2

        self.module.NexcopeControlPage.on_axis_jog_stop(page, "Z")

        self.assertIsNone(page.active_jog_axis)
        self.assertEqual(page.active_jog_direction, 0)
        self.assertEqual(page._jog_error_count, 0)
        self.assertEqual(events, ["stop", "resume_status"])

    def test_serial_task_callbacks_run_from_result_timer_and_resume_polling(self) -> None:
        events = []
        page = object.__new__(self.module.NexcopeControlPage)
        page.driver = types.SimpleNamespace(connected=True)
        page._serial_io_lock = self.module.threading.Lock()
        page._serial_task_results = self.module.Queue()
        page._serial_task_pending_count = 0
        page._serial_task_pause_count = 0
        page.update_timer = types.SimpleNamespace(
            stop=lambda: events.append("pause_status"),
            start=lambda: events.append("resume_status"),
        )
        page.illumination_poll_timer = types.SimpleNamespace(
            stop=lambda: events.append("pause_illumination"),
            start=lambda: events.append("resume_illumination"),
        )
        page.serial_task_timer = types.SimpleNamespace(
            start=lambda: events.append("start_result_timer"),
            stop=lambda: events.append("stop_result_timer"),
        )
        page.log_info = lambda msg: events.append(("log", msg))

        class FakeThread:
            def __init__(self, *, target, daemon):
                self.target = target
                self.daemon = daemon

            def start(self):
                events.append(("start_thread", self.daemon))
                self.target()

        original_thread = self.module.threading.Thread
        self.module.threading.Thread = FakeThread
        try:
            self.module.NexcopeControlPage.run_nexcope_serial_task(
                page,
                lambda: "done",
                on_success=lambda result: events.append(("success", result)),
            )
        finally:
            self.module.threading.Thread = original_thread

        self.assertEqual(
            events,
            ["pause_status", "pause_illumination", ("start_thread", True), "start_result_timer"],
        )

        self.module.NexcopeControlPage.on_serial_task_result_tick(page)

        self.assertEqual(
            events,
            [
                "pause_status",
                "pause_illumination",
                ("start_thread", True),
                "start_result_timer",
                ("success", "done"),
                "resume_status",
                "resume_illumination",
                "stop_result_timer",
            ],
        )

    def test_on_jog_tick_keeps_running_after_transient_send_failure(self) -> None:
        events = []
        page = object.__new__(self.module.NexcopeControlPage)
        page.JOG_MAX_CONSECUTIVE_ERRORS = 3
        page.active_jog_axis = "X"
        page.active_jog_direction = 1
        page._jog_error_count = 0
        page._send_jog_step = lambda: (_ for _ in ()).throw(RuntimeError("serial busy"))
        page.on_axis_jog_stop = lambda: events.append("stop")
        page.log_info = lambda msg: events.append(("log", msg))

        self.module.NexcopeControlPage.on_jog_tick(page)

        self.assertEqual(page._jog_error_count, 1)
        self.assertNotIn("stop", events)
        self.assertEqual(events, [("log", "X 轴Jog发送失败，继续尝试: serial busy")])

    def test_on_jog_tick_stops_after_repeated_send_failures(self) -> None:
        events = []
        page = object.__new__(self.module.NexcopeControlPage)
        page.JOG_MAX_CONSECUTIVE_ERRORS = 3
        page.active_jog_axis = "Y"
        page.active_jog_direction = -1
        page._jog_error_count = 2
        page._send_jog_step = lambda: (_ for _ in ()).throw(RuntimeError("serial busy"))
        page.on_axis_jog_stop = lambda: events.append("stop")
        page.log_info = lambda msg: events.append(("log", msg))

        self.module.NexcopeControlPage.on_jog_tick(page)

        self.assertEqual(events, ["stop", ("log", "Y 轴Jog连续失败并已停止: serial busy")])

    def test_update_status_suppresses_transient_missing_xyz_response(self) -> None:
        scheduled = []
        logs = []
        page = object.__new__(self.module.NexcopeControlPage)
        page.driver = types.SimpleNamespace(
            connected=True,
            get_stage_xyz=lambda: (_ for _ in ()).throw(ValueError("missing XYZ field: X")),
        )
        page.log_info = logs.append
        page.AXIS_NAMES = ["X", "Y", "Z"]
        page.pos_labels = []
        page._status_update_pending = False
        page.run_nexcope_serial_task = (
            lambda worker, on_success=None, on_error=None, pause_polling=True:
            scheduled.append((worker, on_success, on_error, pause_polling))
        )

        self.module.NexcopeControlPage.update_status(page)

        self.assertEqual(len(scheduled), 1)
        worker, _on_success, on_error, pause_polling = scheduled[0]
        self.assertFalse(pause_polling)
        with self.assertRaises(ValueError):
            worker()
        on_error(ValueError("missing XYZ field: X"))
        self.assertEqual(logs, [])
        self.assertFalse(page._status_update_pending)

    def test_update_status_logs_non_transient_status_errors(self) -> None:
        scheduled = []
        logs = []
        page = object.__new__(self.module.NexcopeControlPage)
        page.driver = types.SimpleNamespace(
            connected=True,
            get_stage_xyz=lambda: (_ for _ in ()).throw(RuntimeError("serial offline")),
        )
        page.log_info = logs.append
        page._status_update_pending = False
        page.run_nexcope_serial_task = (
            lambda worker, on_success=None, on_error=None, pause_polling=True:
            scheduled.append((worker, on_success, on_error, pause_polling))
        )

        self.module.NexcopeControlPage.update_status(page)

        self.assertEqual(len(scheduled), 1)
        _worker, _on_success, on_error, pause_polling = scheduled[0]
        self.assertFalse(pause_polling)
        on_error(RuntimeError("serial offline"))
        self.assertEqual(logs, ["状态刷新失败: serial offline"])
        self.assertFalse(page._status_update_pending)

    def test_send_move_to_center_commands_uses_relative_xy_delta_to_fixed_center(self) -> None:
        calls = []
        logs = []
        page = object.__new__(self.module.NexcopeControlPage)
        page.COUNTS_PER_UM = 100
        page.CENTER_XY_UM = (65000.0, 37500.0)
        page.driver = types.SimpleNamespace(
            connected=True,
            get_stage_xyz=lambda: {"X": 6499900, "Y": 3750200, "Z": 123},
            move_left=lambda payload: calls.append(("L", payload)),
            move_right=lambda payload: calls.append(("R", payload)),
            move_forward=lambda payload: calls.append(("F", payload)),
            move_back=lambda payload: calls.append(("B", payload)),
        )
        page.log_info = logs.append

        delta = self.module.NexcopeControlPage._send_move_to_center_commands(page)

        self.assertEqual(calls, [("R", "100"), ("B", "200")])
        self.assertEqual(delta, (100, -200))

    def test_on_move_to_center_starts_background_home_and_pauses_polling(self) -> None:
        events = []
        page = object.__new__(self.module.NexcopeControlPage)
        page.driver = types.SimpleNamespace(connected=True)
        page._home_in_progress = False
        page._home_thread = None
        page._home_error = None
        page._home_delta = None
        page.active_jog_axis = "X"
        page.active_jog_direction = 1
        page.jog_timer = types.SimpleNamespace(stop=lambda: events.append("stop_jog"))
        page.update_timer = types.SimpleNamespace(stop=lambda: events.append("pause_status"))
        page.home_monitor_timer = types.SimpleNamespace(start=lambda: events.append("start_monitor"))
        page.log_info = lambda msg: events.append(("log", msg))

        created = {}

        class FakeThread:
            def __init__(self, *, target, daemon):
                created["target"] = target
                created["daemon"] = daemon

            def start(self):
                events.append("start_thread")

        original_thread = self.module.threading.Thread
        self.module.threading.Thread = FakeThread
        try:
            self.module.NexcopeControlPage.on_move_to_center(page)
        finally:
            self.module.threading.Thread = original_thread

        self.assertTrue(page._home_in_progress)
        self.assertIsNone(page.active_jog_axis)
        self.assertEqual(page.active_jog_direction, 0)
        self.assertEqual(created["daemon"], True)
        self.assertEqual(events, ["stop_jog", "pause_status", "start_thread", "start_monitor", ("log", "回零开始")])

    def test_home_monitor_resumes_status_polling_after_background_home(self) -> None:
        events = []
        page = object.__new__(self.module.NexcopeControlPage)
        page._home_in_progress = False
        page._home_error = None
        page._home_delta = (100, -200)
        page.driver = types.SimpleNamespace(connected=True)
        page.home_monitor_timer = types.SimpleNamespace(stop=lambda: events.append("stop_monitor"))
        page.update_timer = types.SimpleNamespace(start=lambda: events.append("resume_status"))
        page.log_info = lambda msg: events.append(("log", msg))

        self.module.NexcopeControlPage.on_home_monitor_tick(page)

        self.assertEqual(events, ["stop_monitor", ("log", "回零完成: dX=100, dY=-200"), "resume_status"])


class NexcopeIlluminationTests(unittest.TestCase):
    def setUp(self) -> None:
        self.module = _load_controller_module()

    def test_flush_illumination_update_writes_without_immediate_readback(self) -> None:
        scheduled = []
        calls = []
        page = object.__new__(self.module.NexcopeControlPage)
        page.driver = types.SimpleNamespace(
            connected=True,
            set_sled=lambda upper, lower: calls.append(("set", upper, lower)),
            get_sled_levels=lambda: (_ for _ in ()).throw(AssertionError("readback should wait for slider release")),
        )
        page.upper_light_slider = types.SimpleNamespace(value=lambda: 120)
        page.lower_light_slider = types.SimpleNamespace(value=lambda: 340)
        page.log_info = lambda msg: calls.append(("log", msg))
        page.run_nexcope_serial_task = (
            lambda worker, on_success=None, on_error=None, pause_polling=True:
            scheduled.append((worker, on_success, on_error, pause_polling))
        )

        self.module.NexcopeControlPage.flush_illumination_update(page)

        self.assertEqual(calls, [])
        self.assertEqual(len(scheduled), 1)
        worker, _on_success, _on_error, pause_polling = scheduled[0]
        self.assertFalse(pause_polling)
        worker()
        self.assertEqual(calls, [("set", 120, 340)])

    def test_slider_release_writes_and_reads_back_once(self) -> None:
        scheduled = []
        calls = []
        page = object.__new__(self.module.NexcopeControlPage)
        page.driver = types.SimpleNamespace(
            connected=True,
            set_sled=lambda upper, lower: calls.append(("set", upper, lower)),
            get_sled_levels=lambda: (calls.append("readback") or {"ULP": 111, "DLP": 222}),
        )
        page.upper_light_slider = types.SimpleNamespace(value=lambda: 100)
        page.lower_light_slider = types.SimpleNamespace(value=lambda: 200)
        page.illumination_write_timer = types.SimpleNamespace(stop=lambda: calls.append("stop_timer"))
        page.apply_illumination_levels = lambda upper, lower: calls.append(("apply", upper, lower))
        page.log_info = lambda msg: calls.append(("log", msg))
        page.run_nexcope_serial_task = (
            lambda worker, on_success=None, on_error=None, pause_polling=True:
            scheduled.append((worker, on_success, on_error, pause_polling))
        )

        self.module.NexcopeControlPage.on_illumination_slider_released(page)

        self.assertEqual(calls, ["stop_timer"])
        self.assertEqual(len(scheduled), 1)
        worker, on_success, _on_error, pause_polling = scheduled[0]
        self.assertFalse(pause_polling)
        levels = worker()
        on_success(levels)
        self.assertEqual(calls, ["stop_timer", ("set", 100, 200), "readback", ("apply", 111, 222)])


class NexcopeAutofocusTests(unittest.TestCase):
    def setUp(self) -> None:
        self.module = _load_controller_module()

    def test_autofocus_positions_round_trip_to_json_file(self) -> None:
        page = object.__new__(self.module.NexcopeControlPage)
        with tempfile.TemporaryDirectory() as tmpdir:
            page.autofocus_config_path = str(Path(tmpdir) / "autofocus.json")

            self.module.NexcopeControlPage.save_autofocus_positions(page, {"x10": 123.5, "x100": 987.0})
            loaded = self.module.NexcopeControlPage.load_autofocus_positions(page)

        self.assertEqual(loaded, {"x10": 123.5, "x100": 987.0})

    def test_autofocus_calibrate_stores_current_z_for_selected_objective(self) -> None:
        scheduled = []
        saved = []
        logs = []
        page = object.__new__(self.module.NexcopeControlPage)
        page.driver = types.SimpleNamespace(connected=True, get_stage_xyz=lambda: {"X": 1, "Y": 2, "Z": 345.5})
        page.autofocus_combo = types.SimpleNamespace(currentText=lambda: "x50")
        page.load_autofocus_positions = lambda: {"x10": 100.0}
        page.save_autofocus_positions = lambda positions: saved.append(dict(positions))
        page.log_info = logs.append
        page.run_nexcope_serial_task = (
            lambda worker, on_success=None, on_error=None, pause_polling=True:
            scheduled.append((worker, on_success, on_error, pause_polling))
        )

        self.module.NexcopeControlPage.on_autofocus_calibrate(page)

        self.assertEqual(saved, [])
        self.assertEqual(len(scheduled), 1)
        worker, on_success, _on_error, pause_polling = scheduled[0]
        self.assertTrue(pause_polling)
        z_position = worker()
        on_success(z_position)
        self.assertEqual(saved, [{"x10": 100.0, "x50": 345.5}])
        self.assertEqual(logs, ["自动对焦 x50 已校准: Z=345.5"])

    def test_autofocus_go_moves_z_only_to_stored_position(self) -> None:
        scheduled = []
        calls = []
        page = object.__new__(self.module.NexcopeControlPage)
        page.driver = types.SimpleNamespace(
            connected=True,
            get_stage_xyz=lambda: {"X": 1, "Y": 2, "Z": 1000},
            move_up=lambda payload: calls.append(("U", payload)),
            move_down=lambda payload: calls.append(("D", payload)),
        )
        page.autofocus_combo = types.SimpleNamespace(currentText=lambda: "x100")
        page.load_autofocus_positions = lambda: {"x100": 1125.0}
        page.log_info = lambda msg: calls.append(("log", msg))
        page.run_nexcope_serial_task = (
            lambda worker, on_success=None, on_error=None, pause_polling=True:
            scheduled.append((worker, on_success, on_error, pause_polling))
        )

        self.module.NexcopeControlPage.on_autofocus_go(page)

        self.assertEqual(calls, [])
        self.assertEqual(len(scheduled), 1)
        worker, on_success, _on_error, pause_polling = scheduled[0]
        self.assertTrue(pause_polling)
        delta_z = worker()
        on_success(delta_z)
        self.assertEqual(calls, [("U", "125"), ("log", "自动对焦 x100 前往完成: dZ=125")])

    def test_autofocus_go_requires_calibrated_selection(self) -> None:
        logs = []
        page = object.__new__(self.module.NexcopeControlPage)
        page.driver = types.SimpleNamespace(connected=True)
        page.autofocus_combo = types.SimpleNamespace(currentText=lambda: "x10")
        page.load_autofocus_positions = lambda: {}
        page.log_info = logs.append

        self.module.NexcopeControlPage.on_autofocus_go(page)

        self.assertEqual(logs, ["自动对焦 x10 尚未校准"])


if __name__ == "__main__":
    unittest.main()
