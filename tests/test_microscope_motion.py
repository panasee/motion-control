"""Exercise the actual console signals and the driver ownership boundary, offline."""
import csv
import os
import subprocess
import sys
import time
from threading import Event, get_ident
from unittest.mock import Mock

os.environ.setdefault("QT_QPA_PLATFORM", "offscreen")
import pytest
from PyQt6.QtCore import Qt
from PyQt6.QtTest import QTest
from PyQt6.QtWidgets import QApplication

from microscope_motion import MotionConsole
from onway_driver import MCCDriver
from onway_session import DesktopOnwayDriver


def until(check):
    deadline = time.monotonic() + 3
    while not check():
        QApplication.processEvents()
        assert time.monotonic() < deadline, "console did not reach expected state"
        time.sleep(.002)


@pytest.fixture
def console(tmp_path):
    app = QApplication.instance() or QApplication([])
    driver = Mock(spec=MCCDriver)
    driver.get_axis_pos.return_value = [1, 2, 3, 4, 5, 6]
    page = MotionConsole(tmp_path, lambda: driver)
    page.show()
    app.processEvents()
    yield page, driver
    page.timer.stop()
    page.pool.shutdown()
    page.close()


@pytest.mark.parametrize("serial", [False, True])
def test_connect_read_jog_home_record_stop_disconnect(console, serial):
    page, driver = console
    assert not page.home.isEnabled()
    if serial:
        page.transport.setCurrentIndex(1)
        page.serial.setCurrentText("COM8")
        page.baud.setCurrentText("57600")
    thread_id = []
    driver.get_axis_pos.side_effect = lambda axis: thread_id.append(get_ident()) or [1, 2, 3, 4, 5, 6]
    page.connection.click()
    until(lambda: page.connected)
    if serial:
        driver.connect_serial.assert_called_once_with("COM8", 57600)
    else:
        driver.connect.assert_called_once_with("192.168.0.7", 8888)
    assert thread_id[0] != get_ident()
    assert [v.text() for v in page.positions] == [f"{i:.4f} mm" for i in range(1, 7)]
    button = page.jogs[6]  # A, negative
    QTest.mousePress(button, Qt.MouseButton.LeftButton)
    until(lambda: driver.move_at_speed.called and not page.pending)
    driver.move_at_speed.assert_called_once_with(3, -.1, 50.)
    QTest.mouseRelease(button, Qt.MouseButton.LeftButton)
    until(lambda: driver.emergency_stop.called and not page.pending)
    driver.emergency_stop.assert_called_once_with(3)
    page.home.click()
    until(lambda: not page.pending)
    assert [args.args[0] for args in driver.home.call_args_list] == list(range(6))
    page.record.click()
    until(lambda: not page.pending)
    with (page.record_dir / "onway-positions.csv").open() as saved:
        header, sample = csv.reader(saved)
    assert header[1:] == list("XYZABC") and sample[1:] == list("123456")
    page.stop.click()
    until(lambda: not page.pending)
    driver.emergency_stop_all.assert_called_once()
    page.connection.click()
    until(lambda: not page.connected)
    assert not page.stop.isEnabled() and page.positions[0].text() == "—"
    driver.disconnect.assert_called_once()


def test_failed_connection_or_read_never_enables_motion(console):
    page, driver = console
    driver.connect.return_value = False
    page.connection.click()
    until(lambda: not page.pending)
    assert not page.connected and not page.jogs[0].isEnabled()
    driver.connect.return_value = True
    driver.get_axis_pos.return_value = [0] * 5
    page.connection.click()
    until(lambda: not page.pending)
    assert "无法读取" in page.status.text()
    assert not page.connected and page.connection.isEnabled()
    assert driver.disconnect.call_count == 2


def test_release_during_pending_read_cannot_start_a_late_jog(console):
    page, driver = console
    page.connection.click()
    until(lambda: page.connected)
    gate = Event()
    driver.get_axis_pos.side_effect = lambda axis: gate.wait(2) and [1] * 6
    page.poll()
    QTest.mousePress(page.jogs[1], Qt.MouseButton.LeftButton)
    QTest.mouseRelease(page.jogs[1], Qt.MouseButton.LeftButton)
    gate.set()
    until(lambda: not page.pending)
    driver.move_at_speed.assert_not_called()
    page.connection.click()
    until(lambda: not page.connected)


def test_close_requires_acknowledged_stop_and_retains_connection_on_failure(console):
    page, driver = console
    page.connection.click()
    until(lambda: page.connected)
    page.home.click()
    until(lambda: not page.pending)
    driver.emergency_stop_all.return_value = False
    assert not page.prepare_close()
    until(lambda: not page.pending)
    assert page.connected and not page.home.isEnabled() and page.stop.isEnabled()
    driver.disconnect.assert_not_called()
    driver.emergency_stop_all.return_value = True
    closed = []
    page.ready_to_close.connect(lambda: closed.append(True))
    assert not page.prepare_close()
    until(lambda: closed)
    driver.disconnect.assert_called_once()


def test_two_processes_cannot_open_the_same_controller(monkeypatch, tmp_path):
    monkeypatch.setenv("LOCALAPPDATA", str(tmp_path))
    monkeypatch.setattr(MCCDriver, "connect", lambda self, *args: True)
    owner = DesktopOnwayDriver()
    assert owner.connect("localhost", 8888)
    code = """
from onway_session import DesktopOnwayDriver
from onway_driver import MCCDriver
MCCDriver.connect_serial = lambda *args: True
driver = DesktopOnwayDriver()
try:
    assert driver.connect_serial('TEST', 115200)
except RuntimeError as exc:
    assert '占用' in str(exc)
    raise SystemExit(7)
driver.disconnect()
"""
    try:
        assert subprocess.run([sys.executable, "-c", code]).returncode == 7
    finally:
        owner.disconnect()
    assert subprocess.run([sys.executable, "-c", code]).returncode == 0
