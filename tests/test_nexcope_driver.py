"""Protocol-oriented tests for the Nexcope NE950 ASCII driver scaffold."""

import unittest
from pathlib import Path


PROJECT_ROOT = Path(__file__).resolve().parents[1]


class FakeSerial:
    def __init__(self, *args, responses=None, **kwargs):
        self.args = args
        self.kwargs = kwargs
        self.responses = list(responses or [])
        self.written = []
        self.closed = False
        self.is_open = True

    def write(self, data: bytes) -> int:
        self.written.append(data)
        return len(data)

    def flush(self) -> None:
        return None

    def readline(self) -> bytes:
        if self.responses:
            return self.responses.pop(0)
        return b""

    def close(self) -> None:
        self.closed = True
        self.is_open = False


class NexcopeDriverProtocolTest(unittest.TestCase):
    def setUp(self) -> None:
        import sys

        sys.path.insert(0, str(PROJECT_ROOT))
        self.addCleanup(sys.path.remove, str(PROJECT_ROOT))

    def test_format_command_appends_cr_once(self) -> None:
        from nexcope_driver import format_command

        self.assertEqual(format_command("PX"), b"PX\r")
        self.assertEqual(format_command("PX\r"), b"PX\r")

    def test_parse_info_response_extracts_colon_separated_fields(self) -> None:
        from nexcope_driver import parse_info_response

        parsed = parse_info_response("X:7487027, Y:7234386, Z:1508367")

        self.assertEqual(
            parsed,
            {
                "X": "7487027",
                "Y": "7234386",
                "Z": "1508367",
            },
        )

    def test_parse_info_response_supports_single_field_payload(self) -> None:
        from nexcope_driver import parse_info_response

        parsed = parse_info_response("ULP:600_1")

        self.assertEqual(parsed, {"ULP": "600_1"})

    def test_parse_error_response_extracts_negative_code(self) -> None:
        from nexcope_driver import parse_error_response

        parsed = parse_error_response("ERR-10")

        self.assertEqual(parsed, {"code": -10})


class NexcopeDriverSerialShapeTest(unittest.TestCase):
    def setUp(self) -> None:
        import sys

        sys.path.insert(0, str(PROJECT_ROOT))
        self.addCleanup(sys.path.remove, str(PROJECT_ROOT))

    def test_connect_serial_uses_factory_and_marks_connected(self) -> None:
        from nexcope_driver import NexcopeDriver

        created = {}

        def factory(*args, **kwargs):
            created["serial"] = FakeSerial(*args, **kwargs)
            return created["serial"]

        driver = NexcopeDriver(serial_factory=factory, timeout=0.75)

        connected = driver.connect_serial("COM7", baudrate=19200)

        self.assertTrue(connected)
        self.assertTrue(driver.connected)
        self.assertEqual(created["serial"].kwargs["port"], "COM7")
        self.assertEqual(created["serial"].kwargs["baudrate"], 19200)
        self.assertEqual(created["serial"].kwargs["timeout"], 0.75)

    def test_send_command_writes_cr_terminated_ascii(self) -> None:
        from nexcope_driver import NexcopeDriver

        fake_serial = FakeSerial()
        driver = NexcopeDriver(serial_factory=lambda **kwargs: fake_serial)
        driver.connect_serial("COM7")

        driver.send_command("INFRD")

        self.assertEqual(fake_serial.written, [b"INFRD\r"])

    def test_receive_line_decodes_and_strips_trailing_newline(self) -> None:
        from nexcope_driver import NexcopeDriver

        fake_serial = FakeSerial(responses=[b"ULP:600_1\r"])
        driver = NexcopeDriver(serial_factory=lambda **kwargs: fake_serial)
        driver.connect_serial("COM7")

        response = driver.receive_line()

        self.assertEqual(response, "ULP:600_1")

    def test_query_line_sends_command_then_reads_response(self) -> None:
        from nexcope_driver import NexcopeDriver

        fake_serial = FakeSerial(responses=[b"ERR-3\r"])
        driver = NexcopeDriver(serial_factory=lambda **kwargs: fake_serial)
        driver.connect_serial("COM7")

        response = driver.query_line("INFRJ")

        self.assertEqual(response, "ERR-3")
        self.assertEqual(fake_serial.written, [b"INFRJ\r"])

    def test_disconnect_closes_open_serial_connection(self) -> None:
        from nexcope_driver import NexcopeDriver

        fake_serial = FakeSerial()
        driver = NexcopeDriver(serial_factory=lambda **kwargs: fake_serial)
        driver.connect_serial("COM7")

        driver.disconnect()

        self.assertFalse(driver.connected)
        self.assertTrue(fake_serial.closed)


if __name__ == "__main__":
    unittest.main()
