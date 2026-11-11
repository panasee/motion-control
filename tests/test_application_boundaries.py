"""Cross-application boundaries: isolated imports, lifecycle and API compatibility."""
import json
import os
from pathlib import Path
import subprocess
import sys
from urllib.error import HTTPError
from urllib.request import urlopen

import pytest

from camera_protocol import CameraHttpServer, CameraImageClient
from microscope_settings import application_settings_path, settings_read_path
from position_server import PositionHttpServer, PositionState


def test_microscope_constructs_without_motion_stack_or_opencv(tmp_path):
    script = r'''
import importlib.abc, sys
from pathlib import Path
blocked = {"controller_gui", "onway_driver", "nexcope_driver", "position_server",
           "stack_designer", "photo_selection", "cv2", "serial", "peer_apps"}
class Boundary(importlib.abc.MetaPathFinder):
    def find_spec(self, fullname, *args):
        if fullname.split(".")[0] in blocked:
            raise AssertionError("Forbidden microscope dependency: " + fullname)
sys.meta_path.insert(0, Boundary())
from PyQt6.QtWidgets import QApplication
from microscope_app import ApplicationInstance
from microscope_workspace import MicroscopeWindow
app = QApplication([])
window = MicroscopeWindow(settings_path=Path(sys.argv[1]))
assert window.parent() is None
assert not window.camera_session.alive
assert "nikon_ks_bindings" not in sys.modules
window.show()
app.processEvents()
assert window.close()
print("independent")
'''
    result = subprocess.run([sys.executable, "-c", script, str(tmp_path / "settings.json")],
                            capture_output=True, text=True, timeout=15,
                            env={**os.environ, "QT_QPA_PLATFORM": "offscreen"})
    assert result.returncode == 0, result.stdout + result.stderr
    assert "independent" in result.stdout


def test_settings_namespace_and_read_only_legacy_fallback(tmp_path, monkeypatch):
    monkeypatch.setenv("APPDATA", str(tmp_path))
    destination = application_settings_path()
    assert destination == tmp_path / "microscope-workbench" / "settings.json"
    legacy = tmp_path / "motion-control" / "microscope-settings.json"
    legacy.parent.mkdir()
    legacy.write_text('{"legacy":true}', encoding="utf-8")
    assert settings_read_path(destination, migrate_legacy=True) == legacy
    assert settings_read_path(destination) == destination
    destination.parent.mkdir()
    destination.write_text('{"new":true}', encoding="utf-8")
    assert settings_read_path(destination, migrate_legacy=True) == destination
    assert json.loads(legacy.read_text()) == {"legacy": True}


def test_valid_legacy_preferences_migrate_without_overwriting_original(tmp_path, monkeypatch):
    from PyQt6.QtWidgets import QApplication
    from microscope_workspace import MicroscopeWindow
    app = QApplication.instance() or QApplication([])
    monkeypatch.setenv("APPDATA", str(tmp_path))
    legacy = tmp_path / "motion-control" / "microscope-settings.json"
    original = MicroscopeWindow(settings_path=legacy)
    original.exposure_box.setValue(44.4)
    assert original.close()
    # Actual older files predate the edge-parameter menu.
    old_settings = json.loads(legacy.read_text(encoding="utf-8"))
    old_settings.pop("edge_detection")
    legacy.write_text(json.dumps(old_settings), encoding="utf-8")
    before = legacy.read_bytes()
    migrated = MicroscopeWindow()
    assert migrated.settings_path == application_settings_path()
    assert migrated.exposure_box.value() == 44.4
    assert migrated.edge_detection_parameters() == {
        "max_dimension": 720, "threshold": 0, "min_area_percent": 0.01, "simplification": 0.4}
    migrated.exposure_box.setValue(93.2)
    assert migrated.close()
    assert legacy.read_bytes() == before
    assert json.loads(application_settings_path().read_text())["exposure_ms"] == 93.2
    reopened = MicroscopeWindow()
    assert reopened.exposure_box.value() == 93.2
    reopened.close()
    app.processEvents()


def test_direct_camera_api_survives_motion_server_stop_and_legacy_proxy_matches():
    png = b"\x89PNG\r\n\x1a\nfixture-image"
    camera = CameraHttpServer(lambda: (png, "frame-123"), port=0)
    camera.start()
    client = CameraImageClient(camera.url)
    state = PositionState()
    state.update_xy(1, 2)
    motion = PositionHttpServer(state, port=0, image_provider=client.snapshot_png)
    motion.start()
    try:
        path = "/api/v1/camera/image"
        with urlopen(camera.url + path, timeout=2) as direct, urlopen(motion.url + path, timeout=2) as legacy:
            assert direct.read() == legacy.read() == png
            assert direct.headers["X-Image-Id"] == legacy.headers["X-Image-Id"] == "frame-123"
        motion.stop()
        assert client.snapshot_png() == (png, "frame-123")
        with pytest.raises(HTTPError) as error:
            urlopen(camera.url + path, data=b"capture", timeout=2)
        assert error.value.code == 405
    finally:
        motion.stop()
        camera.stop()
    assert client.snapshot_png() is None


def test_compatibility_image_unavailable_does_not_break_position():
    state = PositionState()
    state.update_xy(12, 34)
    motion = PositionHttpServer(state, port=0, image_provider=lambda: None)
    motion.start()
    try:
        with pytest.raises(HTTPError) as error:
            urlopen(motion.url + "/api/v1/camera/image", timeout=2)
        assert error.value.code == 503
        with urlopen(motion.url + "/position", timeout=2) as response:
            assert response.read() == b'{"X":12.0000, "Y":34.0000}'
    finally:
        motion.stop()


def test_proxy_preserves_camera_encoding_failure():
    def fail():
        raise OSError("encoding failed")
    camera = CameraHttpServer(fail, port=0)
    camera.start()
    motion = PositionHttpServer(PositionState(), port=0,
                                image_provider=CameraImageClient(camera.url).snapshot_png)
    motion.start()
    try:
        with pytest.raises(HTTPError) as error:
            urlopen(motion.url + "/api/v1/camera/image", timeout=2)
        assert error.value.code == 500
        assert json.loads(error.value.read())["error"]["code"] == "image_encoding_failed"
    finally:
        motion.stop()
        camera.stop()


def test_single_instance_activation_raises_existing_window(tmp_path):
    from threading import Thread
    import time
    from PyQt6.QtWidgets import QApplication, QWidget
    from microscope_app import ApplicationInstance
    app = QApplication.instance() or QApplication([])
    instance = ApplicationInstance(tmp_path / "settings.json")
    other = ApplicationInstance(tmp_path / "settings.json")
    window = QWidget()
    assert instance.acquire()
    try:
        instance.listen(window)
        assert not other.acquire()
        result = []
        thread = Thread(target=lambda: result.append(other.activate_existing()))
        thread.start()
        deadline = time.monotonic() + 3
        while thread.is_alive() and time.monotonic() < deadline:
            app.processEvents()
            time.sleep(.005)
        thread.join(1)
        assert result == [True]
        assert window.isVisible()
    finally:
        instance.close()
        window.close()
    assert other.acquire()
    other.close()


def test_peer_launch_survives_launcher_process_exit(tmp_path):
    import time
    marker = tmp_path / "peer-survived.txt"
    script = r'''
import subprocess, sys
from pathlib import Path
import peer_apps
marker = Path(sys.argv[1])
code = "import time; from pathlib import Path; time.sleep(.4); Path(" + repr(str(marker)) + ").write_text('alive')"
peer_apps.microscope_command = lambda: ([sys.executable, "-c", code], marker.parent)
print(peer_apps.launch_microscope())
'''
    parent = subprocess.run([sys.executable, "-c", script, str(marker)], capture_output=True,
                            text=True, timeout=10)
    assert parent.returncode == 0, parent.stderr
    assert int(parent.stdout.strip()) > 0
    deadline = time.monotonic() + 5
    while not marker.exists() and time.monotonic() < deadline:
        time.sleep(.02)
    assert marker.read_text() == "alive"
