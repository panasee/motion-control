"""Launch peer applications without importing or owning their windows/services."""
import os
from pathlib import Path
import subprocess
import sys


def microscope_command():
    configured = os.environ.get("MICROSCOPE_WORKBENCH_EXE")
    if configured:
        executable = Path(configured)
        if not executable.is_file():
            raise FileNotFoundError(f"找不到显微镜程序：{executable}")
        return [str(executable)], executable.parent
    if getattr(sys, "frozen", False):
        executable = Path(sys.executable).with_name("MicroscopeWorkbench.exe")
        if not executable.is_file():
            raise FileNotFoundError("请安装独立 MicroscopeWorkbench.exe，或设置 MICROSCOPE_WORKBENCH_EXE。")
        return [str(executable)], executable.parent
    script = Path(__file__).resolve().with_name("microscope_app.py")
    if not script.is_file():
        raise FileNotFoundError("找不到显微镜入口；请设置 MICROSCOPE_WORKBENCH_EXE 指向独立安装。")
    executable = Path(sys.executable)
    if os.name == "nt" and executable.with_name("pythonw.exe").exists():
        executable = executable.with_name("pythonw.exe")
    return [str(executable), str(script)], script.parent


def launch_microscope():
    command, directory = microscope_command()
    options = dict(cwd=str(directory), stdin=subprocess.DEVNULL, stdout=subprocess.DEVNULL,
                   stderr=subprocess.DEVNULL, close_fds=True)
    if os.name == "nt":
        options["creationflags"] = subprocess.DETACHED_PROCESS | subprocess.CREATE_NEW_PROCESS_GROUP
    else:
        options["start_new_session"] = True
    # Intentionally not retained on a controller window; there is no shutdown,
    # wait or kill relationship. The peer handles repeated-launch activation.
    process = subprocess.Popen(command, **options)
    return process.pid
