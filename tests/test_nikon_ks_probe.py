"""Offline failure-path checks; these tests never load a DLL or open hardware."""
import ctypes
from types import SimpleNamespace
from unittest.mock import Mock

import pytest

from tools.nikon_ks_probe import Camera, Feature, RESTORE_IDS, U32, restore_settings


def bare_camera():
    camera = Camera.__new__(Camera)
    camera.log = {}
    camera.handle = U32()
    camera.enumerated = False
    camera.transfer = False
    camera.dll_directory = SimpleNamespace(close=Mock())
    camera.call = Mock()
    return camera


@pytest.mark.parametrize("devices", [
    [],
    [{"name": "DS-Fi3(Sim)", "kind": 6, "index": 0}],
    [{"name": "DS-Fi3", "kind": 6, "index": 0}],
    [{"name": "DS-Fi3", "kind": 5, "index": 0},
     {"name": "DS-Fi3", "kind": 5, "index": 1}],
])
def test_refuses_absent_simulated_or_ambiguous_camera(devices):
    camera = bare_camera()
    with pytest.raises(RuntimeError, match="exactly one real"):
        camera.open(devices)
    camera.call.assert_not_called()


def test_selects_real_camera_even_when_simulator_is_first():
    camera = bare_camera()
    devices = [{"name": "DS-Fi3(Sim)", "kind": 6, "index": 0},
               {"name": "DS-Fi3", "kind": 5, "index": 1}]
    camera.open(devices)
    assert camera.call.call_args.args[:2] == ("CAM_Open", 1)


def test_null_device_list_is_rejected_and_still_released():
    camera = bare_camera()
    def enumerate_without_pointer(name, *args):
        if name == "CAM_OpenDevices":
            ctypes.cast(args[0], ctypes.POINTER(U32)).contents.value = 1
    camera.call.side_effect = enumerate_without_pointer
    with pytest.raises(RuntimeError, match="Invalid or empty"):
        camera.enumerate()
    assert camera.enumerated
    camera.close()
    assert camera.call.call_args.args == ("CAM_CloseDevices",)


def test_release_is_attempted_when_camera_close_fails():
    camera = bare_camera()
    camera.handle = U32(1)
    camera.enumerated = True
    def fail_close(name, *args):
        if name == "CAM_Close":
            raise RuntimeError("close failed")
    camera.call.side_effect = fail_close
    with pytest.raises(RuntimeError, match="close failed"):
        camera.close()
    assert [c.args[0] for c in camera.call.call_args_list] == ["CAM_Close", "CAM_CloseDevices"]
    camera.dll_directory.close.assert_called_once()
    assert camera.log["cleanup_errors"] == ["close failed"]


def test_dropless_restore_failure_does_not_skip_feature_restoration():
    original = {i: Feature() for i in RESTORE_IDS}
    camera = SimpleNamespace(stop=Mock(), command=Mock(side_effect=RuntimeError("dropless error")),
                             set_features=Mock(), features=Mock(return_value=original))
    report = {}
    with pytest.raises(RuntimeError, match="dropless error"):
        restore_settings(camera, original, report, False)
    camera.set_features.assert_called_once()
    camera.features.assert_called_once()
    assert set(report["restored_features"]) == set(RESTORE_IDS)
