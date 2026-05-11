"""Tests for the local read-only position HTTP endpoint."""

import json
import unittest
from urllib.error import HTTPError
from urllib.request import urlopen

from position_server import PositionHttpServer, PositionState


class PositionStateTests(unittest.TestCase):
    def test_snapshot_starts_disconnected(self) -> None:
        state = PositionState()

        snapshot = state.snapshot()

        self.assertIsNone(snapshot)

    def test_update_xy_publishes_only_external_xy(self) -> None:
        state = PositionState()

        state.update_xy(1, 2.5)

        snapshot = state.snapshot()
        self.assertEqual(snapshot, {"X": 1.0, "Y": 2.5})

    def test_mark_disconnected_clears_readable_position(self) -> None:
        state = PositionState()
        state.update_xy(10, 20)

        state.mark_disconnected()

        snapshot = state.snapshot()
        self.assertIsNone(snapshot)


class PositionHttpServerTests(unittest.TestCase):
    def test_default_port_uses_high_private_range_port(self) -> None:
        state = PositionState()
        server = PositionHttpServer(state)

        self.assertEqual(server.port, 51235)
        self.assertEqual(server.url, "http://127.0.0.1:51235")

    def test_position_endpoint_returns_current_state(self) -> None:
        state = PositionState()
        state.update_xy(12, 34)
        server = PositionHttpServer(state, port=0)
        self.addCleanup(server.stop)
        server.start()

        with urlopen(f"{server.url}/position", timeout=2) as response:
            body = response.read().decode("utf-8")
            payload = json.loads(body)

        self.assertEqual(response.status, 200)
        self.assertEqual(body, '{"X":12.0000, "Y":34.0000}')
        self.assertEqual(payload, {"X": 12.0, "Y": 34.0})

    def test_position_endpoint_returns_503_when_disconnected(self) -> None:
        state = PositionState()
        server = PositionHttpServer(state, port=0)
        self.addCleanup(server.stop)
        server.start()

        with self.assertRaises(HTTPError) as raised:
            urlopen(f"{server.url}/position", timeout=2)

        self.assertEqual(raised.exception.code, 503)
        self.assertEqual(json.loads(raised.exception.read().decode("utf-8")), {"error": "not connected"})

    def test_position_endpoint_does_not_accept_writes(self) -> None:
        state = PositionState()
        server = PositionHttpServer(state, port=0)
        self.addCleanup(server.stop)
        server.start()

        with self.assertRaises(HTTPError) as raised:
            urlopen(f"{server.url}/position", data=b'{"X":99}', timeout=2)

        self.assertEqual(raised.exception.code, 405)

    def test_unknown_endpoint_returns_404(self) -> None:
        state = PositionState()
        server = PositionHttpServer(state, port=0)
        self.addCleanup(server.stop)
        server.start()

        with self.assertRaises(HTTPError) as raised:
            urlopen(f"{server.url}/missing", timeout=2)

        self.assertEqual(raised.exception.code, 404)


if __name__ == "__main__":
    unittest.main()
