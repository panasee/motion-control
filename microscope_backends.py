"""Application-level backend selection; the acquisition core has no vendor import."""


def default_camera_backend():
    # Only executed by the camera worker after an explicit connection request.
    from nikon_ks_camera import NikonKsCamera
    return NikonKsCamera()
