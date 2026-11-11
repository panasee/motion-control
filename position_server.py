"""Local read-only API: legacy XY position and versioned camera image access."""

from __future__ import annotations

import threading
from snapshot_http import HttpResponse, SnapshotHttpServer
from camera_protocol import CAMERA_IMAGE_PATH, image_response


class PositionState:
    """Thread-safe cache of the external X/Y position exposed to other programs."""

    def __init__(self) -> None:
        self._lock = threading.Lock()
        self._connected = False
        self._x = 0.0
        self._y = 0.0

    def update_xy(self, x_position: float, y_position: float) -> None:
        with self._lock:
            self._connected = True
            self._x = float(x_position)
            self._y = float(y_position)

    def mark_disconnected(self) -> None:
        with self._lock:
            self._connected = False
            self._x = 0.0
            self._y = 0.0

    def snapshot(self) -> dict | None:
        with self._lock:
            if not self._connected:
                return None
            return {"X": self._x, "Y": self._y}


class PositionHttpServer(SnapshotHttpServer):
    """Motion-owned XY API; optional image route is a compatibility adapter.

    The provider may be a remote client. This server does not own camera state,
    a camera window or an SDK. Original /position response bytes are preserved.
    """
    def __init__(self, state: PositionState, host="127.0.0.1", port=51235, *, image_provider=None):
        self.state = state
        self.image_provider = image_provider
        super().__init__({"/position": self.position_response,
                          CAMERA_IMAGE_PATH: lambda: image_response(self.image_provider)},
                         host=host, port=port)

    def position_response(self):
        snapshot = self.state.snapshot()
        if snapshot is None:
            return HttpResponse(503, b'{"error":"not connected"}')
        payload = f'{{"X":{snapshot["X"]:.4f}, "Y":{snapshot["Y"]:.4f}}}'.encode("utf-8")
        return HttpResponse(200, payload)
