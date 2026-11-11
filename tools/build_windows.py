"""Build either Windows GUI, or both, into release/v<shared version>/."""
import argparse
import os
from pathlib import Path
import subprocess
import sys

ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT))
from app_version import APP_VERSION

TARGETS = {
    "motion": ("MotionController", "controller_gui.py"),
    "microscope": ("MicroscopeWorkbench", "microscope_app.py"),
}


def build(target):
    name, entry = TARGETS[target]
    work = ROOT / "build" / "windows" / name
    work.mkdir(parents=True, exist_ok=True)
    version = tuple(map(int, APP_VERSION.split(".")))
    version += (0,) * (4 - len(version))
    resource = work / "version.txt"
    resource.write_text(f"""VSVersionInfo(
  ffi=FixedFileInfo(filevers={version}, prodvers={version}, mask=63, flags=0,
                    OS=0x40004, fileType=1, subtype=0, date=(0,0)),
  kids=[StringFileInfo([StringTable('040904B0', [
    StringStruct('ProductName', '{name}'), StringStruct('FileDescription', '{name}'),
    StringStruct('OriginalFilename', '{name}.exe'), StringStruct('FileVersion', '{APP_VERSION}'),
    StringStruct('ProductVersion', '{APP_VERSION}')])]),
    VarFileInfo([VarStruct('Translation', [1033, 1200])])])""", encoding="utf-8")
    icon = ROOT / "assets" / "app_icon.ico"
    # Qt imports Windows ICU. An unrelated icuuc.dll on the developer's PATH
    # can be collected instead, producing an EXE that fails to load QtWidgets.
    windows = Path(os.environ["SystemRoot"])
    paths = [p / sub for p in dict.fromkeys(map(Path, [sys.prefix, sys.base_prefix]))
             for sub in ("", "Library/bin", "Scripts")]
    env = {**os.environ, "PATH": os.pathsep.join(map(str, [*paths, windows / "System32", windows])),
           "PYINSTALLER_CONFIG_DIR": str(work / "cache")}
    subprocess.run([
        sys.executable, "-m", "PyInstaller", "--onefile", "--windowed", "--noconfirm",
        "--name", name, "--icon", str(icon), "--version-file", str(resource),
        "--hidden-import", "serial.tools.list_ports", "--add-data", f"{icon};assets",
        "--specpath", str(work), "--workpath", str(work / "analysis"),
        "--distpath", str(ROOT / "release" / f"v{APP_VERSION}"), str(ROOT / entry),
    ], cwd=ROOT, check=True, env=env)


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("target", choices=[*TARGETS, "all"], default="all", nargs="?")
    args = parser.parse_args()
    if os.name != "nt":
        parser.error("Windows builds require Windows.")
    for target in TARGETS if args.target == "all" else [args.target]:
        build(target)
