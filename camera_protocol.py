"""Versioned read-only image contract used across application processes.

Only standard-library transport dependencies. The compatibility client never
imports or launches the camera application, and has no device-command methods.
"""
from urllib.error import HTTPError, URLError
from urllib.request import build_opener, ProxyHandler

from snapshot_http import HttpResponse, SnapshotHttpServer

CAMERA_IMAGE_PATH = "/api/v1/camera/image"
CAMERA_API_PORT = 51236


def image_response(provider):
    try:
        snapshot = provider() if provider else None
    except Exception:
        return HttpResponse(500, b'{"error":{"code":"image_encoding_failed"}}')
    if snapshot is None:
        return HttpResponse(503, b'{"error":{"code":"image_unavailable"}}')
    png, image_id = snapshot
    return HttpResponse(200, png, "image/png", (("X-Image-Id", image_id),))


class CameraHttpServer(SnapshotHttpServer):
    def __init__(self, provider, *, host="127.0.0.1", port=CAMERA_API_PORT):
        super().__init__({CAMERA_IMAGE_PATH: lambda: image_response(provider)}, host=host, port=port)


class CameraImageClient:
    """Compatibility reader for the legacy motion-control API, with bounded I/O."""
    def __init__(self, base_url=f"http://127.0.0.1:{CAMERA_API_PORT}", *, timeout=2):
        self.url = base_url.rstrip("/") + CAMERA_IMAGE_PATH
        self.timeout = timeout

    def snapshot_png(self):
        try:
            # A localhost application protocol must not traverse system proxies.
            with build_opener(ProxyHandler({})).open(self.url, timeout=self.timeout) as response:
                if response.headers.get_content_type() != "image/png":
                    return None
                image_id = response.headers.get("X-Image-Id", "")
                if not image_id or len(image_id) > 128:
                    return None
                limit = 64 * 1024 * 1024
                png = response.read(limit + 1)
                if len(png) > limit or not png.startswith(b"\x89PNG\r\n\x1a\n"):
                    return None
                return png, image_id
        except HTTPError as exc:
            if exc.code == 500:
                raise RuntimeError("Peer image encoding failed") from exc
            return None
        except (URLError, OSError, TimeoutError):
            return None
