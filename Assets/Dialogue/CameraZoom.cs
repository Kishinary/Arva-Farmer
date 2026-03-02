using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public static CameraZoom Instance;

    private float velocity = 0f;
    private float smoothTime = 0.25f;

    private float targetZoom;

    [SerializeField] private Camera cam;
    private Canvas canvas;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (cam == null)
            cam = Camera.main;

        canvas = GetComponent<Canvas>();

        targetZoom = cam.orthographicSize;

        if (canvas != null)
            canvas.worldCamera = cam;
    }

    void Update()
    {
        cam.orthographicSize = Mathf.SmoothDamp(
            cam.orthographicSize,
            targetZoom,
            ref velocity,
            smoothTime
        );
    }

    public void ZoomIn(float minZoom)
    {
        targetZoom = minZoom;
    }

    public void ZoomOut(float maxZoom)
    {
        targetZoom = maxZoom;
    }

    public void SetZoom(float zoom)
    {
        targetZoom = zoom;
    }
}