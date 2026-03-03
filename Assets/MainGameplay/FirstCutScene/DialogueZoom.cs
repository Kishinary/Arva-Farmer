using UnityEngine;
using System.Collections;

public class DialogueZoom : MonoBehaviour
{
    private Camera cam;
    public Transform Target;
    public float duration = 0.3f;

    public float targetZoom = 1f;

    private Camera m_Camera;

    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            cam.GetComponent<CameraZoom>().ZoomIn(1);
            StartCoroutine(MoveCam(Target));
        }
    }


        public IEnumerator MoveCam(Transform target) {
        Vector3 startPos = cam.transform.position;
        Vector3 endPos = target.position;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            cam.transform.position = Vector3.Lerp(startPos, new Vector3(endPos.x, endPos.y, -10), t);

            yield return null;
        }

        transform.position = endPos;
    }

}
