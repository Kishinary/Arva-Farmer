using System.Collections;
using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.UIElements;

public class FirstScene : MonoBehaviour
{
    public GameObject SceneTransformer;
    private Camera cam;

    public Transform villagerPosition;
    public Transform playerPosition;

    private float duration = 0.5f;


    [Header("Dialogue")]
    public GameObject DialogueBox1;

    [Header("Player")]
    public TemporaryPlayer Player;

    
    void Start()
    {
        cam = Camera.main;
        StartCoroutine(RealRoutine());
        Player = GameObject.FindWithTag("Player").GetComponent<TemporaryPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator BlackIn()
    {
        float timer = 0f;

        Renderer rend = SceneTransformer.GetComponent<Renderer>();
        Color tempColor = rend.material.color;

        while (timer < 0.1f)
        {
            timer += Time.deltaTime;
            tempColor.a = Mathf.Lerp(1f, 0f, timer / 0.1f);
            rend.material.color = tempColor;

            yield return null;
        }
    }

    IEnumerator RealRoutine()
    {
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(BlackIn());
        yield return new WaitForSeconds(0.3f);
        yield return MoveCam(villagerPosition);
        yield return StartCoroutine(Player.MoveUp());
        cam.GetComponent<CameraZoom>().ZoomIn(5);
        DialogueBox1.SetActive(true);
    }

    public IEnumerator MoveCam(Transform target)
    {
        Vector3 startPos = cam.transform.position;
        Vector3 endPos = target.position;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            cam.transform.position = Vector3.Lerp(startPos, endPos, t);

            yield return null;
        }

        transform.position = endPos;
    }
}
