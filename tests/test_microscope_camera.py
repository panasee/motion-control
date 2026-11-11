"""Offline camera contract/lifecycle regressions. No SDK or hardware is loaded."""
import json
from pathlib import Path
from threading import get_ident
import time
from unittest.mock import patch

import numpy as np
import pytest
from PyQt6.QtCore import QPointF
from PyQt6.QtWidgets import QApplication, QMessageBox, QPushButton

from microscope_camera import CameraFrame, CameraSession
from microscope_workspace import MicroscopeWindow


def wait_until(predicate, app=None, timeout=3):
    end = time.monotonic() + timeout
    while time.monotonic() < end:
        if app:
            app.processEvents()
        if predicate():
            return
        time.sleep(.005)
    raise AssertionError("Timed out waiting for camera lifecycle")


class FakeCamera:
    def __init__(self, *, fail=None, frames=True):
        self.fail = fail
        self.frames = frames
        self.calls = []
        self.number = 0
        self.active = False
        self.settings = dict(device={"serial": 123}, exposure_ms=20, gain=2, mode=1, color=1,
                             exposure_auto=True, modes=[dict(id=1, width=120, height=80),
                                                        dict(id=3, width=60, height=40)],
                             ranges={"exposure_ms": [.1, 30000, .02], "gain": [1, 64, .01]})

    def record(self, name):
        self.calls.append((name, get_ident()))
        if self.fail == name:
            raise RuntimeError(f"fake {name} failure")

    def open(self):
        self.record("open")
        return self.settings.copy()

    def configure(self, **values):
        self.record("configure")
        self.settings.update(values, exposure_auto=False)
        return self.settings.copy()

    def start(self):
        self.record("start")
        self.active = True

    def poll(self):
        self.record("poll")
        if not self.frames:
            return None
        assert self.active
        time.sleep(.002)
        self.number += 1
        width, height = (120, 80) if self.settings["mode"] == 1 else (60, 40)
        rgb = np.empty((height, width, 3), np.uint8)
        rgb[:] = [self.number % 200, 40, 60]
        metadata = dict(device_serial=123, mode=self.settings["mode"], frame_no=self.number,
                        fps=30, missing_frame_ids=0, exposure_ms=self.settings["exposure_ms"],
                        gain=self.settings["gain"], width=width, height=height)
        return CameraFrame(rgb, metadata)

    def stop(self):
        self.record("stop")
        self.active = False

    def close(self):
        self.record("close")
        self.active = False


def test_session_serializes_sdk_and_bounds_mailbox_and_takes_fresh_single():
    backend = FakeCamera()
    session = CameraSession(lambda: backend)
    session.connect()
    try:
        wait_until(lambda: not session.events.empty())
        session.request("preview")
        wait_until(lambda: backend.number >= 12)
        assert session.display_skipped > 0
        latest = session.take_frame()
        assert latest.metadata["frame_no"] >= 10
        assert session.take_frame() is None
        session.request("stop")
        wait_until(lambda: not backend.active)
        session.clear_frame()
        number = backend.number
        session.request("capture")
        wait_until(lambda: backend.number > number and not backend.active)
        assert backend.number == number + 1
        assert session.take_frame().metadata["frame_no"] == number + 1
    finally:
        session.request("disconnect")
        session.thread.join(3)
    assert not session.alive
    assert {tid for _, tid in backend.calls} == {session.thread.ident}
    assert session.thread.ident != get_ident()
    assert backend.calls[-1][0] == "close"


@pytest.mark.parametrize("failure", ["open", "configure", "start", "poll"])
def test_partial_failure_always_closes_and_reports(failure):
    backend = FakeCamera(fail=failure)
    session = CameraSession(lambda: backend)
    session.connect()
    if failure != "open":
        wait_until(lambda: not session.events.empty())
        if failure == "configure":
            session.request("configure", exposure_ms=20, gain=2, mode=1)
        else:
            session.request("preview")
    session.thread.join(3)
    assert not session.alive
    assert backend.calls[-1][0] == "close"
    events = list(session.events.queue)
    assert any(event == "error" and failure in text for event, text in events)
    assert events[-1] == ("disconnected", None)


def test_disconnect_does_not_wait_for_long_exposure():
    backend = FakeCamera(frames=False)
    backend.settings["exposure_ms"] = 30000
    session = CameraSession(lambda: backend)
    session.connect()
    session.request("capture")
    wait_until(lambda: backend.active)
    started = time.monotonic()
    session.request("disconnect")
    session.thread.join(1)
    assert not session.alive
    assert time.monotonic() - started < 1
    assert backend.calls[-1][0] == "close"


@pytest.fixture(scope="module")
def app():
    return QApplication.instance() or QApplication([])


@pytest.fixture
def window(app, tmp_path):
    backend = FakeCamera()
    window = MicroscopeWindow(settings_path=tmp_path / "settings.json", camera_factory=lambda: backend)
    window.show()
    yield window, backend
    window.camera_session.request("disconnect")
    if window.camera_session.thread:
        window.camera_session.thread.join(3)
    window.close()
    window.deleteLater()
    app.processEvents()


def connect(window, app):
    window.camera_connect.click()
    wait_until(lambda: window.camera_state == "idle", app)


def capture(window, backend, app):
    previous = backend.number
    window.camera_capture.click()
    wait_until(lambda: window.camera_state == "idle" and window.camera_frame_metadata is not None
               and window.camera_frame_metadata["frame_no"] > previous, app)


def test_ui_explicit_connect_and_live_preserves_annotations_and_view(window, app):
    win, backend = window
    assert not backend.calls
    assert win.camera_connect.isEnabled()
    assert not win.camera_preview.isEnabled()
    connect(win, app)
    assert win.exposure_box.value() == 20  # hardware readback replaces saved preset
    capture(win, backend, app)
    win.set_calibration(.5)
    win.add_annotation("profile", [QPointF(2, 2), QPointF(30, 20)])
    item = win.annotations[0]
    win.view.scale(1.4, 1.4)
    transform = win.view.transform()
    pixmap_item = win.image_item
    win.camera_preview.click()
    wait_until(lambda: backend.number >= 15 and win.camera_state == "streaming"
               and win.camera_frame_metadata["frame_no"] > 1, app)
    assert win.image_item is pixmap_item
    assert win.annotations == [item]
    assert win.view.transform() == transform
    assert win.microns_per_pixel == .5
    assert win.profile_data is not None
    motion = win.device_tabs.widget(1)
    assert all(not b.isEnabled() for b in motion.findChildren(QPushButton)
               if b.property("hardware_command"))
    win.camera_preview.click()
    wait_until(lambda: win.camera_state == "idle", app)
    number = backend.number
    for _ in range(5):
        app.processEvents()
        time.sleep(.02)
    assert backend.number == number


def test_ui_resolution_change_invalidates_geometry_and_saves_raw(window, app, tmp_path):
    win, backend = window
    connect(win, app)
    capture(win, backend, app)
    win.set_calibration(.5)
    win.add_annotation("line", [QPointF(2, 2), QPointF(30, 20)])
    win.camera_mode.setCurrentIndex(1)
    with patch.object(QMessageBox, "question", return_value=QMessageBox.StandardButton.Yes):
        win.camera_apply.click()
        wait_until(lambda: win.camera_state == "idle", app)
        capture(win, backend, app)
    assert win.image.width() == 60
    assert win.microns_per_pixel is None
    assert not win.annotations
    win.set_white_balance([2, 1, 1])
    raw = win.source_rgb.copy()
    destination = tmp_path / "raw.png"
    with patch("microscope_workspace.QFileDialog.getSaveFileName", return_value=(str(destination), "")):
        win.save_camera_frame()
    from PyQt6.QtGui import QImage
    from microscope_imaging import image_rgb8
    np.testing.assert_array_equal(image_rgb8(QImage(str(destination))), raw)
    saved = json.loads(destination.with_suffix(".json").read_text(encoding="utf-8"))
    assert saved["frame_no"] == win.camera_frame_metadata["frame_no"]
    assert saved["software_white_balance"] == [2, 1, 1]


def test_window_close_stops_and_reopen_requires_explicit_connect(window, app):
    win, backend = window
    connect(win, app)
    win.camera_preview.click()
    wait_until(lambda: backend.number >= 3, app)
    assert not win.close()  # asynchronous SDK cleanup
    wait_until(lambda: not win.isVisible() and not win.camera_session.alive, app)
    assert backend.calls[-1][0] == "close"
    calls = len(backend.calls)
    win.show()
    app.processEvents()
    assert len(backend.calls) == calls
    assert win.camera_state == "disconnected"
    assert win.camera_connect.isEnabled()
    connect(win, app)
    assert win.camera_state == "idle"


def test_closing_motion_does_not_stop_peer_camera(window, app):
    import controller_gui
    win, backend = window
    with patch.object(controller_gui.PositionHttpServer, "start"), patch.object(controller_gui.PositionHttpServer, "stop"):
        shell = controller_gui.ControllerMainWindow()
        shell.show()
        connect(win, app)
        win.camera_preview.click()
        wait_until(lambda: backend.number >= 3, app)
        before = backend.number
        assert shell.close()
        wait_until(lambda: backend.number > before + 3, app)
        assert win.isVisible() and win.camera_session.alive
        assert not hasattr(shell, "camera_image_state")
        assert shell.position_server.image_provider.__self__ is shell.camera_image_client
        shell.deleteLater()


def test_current_image_api_uses_raw_camera_frame_and_clears_on_source_or_connection_change(window, app):
    from urllib.error import HTTPError
    from urllib.request import urlopen
    from PyQt6.QtGui import QImage
    from microscope_imaging import image_rgb8
    from camera_protocol import CameraHttpServer

    win, backend = window
    server = CameraHttpServer(win.image_state.snapshot_png, port=0)
    server.start()
    url = server.url + "/api/v1/camera/image"

    def unavailable():
        with pytest.raises(HTTPError) as error:
            urlopen(url, timeout=3)
        assert error.value.code == 503

    try:
        unavailable()
        connect(win, app)
        unavailable()  # Connecting alone does not acquire an image.
        capture(win, backend, app)
        win.set_white_balance([2, 1, 1])
        win.add_annotation("line", [QPointF(2, 2), QPointF(30, 20)])
        calls = len(backend.calls)
        with urlopen(url, timeout=3) as response:
            image = QImage.fromData(response.read(), "PNG")
            metadata = json.loads(image.text("microscope_frame"))
            assert metadata["image_id"] == response.headers["X-Image-Id"]
        np.testing.assert_array_equal(image_rgb8(image), win.source_rgb)
        assert metadata["camera"] == win.camera_frame_metadata
        assert len(backend.calls) == calls  # GET never asks the SDK to capture.
        win.load_image(image, "local.png")
        unavailable()
        capture(win, backend, app)
        assert win.image_state.snapshot_png() is not None
        win.camera_connect.click()
        unavailable()  # Invalid immediately, even before SDK cleanup completes.
        wait_until(lambda: win.camera_state == "disconnected", app)
        unavailable()
        connect(win, app)
        unavailable()  # Old image must not reappear on reconnect.
    finally:
        server.stop()
