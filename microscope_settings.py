"""Microscope-owned configuration namespace; legacy location is read-only input."""
import os
from math import isfinite
from pathlib import Path


EDGE_PROCESSING_SIZES = (720, 1440, 1600)
DEFAULT_EDGE_DETECTION_PARAMETERS = {
    "max_dimension": 720, "threshold": 0, "min_area_percent": 0.01,
    "simplification": 0.4,
}


def validate_edge_detection_parameters(parameters):
    """Validate microscope-owned preferences without loading an imaging backend."""
    if not isinstance(parameters, dict) or set(parameters) != set(DEFAULT_EDGE_DETECTION_PARAMETERS):
        raise ValueError("边缘识别参数格式无效。")
    size = parameters["max_dimension"]
    if type(size) is not int or size not in EDGE_PROCESSING_SIZES:
        raise ValueError("边缘识别处理分辨率无效。")
    threshold = parameters["threshold"]
    if type(threshold) is not int or not 0 <= threshold <= 255:
        raise ValueError("色差阈值必须为 0–255 的整数，0 表示自动。")
    for key, low, high in (("min_area_percent", 0.001, 50), ("simplification", 0.01, 5)):
        value = parameters[key]
        if type(value) not in (int, float) or not isfinite(value) or not low <= value <= high:
            raise ValueError(f"边缘识别参数 {key} 超出范围 [{low}, {high}]。")
    return dict(parameters)


def configuration_root():
    return Path(os.environ.get("APPDATA") or Path.home() / ".config")


def application_settings_path():
    return configuration_root() / "microscope-workbench" / "settings.json"


def settings_read_path(destination, *, migrate_legacy=False):
    destination = Path(destination)
    if destination.exists() or not migrate_legacy:
        return destination
    legacy = configuration_root() / "motion-control" / "microscope-settings.json"
    return legacy if legacy.exists() else destination
