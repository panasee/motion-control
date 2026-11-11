"""Camera-only diagnostic; shared ABI lives in nikon_ks_bindings."""
import argparse
from datetime import datetime
import hashlib
import json
from pathlib import Path
import sys
# Support both python -m tools.nikon_ks_probe and direct script execution.
if __package__ in (None, ""):
    sys.path.insert(0, str(Path(__file__).resolve().parents[1]))
from nikon_ks_bindings import *


RESTORE_IDS = (80, 81, 6, 18, 19, 82, 3, 4, 1)


def restore_settings(camera, original, report, initial_dropless=None):
    """Attempt every restoration step even when an earlier SDK operation fails."""
    errors = []
    try:
        camera.stop()
    except Exception as exc:
        errors.append(f"stop: {exc}")
    if initial_dropless is not None:
        try:
            camera.command("CAM_CMD_FRAME_DROPLESS", Dropless(True, initial_dropless))
            check = Dropless(False, False)
            camera.command("CAM_CMD_FRAME_DROPLESS", check)
            report.setdefault("dropless", {})["restored"] = check.enabled
            if check.enabled != initial_dropless:
                raise RuntimeError("Dropless state was not restored")
        except Exception as exc:
            errors.append(f"dropless: {exc}")
    try:
        camera.set_features([original[i] for i in RESTORE_IDS])
    except Exception as exc:
        errors.append(f"set features: {exc}")
    try:
        restored = camera.features()
        report["restored_features"] = {k: variant_json(restored[k].value) for k in RESTORE_IDS}
        if any(bytes(restored[k].value) != bytes(original[k].value) for k in RESTORE_IDS):
            raise RuntimeError("Camera settings readback differs after restoration")
    except Exception as exc:
        errors.append(f"readback: {exc}")
    report["restoration_errors"] = errors
    if errors:
        raise RuntimeError("; ".join(errors))


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--dll", type=Path, default=Path(r"C:\Program Files\NIS-Elements D\KsCam.dll"))
    parser.add_argument("--output", type=Path, default=Path("docs/camera-validation/kscam-readonly.json"))
    parser.add_argument("--capture", action="store_true", help="Capture one frame; restore changed camera settings")
    parser.add_argument("--validate", action="store_true", help="Also test exposure, continuous frames and channel order")
    parser.add_argument("--dropless", action="store_true", help="Request SDK dropless transfer during --validate")
    args = parser.parse_args()
    if args.dropless and not args.validate:
        parser.error("--dropless requires --validate")
    report = {"started_at": datetime.now().astimezone().isoformat(), "dll": str(args.dll),
              "dll_sha256": hashlib.sha256(args.dll.read_bytes()).hexdigest(),
              "stage_operations": [], "status": "running"}
    camera = None
    try:
        camera = Camera(args.dll, report)
        devices = camera.enumerate()
        print(json.dumps(devices, indent=2), flush=True)
        camera.open(devices)
        features = camera.features()
        report["initial_features"] = {k: variant_json(v.value) for k, v in features.items()}
        formats = camera.describe(80)
        report["format_descriptor_header"] = {"id": formats.id, "count": formats.count,
                                               "kind": formats.kind,
                                               "prefix_hex": bytes(formats)[:176].hex()}
        # KsCam 2.1 uses tag 6; the older public header uses tag 8.
        # Validate every descriptor before using its dimensions or format IDs.
        if formats.kind not in (6, 8):
            raise RuntimeError("Unexpected format descriptor type")
        report["formats"] = [{"color": f.format.color, "mode": f.format.mode,
                              "width": f.width, "height": f.height,
                              "bits_per_pixel": f.bits_per_pixel,
                              "comment": wtext(f.comment)} for f in formats.data.formats[:formats.count]]
        if not all(f["color"] in (1, 2, 3) and 0 < f["width"] <= 10000
                   and 0 < f["height"] <= 10000 and f["bits_per_pixel"] in (16, 24)
                   for f in report["formats"]):
            raise RuntimeError("Format descriptor ABI validation failed")
        exposure = camera.describe(3)
        report["exposure_description"] = {"kind": exposure.kind, **{
            key: variant_json(getattr(exposure.data.range, key)) for key in
            ("minimum", "maximum", "step", "default")}}
        frame_size = FrameSize()
        camera.command("CAM_CMD_GET_FRAMESIZE", frame_size)
        report["frame_size"] = {name: getattr(frame_size, name) for name, _ in frame_size._fields_}
        if args.capture or args.validate:
            import numpy as np
            args.output.parent.mkdir(parents=True, exist_ok=True)
            initial_dropless = None
            try:
                if args.dropless:
                    dropless = Dropless(False, False)
                    camera.command("CAM_CMD_FRAME_DROPLESS", dropless)
                    initial_dropless = dropless.enabled
                    camera.command("CAM_CMD_FRAME_DROPLESS", Dropless(True, True))
                    camera.command("CAM_CMD_FRAME_DROPLESS", dropless)
                    report["dropless"] = {"initial": initial_dropless, "readback": dropless.enabled}
                camera.set_scalar(features[1], 2)  # manual exposure, restored below
                camera.set_scalar(features[82], 0)  # free run, no software trigger
                current_format = next(f for f in report["formats"] if
                                      (f["color"], f["mode"]) ==
                                      (features[80].value.data.format.color, features[80].value.data.format.mode))
                single, first, _ = camera.acquire(1, current_format)
                report["single_frame"] = single
                np.save(args.output.with_suffix(".raw.npy"), first)
                from PyQt6.QtGui import QImage
                rgb = np.ascontiguousarray(first[:, :, ::-1])
                image = QImage(rgb.data, rgb.shape[1], rgb.shape[0], rgb.strides[0], QImage.Format.Format_RGB888)
                if not image.save(str(args.output.with_suffix(".bgr-assumed.png"))):
                    raise RuntimeError("Failed to save preview")
                if args.validate:
                    report["exposure_tests"] = []
                    for requested in (10000, 20000, 50000):
                        camera.set_scalar(features[3], requested)
                        readback = camera.features()[3].value.data.u32
                        captured, _, _ = camera.acquire(3, current_format)
                        observed = [f["exposure_us"] for f in captured["frames"]]
                        report["exposure_tests"].append({"requested_us": requested,
                                                        "hardware_readback_us": readback,
                                                        "frame_exposure_us": observed})
                        if readback != requested or any(v != requested for v in observed):
                            raise RuntimeError("Exposure request/readback/frame metadata mismatch")
                    camera.set_scalar(features[3], 20000)
                    report["continuous"] = []
                    for mode in (1, 3):
                        selected = next(f for f in report["formats"] if f["color"] == 1 and f["mode"] == mode)
                        feature = Feature.from_buffer_copy(features[80])
                        feature.value.data.format = Format(1, mode)
                        camera.set_features([feature])
                        stream, first, last = camera.acquire(60, selected)
                        frame_numbers = [f["frame_no"] for f in stream["frames"]]
                        gaps = [(b - a) % 65536 for a, b in zip(frame_numbers, frame_numbers[1:])]
                        stream["format"] = selected
                        stream["frame_number_deltas"] = gaps
                        stream["unique_payloads"] = len({f["sha256"] for f in stream["frames"]})
                        stream["missing_frame_numbers"] = sum(max(d - 1, 0) for d in gaps)
                        stream["duplicate_frames"] = gaps.count(0)
                        report["continuous"].append(stream)
                        if any(d == 0 for d in gaps):
                            raise RuntimeError("Continuous capture repeated a frame number")
                        if any(d != 1 for d in gaps):
                            report.setdefault("observations", []).append(
                                f"Mode {mode}: frame-number gaps; loss location not established")
                        np.save(args.output.with_name(f"continuous-mode-{mode}-last.raw.npy"), last)
                    # The two independent color controls identify byte order empirically.
                    # Use the small non-ROI mode and unchanged illumination/exposure/gain.
                    report["channel_order_test"] = {}
                    for label, changed in (("baseline", None), ("red_plus_30", 18), ("blue_plus_30", 19), ("baseline_after", None)):
                        camera.set_features([features[18], features[19]])
                        if changed is not None:
                            desc = camera.describe(changed)
                            target = features[changed].value.data.u32 + 30
                            if target > desc.data.range.maximum.data.u32:
                                raise RuntimeError("White-balance probe would exceed camera limits")
                            camera.set_scalar(features[changed], target)
                        captured, _, last = camera.acquire(5, selected)
                        report["channel_order_test"][label] = {
                            "mean": last[::16, ::16].mean(axis=(0, 1)).tolist(),
                            "metadata": captured["frames"][-1]}
                    base = np.array(report["channel_order_test"]["baseline"]["mean"])
                    red = np.array(report["channel_order_test"]["red_plus_30"]["mean"])
                    blue = np.array(report["channel_order_test"]["blue_plus_30"]["mean"])
                    red_delta, blue_delta = red - base, blue - base
                    report["channel_order_test"]["red_delta"] = red_delta.tolist()
                    report["channel_order_test"]["blue_delta"] = blue_delta.tolist()
                    red_channel, blue_channel = int(np.argmax(red_delta)), int(np.argmax(blue_delta))
                    if (red_channel, blue_channel) != (2, 0):
                        raise RuntimeError("BGR channel order was not confirmed by color controls")
                    if red_delta[2] < 5 or blue_delta[0] < 5:
                        raise RuntimeError("Insufficient channel response to establish byte order")
                    report["channel_order_test"]["confirmed_order"] = "BGR"
            finally:
                restore_settings(camera, features, report, initial_dropless)
        report["status"] = "passed"
    except Exception as exc:
        report["status"] = "failed"
        report["error"] = repr(exc)
    finally:
        if camera is not None:
            try:
                camera.close()
            except Exception as exc:
                report["status"] = "failed"
                report["cleanup_exception"] = repr(exc)
        report["finished_at"] = datetime.now().astimezone().isoformat()
        args.output.parent.mkdir(parents=True, exist_ok=True)
        args.output.write_text(json.dumps(report, indent=2), encoding="utf-8")
    print(json.dumps(report, indent=2))
    return 0 if report["status"] == "passed" else 1


if __name__ == "__main__":
    sys.exit(main())
