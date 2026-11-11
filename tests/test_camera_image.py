"""Offline HTTP/image contract checks; no camera SDK or hardware involved."""

from concurrent.futures import ThreadPoolExecutor
import json
from threading import Event
from unittest.mock import patch
from urllib.error import HTTPError
from urllib.request import urlopen

import pytest
from PyQt6.QtGui import QColor, QImage

import camera_image
from camera_image import CameraImageState
from position_server import PositionHttpServer, PositionState


def make_image(color="red", width=13, height=7):
    image = QImage(width, height, QImage.Format.Format_RGB888)
    image.fill(QColor(color))
    return image


def decode(snapshot):
    png, image_id = snapshot
    image = QImage.fromData(png, "PNG")
    assert not image.isNull()
    metadata = json.loads(image.text("microscope_frame"))
    assert metadata["image_id"] == image_id
    return image, metadata


def test_snapshot_owns_pixels_and_metadata_and_invalidates_cached_png():
    state = CameraImageState()
    assert state.snapshot_png() is None
    source = make_image()
    metadata = {"frame_no": 1, "device": {"serial": 123}}
    state.publish(source, metadata)
    source.fill(QColor("blue"))
    metadata["device"]["serial"] = 456
    first = state.snapshot_png()
    image, saved = decode(first)
    assert (image.width(), image.height()) == (13, 7)
    assert image.pixelColor(0, 0) == QColor("red")
    assert saved["camera"] == {"frame_no": 1, "device": {"serial": 123}}
    assert state.snapshot_png() is first
    state.publish(source, {"frame_no": 2})
    second = state.snapshot_png()
    assert second[1] != first[1]
    assert decode(second)[0].pixelColor(0, 0) == QColor("blue")
    state.clear()
    assert state.snapshot_png() is None


@pytest.mark.parametrize("clear", [False, True])
def test_publish_or_clear_during_encoding_does_not_mix_frames_or_restore_stale_cache(clear):
    state = CameraImageState()
    state.publish(make_image(), {"frame_no": 1})
    entered, release = Event(), Event()
    real_buffer = camera_image.QBuffer

    def slow_buffer():
        entered.set()
        assert release.wait(3)
        return real_buffer()

    with ThreadPoolExecutor(max_workers=1) as pool:
        with patch.object(camera_image, "QBuffer", side_effect=slow_buffer):
            pending = pool.submit(state.snapshot_png)
            try:
                assert entered.wait(3)
                if clear:
                    state.clear()
                else:
                    state.publish(make_image("blue"), {"frame_no": 2})
            finally:
                release.set()
            image, metadata = decode(pending.result(timeout=3))
    assert image.pixelColor(0, 0) == QColor("red")
    assert metadata["camera"]["frame_no"] == 1
    if clear:
        assert state.snapshot_png() is None
    else:
        image, metadata = decode(state.snapshot_png())
        assert image.pixelColor(0, 0) == QColor("blue")
        assert metadata["camera"]["frame_no"] == 2


def test_http_image_content_metadata_errors_and_parallel_reads():
    state = CameraImageState()
    server = PositionHttpServer(PositionState(), port=0, image_provider=state.snapshot_png)
    server.start()
    url = server.url + "/api/v1/camera/image"
    try:
        with pytest.raises(HTTPError) as error:
            urlopen(url, timeout=3)
        assert error.value.code == 503
        assert json.load(error.value)["error"]["code"] == "image_unavailable"
        state.publish(make_image(), {"frame_no": 42})

        def fetch(_):
            with urlopen(url, timeout=3) as response:
                png = response.read()
                assert response.headers["Content-Type"] == "image/png"
                assert response.headers["Cache-Control"] == "no-store"
                assert int(response.headers["Content-Length"]) == len(png)
                return png, response.headers["X-Image-Id"]

        with ThreadPoolExecutor(max_workers=4) as pool:
            results = list(pool.map(fetch, range(4)))
        assert all(result == results[0] for result in results)
        assert decode(results[0])[1]["camera"]["frame_no"] == 42
        with pytest.raises(HTTPError) as error:
            urlopen(url, data=b"capture", timeout=3)
        assert error.value.code == 405
    finally:
        server.stop()


@pytest.mark.parametrize("fails", [False, True])
def test_http_missing_provider_or_encoding_failure(fails):
    def broken_provider():
        raise OSError("private internal details")

    server = PositionHttpServer(PositionState(), port=0,
                                image_provider=broken_provider if fails else None)
    server.start()
    try:
        with pytest.raises(HTTPError) as error:
            urlopen(server.url + "/api/v1/camera/image", timeout=3)
        assert error.value.code == (500 if fails else 503)
        payload = json.load(error.value)
        assert payload["error"]["code"] == ("image_encoding_failed" if fails else "image_unavailable")
    finally:
        server.stop()
