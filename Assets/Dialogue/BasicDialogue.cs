using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UIElements;

public class Dialogue : MonoBehaviour
{
    [Header("Text Stuff")]
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed = 0.05f;

    [SerializeField]
    private int index;

    public int lineschoiceindex;

    [Header("Panel for Choice2")]
    public GameObject panel2;


    [Header("ButtonChoice")]
    private bool ButtonCreated = false;
    public GameObject Button1;
    public GameObject Button2;

    public string TextforB1;
    public string TextforB2;

    // Start is called before the first frame update
    void Start()
    {
        textComponent.text = string.Empty;
        StartDialogue();

        TextMeshProUGUI TextB1 = Button1.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI TextB2 = Button2.GetComponentInChildren<TextMeshProUGUI>();

        TextB1.text = TextforB1;
        TextB2.text = TextforB2;
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (textComponent.text == lines[index])
            {
                if (lineschoiceindex == index)
                {
                    Button1.gameObject.SetActive(true);
                    Button2.gameObject.SetActive(true);
                }
                else
                {
                    NextLine();
                }
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }
    }

    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
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
        }
    }


    public void OnButton1()
    {
        panel2.SetActive(true);
        Destroy(this.gameObject);
    }

    public void OnButton2()
    {
        NextLine();
        Button1.gameObject.SetActive(false);
        Button2.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Button1.SetActive(false);
        Button2.SetActive(false);
    }
}