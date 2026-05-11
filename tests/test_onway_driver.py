"""Protocol-oriented tests for the Onway MCC driver."""

import types
import unittest
from pathlib import Path


PROJECT_ROOT = Path(__file__).resolve().parents[1]


class OnwayDriverProtocolTest(unittest.TestCase):
    def setUp(self) -> None:
        import sys

        sys.path.insert(0, str(PROJECT_ROOT))
        self.addCleanup(sys.path.remove, str(PROJECT_ROOT))

    def test_group_move_accepts_zero_value_ack(self) -> None:
        from onway_driver import CMMND, MCCDriver

        calls = []
        driver = MCCDriver()

        def fake_send_command(cmd_type, data=b"", expected_values=0, retry=4):
            calls.append((cmd_type, data, expected_values, retry))
            return types.SimpleNamespace(status=0)

        driver._send_command = fake_send_command

        moved = driver.group_abs_move_ptp(
            [0, 0, 0, 1, 1, 1],
            [0.0, 0.0, 0.0, 10.0, 20.0, 30.0],
            [0.0, 0.0, 0.0, 2.0, 2.0, 2.0],
            [0.0, 0.0, 0.0, 50.0, 50.0, 50.0],
        )

        self.assertTrue(moved)
        self.assertEqual(len(calls), 1)
        cmd_type, data, expected_values, _retry = calls[0]
        self.assertEqual(cmd_type, CMMND.GCODECMND)
        self.assertEqual(expected_values, 0)
        self.assertEqual(data[0], 0x00)
        self.assertEqual(data[1], 0b111000)


if __name__ == "__main__":
    unittest.main()
