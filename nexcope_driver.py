"""Thin protocol helpers and serial scaffold for Nexcope NE950 ASCII commands."""

from __future__ import annotations

from typing import Callable, Optional

try:
    import serial

    SERIAL_AVAILABLE = True
except ImportError:  # pragma: no cover - exercised through injected factories in tests
    serial = None
    SERIAL_AVAILABLE = False


DEFAULT_TERMINATOR = "\r"


def format_command(command: str, terminator: str = DEFAULT_TERMINATOR) -> bytes:
    """Normalize a command to a single ASCII line terminated for serial transport."""
    normalized = command.rstrip("\r\n")
    return f"{normalized}{terminator}".encode("ascii")


def parse_info_response(response: str) -> dict[str, str]:
    """Parse comma-separated key:value response fields."""
    text = response.strip()
    if not text:
        raise ValueError("response is empty")

    fields: dict[str, str] = {}
    for token in text.split(","):
        item = token.strip()
        if ":" not in item:
            continue
        key, value = item.split(":", 1)
        fields[key.strip().upper()] = value.strip()
    if not fields:
        raise ValueError("response does not contain key:value fields")
    return fields


def parse_error_response(response: str) -> dict[str, object]:
    """Parse an ERR-<number> line into a numeric error code."""
    text = response.strip()
    if not text.startswith("ERR-"):
        raise ValueError("response does not start with ERR-")

    return {"code": -int(text[4:])}


class NexcopeDriver:
    """Minimal serial transport wrapper for line-based NE950 ASCII exchanges."""

    def __init__(
        self,
        *,
        timeout: float = 1.0,
        encoding: str = "ascii",
        serial_factory: Optional[Callable[..., object]] = None,
    ) -> None:
        self.timeout = timeout
        self.encoding = encoding
        self._serial_factory = serial_factory
        self._serial_conn: Optional[object] = None

    @property
    def connected(self) -> bool:
        return self._serial_conn is not None and bool(getattr(self._serial_conn, "is_open", True))

    def connect_serial(self, port: str, baudrate: int = 115200) -> bool:
        factory = self._serial_factory
        if factory is None:
            if not SERIAL_AVAILABLE:
                raise ImportError("pyserial is required for serial connections")
            factory = serial.Serial

        self._serial_conn = factory(port=port, baudrate=baudrate, timeout=self.timeout)
        return self.connected

    def disconnect(self) -> None:
        if self._serial_conn is None:
            return
        close = getattr(self._serial_conn, "close", None)
        if callable(close):
            close()
        self._serial_conn = None

    def send_command(self, command: str) -> None:
        conn = self._require_connection()
        conn.write(format_command(command))
        flush = getattr(conn, "flush", None)
        if callable(flush):
            flush()

    def receive_line(self) -> str:
        conn = self._require_connection()
        raw = conn.readline()
        if isinstance(raw, bytes):
            return raw.decode(self.encoding).rstrip("\r\n")
        return str(raw).rstrip("\r\n")

    def query_line(self, command: str) -> str:
        self.send_command(command)
        return self.receive_line()

    def _require_connection(self):
        if not self.connected:
            raise RuntimeError("serial connection is not open")
        return self._serial_conn
