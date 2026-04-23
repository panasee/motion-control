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
STAGE_XYZ_COMMAND = "INFRD"
OPTICAL_INFO_COMMAND = "INFRJ"
UNSUPPORTED_ERROR_CODES = {-3}


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


def is_unsupported_response(response: str) -> bool:
    """Return True when the device answered with an ERR-line."""
    return response.strip().startswith("ERR-")


def parse_xyz_response(response: str) -> dict[str, int]:
    """Parse a validated X/Y/Z key:value line into integer stage coordinates."""
    fields = parse_info_response(response)
    xyz: dict[str, int] = {}
    for axis in ("X", "Y", "Z"):
        if axis not in fields:
            raise ValueError(f"missing XYZ field: {axis}")
        xyz[axis] = int(fields[axis])
    return xyz


def parse_axis_position_response(response: str, axis: str) -> int:
    """Parse a single-axis key:value response such as X:123 into an integer."""
    normalized_axis = axis.strip().upper()
    fields = parse_info_response(response)
    if normalized_axis not in fields:
        raise ValueError(f"missing axis field: {normalized_axis}")
    return int(fields[normalized_axis])


class NexcopeCommandError(RuntimeError):
    """Raised when the device returns a command error that is not unsupported."""

    def __init__(self, command: str, code: int) -> None:
        super().__init__(f"{command} failed with Nexcope error code {code}")
        self.command = command
        self.code = code


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

    def query_supported_line(self, command: str) -> str:
        response = self.query_line(command)
        if is_unsupported_response(response):
            error = parse_error_response(response)
            code = int(error["code"])
            if code in UNSUPPORTED_ERROR_CODES:
                raise NotImplementedError(f"{command} is not supported by the connected device")
            raise NexcopeCommandError(command, code)
        return response

    def query_info_fields(self, command: str) -> dict[str, str]:
        return parse_info_response(self.query_supported_line(command))

    def get_stage_xyz(self) -> dict[str, int]:
        return parse_xyz_response(self.query_supported_line(STAGE_XYZ_COMMAND))

    def get_px(self) -> int:
        return parse_axis_position_response(self.query_supported_line("PX"), "X")

    def get_py(self) -> int:
        return parse_axis_position_response(self.query_supported_line("PY"), "Y")

    def get_pz(self) -> int:
        return parse_axis_position_response(self.query_supported_line("PZ"), "Z")

    def get_optical_info(self) -> dict[str, str]:
        return self.query_info_fields(OPTICAL_INFO_COMMAND)

    def get_upper_light_path(self) -> str:
        return self.get_optical_info()["ULP"]

    def move_gpx(self, payload: object) -> None:
        self.send_command(f"GPX{payload}")

    def move_gpy(self, payload: object) -> None:
        self.send_command(f"GPY{payload}")

    def move_gpz(self, payload: object) -> None:
        self.send_command(f"GPZ{payload}")

    def move_gp(self, payload: object) -> None:
        self.send_command(f"GP{payload}")

    def move_left(self, payload: object) -> None:
        self.send_command(f"L{payload}")

    def move_right(self, payload: object) -> None:
        self.send_command(f"R{payload}")

    def move_forward(self, payload: object) -> None:
        self.send_command(f"F{payload}")

    def move_back(self, payload: object) -> None:
        self.send_command(f"B{payload}")

    def move_up(self, payload: object) -> None:
        self.send_command(f"U{payload}")

    def move_down(self, payload: object) -> None:
        self.send_command(f"D{payload}")

    def get_sled(self) -> str:
        return self.query_supported_line("SLED")

    def get_ygtn(self) -> str:
        return self.query_supported_line("YGTN")

    def get_jgtn(self) -> str:
        return self.query_supported_line("JGTN")

    def get_fgtn(self) -> str:
        return self.query_supported_line("FGTN")

    def get_wjtn(self) -> str:
        return self.query_supported_line("WJTN")

    def _require_connection(self):
        if not self.connected:
            raise RuntimeError("serial connection is not open")
        return self._serial_conn
