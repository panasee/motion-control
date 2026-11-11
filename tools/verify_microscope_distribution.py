"""Prove a built microscope wheel runs without the motion-control source tree."""
import argparse
import json
import os
from pathlib import Path
import subprocess
import sys
import tempfile
import zipfile


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("wheel", type=Path)
    args = parser.parse_args()
    with zipfile.ZipFile(args.wheel) as archive:
        modules = sorted(n for n in archive.namelist() if n.endswith(".py"))
        forbidden = {"controller_gui.py", "position_server.py", "nexcope_driver.py", "peer_apps.py"}
        assert not forbidden.intersection(modules), modules
        assert {"stack_designer.py", "photo_selection.py", "onway_driver.py", "onway_session.py",
                "microscope_motion.py", "app_version.py"}.issubset(modules), modules
        with tempfile.TemporaryDirectory(prefix="microscope-standalone-") as folder:
            archive.extractall(folder)
            script = r'''
import importlib.abc, json, sys
from pathlib import Path
root = Path(sys.argv[1]).resolve()
sys.path.insert(0, str(root))
blocked = {"controller_gui", "onway_driver", "nexcope_driver", "position_server",
           "serial", "peer_apps"}
class Boundary(importlib.abc.MetaPathFinder):
    def find_spec(self, fullname, *args):
        if fullname.split(".")[0] in blocked:
            raise AssertionError("Forbidden application dependency: " + fullname)
sys.meta_path.insert(0, Boundary())
from PyQt6.QtWidgets import QApplication
from microscope_app import ApplicationInstance
from microscope_workspace import MicroscopeWindow
from camera_protocol import CameraHttpServer
from urllib.request import urlopen
from urllib.error import HTTPError
app = QApplication([])
window = MicroscopeWindow(settings_path=root / "settings.json")
assert window.parent() is None and not window.camera_session.alive
assert "stack_designer" not in sys.modules and "cv2" not in sys.modules
instance = ApplicationInstance(root / "settings.json")
assert instance.acquire()
instance.listen(window)
server = CameraHttpServer(window.image_state.snapshot_png, port=0)
server.start()
try:
    try:
        urlopen(server.url + "/api/v1/camera/image", timeout=2)
        raise AssertionError("No camera was connected")
    except HTTPError as error:
        assert error.code == 503
    window.show()
    app.processEvents()
    menu = next(action.menu() for action in window.menuBar().actions() if "工具" in action.text())
    next(action for action in menu.actions() if "叠层" in action.text()).trigger()
    assert window.stack_designer.isVisible()
    assert window.stack_designer.parentWidget() is window
    from stack_designer import FlakeItem
    layer = FlakeItem([(0, 0), (10, 0), (10, 10)], "distribution-test")
    window.stack_designer.layers.append(layer)
    window.stack_designer.board_scene.addItem(layer)
    assert not window.stack_designer.render_outline_image().isNull()
    assert window.close()
    assert not window.stack_designer.isVisible()
    assert "nikon_ks_bindings" not in sys.modules
    local = sorted(name for name, module in list(sys.modules.items())
                   if getattr(module, "__file__", None) and root in Path(module.__file__).resolve().parents)
    print(json.dumps({"status": "passed", "loaded_from_wheel": local,
                      "blocked_dependencies": sorted(blocked), "hardware_opened": False}))
finally:
    server.stop()
    instance.close()
'''
            result = subprocess.run([sys.executable, "-I", "-c", script, folder], cwd=folder,
                                    capture_output=True, text=True, timeout=20,
                                    env={**os.environ, "QT_QPA_PLATFORM": "offscreen"})
            if result.returncode:
                raise RuntimeError(result.stdout + result.stderr)
            report = json.loads(result.stdout)
            report["wheel_modules"] = modules
            print(json.dumps(report, indent=2))


if __name__ == "__main__":
    main()
