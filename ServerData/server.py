import http.server
import socketserver
import os
from urllib.parse import urlparse
import json

class CORSHTTPRequestHandler(http.server.SimpleHTTPRequestHandler):
    def end_headers(self):
        # Thêm CORS headers
        self.send_header('Access-Control-Allow-Origin', '*')
        self.send_header('Access-Control-Allow-Methods', 'GET, POST, OPTIONS')
        self.send_header('Access-Control-Allow-Headers', 'Content-Type')
        super().end_headers()

    def do_OPTIONS(self):
        # Xử lý preflight requests
        self.send_response(200)
        self.end_headers()

def run_server(port=8000, directory=None):
    if directory:
        os.chdir(directory)
    
    with socketserver.TCPServer(("", port), CORSHTTPRequestHandler) as httpd:
        print(f"Server running at http://localhost:{port}/")
        print(f"Serving directory: {os.getcwd()}")
        print("Press Ctrl+C to stop the server")
        try:
            httpd.serve_forever()
        except KeyboardInterrupt:
            print("\nServer stopped")

if __name__ == "__main__":
    import argparse
    
    parser = argparse.ArgumentParser(description='Python Server for Unity Addressables')
    parser.add_argument('--port', type=int, default=8000, help='Port number')
    parser.add_argument('--dir', type=str, default='.', help='Directory to serve')
    
    args = parser.parse_args()
    
    run_server(args.port, args.dir)