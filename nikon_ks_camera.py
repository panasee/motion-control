"""DS-Fi3 backend for the locally validated KsCam 2.1.2.3 x64 runtime.

Replace this adapter (not the UI/session) when the official Digital Sight SDK
arrives. ABI, color order, feature units and lifetimes require revalidation.
"""
import ctypes as C
from datetime import datetime
import math
import os
from pathlib import Path
import struct
import time

import numpy as np

from microscope_camera import CameraFrame
from nikon_ks_bindings import Camera, Feature, Format, FrameSize, Image, Event, U8, U32

RESTORE_IDS = (80, 81, 6, 18, 19, 82, 3, 4, 1)


class NikonKsCamera:
    def __init__(self, dll_path=None):
        self.path = Path(dll_path or os.environ.get("NIKON_KSCAM_DLL") or
                         r"C:\Program Files\NIS-Elements D\KsCam.dll")
        self.camera = None
        self.original = None
        self.settings = None
        self.buffer = None
        self.changed = False

    def open(self):
        self.camera = Camera(self.path, {})
        self.camera.log_limit = 128
        self.camera.open(self.camera.enumerate())
        self.original = self.camera.features()
        desc = self.camera.describe(80)
        if desc.kind not in (6, 8):
            raise RuntimeError("不支持的图像格式描述；需重新验证 SDK。")
        self.modes = [{"id": f.format.mode, "width": f.width, "height": f.height}
                      for f in desc.data.formats[:desc.count]
                      if f.format.color == 1 and f.format.mode in (1, 3)
                      and f.bits_per_pixel == 24 and 0 < f.width <= 10000 and 0 < f.height <= 10000]
        if {m["id"] for m in self.modes} != {1, 3}:
            raise RuntimeError("相机未提供已验证的全幅 / 平均采样 RGB24 格式。")
        self.ranges = {}
        for name, fid, scale in (("exposure_ms", 3, 1000), ("gain", 4, 100)):
            r = self.camera.describe(fid).data.range
            values = [getattr(r, k).data.u32 / scale for k in ("minimum", "maximum", "step")]
            if not 0 < values[0] <= values[1] or values[2] <= 0:
                raise RuntimeError(f"无效的参数范围：{name}")
            self.ranges[name] = values
        # Connecting reads only. Manual/free-run configuration happens explicitly
        # on Apply or at the first acquisition, and is restored on disconnect.
        return self._read_settings()

    def _read_settings(self):
        features = self.camera.features()
        fmt = features[80].value.data.format
        self.settings = dict(device=self.camera.log["opened"].copy(), modes=self.modes,
                             ranges=self.ranges, mode=fmt.mode, color=fmt.color,
                             exposure_ms=features[3].value.data.u32 / 1000,
                             gain=features[4].value.data.u32 / 100,
                             exposure_auto=features[1].value.data.u32 != 2)
        return self.settings.copy()

    def configure(self, exposure_ms, gain, mode):
        self.stop()
        if mode not in {m["id"] for m in self.modes}:
            raise ValueError("未验证的采样模式")
        scalars = []
        for name, value, fid, scale in (("exposure_ms", exposure_ms, 3, 1000), ("gain", gain, 4, 100)):
            lo, hi, step = self.ranges[name]
            if not math.isfinite(value) or not lo <= value <= hi:
                raise ValueError(f"{name} 必须在 {lo}–{hi} 之间")
            lo_i, hi_i, step_i = round(lo * scale), round(hi * scale), round(step * scale)
            raw = min(hi_i, max(lo_i, lo_i + round((value * scale - lo_i) / step_i) * step_i))
            f = Feature.from_buffer_copy(self.original[fid])
            f.value.data.u32 = raw
            scalars.append(f)
        fmt = Feature.from_buffer_copy(self.original[80])
        fmt.value.data.format = Format(1, mode)
        self.changed = True  # also restore after partial SDK failure
        self.camera.set_scalar(self.original[1], 2)
        self.camera.set_scalar(self.original[82], 0)
        self.camera.set_features([fmt, *scalars])
        result = self._read_settings()
        if result["mode"] != mode or result["color"] != 1 or result["exposure_auto"]:
            raise RuntimeError("相机格式 / 手动曝光读回不匹配")
        if (round(result["exposure_ms"] * 1000), round(result["gain"] * 100)) != tuple(f.value.data.u32 for f in scalars):
            raise RuntimeError("相机曝光 / 增益读回不匹配")
        return result

    def start(self):
        if self.settings["color"] != 1 or self.settings["mode"] not in (1, 3):
            raise RuntimeError("请先应用全幅或平均采样 RGB24 模式。")
        self.configure(self.settings["exposure_ms"], self.settings["gain"], self.settings["mode"])
        # STOP clears image buffers but the installed runtime can retain image
        # notifications. Drain those while stopped, before starting a new run.
        self._drain_image_events()
        self.format = next(m for m in self.modes if m["id"] == self.settings["mode"])
        self.pixel_bytes = self.format["width"] * self.format["height"] * 3
        size = FrameSize()
        self.camera.command("CAM_CMD_GET_FRAMESIZE", size)
        if not self.pixel_bytes + 256 <= size.buffer_bytes <= self.pixel_bytes + self.format["width"] * 3:
            raise RuntimeError("相机缓冲区布局与已验证格式不符")
        self.buffer = (U8 * (size.buffer_bytes + 64))()
        C.memset(C.addressof(self.buffer) + size.buffer_bytes, 0xA5, 64)
        self.image = Image()
        self.image.buffer = C.addressof(self.buffer)
        self.image.buffer_bytes = size.buffer_bytes
        self.buffer_bytes = size.buffer_bytes
        self.last_number = None
        self.missing_ids = self.duplicates = self.received = 0
        self.started = time.monotonic()
        self.camera.transfer = True
        self.camera.command("CAM_CMD_START_FRAMETRANSFER", U32(5))

    def _drain_image_events(self):
        for _ in range(1024):
            event = Event()
            result = self.camera.dll.CAM_EventPolling(self.camera.handle, None, 0, C.byref(event))
            if result == -10:  # empirically empty nonblocking queue in pinned runtime
                return
            if result:
                raise RuntimeError(f"CAM_EventPolling while stopped: {result}")
        raise RuntimeError("相机停止后的图像事件队列未能清空")

    def poll(self):
        event = Event()
        result = self.camera.dll.CAM_EventPolling(self.camera.handle, None, 0, C.byref(event))
        if result == -10:
            return None
        if result:
            raise RuntimeError(f"CAM_EventPolling: {result}")
        remaining = U32()
        self.camera.call("CAM_GetImage", self.camera.handle, False, C.byref(self.image), C.byref(remaining))
        if (bytes(self.buffer[self.buffer_bytes:]) != b"\xA5" * 64 or
                self.image.image_bytes != self.pixel_bytes or self.image.buffer != C.addressof(self.buffer)):
            raise RuntimeError("相机帧缓冲区校验失败")
        footer = bytes(self.buffer[self.pixel_bytes:self.pixel_bytes + 256])
        width, height = struct.unpack_from("<HH", footer, 84)
        serial = struct.unpack_from("<I", footer, 68)[0]
        device = self.settings["device"]
        if ((width, height) != (self.format["width"], self.format["height"]) or
                footer[66] != 1 or footer[65] != self.settings["mode"] or serial not in (0, device["serial"])):
            raise RuntimeError("相机帧元数据与当前配置不符")
        number = struct.unpack_from("<H", footer)[0]
        if self.last_number is not None:
            delta = (number - self.last_number) % 65536
            self.missing_ids += max(0, delta - 1)
            self.duplicates += delta == 0
        self.last_number = number
        now = time.monotonic()
        if not self.received:
            self.first_frame_time = now
        self.received += 1
        bgr = np.ctypeslib.as_array(self.buffer)[:self.pixel_bytes].reshape(height, width, 3)
        rgb = np.ascontiguousarray(bgr[:, :, ::-1])
        rgb.flags.writeable = False
        metadata = dict(backend="KsCam 2.1.2.3", device_serial=device["serial"],
                        width=width, height=height, pixel_format="RGB8", source_channel_order="BGR",
                        mode=self.settings["mode"], frame_no=number,
                        exposure_ms=struct.unpack_from("<I", footer, 8)[0] / 1000,
                        gain=struct.unpack_from("<H", footer, 104)[0] / 100,
                        captured_at=datetime.now().astimezone().isoformat(),
                        received_frames=self.received, missing_frame_ids=self.missing_ids,
                        duplicate_frame_ids=self.duplicates,
                        fps=(self.received - 1) / max(now - self.first_frame_time, 1e-9),
                        buffer_bytes=self.buffer_bytes, stride_bytes=width * 3)
        return CameraFrame(rgb, metadata)

    def stop(self):
        if self.camera is not None:
            self.camera.stop()

    def close(self):
        if self.camera is None:
            return
        errors = []
        try:
            self.stop()
        except Exception as exc:
            errors.append(str(exc))
        if self.changed and self.original is not None:
            try:
                self.camera.set_features([self.original[i] for i in RESTORE_IDS])
                actual = self.camera.features()
                if any(bytes(actual[i].value) != bytes(self.original[i].value) for i in RESTORE_IDS):
                    raise RuntimeError("断开时相机参数恢复读回不匹配")
            except Exception as exc:
                errors.append(str(exc))
        try:
            self.camera.close()
        except Exception as exc:
            errors.append(str(exc))
        self.camera = None
        if errors:
            raise RuntimeError("; ".join(errors))
