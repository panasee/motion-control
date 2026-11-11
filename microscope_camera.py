"""SDK-neutral camera contract and serialized, bounded acquisition session.

No DLL or hardware is opened until connect(). Backends own SDK buffer conversion;
consumers receive an owned, contiguous RGB8 image, milliseconds and gain ratio.
There are deliberately no motion-controller imports in this module.
"""
from dataclasses import dataclass
from queue import Queue, Empty
from threading import Thread, Lock, Event
import time
from typing import Protocol

import numpy as np


@dataclass(frozen=True)
class CameraFrame:
    rgb: np.ndarray
    metadata: dict


class CameraBackend(Protocol):
    def open(self) -> dict: ...
    def configure(self, exposure_ms: float, gain: float, mode: int) -> dict: ...
    def start(self) -> None: ...
    def poll(self) -> CameraFrame | None: ...
    def stop(self) -> None: ...
    def close(self) -> None: ...


class CameraSession:
    """One worker owns all SDK calls. UI drains events and a latest-frame mailbox.

    Stop/disconnect use out-of-band flags, so neither waits behind a long exposure
    or a queue of controls. The worker polls nonblocking, with an exposure-aware
    timeout. A backend error terminates the session and always attempts cleanup.
    """
    def __init__(self, factory):
        self.factory = factory
        self.events = Queue()
        self.commands = Queue(maxsize=8)
        self._lock = Lock()
        self._latest = None
        self._disconnect = Event()
        self._stop = Event()
        self.thread = None
        self.display_skipped = 0

    @property
    def alive(self):
        return self.thread is not None and self.thread.is_alive()

    def connect(self):
        if self.alive:
            return
        self._disconnect.clear()
        self._stop.clear()
        self.commands = Queue(maxsize=8)
        self.events = Queue()
        self.clear_frame()
        self.display_skipped = 0
        self.thread = Thread(target=self._run, name="microscope-camera", daemon=False)
        self.thread.start()

    def request(self, command, **values):
        if command == "disconnect":
            self._disconnect.set()
        elif command == "stop":
            self._stop.set()
        elif self.alive:
            self.commands.put_nowait((command, values))

    def clear_frame(self):
        with self._lock:
            self._latest = None

    def take_frame(self):
        with self._lock:
            frame, self._latest = self._latest, None
            return frame

    def _publish(self, frame):
        with self._lock:
            if self._latest is not None:
                self.display_skipped += 1
            self._latest = frame

    def _run(self):
        backend = None
        streaming = single = False
        try:
            backend = self.factory()
            settings = backend.open()
            self.events.put(("ready", settings))
            while not self._disconnect.is_set():
                if self._stop.is_set():
                    self._stop.clear()
                    backend.stop()
                    streaming = single = False
                    self.events.put(("idle", None))
                try:
                    command, values = self.commands.get(timeout=0 if streaming else .02)
                except Empty:
                    command = None
                if command == "configure":
                    backend.stop()
                    streaming = single = False
                    self.clear_frame()
                    settings = backend.configure(**values)
                    self.events.put(("ready", settings))
                elif command in ("preview", "capture"):
                    backend.stop()
                    self.clear_frame()
                    backend.start()
                    streaming, single = True, command == "capture"
                    deadline = time.monotonic() + max(5, settings["exposure_ms"] / 1000 + 3)
                    self.events.put(("capturing" if single else "streaming", None))
                if streaming:
                    frame = backend.poll()
                    if frame is not None:
                        self._publish(frame)
                        deadline = time.monotonic() + max(5, settings["exposure_ms"] / 1000 + 3)
                        if single:
                            backend.stop()
                            streaming = single = False
                            self.events.put(("captured", frame.metadata))
                    elif time.monotonic() > deadline:
                        raise TimeoutError("相机取帧超时；请检查 USB 连接及设备占用。")
                    else:
                        self._disconnect.wait(.002)
        except Exception as exc:
            self.events.put(("error", str(exc)))
        finally:
            if backend is not None:
                try:
                    backend.close()
                except Exception as exc:
                    self.events.put(("error", f"相机清理失败：{exc}"))
            self.events.put(("disconnected", None))
