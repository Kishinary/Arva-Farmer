using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstScene : MonoBehaviour
{
    private Camera cam;

    public Transform villagerPosition;
    public Transform playerPosition;

    private float duration = 0.5f;

    public bool finallyy = false;

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
    public void Update()
    {
        if (finallyy)
        {
            finallyy = false;
            Debug.Log("Pls");
            StartCoroutine(FinalWalk());
        }
    }

    IEnumerator BlackIn()
    {
        float timer = 0f;

        Renderer rend = gameObject.GetComponent<Renderer>();
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

    IEnumerator FinalWalk()
    {
        StartCoroutine(Player.MoveDown());
        StartCoroutine(Blacker());
        yield return new WaitForSeconds(1.1f);
        SceneManager.LoadScene("Lobby");
    }
    IEnumerator Blacker()
    {
        Renderer rend = gameObject.GetComponent<Renderer>();

        Material mat = rend.material;
        Color tempColor = mat.color;
        float startAlpha = tempColor.a;
        float targetAlpha = 1f; // fade to fully opaque (black overlay) before scene change

        float timer = 0f;

        while (timer < 1)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / 1f);
            tempColor.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            mat.color = tempColor;

            yield return null;
        }

        // Ensure final alpha
        tempColor.a = targetAlpha;
        mat.color = tempColor;
    }
}
