using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BasicDialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    [TextArea(3, 10)]
    public string[] lines; 
    public float textSpeed; 

    private int index;

    [Header("Camera")]
    public CameraZoom m_camera;
    public Camera cam;

    [Header("Checker")]
    private bool isChatting;

    private bool notSpawnyet = true;

    [Header("Changer")]
    public GameObject NextDialogue;

    [Header("Sound Effect")]
    public AudioSource audioSource;
    public AudioClip typingSound;
    // Start is called before the first frame update
    void Start()
    {
        m_camera = GetComponentInParent<CameraZoom>();
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        textComponent = GetComponentInChildren<TextMeshProUGUI>();
        textComponent.text = string.Empty; 
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !notSpawnyet) 
        {
            isChatting = true;
            if (textComponent.text == lines[index]) 
            {
                NextLine(); 
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index]; 
            }
        }
        StartDialogue();
    }

    void StartDialogue()
    {
        if (Input.GetMouseButtonDown(0) && notSpawnyet) {
            index = 0;
            StartCoroutine(TypeLine());
            //Player.DisablePlayerInput();
            //m_camera.ZoomIn();
            notSpawnyet = false;
        }
    }

    IEnumerator TypeLine()
    {
        // 1. Start the looping audio
        if (audioSource != null && typingSound != null)
        {
            audioSource.clip = typingSound;
            audioSource.loop = true; // Make sure it repeats
            audioSource.Play();
        }

        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        // 2. Stop the audio once the foreach loop is finished
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1) 
        {
            index++; 
            textComponent.text = string.Empty; 
            StartCoroutine(TypeLine()); 
        }
        else
        {
            gameObject.SetActive(false);
            isChatting = false;
            //m_camera.ZoomOut();
            //Player.EnablePlayerInput();
            if (NextDialogue)
            {
                NextDialogue.SetActive(true);
            }
        }
    }
}