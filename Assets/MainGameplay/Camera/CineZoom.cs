using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class CineZoom : MonoBehaviour
{
    public static CineZoom instance;

    private CinemachineCamera vcam;
    private Transform player;

    Coroutine zoomRoutine;

    [Header("Zoom Settings")]
    public float normalSize = 8f;
    public float zoomSize = 3.5f;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        vcam = GetComponent<CinemachineCamera>();
        player = GameObject.FindWithTag("Player").transform;
    }

    public void ZoomIn(Vector2 position, float duration)
    {
        if (zoomRoutine != null)
            StopCoroutine(zoomRoutine);

        zoomRoutine = StartCoroutine(ZoomInRoutine(position, duration));
    }

    IEnumerator ZoomInRoutine(Vector2 position, float duration)
    {
        vcam.Follow = null;

        Vector3 pos = new Vector3(position.x, position.y + 0.4f, -10);
        vcam.transform.position = pos;

        float startSize = vcam.Lens.OrthographicSize;
        float timer = 0;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, timer / duration);

            vcam.Lens.OrthographicSize = Mathf.Lerp(startSize, zoomSize, t);

            yield return null;
        }

        vcam.Lens.OrthographicSize = zoomSize;
    }

    public void ZoomOut(float duration = 0.5f)
    {
        if (zoomRoutine != null)
            StopCoroutine(zoomRoutine);

        zoomRoutine = StartCoroutine(ZoomOutRoutine(duration));
    }

    IEnumerator ZoomOutRoutine(float duration)
    {
        vcam.Follow = player;

        float startSize = vcam.Lens.OrthographicSize;
        float timer = 0;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, timer / duration);

            vcam.Lens.OrthographicSize = Mathf.Lerp(startSize, normalSize, t);

            yield return null;
        }

        vcam.Lens.OrthographicSize = normalSize;
    }
}