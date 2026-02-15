using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class Cutscence : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource AS;

    [Header("Door")]
    public GameObject door;
    public Sprite OpennedDoor;
    public Sprite ClosedDoor;
    public AudioClip OpenSound;
    public AudioClip ClosedSound;


    [Header("Panels")]
    public GameObject FirstPanel;

    [Header("Light")]
    public Light2D GlobalLight;


    [Header("Player")]
    public GameObject Player;
    public Sprite PlayerSprite;

    public float timer = 0f;
    void Start()
    {
        StartCoroutine(FirstCutScence());
    }

    void Update()
    {
        timer += Time.deltaTime;
    }


    IEnumerator FirstCutScence()
    {
        timer = 0f;
        yield return StartCoroutine(LightGoesUp(0f,3f));
        StartCoroutine(Door(3f));
    }


    IEnumerator LightGoesUp(float start, float end)
    {
        while (timer <= start) {
            yield return null;
        }
        while (timer <= end)
        {
            GlobalLight.intensity = Mathf.Lerp(0f, 1f, timer / 3);
            yield return null;
        }
    }

    IEnumerator Door(float start)
    {
        while (timer <= start)
        {
            yield return null;
        }
        door.GetComponent<SpriteRenderer>().sprite = OpennedDoor;
        AS.PlayOneShot(OpenSound);
        GameObject NewPlayer = Instantiate(Player, door.transform.position - new Vector3(-0.05f, 0.1f, 0f), Quaternion.identity);
        NewPlayer.transform.localScale = new Vector3(-1.5f, 1.5f);
    }

}
