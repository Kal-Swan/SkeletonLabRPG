import threading
import requests

def _send_progress_request(api_url, headers, build_request_id, percent):
    try:
        requests.get(
            f"{api_url}/api/v1/buildrequest/progress/{build_request_id}?percent={percent}",
            headers=headers,
            timeout=5
        )
    except Exception as e:
        print(f"Progress update failed: {e}")

def send_progress(api_url: str, headers: dict, build_request_id: str, percent: int):
    print(f"In send_progress with percent: {percent}")

    threading.Thread(
        target=_send_progress_request,
        args=(api_url, headers, build_request_id, percent),
        daemon=True
    ).start()
