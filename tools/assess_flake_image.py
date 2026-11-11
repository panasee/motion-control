"""Offline speed/contour assessment of a supplied photo; no hardware access.

Compares production extraction with the experimental LUT pipeline. References
are algorithm outputs, never ground truth. All plotting/quality calculations
run outside timing. No camera, motion control or application settings are used.
"""
import argparse
from datetime import datetime
import hashlib
import inspect
import json
from pathlib import Path
import platform
import sys
import time

if __package__ in (None, ""):
    sys.path.insert(0, str(Path(__file__).resolve().parents[1]))

from tools.benchmark_flake_detection import (
    cv2, np, extract_contours, load_rgb, measure, pipeline, production,
    quality_against_reference, worker_operation,
)


def polygon_matches(polygons, reference, shape, minimum_iou=0.5):
    """One-to-one polygon agreement and overlap relationships, not recall."""
    def raster(p):
        points = np.rint(p).astype(np.int32)
        x0, y0 = np.maximum(points.min(axis=0), 0)
        x1, y1 = np.minimum(points.max(axis=0) + 1, [shape[1], shape[0]])
        mask = np.zeros((y1-y0, x1-x0), np.uint8)
        cv2.fillPoly(mask, [points - [x0, y0]], 1)
        return (int(x0), int(y0), int(x1), int(y1)), mask, int(mask.sum())

    actual, expected = [raster(p) for p in polygons], [raster(p) for p in reference]
    pairs, coverage = [], [[] for _ in polygons]
    for i, (a, am, aa) in enumerate(actual):
        for j, (b, bm, ba) in enumerate(expected):
            x0, y0, x1, y1 = max(a[0], b[0]), max(a[1], b[1]), min(a[2], b[2]), min(a[3], b[3])
            if x1 <= x0 or y1 <= y0:
                continue
            intersection = int(np.count_nonzero(
                am[y0-a[1]:y1-a[1], x0-a[0]:x1-a[0]] &
                bm[y0-b[1]:y1-b[1], x0-b[0]:x1-b[0]]))
            iou = intersection / (aa + ba - intersection)
            if iou >= minimum_iou:
                pairs.append((iou, i, j))
            if intersection >= ba * .5:
                coverage[i].append(j)
    used_actual, used_expected, matched = set(), set(), []
    for iou, i, j in sorted(pairs, reverse=True):
        if i not in used_actual and j not in used_expected:
            matched.append(dict(actual=i, reference=j, iou=iou))
            used_actual.add(i)
            used_expected.add(j)
    return dict(reference_is_ground_truth=False, minimum_iou=minimum_iou,
                matching="greedy descending IoU, one-to-one", matched_count=len(matched), pairs=matched,
                unmatched_actual=[i for i in range(len(polygons)) if i not in used_actual],
                unmatched_reference=[i for i in range(len(reference)) if i not in used_expected],
                covers_multiple_reference=[dict(actual=i, references=ids) for i, ids in enumerate(coverage) if len(ids)>1])


def render_figures(rgb, cases, output, sample_name):
    import matplotlib
    matplotlib.use("Agg")
    import matplotlib.pyplot as plt
    from matplotlib.patches import Polygon

    by_label = {c["label"]: c for c in cases}
    selected = [
        ("existing_extractor_default", "Original | area >= 0.1%"),
        ("existing_extractor_small", "Original | area >= 0.01%"),
        ("lut_720_small", "720 x 512 LUT | area >= 0.01%"),
        ("lut_1440_small", "1440 x 1024 LUT | area >= 0.01%"),
    ]

    def draw(ax, case, title, region=None):
        ax.imshow(rgb)
        if case is not None:
            for p in case["polygons"]:
                ax.add_patch(Polygon(p, closed=True, fill=False, edgecolor="#00ffff", linewidth=1))
        if region:
            x0, y0, x1, y1 = region
            ax.set_xlim(x0, x1)
            ax.set_ylim(y1, y0)
        ax.set_title(title, fontsize=11)
        ax.set_xticks([])
        ax.set_yticks([])

    fig, axes = plt.subplots(2, 2, figsize=(14, 10.4), constrained_layout=True)
    for ax, (label, title) in zip(axes.flat, selected):
        c = by_label[label]
        draw(ax, c, f"{title}\n{c['accepted_polygons']} candidates | P50 {c['p50_ms']:.2f} ms, P95 {c['p95_ms']:.2f} ms")
    fig.suptitle("Candidate outlines on the original image (counts are NOT verified flake counts)", fontsize=13)
    fig.savefig(output / "contour-overview.png", dpi=160)
    plt.close(fig)

    # Fixed source-coordinate regions chosen for Live3. These are qualitative
    # inspection windows, not hand-labelled objects or segmentation truth.
    if sample_name.lower() == "live3" and rgb.shape[:2] == (2048, 2880):
        regions = [(180, 1390, 1030, 1970), (1220, 380, 1710, 810), (1880, 380, 2730, 960)]
        names = ["Small bright fragments", "Touching / folded fragments", "Faint green structures"]
        fig, axes = plt.subplots(3, 4, figsize=(16, 10), constrained_layout=True)
        columns = [(None, "Input"), selected[1], selected[2],
                   ("lut_720_low_contrast", "720 x 512 LUT | area >= 0.01%, threshold 10")]
        for row, (region, name) in enumerate(zip(regions, names)):
            for col, (label, title) in enumerate(columns):
                draw(axes[row, col], by_label.get(label), title if row == 0 else "", region)
            axes[row, 0].set_ylabel(name, fontsize=10)
        fig.suptitle("Live3 detail inspection | all outlines mapped to original coordinates", fontsize=13)
        fig.savefig(output / "contour-details.png", dpi=160)
        plt.close(fig)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--image", type=Path, required=True)
    parser.add_argument("--output", type=Path, required=True)
    parser.add_argument("--iterations", type=int, default=120)
    parser.add_argument("--render-only", action="store_true")
    args = parser.parse_args()
    if args.iterations < 1:
        parser.error("--iterations must be positive")
    rgb = load_rgb(args.image)
    digest = hashlib.sha256(args.image.read_bytes()).hexdigest()
    if args.render_only:
        report = json.loads((args.output / "results.json").read_text(encoding="utf-8"))
        if digest != report["source_sha256"]:
            parser.error("Image hash differs from the measured input")
        render_figures(rgb, report["cases"], args.output, args.image.stem)
        return
    args.output.mkdir(parents=True, exist_ok=True)
    report = dict(started_at=datetime.now().astimezone().isoformat(), source=str(args.image.resolve()),
                  source_sha256=digest, source_size=[rgb.shape[1], rgb.shape[0]],
                  python=sys.version, platform=platform.platform(), numpy=np.__version__, opencv=cv2.__version__,
                  opencv_threads=cv2.getNumThreads(), opencv_optimized=cv2.useOptimized(),
                  warmup_iterations=15, hardware_opened=False, argv=sys.argv, cases=[], checks=[],
                  source_function_sha256=hashlib.sha256(inspect.getsource(extract_contours).encode()).hexdigest(),
                  timing_scope="Resident RGB8 to original-coordinate polygons; includes resize, excludes IO/exposure/SDK/GUI/thread scheduling")
    for path in (Path(__file__), Path(__file__).with_name("benchmark_flake_detection.py")):
        (args.output / path.name).write_bytes(path.read_bytes())
        report[path.stem + "_sha256"] = hashlib.sha256(path.read_bytes()).hexdigest()
    (args.output / "opencv-build.txt").write_text(cv2.getBuildInformation(), encoding="utf-8")

    start = time.perf_counter_ns()
    default_polygons = production(rgb)
    report["cold_extraction_ms"] = (time.perf_counter_ns() - start) / 1e6
    references = {area: production(rgb, min_area_percent=area) for area in (.1, .01, .003)}
    report["reference_counts"] = {str(k): len(v) for k, v in references.items()}
    assert default_polygons == references[.1]
    for area, reference in references.items():
        assert pipeline(rgb, min_area_percent=area)[0] == reference, "Instrumented pipeline differs from production"

    def save():
        (args.output / "results.json").write_text(json.dumps(report, indent=2), encoding="utf-8")

    configs = [
        ("existing_worker", "worker", 1600, .1, 0),
        ("existing_extractor_default", "production", 1600, .1, 0),
        ("existing_extractor_small", "production", 1600, .01, 0),
        ("lut_1600_small", "lut", 1600, .01, 0),
        ("lut_1440_small", "lut", 1440, .01, 0),
        ("lut_720_default", "lut", 720, .1, 0),
        ("lut_720_small", "lut", 720, .01, 0),
        ("lut_720_tiny", "lut", 720, .003, 0),
        ("lut_1440_tiny", "lut", 1440, .003, 0),
        ("lut_720_low_contrast", "lut", 720, .01, 10),
    ]
    for label, method, cap, area, threshold in configs:
        parameters = dict(min_area_percent=area, threshold=threshold)
        if method == "worker":
            operation = worker_operation(rgb)
        elif method == "production":
            operation = lambda parameters=parameters: production(rgb, **parameters)
        else:
            operation = lambda cap=cap, parameters=parameters: pipeline(rgb, cap, "lut", **parameters)
        row = dict(label=label, method=method, cap=cap, parameters=parameters,
                   minimum_area_original_px2=rgb.shape[0] * rgb.shape[1] * area / 100,
                   **measure(operation, args.iterations))
        result = operation()
        polygons = result[0] if isinstance(result, tuple) else result
        row["polygons"] = polygons
        reference = references[area] if threshold == 0 else production(rgb, **parameters)
        row["agreement_with_1600_same_parameters"] = {
            **quality_against_reference(rgb, polygons, reference),
            **polygon_matches(polygons, reference, rgb.shape)}
        report["cases"].append(row)
        print(json.dumps({k: row[k] for k in ("label", "p50_ms", "p95_ms", "accepted_polygons")}), flush=True)
        if method == "lut":
            slow_polygons, slow = pipeline(rgb, cap, "numpy", True, **parameters)
            fast_polygons, fast = pipeline(rgb, cap, "lut", True, **parameters)
            check = dict(label=label,
                         distance_different_pixels=int(np.count_nonzero(slow["distance"] != fast["distance"])),
                         mask_different_pixels=int(np.count_nonzero(slow["mask"] != fast["mask"])),
                         polygons_equal=slow_polygons == fast_polygons)
            report["checks"].append(check)
        save()
    report["finished_at"] = datetime.now().astimezone().isoformat()
    save()
    render_figures(rgb, report["cases"], args.output, args.image.stem)


if __name__ == "__main__":
    main()
