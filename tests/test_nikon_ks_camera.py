"""Backend ABI/buffer checks use memory fixtures only; never load a vendor DLL."""
import ctypes as C
import struct
from types import SimpleNamespace
from unittest.mock import Mock

import numpy as np
import pytest

from nikon_ks_bindings import Feature, Image, U8, U32
from nikon_ks_camera import NikonKsCamera, RESTORE_IDS


def test_stale_events_are_drained_before_new_transfer_and_errors_are_not_hidden():
    backend = NikonKsCamera()
    polling = Mock(side_effect=[0, 0, -10])
    backend.camera = SimpleNamespace(handle=U32(1), dll=SimpleNamespace(CAM_EventPolling=polling))
    backend._drain_image_events()
    assert polling.call_count == 3
    polling.side_effect = [-7]
    with pytest.raises(RuntimeError, match="-7"):
        backend._drain_image_events()
    polling.side_effect = None
    polling.return_value = 0
    with pytest.raises(RuntimeError, match="队列"):
        backend._drain_image_events()


def test_bgr_footer_removed_owned_rgb_and_frame_id_wrap():
    backend = NikonKsCamera()
    backend.format = {"width": 2, "height": 1}
    backend.pixel_bytes = 6
    backend.buffer_bytes = 262
    backend.buffer = (U8 * 326)()
    backend.buffer[:6] = [1, 2, 3, 4, 5, 6]
    footer = bytearray(256)
    footer[65:67] = bytes([1, 1])
    struct.pack_into("<HH", footer, 84, 2, 1)
    struct.pack_into("<I", footer, 8, 20000)
    struct.pack_into("<H", footer, 104, 200)
    backend.buffer[6:262] = footer
    backend.buffer[262:] = b"\xA5" * 64
    backend.image = Image()
    backend.image.buffer = C.addressof(backend.buffer)
    backend.image.image_bytes = 6
    backend.camera = SimpleNamespace(handle=U32(1), call=Mock(),
                                     dll=SimpleNamespace(CAM_EventPolling=Mock(return_value=0)))
    backend.settings = dict(mode=1, device=dict(serial=123))
    backend.last_number = 65535
    backend.missing_ids = backend.duplicates = backend.received = 0
    frame = backend.poll()
    np.testing.assert_array_equal(frame.rgb, [[[3, 2, 1], [6, 5, 4]]])
    backend.buffer[0] = 99
    assert frame.rgb[0, 0, 2] == 1  # no pointer into reusable SDK buffer
    assert frame.rgb.flags.c_contiguous and not frame.rgb.flags.writeable
    assert frame.metadata["missing_frame_ids"] == 0
    assert frame.metadata["exposure_ms"] == 20
    backend.buffer[-1] = 0
    with pytest.raises(RuntimeError, match="缓冲区"):
        backend.poll()


def test_restore_error_still_closes_runtime():
    backend = NikonKsCamera()
    backend.changed = True
    backend.original = {i: Feature() for i in RESTORE_IDS}
    camera = SimpleNamespace(stop=Mock(), set_features=Mock(side_effect=RuntimeError("restore failed")),
                             close=Mock())
    backend.camera = camera
    with pytest.raises(RuntimeError, match="restore failed"):
        backend.close()
    camera.close.assert_called_once()
    assert backend.camera is None
