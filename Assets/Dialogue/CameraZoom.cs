using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    private float zoom;
    private float zoomMultiplier = 4f;
    private float minZoom = 2f;
    private float maxZoom = 8f;
    private float velocity = 0f;
    private float smoothTime = 0.25f;

    [SerializeField] private Camera cam;
    private Canvas canvas;

    private void Start()
    {
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        canvas = GetComponent<Canvas>();

        zoom = cam.orthographicSize;
        canvas.worldCamera = cam;
    }
    public void ZoomIn(Camera cam)
    {
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, maxZoom, ref velocity, smoothTime);
    }
    public void ZoomOut(Camera cam)
    {
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, minZoom, ref velocity, smoothTime);

    }
}