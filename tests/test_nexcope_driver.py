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


class FakeCrBufferedSerial(FakeSerial):
    def __init__(self, *args, buffer=b"", **kwargs):
        super().__init__(*args, **kwargs)
        self.buffer = bytearray(buffer)

    def readline(self) -> bytes:
        data = bytes(self.buffer)
        self.buffer.clear()
        return data

    def read_until(self, expected: bytes = b"\n", size=None) -> bytes:
        index = self.buffer.find(expected)
        if index < 0:
            data = bytes(self.buffer)
            self.buffer.clear()
            return data
        end = index + len(expected)
        data = bytes(self.buffer[:end])
        del self.buffer[:end]
        return data


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

    def test_parse_sled_response_extracts_upper_and_lower_levels(self) -> None:
        from nexcope_driver import parse_sled_response

        parsed = parse_sled_response("ULP:600, DLP:10")

        self.assertEqual(parsed, {"ULP": 600, "DLP": 10})

    def test_parse_error_response_extracts_negative_code(self) -> None:
        from nexcope_driver import parse_error_response

        parsed = parse_error_response("ERR-10")

        self.assertEqual(parsed, {"code": -10})

    def test_is_unsupported_response_detects_err_lines(self) -> None:
        from nexcope_driver import is_unsupported_response

        self.assertTrue(is_unsupported_response("ERR-3"))
        self.assertFalse(is_unsupported_response("X:7487027, Y:7234386, Z:1508367"))

    def test_parse_xyz_response_converts_xyz_fields_to_ints(self) -> None:
        from nexcope_driver import parse_xyz_response

        parsed = parse_xyz_response("X:7487027, Y:7234386, Z:1508367")

        self.assertEqual(
            parsed,
            {
                "X": 7487027,
                "Y": 7234386,
                "Z": 1508367,
            },
        )

    def test_parse_xyz_response_requires_xyz_fields(self) -> None:
        from nexcope_driver import parse_xyz_response

        with self.assertRaisesRegex(ValueError, "missing XYZ field"):
            parse_xyz_response("X:7487027, Y:7234386")


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

    def test_receive_line_reads_single_cr_terminated_frame_from_buffer(self) -> None:
        from nexcope_driver import NexcopeDriver

        fake_serial = FakeCrBufferedSerial(buffer=b"0\rR\rR\rULP:500, DLP:250\r")
        driver = NexcopeDriver(serial_factory=lambda **kwargs: fake_serial)
        driver.connect_serial("COM7")

        self.assertEqual(driver.receive_line(), "0")
        self.assertEqual(driver.receive_line(), "R")
        self.assertEqual(driver.receive_line(), "R")
        self.assertEqual(driver.receive_line(), "ULP:500, DLP:250")

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

    def test_query_info_fields_parses_supported_info_payload(self) -> None:
        from nexcope_driver import NexcopeDriver

        fake_serial = FakeSerial(responses=[b"ULP:600_1\r"])
        driver = NexcopeDriver(serial_factory=lambda **kwargs: fake_serial)
        driver.connect_serial("COM7")

        response = driver.query_info_fields("INFRD")

        self.assertEqual(response, {"ULP": "600_1"})
        self.assertEqual(fake_serial.written, [b"INFRD\r"])

    def test_query_info_fields_raises_not_implemented_for_unsupported_response(self) -> None:
        from nexcope_driver import NexcopeDriver

        fake_serial = FakeSerial(responses=[b"ERR-3\r"])
        driver = NexcopeDriver(serial_factory=lambda **kwargs: fake_serial)
        driver.connect_serial("COM7")

        with self.assertRaisesRegex(NotImplementedError, "INFRJ"):
            driver.query_info_fields("INFRJ")

    def test_query_supported_line_preserves_non_unsupported_err_codes(self) -> None:
        from nexcope_driver import NexcopeCommandError, NexcopeDriver

        fake_serial = FakeSerial(responses=[b"ERR-10\r"])
        driver = NexcopeDriver(serial_factory=lambda **kwargs: fake_serial)
        driver.connect_serial("COM7")

        with self.assertRaisesRegex(NexcopeCommandError, "SLED failed with Nexcope error code -10"):
            driver.query_supported_line("SLED")

    def test_query_supported_line_returns_non_error_line(self) -> None:
        from nexcope_driver import NexcopeDriver

        fake_serial = FakeSerial(responses=[b"SLED:1\r"])
        driver = NexcopeDriver(serial_factory=lambda **kwargs: fake_serial)
        driver.connect_serial("COM7")

        response = driver.query_supported_line("SLED")

        self.assertEqual(response, "SLED:1")
        self.assertEqual(fake_serial.written, [b"SLED\r"])

    def test_get_stage_xyz_uses_validated_info_command_shape(self) -> None:
        from nexcope_driver import NexcopeDriver

        fake_serial = FakeSerial(responses=[b"X:7487027, Y:7234386, Z:1508367\r"])
        driver = NexcopeDriver(serial_factory=lambda **kwargs: fake_serial)
        driver.connect_serial("COM7")

        response = driver.get_stage_xyz()

        self.assertEqual(
            response,
            {
                "X": 7487027,
                "Y": 7234386,
                "Z": 1508367,
            },
        )
        self.assertEqual(fake_serial.written, [b"INFRD\r"])

    def test_get_stage_xyz_skips_stale_jog_acks_before_info_response(self) -> None:
        from nexcope_driver import NexcopeDriver

        fake_serial = FakeSerial(
            responses=[
                b"F\r",
                b"R\r",
                b"X:7487027, Y:7234386, Z:1508367\r",
            ]
        )
        driver = NexcopeDriver(serial_factory=lambda **kwargs: fake_serial)
        driver.connect_serial("COM7")

        response = driver.get_stage_xyz()

        self.assertEqual(response, {"X": 7487027, "Y": 7234386, "Z": 1508367})
        self.assertEqual(fake_serial.written, [b"INFRD\r"])

    def test_get_upper_light_path_uses_optical_info_wrapper(self) -> None:
        from nexcope_driver import NexcopeDriver

        fake_serial = FakeSerial(responses=[b"ULP:600_1\r"])
        driver = NexcopeDriver(serial_factory=lambda **kwargs: fake_serial)
        driver.connect_serial("COM7")

        response = driver.get_upper_light_path()

        self.assertEqual(response, "600_1")
        self.assertEqual(fake_serial.written, [b"INFRJ\r"])

    def test_get_px_reads_single_axis_position(self) -> None:
        from nexcope_driver import NexcopeDriver

        fake_serial = FakeSerial(responses=[b"X:7487027\r"])
        driver = NexcopeDriver(serial_factory=lambda **kwargs: fake_serial)
        driver.connect_serial("COM7")

        response = driver.get_px()

        self.assertEqual(response, 7487027)
        self.assertEqual(fake_serial.written, [b"PX\r"])

    def test_get_py_reads_single_axis_position(self) -> None:
        from nexcope_driver import NexcopeDriver

        fake_serial = FakeSerial(responses=[b"Y:7234386\r"])
        driver = NexcopeDriver(serial_factory=lambda **kwargs: fake_serial)
        driver.connect_serial("COM7")

        response = driver.get_py()

        self.assertEqual(response, 7234386)
        self.assertEqual(fake_serial.written, [b"PY\r"])

    def test_get_pz_reads_single_axis_position(self) -> None:
        from nexcope_driver import NexcopeDriver

        fake_serial = FakeSerial(responses=[b"Z:1508367\r"])
        driver = NexcopeDriver(serial_factory=lambda **kwargs: fake_serial)
        driver.connect_serial("COM7")

        response = driver.get_pz()

        self.assertEqual(response, 1508367)
        self.assertEqual(fake_serial.written, [b"PZ\r"])

    def test_gpx_gpy_gpz_wrappers_send_prefixed_commands(self) -> None:
        from nexcope_driver import NexcopeDriver

        fake_serial = FakeSerial()
        driver = NexcopeDriver(serial_factory=lambda **kwargs: fake_serial)
        driver.connect_serial("COM7")

        driver.move_gpx("100")
        driver.move_gpy("200")
        driver.move_gpz("300")

        self.assertEqual(fake_serial.written, [b"GPX100\r", b"GPY200\r", b"GPZ300\r"])

    def test_move_gp_wrapper_sends_multi_axis_payload(self) -> None:
        from nexcope_driver import NexcopeDriver

        fake_serial = FakeSerial()
        driver = NexcopeDriver(serial_factory=lambda **kwargs: fake_serial)
        driver.connect_serial("COM7")

        driver.move_gp("100,200,300")

        self.assertEqual(fake_serial.written, [b"GP100,200,300\r"])

    def test_relative_move_wrappers_send_validated_command_names(self) -> None:
        from nexcope_driver import NexcopeDriver

        fake_serial = FakeSerial()
        driver = NexcopeDriver(serial_factory=lambda **kwargs: fake_serial)
        driver.connect_serial("COM7")

        driver.move_left("10")
        driver.move_right("20")
        driver.move_forward("30")
        driver.move_back("40")
        driver.move_up("50")
        driver.move_down("60")

        self.assertEqual(
            fake_serial.written,
            [b"L 10\r", b"R 20\r", b"F 30\r", b"B 40\r", b"U 50\r", b"D 60\r"],
        )

    def test_relative_move_consumes_ack_before_next_status_read(self) -> None:
        from nexcope_driver import NexcopeDriver

        fake_serial = FakeSerial(
            responses=[
                b"R\r",
                b"X:7487027, Y:7234386, Z:1508367\r",
            ]
        )
        driver = NexcopeDriver(serial_factory=lambda **kwargs: fake_serial)
        driver.connect_serial("COM7")

        driver.move_left("10")
        xyz = driver.get_stage_xyz()

        self.assertEqual(xyz, {"X": 7487027, "Y": 7234386, "Z": 1508367})
        self.assertEqual(fake_serial.written, [b"L 10\r", b"INFRD\r"])

    def test_illumination_wrappers_query_and_set_supported_lines(self) -> None:
        from nexcope_driver import NexcopeDriver

        fake_serial = FakeSerial(responses=[b"SLED:1\r"])
        driver = NexcopeDriver(serial_factory=lambda **kwargs: fake_serial)
        driver.connect_serial("COM7")

        self.assertEqual(driver.get_sled(), "SLED:1")
        driver.set_sled(0, 0)
        self.assertEqual(
            fake_serial.written,
            [b"SLED\r", b"SLED 0 0\r"],
        )

    def test_set_sled_consumes_write_ack_before_next_read(self) -> None:
        from nexcope_driver import NexcopeDriver

        fake_serial = FakeSerial(responses=[b"0\r", b"ULP:500, DLP:250\r"])
        driver = NexcopeDriver(serial_factory=lambda **kwargs: fake_serial)
        driver.connect_serial("COM7")

        driver.set_sled(500, 250)
        levels = driver.get_sled_levels()

        self.assertEqual(levels, {"ULP": 500, "DLP": 250})
        self.assertEqual(fake_serial.written, [b"SLED 500 250\r", b"SLED\r"])

    def test_get_sled_levels_skips_stale_jog_ack_before_illumination_response(self) -> None:
        from nexcope_driver import NexcopeDriver

        fake_serial = FakeSerial(responses=[b"L\r", b"ULP:500, DLP:250\r"])
        driver = NexcopeDriver(serial_factory=lambda **kwargs: fake_serial)
        driver.connect_serial("COM7")

        levels = driver.get_sled_levels()

        self.assertEqual(levels, {"ULP": 500, "DLP": 250})
        self.assertEqual(fake_serial.written, [b"SLED\r"])


if __name__ == "__main__":
    unittest.main()
