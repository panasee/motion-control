"""Independent microscope executable composition root; no motion-app imports."""
import argparse
import hashlib
import sys

from PyQt6.QtCore import QLockFile
from PyQt6.QtNetwork import QLocalServer, QLocalSocket
from PyQt6.QtWidgets import QApplication, QMessageBox

from camera_protocol import CameraHttpServer, CAMERA_API_PORT
from microscope_settings import application_settings_path
from microscope_workspace import MicroscopeWindow
from app_version import APP_VERSION


class ApplicationInstance:
    """Per-user lock and activation only; this IPC cannot operate hardware."""
    def __init__(self, settings_path):
        settings_path.parent.mkdir(parents=True, exist_ok=True)
        digest = hashlib.sha256(str(settings_path.resolve()).encode()).hexdigest()[:24]
        self.name = "microscope-workbench-" + digest
        self.lock = QLockFile(str(settings_path.parent / "application.lock"))
        self.server = None

    def acquire(self):
        return self.lock.tryLock(0)

    def activate_existing(self):
        socket = QLocalSocket()
        socket.connectToServer(self.name)
        connected = socket.waitForConnected(1000)
        if connected:
            socket.waitForReadyRead(1000)
            socket.disconnectFromServer()
        return connected

    def listen(self, window):
        self.server = QLocalServer()
        self.server.setSocketOptions(QLocalServer.SocketOption.UserAccessOption)
        QLocalServer.removeServer(self.name)  # only the lock owner removes stale endpoints
        if not self.server.listen(self.name):
            raise RuntimeError(self.server.errorString())

        def activate():
            while self.server.hasPendingConnections():
                socket = self.server.nextPendingConnection()
                window.showNormal() if window.isMinimized() else window.show()
                window.raise_()
                window.activateWindow()
                socket.write(b"activated")
                socket.flush()
                socket.disconnected.connect(socket.deleteLater)
                socket.disconnectFromServer()
        self.server.newConnection.connect(activate)

    def close(self):
        if self.server is not None:
            self.server.close()
        self.lock.unlock()


def main(argv=None):
    parser = argparse.ArgumentParser(description="Independent microscope workbench")
    parser.add_argument("--no-api", action="store_true", help="Disable the read-only image HTTP service")
    parser.add_argument("--api-port", type=int, default=CAMERA_API_PORT)
    args = parser.parse_args(argv)
    if not 0 <= args.api_port <= 65535:
        parser.error("--api-port must be in 0..65535")
    app = QApplication([sys.argv[0]])
    app.setApplicationName("MicroscopeWorkbench")
    app.setApplicationVersion(APP_VERSION)
    app.setStyle("Fusion")
    instance = ApplicationInstance(application_settings_path())
    if not instance.acquire():
        if not instance.activate_existing():
            QMessageBox.warning(None, "显微镜程序正在运行", "已有实例正在启动或暂未响应，请稍后重试。")
        return 0
    server = None
    try:
        window = MicroscopeWindow()
        instance.listen(window)
        if not args.no_api:
            server = CameraHttpServer(window.image_state.snapshot_png, port=args.api_port)
            try:
                server.start()
            except OSError as exc:
                QMessageBox.warning(window, "图像接口不可用", f"{server.url}\n{exc}\n显微镜仍可独立使用。")
        window.show()
        return app.exec()
    finally:
        if server is not None:
            server.stop()
        instance.close()


if __name__ == "__main__":
    raise SystemExit(main())
