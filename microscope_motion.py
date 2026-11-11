"""Onway console. All controller I/O is serialized off the Qt event thread."""
import csv
from concurrent.futures import ThreadPoolExecutor
from datetime import datetime
from math import isfinite
from threading import Event

from PyQt6.QtCore import Qt, QTimer, pyqtSignal
from PyQt6.QtWidgets import (QComboBox, QDoubleSpinBox, QFormLayout, QGridLayout, QHBoxLayout,
                             QLabel, QLineEdit, QPushButton, QSpinBox, QVBoxLayout, QWidget)


def create_driver():
    from onway_session import DesktopOnwayDriver
    return DesktopOnwayDriver()


class MotionConsole(QWidget):
    status_changed = pyqtSignal(str)
    ready_to_close = pyqtSignal()
    completed = pyqtSignal(str, object, str)

    def __init__(self, record_dir, factory=create_driver):
        super().__init__()
        self.record_dir = record_dir
        self.factory = factory
        self.driver = None  # worker thread owns this and all device calls
        self.stop_needed = False
        self.worker_fault = False
        self.pool = ThreadPoolExecutor(max_workers=1, thread_name_prefix="onway")
        self.pending = set()
        self.connected = self.fault = self.closing = False
        self.held = None
        self.homing = Event()
        self.completed.connect(self.finish, Qt.ConnectionType.QueuedConnection)
        layout = QVBoxLayout(self)
        form = QFormLayout()
        self.transport = QComboBox()
        self.transport.addItems(["TCP/IP", "串口"])
        form.addRow("Onway 连接方式", self.transport)
        self.host = QLineEdit("192.168.0.7")
        self.port = QSpinBox()
        self.port.setRange(1, 65535)
        self.port.setValue(8888)
        self.serial = QComboBox()
        self.serial.setEditable(True)
        self.serial.setPlaceholderText("如 COM3")
        self.refresh = QPushButton("刷新串口")
        self.refresh.clicked.connect(self.refresh_ports)
        self.baud = QComboBox()
        self.baud.addItems(["9600", "19200", "38400", "57600", "115200"])
        self.baud.setCurrentText("115200")
        for label, field in (("IP 地址", self.host), ("TCP 端口", self.port),
                             ("串口号", self.serial), ("", self.refresh), ("波特率", self.baud)):
            form.addRow(label, field)
        def show_transport(index):
            for field in (self.host, self.port):
                form.setRowVisible(field, index == 0)
            for field in (self.serial, self.refresh, self.baud):
                form.setRowVisible(field, index == 1)
        self.transport.currentIndexChanged.connect(show_transport)
        show_transport(0)
        layout.addLayout(form)
        self.connection = QPushButton("连接 Onway")
        self.connection.clicked.connect(self.toggle)
        layout.addWidget(self.connection)
        self.status = QLabel("未连接")
        self.status.setWordWrap(True)
        layout.addWidget(self.status)
        self.speed = QDoubleSpinBox()
        self.speed.setDecimals(3)
        self.speed.setRange(.001, 2.)
        self.speed.setValue(.1)
        self.speed.setSingleStep(.001)
        self.speed.setSuffix(" mm/s")
        form.addRow("点动速度", self.speed)
        grid = QGridLayout()
        self.positions, self.jogs = [], []
        for axis, name in enumerate("XYZABC"):
            grid.addWidget(QLabel(name), axis, 0)
            value = QLabel("—")
            self.positions.append(value)
            grid.addWidget(value, axis, 1)
            for col, sign in enumerate((-1, 1), 2):
                button = QPushButton("−" if sign < 0 else "＋")
                button.setToolTip("按住点动，松开停止")
                button.pressed.connect(lambda a=axis, s=sign: self.press(a, s))
                button.released.connect(lambda a=axis: self.release(a))
                self.jogs.append(button)
                grid.addWidget(button, axis, col)
        layout.addLayout(grid)
        row = QHBoxLayout()
        self.home = QPushButton("六轴回零")
        self.record = QPushButton("记录位置")
        self.stop = QPushButton("停止运动")
        self.home.clicked.connect(self.home_all)
        self.record.clicked.connect(lambda: self.submit("record"))
        self.stop.clicked.connect(self.stop_all)
        for button in (self.home, self.record, self.stop):
            row.addWidget(button)
        layout.addLayout(row)
        layout.addStretch()
        for button in [*self.jogs, self.home, self.record, self.stop]:
            button.setProperty("hardware_command", True)
        self.timer = QTimer(self)
        self.timer.setInterval(300)
        self.timer.timeout.connect(self.poll)
        self.controls()

    def refresh_ports(self):
        from serial.tools import list_ports
        selected = self.serial.currentText()
        self.serial.clear()
        self.serial.addItems([p.device for p in list_ports.comports()])
        if selected:
            self.serial.setCurrentText(selected)

    def controls(self):
        ending = self.closing or "disconnect" in self.pending
        idle = not self.connected and not self.pending and not ending
        for field in (self.transport, self.host, self.port, self.serial, self.refresh, self.baud):
            field.setEnabled(idle)
        self.connection.setText("断开 Onway" if self.connected else "连接 Onway")
        self.connection.setEnabled(idle or (self.connected and not ending))
        active = self.connected and not self.fault and not ending
        busy = bool(self.pending - {"read"}) or self.held is not None
        for button in self.jogs:
            # Keep the pressed button enabled so Qt will deliver its release.
            button.setEnabled(active and (not busy or button.isDown()))
        self.home.setEnabled(active and not busy)
        self.record.setEnabled(active and not busy)
        self.stop.setEnabled(self.connected and not ending and "stop" not in self.pending)

    def toggle(self):
        if self.connected:
            self.disconnect_device()
            return
        tcp = self.transport.currentIndex() == 0
        name = (self.host.text() if tcp else self.serial.currentText()).strip()
        if not name:
            self.status.setText("请输入 IP 地址。" if tcp else "请选择或输入串口号。")
            return
        self.status.setText("连接中…")
        self.submit("connect", tcp, name, self.port.value() if tcp else int(self.baud.currentText()))

    def press(self, axis, sign):
        if self.held is None and self.connected and not self.fault:
            valid = Event()
            valid.set()
            self.held = axis, valid
            self.submit("jog", axis, sign * self.speed.value(), valid)

    def release(self, axis):
        if self.held and axis == self.held[0]:
            self.held[1].clear()
            self.held = None
            self.submit("release", axis)

    def hideEvent(self, event):
        if self.held:
            self.release(self.held[0])
        super().hideEvent(event)

    def home_all(self):
        self.homing.set()
        self.submit("home")

    def cancel_hold(self):
        self.homing.clear()
        if self.held:
            self.held[1].clear()
            self.held = None

    def stop_all(self):
        self.cancel_hold()
        self.submit("stop")

    def poll(self):
        if self.connected and not self.pending and not self.held and not self.fault:
            self.submit("read")

    def disconnect_device(self):
        self.cancel_hold()
        self.timer.stop()
        self.submit("disconnect")

    def prepare_close(self):
        if self.connected or self.pending:
            self.closing = True
            if self.connected and "disconnect" not in self.pending:
                self.disconnect_device()
            return False
        return True

    def submit(self, op, *args):
        if op in self.pending:
            return
        self.pending.add(op)
        self.controls()
        self.pool.submit(self.run_operation, op, args)

    def read_positions(self):
        values = self.driver.get_axis_pos(0xFF)
        if values is None or len(values) != 6 or not all(isfinite(x) for x in values):
            raise OSError("无法读取六轴位置。")
        return tuple(values)

    def run_operation(self, op, args):
        try:
            if self.worker_fault and op in ("jog", "home", "read", "record"):
                raise RuntimeError("设备故障；请断开并重新连接。")
            data = self.operate(op, *args)
        except Exception as exc:
            # Surface worker failures in the console and disable new motion until reconnect.
            self.worker_fault = True
            error = f"{type(exc).__name__}: {exc}"
            if self.stop_needed and op in ("home", "jog", "release", "read", "record"):
                try:
                    self.operate("stop")
                except (OSError, RuntimeError) as stop_error:
                    error += f" 全轴停止未确认：{stop_error}"
            self.completed.emit(op, None, error)
        else:
            self.completed.emit(op, data, "")

    def operate(self, op, *args):
        if op == "connect":
            tcp, name, port = args
            self.driver = self.factory()
            try:
                opener = self.driver.connect if tcp else self.driver.connect_serial
                if not opener(name, port):
                    raise OSError(f"Onway 未响应：{name} / {port}")
                self.worker_fault = False
                return self.read_positions()
            except Exception:
                self.driver.disconnect()
                self.driver = None
                raise
        if op in ("read", "record"):
            values = self.read_positions()
            if op == "record":
                self.record_dir.mkdir(parents=True, exist_ok=True)
                with (self.record_dir / "onway-positions.csv").open("a", newline="", encoding="utf-8") as out:
                    writer = csv.writer(out)
                    if out.tell() == 0:
                        writer.writerow(["time", *"XYZABC"])
                    writer.writerow([datetime.now().astimezone().isoformat(), *values])
            return values
        if op == "disconnect":
            if self.stop_needed:
                self.operate("stop")
            self.driver.disconnect()
            self.driver = None
            return
        if op == "jog":
            axis, speed, valid = args
            if not valid.is_set():
                return
            self.stop_needed = True  # even a lost acknowledgment can mean motion
            ok = self.driver.move_at_speed(axis, speed, 50.)
        elif op == "release":
            ok = self.driver.emergency_stop(args[0])
        elif op == "home":
            for axis in range(6):
                if not self.homing.is_set():
                    return
                self.stop_needed = True
                if not self.driver.home(axis):
                    raise OSError(f"{'XYZABC'[axis]} 轴回零未确认。")
            return
        elif op == "stop":
            ok = self.driver.emergency_stop_all()
            if ok:
                self.stop_needed = False
        else:
            raise ValueError(op)
        if not ok:
            raise OSError("控制器未确认指令，请检查设备并确认已停止。")

    def finish(self, op, data, error):
        self.pending.remove(op)
        if error:
            self.fault = True
            self.timer.stop()
            self.status.setText(error)
            self.status_changed.emit("故障" if self.connected else "连接失败")
            for label in self.positions:
                label.setText("—")
            if self.held:
                self.release(self.held[0])
            if op == "disconnect":
                self.closing = False  # stop not acknowledged; keep ownership and allow retry
        elif op == "connect":
            self.connected, self.fault = True, False
            self.status.setText("已连接 Onway")
            self.status_changed.emit("已连接")
            self.timer.start()
        elif op == "disconnect":
            self.connected, self.fault = False, False
            self.status.setText("未连接")
            self.status_changed.emit("未连接")
            for label in self.positions:
                label.setText("—")
        elif op == "record":
            self.status.setText(f"位置已记录：{self.record_dir / 'onway-positions.csv'}")
        if data is not None:
            for label, value in zip(self.positions, data):
                label.setText(f"{value:.4f} mm")
        self.controls()
        if self.closing and not self.pending:
            if self.connected:
                self.disconnect_device()
            else:
                self.closing = False
                self.ready_to_close.emit()
