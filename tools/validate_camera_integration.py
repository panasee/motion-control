"""Explicit hardware acceptance of the standalone workspace, with no motion imports.

Run with --hardware; outputs under docs/camera-validation/integration/.
The regular pytest suite uses a fake camera instead.
"""
import argparse
import json
from pathlib import Path
import sys
import time
from unittest.mock import patch

if __package__ in (None, ""):
    sys.path.insert(0, str(Path(__file__).resolve().parents[1]))

from PyQt6.QtCore import QPointF, QTimer
from PyQt6.QtGui import QFontDatabase
from PyQt6.QtWidgets import QApplication, QMessageBox
from microscope_workspace import MicroscopeWindow


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--hardware", action="store_true")
    args = parser.parse_args()
    if not args.hardware:
        parser.error("--hardware is required to open the real camera")
    destination = Path("docs/camera-validation/integration").resolve()
    destination.mkdir(parents=True, exist_ok=True)
    app = QApplication.instance() or QApplication([])
    if Path("C:/Windows/Fonts/msyh.ttc").exists():
        QFontDatabase.addApplicationFont("C:/Windows/Fonts/msyh.ttc")
    window = MicroscopeWindow(settings_path=destination / "settings.json")
    window.show()
    report = dict(stage_operations=[], status="running", cases=[])
    heartbeats = []
    timer = QTimer()
    timer.setInterval(20)
    timer.timeout.connect(lambda: heartbeats.append(time.monotonic()))
    timer.start()

    def wait(predicate, seconds=12):
        deadline = time.monotonic() + seconds
        while time.monotonic() < deadline:
            app.processEvents()
            if window.camera_error:
                raise RuntimeError(window.camera_error)
            if predicate():
                return
            time.sleep(.003)
        raise TimeoutError(f"UI state {window.camera_state}")

    try:
        window.camera_connect.click()
        wait(lambda: window.camera_state == "idle")
        report["connection"] = window.camera_settings
        for mode, exposure, frames in ((1, 100, 25), (3, 20, 65), (3, 50, 20)):
            window.camera_mode.setCurrentIndex(window.camera_mode.findData(mode))
            window.exposure_box.setValue(exposure)
            window.gain_box.setValue(2)
            window.camera_apply.click()
            wait(lambda: window.camera_state == "idle")
            settings = window.camera_settings.copy()
            assert settings["exposure_ms"] == exposure and settings["mode"] == mode
            window.camera_preview.click()
            wait(lambda: window.camera_state == "streaming" and window.camera_frame_metadata is not None
                 and window.camera_frame_metadata["mode"] == mode
                 and window.camera_frame_metadata["exposure_ms"] == exposure
                 and window.camera_frame_metadata["received_frames"] >= frames)
            window.camera_preview.click()
            wait(lambda: window.camera_state == "idle")
            metadata = window.camera_frame_metadata.copy()
            assert metadata["exposure_ms"] == exposure and metadata["gain"] == 2
            assert not metadata["duplicate_frame_ids"]
            case = dict(requested_exposure_ms=exposure, readback=settings, last_frame=metadata,
                        display_skipped=window.camera_session.display_skipped)
            report["cases"].append(case)
            print(json.dumps(case), flush=True)
        report["stream_ui_heartbeat_max_gap_ms"] = max((b-a)*1000 for a,b in zip(heartbeats, heartbeats[1:]))
        # Same-mode single acquisition produces a new frame and preserves geometry.
        window.add_annotation("profile", [QPointF(10, 10), QPointF(100, 50)])
        annotation = window.annotations[0]
        item = window.image_item
        window.view.scale(1.2, 1.2)
        transform = window.view.transform()
        window.set_calibration(.25)
        previous = window.camera_frame_metadata["captured_at"]
        window.camera_capture.click()
        wait(lambda: window.camera_state == "idle" and window.camera_frame_metadata["captured_at"] != previous)
        assert window.annotations == [annotation] and window.image_item is item
        assert window.view.transform() == transform and window.microns_per_pixel == .25
        assert window.profile_data is not None
        report["single_frame"] = window.camera_frame_metadata.copy()
        with patch("microscope_workspace.QFileDialog.getSaveFileName",
                   return_value=(str(destination / "single-original.png"), "")):
            window.save_camera_frame()
        assert (destination / "single-original.png").exists()
        assert (destination / "single-original.json").exists()
        window.grab().save(str(destination / "workspace.png"))
        # Close during active preview, then prove the device can be reopened.
        window.camera_preview.click()
        wait(lambda: window.camera_state == "streaming")
        assert not window.close()
        wait(lambda: not window.camera_session.alive and not window.isVisible())
        window.show()
        window.camera_connect.click()
        wait(lambda: window.camera_state == "idle")
        report["reopened"] = window.camera_settings
        # Cancellation must not wait for a 30-second exposure to finish.
        window.exposure_box.setValue(30000)
        window.camera_apply.click()
        wait(lambda: window.camera_state == "idle")
        with patch.object(QMessageBox, "question", return_value=QMessageBox.StandardButton.Yes):
            window.camera_capture.click()
        wait(lambda: window.camera_state == "capturing")
        # The UI enters capturing before START returns; wait for worker event.
        for _ in range(30):
            app.processEvents()
            time.sleep(.01)
        cancelled = time.monotonic()
        window.camera_connect.click()
        wait(lambda: window.camera_state == "disconnected" and not window.camera_session.alive)
        report["long_exposure_disconnect_ms"] = (time.monotonic() - cancelled) * 1000
        report["status"] = "passed"
    except Exception as exc:
        report["status"] = "failed"
        report["error"] = repr(exc)
        raise
    finally:
        window.camera_session.request("disconnect")
        if window.camera_session.thread:
            window.camera_session.thread.join(10)
        # Keep cleanup errors in the report even after a failed acceptance check.
        for event, data in list(window.camera_session.events.queue):
            if event == "error":
                report.setdefault("cleanup_errors", []).append(data)
                report["status"] = "failed"
        report["thread_stopped"] = not window.camera_session.alive
        report["ui_heartbeat_max_gap_ms"] = max((b-a)*1000 for a,b in zip(heartbeats, heartbeats[1:]))
        report["imported_motion_modules"] = [m for m in sys.modules if m in ("controller_gui", "onway_driver", "nexcope_driver", "serial")]
        timer.stop()
        window.close()
        (destination / "result.json").write_text(json.dumps(report, ensure_ascii=False, indent=2), encoding="utf-8")
    return 0 if report["status"] == "passed" else 1


if __name__ == "__main__":
    sys.exit(main())
