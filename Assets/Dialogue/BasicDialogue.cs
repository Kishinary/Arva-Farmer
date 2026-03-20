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

    [Header("Checker")]
    public bool isChatting;

    private bool notSpawnyet = true;

    [Header("Changer")]
    public GameObject NextDialogue;
    public GameObject Player;
    [Header("Sound Effect")]
    public AudioSource audioSource;
    public AudioClip typingSound;
   
    // Start is called before the first frame update
    void Start()
    {
        textComponent = GetComponentInChildren<TextMeshProUGUI>();
        textComponent.text = string.Empty;
        Player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !notSpawnyet)
        {
            isChatting = true;
            if (textComponent.text == lines[index])
            {
                audioSource.Stop();
                NextLine();

            }
            else
            {
                audioSource.Stop();
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }
        else
        {
            StartDialogue();
            if (Player) Player.GetComponent<PlayerMovement>().DisablePlayerInput();
        }
    }

    void StartDialogue()
    {
        if (notSpawnyet) {
            index = 0;
            StartCoroutine(TypeLine());
            notSpawnyet = false;
            if (CineZoom.instance) CineZoom.instance.ZoomIn(transform.position, 0.5f);
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
            isChatting = false;
            if (NextDialogue)
            {
                NextDialogue.SetActive(true);
            }
            Reset();
        }
    }
    private void OnEnable()
    {
        if (Player) Player.GetComponent<PlayerMovement>().DisablePlayerInput();
    }
    private void Reset()
    {
        if (Player) Player.GetComponent<PlayerMovement>().EnablePlayerInput();
        if (CineZoom.instance) CineZoom.instance.ZoomOut();
        index = 0;
        notSpawnyet = true;
        gameObject.SetActive(false);
        textComponent.text = string.Empty;
    }
}