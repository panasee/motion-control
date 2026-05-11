"""Local read-only HTTP endpoint for exposing current XY controller position."""

from __future__ import annotations

import threading
from http.server import BaseHTTPRequestHandler, ThreadingHTTPServer


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


class PositionHttpServer:
    """Small localhost HTTP server exposing the shared PositionState."""

    def __init__(self, state: PositionState, host: str = "127.0.0.1", port: int = 51235) -> None:
        self.state = state
        self.host = host
        self.port = port
        self._server: ThreadingHTTPServer | None = None
        self._thread: threading.Thread | None = None

    @property
    def url(self) -> str:
        if self._server is None:
            return f"http://{self.host}:{self.port}"
        host, port = self._server.server_address
        return f"http://{host}:{port}"

    def start(self) -> None:
        if self._server is not None:
            return

        state = self.state

        class Handler(BaseHTTPRequestHandler):
            def do_GET(self) -> None:
                if self.path == "/position":
                    snapshot = state.snapshot()
                    if snapshot is None:
                        payload = b'{"error":"not connected"}'
                        self.send_response(503)
                        self.send_header("Content-Type", "application/json; charset=utf-8")
                        self.send_header("Content-Length", str(len(payload)))
                        self.end_headers()
                        self.wfile.write(payload)
                        return

                    payload = (
                        f'{{"X":{snapshot["X"]:.4f}, "Y":{snapshot["Y"]:.4f}}}'
                    ).encode("utf-8")
                    self.send_response(200)
                    self.send_header("Content-Type", "application/json; charset=utf-8")
                    self.send_header("Content-Length", str(len(payload)))
                    self.end_headers()
                    self.wfile.write(payload)
                    return

                self.send_error(404)

            def do_POST(self) -> None:
                self.send_error(405)

            def log_message(self, format: str, *args) -> None:
                return

        self._server = ThreadingHTTPServer((self.host, self.port), Handler)
        self._thread = threading.Thread(target=self._server.serve_forever, daemon=True)
        self._thread.start()

    def stop(self) -> None:
        if self._server is None:
            return

        server = self._server
        thread = self._thread
        self._server = None
        self._thread = None
        server.shutdown()
        server.server_close()
        if thread is not None:
            thread.join(timeout=2)
