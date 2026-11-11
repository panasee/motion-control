"""Thread-safe, on-demand PNG snapshots of the microscope's current camera frame.

The GUI publishes owned QImages; HTTP workers never touch widgets or the SDK.
PNG encoding is lazy and serialized, so preview does not encode every frame and
parallel requests cannot start an unbounded number of full-resolution encoders.
"""

import json
import threading
from uuid import uuid4

from PyQt6.QtCore import QBuffer, QIODevice
from PyQt6.QtGui import QImage


class CameraImageState:
    def __init__(self):
        self._lock = threading.Lock()
        self._encode_lock = threading.Lock()
        self._frame = None
        self._encoded = None

    def publish(self, image: QImage, metadata: dict) -> None:
        if image.isNull():
            raise ValueError("Cannot publish an empty camera image")
        image_id = uuid4().hex
        metadata_json = json.dumps({
            "schema_version": 1,
            "image_id": image_id,
            "source": "camera",
            "pixels": "original_rgb8_without_software_white_balance_or_annotations",
            "camera": metadata,
        }, ensure_ascii=True, allow_nan=False)
        # Implicit sharing gives us our own QImage without copying every preview
        # frame again. Mutating either QImage subsequently detaches its pixels.
        owned_image = QImage(image)
        with self._lock:
            self._frame = (owned_image, metadata_json, image_id)
            self._encoded = None

    def clear(self) -> None:
        with self._lock:
            self._frame = None
            self._encoded = None

    def snapshot_png(self) -> tuple[bytes, str] | None:
        """Return PNG bytes and a unique image ID from one atomic snapshot.

        A request already encoding a frame may finish after clear()/publish().
        That response still contains exactly the image and metadata it selected.
        """
        with self._encode_lock:
            with self._lock:
                if self._frame is None:
                    return None
                if self._encoded is not None:
                    return self._encoded
                source, metadata_json, image_id = self._frame
                image = QImage(source)
            image.setText("microscope_frame", metadata_json)
            buffer = QBuffer()
            if not buffer.open(QIODevice.OpenModeFlag.WriteOnly) or not image.save(buffer, "PNG"):
                raise OSError("Could not encode camera frame as PNG")
            result = (bytes(buffer.data()), image_id)
            with self._lock:
                if self._frame is not None and self._frame[2] == image_id:
                    self._encoded = result
            return result
