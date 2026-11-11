"""Validated low-level camera bindings for the installed Nikon KsCam 2.1 runtime (Windows x64).

ABI reference: andrewgomella/NikonKsAdapter, commit
38c8cd81942fcaa2b7a91087f132e3feaee18e0f, SDK/KsCam*.h.
No motion driver, serial port, NIS application or microscope configuration is loaded.
The default operation only enumerates and reads camera features, then closes it.
"""
from __future__ import annotations

import argparse
import ctypes as C
from datetime import datetime
import hashlib
import json
import os
from pathlib import Path
import sys
import struct
import time

U8, U16, U32, I32, U64 = C.c_uint8, C.c_uint16, C.c_uint32, C.c_int32, C.c_uint64


class Device(C.Structure):
    _fields_ = [("kind", I32), ("serial", U32)] + [
        (name, U16 * count) for name, count in
        (("firmware", 16), ("fpga", 16), ("fx3", 16), ("usb", 16),
         ("driver", 16), ("name", 32))]


class Format(C.Structure):
    _fields_ = [("color", I32), ("mode", I32)]


class VariantData(C.Union):
    _fields_ = [("u32", U32), ("i32", I32), ("u64", U64),
                ("number", C.c_double), ("text", U16 * 256),
                ("format", Format), ("words", U32 * 128)]


class Variant(C.Structure):
    _fields_ = [("kind", I32), ("data", VariantData)]


class Feature(C.Structure):
    _fields_ = [("id", U32), ("value", Variant), ("transfer_size", U8)]


class FeatureVector(C.Structure):
    _fields_ = [("used", U32), ("capacity", U32), ("pause", U32),
                ("values", C.POINTER(Feature))]


class Element(C.Structure):
    _fields_ = [("value", Variant), ("comment", U16 * 64)]


class Range(C.Structure):
    _fields_ = [(name, Variant) for name in ("minimum", "maximum", "step", "default")]


class FormatDescription(C.Structure):
    _fields_ = [("format", Format), ("width", U32), ("height", U32),
                ("bits_per_pixel", U32), ("comment", U16 * 64),
                ("trigger_count", U32), ("triggers", Element * 3),
                ("area", U32 * 16), ("position", U32 * 8)]


class DescriptionData(C.Union):
    _fields_ = [("formats", FormatDescription * 256),
                ("elements", Element * 256), ("range", Range)]


class Description(C.Structure):
    _fields_ = [("id", U32), ("count", U32), ("kind", I32), ("data", DescriptionData)]


class FrameSize(C.Structure):
    _fields_ = [("buffer_bytes", U32), ("interval", U32), ("rolling_delay_us", U32)]


class Dropless(C.Structure):
    _fields_ = [("set_value", C.c_bool), ("enabled", C.c_bool)]


class Image(C.Structure):
    # KsCam.dll 2.1.2.3 x64: leading C++ vtable slot plus uiEndTime64.
    # Offsets checked against CAM_GetImage in the installed binary, hash below.
    _fields_ = [("vtable", C.c_void_p), ("buffer", C.c_void_p),
                ("buffer_bytes", U32), ("image_bytes", U32), ("end_time", U32),
                ("end_time64", U64), ("frame_count", U64), ("ref_count", U32)]


class EventData(C.Union):
    # Allow the newer 64-bit timestamps in the runtime's event union.
    _fields_ = [("words", U32 * 256), ("align", U64)]


class Event(C.Structure):
    _fields_ = [("kind", I32), ("data", EventData)]


KSCAM_SHA256 = "4b44e3d45b148c9c9580525f0e51ed39743dc8d837bbe06e36343f9a6061335e"


def wtext(value):
    return bytes(value).decode("utf-16-le").split("\0", 1)[0]


def variant_json(value):
    if value.kind == 13:
        return {"kind": value.kind, "color": value.data.format.color,
                "mode": value.data.format.mode}
    if value.kind in (1, 2):
        return {"kind": value.kind, "value": value.data.i32 if value.kind == 1 else value.data.u32}
    return {"kind": value.kind, "words": list(value.data.words[:16])}


class Camera:
    def __init__(self, path, log):
        if os.name != "nt" or C.sizeof(C.c_void_p) != 8:
            raise RuntimeError("Requires Windows x64")
        if hashlib.sha256(path.read_bytes()).hexdigest() != KSCAM_SHA256:
            raise RuntimeError("Unverified KsCam runtime; revalidate the ABI before use")
        assert (C.sizeof(Device), C.sizeof(Variant), C.sizeof(Feature),
                C.sizeof(FeatureVector), C.sizeof(FormatDescription)) == (232, 520, 536, 24, 2192)
        self.log = log
        self.handle = U32()
        self.enumerated = False
        self.transfer = False
        self.dll_directory = os.add_dll_directory(str(path.parent))
        self.dll = C.CDLL(str(path))
        signatures = {
            "CAM_OpenDevices": [C.POINTER(U32), C.POINTER(C.POINTER(Device))],
            "CAM_CloseDevices": [],
            "CAM_Open": [U32, C.POINTER(U32), U32, C.POINTER(U16)],
            "CAM_Close": [U32],
            "CAM_GetAllFeatures": [U32, C.POINTER(FeatureVector)],
            "CAM_GetFeatures": [U32, C.POINTER(FeatureVector)],
            "CAM_SetFeatures": [U32, C.POINTER(FeatureVector)],
            "CAM_GetFeatureDesc": [U32, U32, C.POINTER(Description)],
            "CAM_Command": [U32, C.c_wchar_p, C.c_void_p],
            "CAM_GetImage": [U32, C.c_bool, C.POINTER(Image), C.POINTER(U32)],
            "CAM_EventPolling": [U32, C.c_void_p, I32, C.POINTER(Event)],
        }
        for name, args in signatures.items():
            method = getattr(self.dll, name)
            method.argtypes = args
            method.restype = I32

    def call(self, name, *args):
        result = getattr(self.dll, name)(*args)
        self.log.setdefault("calls", []).append({"api": name, "result": result})
        limit = getattr(self, "log_limit", None)
        if limit:
            del self.log["calls"][:-limit]
        if result != 0:
            raise RuntimeError(f"{name}: {result}")

    def enumerate(self):
        count, pointer = U32(), C.POINTER(Device)()
        self.call("CAM_OpenDevices", C.byref(count), C.byref(pointer))
        self.enumerated = True
        if not 0 < count.value <= 100 or not pointer:
            raise RuntimeError(f"Invalid or empty device list: {count.value}")
        result = []
        for i in range(count.value):
            dev = pointer[i]
            item = {"index": i, "kind": dev.kind, "serial": dev.serial}
            item.update({key: wtext(getattr(dev, key)) for key in
                         ("firmware", "fpga", "fx3", "usb", "driver", "name")})
            result.append(item)
        self.log["devices"] = result
        return result

    def open(self, devices):
        candidates = [d for d in devices if d["name"] == "DS-Fi3" and d["kind"] == 5]
        if len(candidates) != 1:
            raise RuntimeError(f"Expected exactly one real DS-Fi3, got {candidates}")
        error = (U16 * 256)()
        try:
            self.call("CAM_Open", candidates[0]["index"], C.byref(self.handle), 256, error)
        except Exception as exc:
            raise RuntimeError(f"{exc}: {wtext(error)}") from exc
        self.log["opened"] = candidates[0]

    def features(self):
        array = (Feature * 25)()
        vector = FeatureVector(0, 25, 0, array)
        self.call("CAM_GetAllFeatures", self.handle, C.byref(vector))
        if not 0 < vector.used <= vector.capacity:
            raise RuntimeError("Invalid feature count")
        return {array[i].id: Feature.from_buffer_copy(array[i]) for i in range(vector.used)}

    def describe(self, feature_id):
        desc = Description()
        self.call("CAM_GetFeatureDesc", self.handle, feature_id, C.byref(desc))
        if desc.id != feature_id or desc.count > 256:
            raise RuntimeError("Invalid feature description")
        return desc

    def command(self, name, payload=None):
        self.call("CAM_Command", self.handle, name, C.byref(payload) if payload is not None else None)
        self.log["calls"][-1]["command"] = name

    def set_features(self, features):
        array = (Feature * len(features))(*features)
        vector = FeatureVector(len(array), len(array), 1, array)
        self.call("CAM_SetFeatures", self.handle, C.byref(vector))

    def set_scalar(self, template, value):
        feature = Feature.from_buffer_copy(template)
        if feature.value.kind not in (1, 2, 6):
            raise RuntimeError("Not a scalar feature")
        feature.value.data.u32 = value
        self.set_features([feature])

    def stop(self):
        if self.transfer:
            self.command("CAM_CMD_STOP_FRAMETRANSFER")
            self.transfer = False

    def acquire(self, count, format_info, timeout=12.0):
        import numpy as np
        assert C.sizeof(Image) == 56 and Image.buffer.offset == 8
        assert Image.frame_count.offset == 40 and C.sizeof(Event) == 1032
        size = FrameSize()
        self.command("CAM_CMD_GET_FRAMESIZE", size)
        width, height = format_info["width"], format_info["height"]
        pixels_size = width * height * 3
        if format_info["color"] != 1 or not pixels_size + 256 <= size.buffer_bytes <= pixels_size + width * 3:
            raise RuntimeError(f"Unsupported frame layout: {size.buffer_bytes}, {format_info}")
        buffer = (U8 * (size.buffer_bytes + 64))()
        C.memset(C.addressof(buffer) + size.buffer_bytes, 0xA5, 64)
        frame = Image()
        frame.buffer = C.addressof(buffer)
        frame.buffer_bytes = size.buffer_bytes
        frames, first_image, last_image = [], None, None
        buffers = U32(5)
        # Stop even if START reports a partial failure.
        self.transfer = True
        started = time.perf_counter()
        self.command("CAM_CMD_START_FRAMETRANSFER", buffers)
        deadline = started + timeout
        try:
            while len(frames) < count:
                if time.perf_counter() > deadline:
                    raise TimeoutError(f"Only received {len(frames)}/{count} frames")
                event = Event()
                result = self.dll.CAM_EventPolling(self.handle, None, 0, C.byref(event))
                if result == -10:  # documented no-event / nonblocking result
                    time.sleep(0.002)
                    continue
                if result:
                    raise RuntimeError(f"CAM_EventPolling: {result}")
                remaining = U32()
                self.call("CAM_GetImage", self.handle, False, C.byref(frame), C.byref(remaining))
                received = time.perf_counter()
                if bytes(buffer[size.buffer_bytes:]) != b"\xA5" * 64:
                    raise RuntimeError("Frame buffer guard overwritten")
                if frame.image_bytes != pixels_size or frame.buffer != C.addressof(buffer):
                    raise RuntimeError(f"Unexpected image buffer: {frame.image_bytes}")
                footer = bytes(buffer[pixels_size:pixels_size + 256])
                fw, fh = struct.unpack_from("<HH", footer, 84)
                if (fw, fh) != (width, height) or footer[66] != 1:
                    raise RuntimeError("Frame metadata does not match selected RGB24 format")
                metadata = {"frame_no": struct.unpack_from("<H", footer)[0],
                            "exposure_us": struct.unpack_from("<I", footer, 8)[0],
                            "footer_serial": struct.unpack_from("<I", footer, 68)[0],
                            "camera_type": footer[64],
                            "device_serial": self.log["opened"]["serial"],
                            "width": fw, "height": fh, "color": footer[66],
                            "mode": footer[65], "gain": struct.unpack_from("<H", footer, 104)[0],
                            "wb_red": struct.unpack_from("<H", footer, 124)[0],
                            "wb_blue": struct.unpack_from("<H", footer, 126)[0],
                            "exposure_mode": footer[96], "image_bytes": frame.image_bytes,
                            "buffer_bytes": size.buffer_bytes, "stride_bytes": frame.image_bytes // height,
                            "elapsed_s": received - started, "remaining": remaining.value,
                            "driver_end_time": frame.end_time, "driver_end_time64": frame.end_time64,
                            "event_frame_no": struct.unpack_from("<I", bytes(event), 24)[0],
                            "event_prefix_hex": bytes(event)[:32].hex()}
                # Fi3 firmware leaves the legacy footer serial field at zero.
                # Identity comes from CAM_Open on the exact non-simulator device.
                if metadata["footer_serial"] not in (0, self.log["opened"]["serial"]):
                    raise RuntimeError("Nonzero frame serial differs from selected camera")
                image = np.ctypeslib.as_array(buffer)[:pixels_size].reshape(height, width, 3).copy()
                metadata["sha256"] = hashlib.sha256(image.tobytes()).hexdigest()
                metadata["channel_mean_sample_16px"] = image[::16, ::16].mean(axis=(0, 1)).tolist()
                frames.append(metadata)
                if first_image is None:
                    first_image = image
                last_image = image
            fps = ((len(frames) - 1) / (frames[-1]["elapsed_s"] - frames[0]["elapsed_s"])) if count > 1 else None
            return {"count": count, "fps_host": fps, "frames": frames}, first_image, last_image
        finally:
            self.stop()

    def close(self):
        errors = []
        try:
            self.stop()
        except Exception as exc:
            errors.append(str(exc))
        if self.handle.value:
            try:
                self.call("CAM_Close", self.handle)
                self.handle.value = 0
            except Exception as exc:
                errors.append(str(exc))
        if self.enumerated:
            try:
                self.call("CAM_CloseDevices")
                self.enumerated = False
            except Exception as exc:
                errors.append(str(exc))
        self.log["cleanup_errors"] = errors
        self.dll_directory.close()
        if errors:
            raise RuntimeError("; ".join(errors))
