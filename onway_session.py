"""Give one desktop application at a time ownership of the Onway controller."""
import os
from pathlib import Path

from onway_driver import MCCDriver


class DesktopOnwayDriver(MCCDriver):
    def _open_exclusive(self, opener, *args):
        from PyQt6.QtCore import QLockFile

        if self.connected:
            raise RuntimeError("请先断开现有 Onway 连接。")
        root = Path(os.environ.get("LOCALAPPDATA") or Path.home()) / "motion-control"
        root.mkdir(parents=True, exist_ok=True)
        lock = QLockFile(str(root / "onway-device.lock"))
        lock.setStaleLockTime(0)  # a live owner may hold the controller for hours
        if not lock.tryLock(0):
            if lock.error() == QLockFile.LockError.LockFailedError:
                raise RuntimeError("Onway 已由另一个程序占用，请先在该程序中断开。")
            raise OSError(f"无法锁定 Onway 设备：{root} ({lock.error().name})")
        try:
            if opener(*args):
                self._owner_lock = lock
                return True
            return False
        finally:
            if getattr(self, "_owner_lock", None) is not lock:
                super().disconnect()
                lock.unlock()

    def connect(self, ip, port):
        return self._open_exclusive(super().connect, ip, port)

    def connect_serial(self, port, baudrate):
        return self._open_exclusive(super().connect_serial, port, baudrate)

    def disconnect(self):
        super().disconnect()
        lock = getattr(self, "_owner_lock", None)
        if lock is not None:
            lock.unlock()
            self._owner_lock = None
