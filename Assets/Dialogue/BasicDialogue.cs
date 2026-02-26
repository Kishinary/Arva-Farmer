using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BasicDialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent; 
    public string[] lines; 
    public float textSpeed; 

    private int index;

    [Header("Camera")]
    public CameraZoom m_camera;
    public PlayerMovement Player;
    public Camera cam;

    [Header("Checker")]
    private bool isChatting;
    // Start is called before the first frame update
    void Start()
    {
        m_camera = GetComponentInParent<CameraZoom>();
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        textComponent = GetComponentInChildren<TextMeshProUGUI>();
        Player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        textComponent.text = string.Empty; 
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isChatting) 
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
        if (Input.GetKeyDown(KeyCode.Space)) {
            index = 0;
            StartCoroutine(TypeLine());
            Player.DisablePlayerInput();
            m_camera.ZoomIn(cam);
        }
    }

    IEnumerator TypeLine()
    {
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c; 
            yield return new WaitForSeconds(textSpeed);
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
            m_camera.ZoomOut(cam);
            Player.EnablePlayerInput();
        }
    }
}