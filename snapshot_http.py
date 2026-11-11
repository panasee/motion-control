"""Small application-neutral read-only HTTP transport; no hardware or Qt imports."""
from dataclasses import dataclass
from http.server import BaseHTTPRequestHandler, ThreadingHTTPServer
import threading


@dataclass(frozen=True)
class HttpResponse:
    status: int
    body: bytes
    content_type: str = "application/json; charset=utf-8"
    headers: tuple = ()


class SnapshotHttpServer:
    def __init__(self, routes, *, host="127.0.0.1", port=0):
        self.routes = routes
        self.host, self.port = host, port
        self._server = self._thread = None

    @property
    def url(self):
        host, port = self._server.server_address if self._server else (self.host, self.port)
        return f"http://{host}:{port}"

    def start(self):
        if self._server is not None:
            return
        routes = self.routes

        class Handler(BaseHTTPRequestHandler):
            def do_GET(self):
                route = routes.get(self.path)
                if route is None:
                    self.send_error(404)
                    return
                try:
                    response = route()
                except Exception:
                    response = HttpResponse(500, b'{"error":{"code":"internal_error"}}')
                self.send_response(response.status)
                self.send_header("Content-Type", response.content_type)
                self.send_header("Content-Length", str(len(response.body)))
                self.send_header("Cache-Control", "no-store")
                for name, value in response.headers:
                    self.send_header(name, value)
                self.end_headers()
                try:
                    self.wfile.write(response.body)
                except (BrokenPipeError, ConnectionResetError):
                    pass

            def do_POST(self):
                self.send_error(405)

            def log_message(self, format, *args):
                pass

        self._server = ThreadingHTTPServer((self.host, self.port), Handler)
        self._thread = threading.Thread(target=self._server.serve_forever,
                                        kwargs={"poll_interval": .05}, daemon=True)
        self._thread.start()

    def stop(self):
        if self._server is None:
            return
        server, thread = self._server, self._thread
        self._server = self._thread = None
        server.shutdown()
        server.server_close()
        if thread is not None:
            thread.join(timeout=2)
