using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Guidance : MonoBehaviour
{
    public Image Image;
    public string text;
    public TextMeshPro textUI;
    void Start()
    {
        this.GetComponent<Canvas>().worldCamera = Camera.main;
        textUI = GetComponentInChildren<TextMeshPro>();
        Image = GetComponentInChildren<Image>();
        textUI.text = text;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
