"""Offline latency assessment; never opens camera/motion hardware or changes the UI.

Measures the production extractor/worker and experimental lower-resolution/LUT
pipelines on resident RGB8 images. LUT results are compared against the original
NumPy distance computation; this is an experiment, not a production replacement.
"""
import argparse
from datetime import datetime
import hashlib
import inspect
import json
import os
from pathlib import Path
import platform
import sys
import time

if __package__ in (None, ""):
    sys.path.insert(0, str(Path(__file__).resolve().parents[1]))

import cv2
import numpy as np
from PyQt6.QtGui import QImage
from stack_designer import ContourWorker, extract_contours


def load_rgb(path):
    bgr = cv2.imdecode(np.fromfile(path, np.uint8), cv2.IMREAD_COLOR)
    if bgr is None:
        raise ValueError(f"Cannot decode {path}")
    return cv2.cvtColor(bgr, cv2.COLOR_BGR2RGB)


def production(rgb, **parameters):
    try:
        return extract_contours(rgb, **parameters)
    except ValueError as exc:
        if "未识别到有效" not in str(exc):
            raise
        return []


def pipeline(rgb, cap=1600, distance_method="numpy", inspect_arrays=False, *,
             threshold=0, min_area_percent=0.1, simplification=0.4):
    """Original stages, with an explicit cap and interchangeable distance kernel."""
    stages = {}
    tick = time.perf_counter_ns()

    def mark(name):
        nonlocal tick
        now = time.perf_counter_ns()
        stages[name] = (now - tick) / 1e6
        tick = now

    height, width = rgb.shape[:2]
    ratio = min(1., cap / max(height, width))
    small = cv2.resize(rgb, (round(width * ratio), round(height * ratio)), interpolation=cv2.INTER_AREA)
    mark("resize")
    lab = cv2.cvtColor(small, cv2.COLOR_RGB2LAB)
    if distance_method == "numpy":
        lab = lab.astype(np.float32)
    mark("rgb_to_lab")
    border = np.concatenate((lab[0], lab[-1], lab[:, 0], lab[:, -1]))
    # float32 gives the same median/intermediate units as the production code.
    background = np.median(border.astype(np.float32, copy=False), axis=0)
    mark("border_median")
    if distance_method == "numpy":
        distance = np.clip(np.linalg.norm(lab - background, axis=2), 0, 255).astype(np.uint8)
    elif distance_method == "lut":
        # Exact squared channel distances for each possible 8-bit Lab value,
        # including half-integer medians. Avoid the full-frame float subtraction.
        lut = ((np.arange(256, dtype=np.float32)[:, None] - background) ** 2).reshape(256, 1, 3)
        squared = cv2.LUT(lab, lut)
        summed = cv2.transform(squared, np.ones((1, 3), np.float32))
        distance = cv2.min(cv2.sqrt(summed), 255).astype(np.uint8)
    else:
        raise ValueError(distance_method)
    mark("color_distance")
    blurred = cv2.GaussianBlur(distance, (5, 5), 0)
    mark("gaussian_blur")
    mode = cv2.THRESH_BINARY | (cv2.THRESH_OTSU if threshold == 0 else 0)
    threshold, mask = cv2.threshold(blurred, threshold, 255, mode)
    mark("otsu_threshold")
    contours, _ = cv2.findContours(mask, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
    mark("find_contours")
    polygons = []
    image_area = small.shape[0] * small.shape[1]
    for contour in sorted(contours, key=cv2.contourArea, reverse=True):
        area = cv2.contourArea(contour)
        if area < image_area * min_area_percent / 100 or area > image_area * .95:
            continue
        polygon = cv2.approxPolyDP(contour, simplification / 100 * cv2.arcLength(contour, True), True)
        if len(polygon) >= 3:
            points = polygon.reshape(-1, 2).astype(float)
            points *= [width / small.shape[1], height / small.shape[0]]
            polygons.append(points.tolist())
    mark("filter_approximate_map")
    info = dict(stages_ms=stages, processed_size=[small.shape[1], small.shape[0]],
                raw_contours=len(contours), accepted_polygons=len(polygons), threshold=threshold)
    if inspect_arrays:
        info.update(distance=distance, mask=mask)
    return polygons, info


def statistics(values):
    a = np.asarray(values)
    return dict(n=len(a), p50_ms=float(np.percentile(a, 50)), p95_ms=float(np.percentile(a, 95)),
                p99_ms=float(np.percentile(a, 99)), minimum_ms=float(a.min()), maximum_ms=float(a.max()),
                fraction_over_10ms=float(np.mean(a > 10)),
                fraction_over_30fps_budget=float(np.mean(a > 1000/30)),
                samples_ms=a.tolist())


def measure(operation, iterations, warmup=15):
    for _ in range(warmup):
        operation()
    samples = []
    stage_samples = {}
    result = None
    for _ in range(iterations):
        start = time.perf_counter_ns()
        result = operation()
        samples.append((time.perf_counter_ns() - start) / 1e6)
        if isinstance(result, tuple):
            for key, value in result[1]["stages_ms"].items():
                stage_samples.setdefault(key, []).append(value)
    summary = statistics(samples)
    if isinstance(result, tuple):
        summary.update({k: v for k, v in result[1].items() if k != "stages_ms"})
        summary["stage_p50_ms"] = {k: float(np.median(v)) for k, v in stage_samples.items()}
        summary["stage_p95_ms"] = {k: float(np.percentile(v, 95)) for k, v in stage_samples.items()}
    else:
        summary["accepted_polygons"] = len(result)
    return summary


def quality_against_reference(rgb, polygons, reference):
    shape = rgb.shape[:2]
    def raster(points):
        mask = np.zeros(shape, np.uint8)
        if points:
            cv2.fillPoly(mask, [np.rint(p).astype(np.int32) for p in points], 1)
        return mask
    actual, expected = raster(polygons), raster(reference)
    union = np.count_nonzero(actual | expected)
    intersection = np.count_nonzero(actual & expected)
    largest, largest_reference = raster(polygons[:1]), raster(reference[:1])
    main_union = np.count_nonzero(largest | largest_reference)
    return dict(reference_is_ground_truth=False, polygon_count=len(polygons),
                union_iou=intersection / union if union else 1.,
                largest_polygon_iou=np.count_nonzero(largest & largest_reference) / main_union if main_union else 1.)


def worker_operation(rgb):
    h, w = rgb.shape[:2]
    # Pre-existing QImage matches the UI input; file decode and initial wrapping
    # are excluded. Worker scaling, RGB conversion and coordinate mapping count.
    image = QImage(rgb.data, w, h, rgb.strides[0], QImage.Format.Format_RGB888).copy()
    worker = ContourWorker(image, {})
    box = {"polygons": []}
    worker.extracted.connect(lambda polygons: box.update(polygons=polygons))
    worker.failed.connect(lambda message: box.update(polygons=[]))
    def run():
        worker.run()  # synchronous compute body; QThread launch/queue delay excluded
        return box["polygons"]
    return run


def synthetic_scene(width, height, radius_divisor=65):
    rng = np.random.default_rng(20261111)
    rgb = np.full((height, width, 3), (170, 160, 190), np.uint8)
    for row in range(7):
        for column in range(10):
            x = int((column + .6) * width / 11)
            y = int((row + .6) * height / 8)
            radius = max(8, width // radius_divisor)
            angles = np.linspace(0, 2 * np.pi, 7, endpoint=False)
            lengths = rng.uniform(.6, 1.3, 7) * radius
            polygon = np.stack([x + np.cos(angles)*lengths, y + np.sin(angles)*lengths], axis=1).astype(np.int32)
            cv2.fillPoly(rgb, [polygon], (80, 120, 155))
    noise = rng.normal(0, 1.5, rgb.shape).astype(np.int16)
    return np.clip(rgb.astype(np.int16) + noise, 0, 255).astype(np.uint8)


def compare_kernels(rgb, cap, reference=None):
    slow_polygons, slow = pipeline(rgb, cap, "numpy", True)
    fast_polygons, fast = pipeline(rgb, cap, "lut", True)
    check = dict(cap=cap,
                 distance_different_pixels=int(np.count_nonzero(slow["distance"] != fast["distance"])),
                 mask_different_pixels=int(np.count_nonzero(slow["mask"] != fast["mask"])),
                 polygons_equal=slow_polygons == fast_polygons)
    if reference is not None:
        check["quality"] = quality_against_reference(rgb, fast_polygons, reference)
    return check


def refinement(rgb, reference, record, report):
    """Investigate resize cost and small-target rejection found in the first run."""
    for cap in (720, 480):
        report["checks"].append(dict(label="full_frame", **compare_kernels(rgb, cap, reference)))
        record(f"live2_full_lut_{cap}", rgb, lambda cap=cap: pipeline(rgb, cap, "lut"), cap=cap)

    # Represents the array size of the camera's average mode, NOT a fresh
    # hardware capture or proof of the sensor's averaging/aliasing behaviour.
    half = cv2.resize(rgb, (rgb.shape[1] // 2, rgb.shape[0] // 2), interpolation=cv2.INTER_AREA)
    half_reference = production(half)
    report["checks"].append(dict(label="resampled_half", **compare_kernels(half, 720, half_reference)))
    record("live2_resampled_1440_lut_720", half, lambda: pipeline(half, 720, "lut"), cap=720,
           input_preparation="Pre-resampled offline; resize from 2880 to 1440 excluded")

    # The original 70 small polygons straddle the default 0.1% area cutoff.
    # A separate larger-object scene tests contour load, without changing the
    # production threshold or pretending that the original scene detected 70.
    synthetic = synthetic_scene(rgb.shape[1], rgb.shape[0], radius_divisor=40)
    synthetic_reference = production(synthetic)
    report["synthetic_larger_scene"] = dict(seed=20261111, drawn=70, radius_divisor=40,
                                             production_accepted=len(synthetic_reference))
    report["checks"].append(dict(label="synthetic_larger", **compare_kernels(synthetic, 720, synthetic_reference)))
    record("synthetic_70_larger_production", synthetic, lambda: production(synthetic))
    record("synthetic_70_larger_lut_720", synthetic, lambda: pipeline(synthetic, 720, "lut"), cap=720)

    # Revisit the stage profile because the first profiling batch had much
    # greater jitter than the adjacent production batch. Keep both results.
    record("live2_production_repeat", rgb, lambda: production(rgb))
    record("live2_profile_repeat", rgb, lambda: pipeline(rgb))
    record("live2_full_lut_720_repeat", rgb, lambda: pipeline(rgb, 720, "lut"), cap=720)

    roi = rgb[400:912, 1000:1512].copy()
    report["checks"].append(dict(label="roi_512", **compare_kernels(roi, 512, production(roi))))
    record("live2_roi_512_lut_repeat", roi, lambda: pipeline(roi, 512, "lut"))
    # Seeded non-flake inputs exercise half-integer background medians and
    # different distance distributions. This checks equivalence, not recall.
    rng = np.random.default_rng(20261111)
    for index, shape in enumerate(((37, 51, 3), (128, 128, 3), (257, 319, 3))):
        noise = rng.integers(0, 256, shape, dtype=np.uint8)
        report["checks"].append(dict(label=f"noise_{index}", **compare_kernels(noise, 1600)))


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--image", type=Path, required=True)
    parser.add_argument("--output", type=Path)
    parser.add_argument("--iterations", type=int, default=120)
    parser.add_argument("--suite", choices=("baseline", "refinement"), default="baseline")
    args = parser.parse_args()
    if args.iterations < 1:
        parser.error("--iterations must be positive")
    if args.output is None:
        args.output = Path("docs/flake-benchmark" if args.suite == "baseline" else "docs/flake-benchmark-refinement")
    args.output.mkdir(parents=True, exist_ok=True)
    original_threads = cv2.getNumThreads()
    rgb = load_rgb(args.image)
    report = dict(started_at=datetime.now().astimezone().isoformat(),
                  source=str(args.image.resolve()), source_sha256=hashlib.sha256(args.image.read_bytes()).hexdigest(),
                  source_size=[rgb.shape[1], rgb.shape[0]],
                  platform=platform.platform(), python=sys.version, numpy=np.__version__, opencv=cv2.__version__,
                  opencv_threads=original_threads, logical_cpus=os.cpu_count(), opencv_optimized=cv2.useOptimized(),
                  source_function_sha256=hashlib.sha256(inspect.getsource(extract_contours).encode()).hexdigest(),
                  benchmark_script_sha256=hashlib.sha256(Path(__file__).read_bytes()).hexdigest(),
                  suite=args.suite, argv=sys.argv, warmup_iterations=15,
                  timing_scope="Resident RGB8 input; scaling through original-coordinate polygons; excludes file IO, SDK, exposure, GUI and QThread scheduling",
                  hardware_opened=False, cases=[], checks=[])
    (args.output / "benchmark-script.py").write_bytes(Path(__file__).read_bytes())
    (args.output / "opencv-build.txt").write_text(cv2.getBuildInformation(), encoding="utf-8")
    start = time.perf_counter_ns()
    reference = production(rgb)
    report["cold_first_extraction_ms"] = (time.perf_counter_ns() - start) / 1e6
    report["reference_polygons"] = reference

    def record(label, image, operation, **extra):
        result = measure(operation, args.iterations)
        row = dict(label=label, input_size=[image.shape[1], image.shape[0]], threads=cv2.getNumThreads(), **extra, **result)
        report["cases"].append(row)
        print(json.dumps({k: row[k] for k in ("label", "threads", "input_size", "p50_ms", "p95_ms", "accepted_polygons")}), flush=True)
        (args.output / "results.json").write_text(json.dumps(report, indent=2), encoding="utf-8")
        return row

    if args.suite == "refinement":
        refinement(rgb, reference, record, report)
        report["finished_at"] = datetime.now().astimezone().isoformat()
        (args.output / "results.json").write_text(json.dumps(report, indent=2), encoding="utf-8")
        return

    record("live2_production_numpy", rgb, lambda: production(rgb))
    record("live2_existing_qimage_worker", rgb, worker_operation(rgb))
    traced, info = pipeline(rgb)
    assert traced == reference, "Instrumented baseline differs from production"
    record("live2_profile_numpy", rgb, lambda: pipeline(rgb))
    for cap in (1600, 1024, 800, 640, 512):
        slow_polygons, slow = pipeline(rgb, cap, "numpy", True)
        fast_polygons, fast = pipeline(rgb, cap, "lut", True)
        check = dict(cap=cap,
                     distance_different_pixels=int(np.count_nonzero(slow["distance"] != fast["distance"])),
                     mask_different_pixels=int(np.count_nonzero(slow["mask"] != fast["mask"])),
                     polygons_equal=slow_polygons == fast_polygons,
                     quality=quality_against_reference(rgb, fast_polygons, reference))
        report["checks"].append(check)
        if cap != 1600:
            record(f"live2_numpy_cap_{cap}", rgb, lambda cap=cap: pipeline(rgb, cap, "numpy"), cap=cap)
        record(f"live2_lut_cap_{cap}", rgb, lambda cap=cap: pipeline(rgb, cap, "lut"), cap=cap)

    # True sensor-sized background frames, separate from an annotated flake photo.
    for label, path in (("camera_full_background", Path("docs/camera-validation/kscam-final-single.bgr-assumed.png")),
                        ("camera_average_background", Path("docs/camera-validation/integration/single-original.png"))):
        if path.exists():
            image = load_rgb(path)
            record(label + "_production", image, lambda image=image: production(image))
            record(label + "_lut_800", image, lambda image=image: pipeline(image, 800, "lut"))
    synthetic = synthetic_scene(rgb.shape[1], rgb.shape[0])
    report["synthetic_seed"] = 20261111
    record("synthetic_70_targets_production", synthetic, lambda: production(synthetic))
    record("synthetic_70_targets_lut_800", synthetic, lambda: pipeline(synthetic, 800, "lut"))
    # Pre-cropped ROI timing; ROI selection/copy and GUI/SDK are excluded.
    roi = rgb[400:912, 1000:1512].copy()
    record("live2_roi_512_production", roi, lambda: production(roi))
    record("live2_roi_512_lut", roi, lambda: pipeline(roi, 512, "lut"))
    # Thread count is process-local and restored; never change the running UI.
    for threads in (1, 4):
        cv2.setNumThreads(threads)
        record(f"live2_production_threads_{threads}", rgb, lambda: production(rgb))
        record(f"live2_lut_800_threads_{threads}", rgb, lambda: pipeline(rgb, 800, "lut"))
    cv2.setNumThreads(original_threads)
    report["finished_at"] = datetime.now().astimezone().isoformat()
    (args.output / "results.json").write_text(json.dumps(report, indent=2), encoding="utf-8")


if __name__ == "__main__":
    main()
