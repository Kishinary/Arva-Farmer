using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Guidance : MonoBehaviour
{
    public Image Image;
    public string text;
    public TMP_Text textUI;
    void Start()
    {
        this.GetComponent<Canvas>().worldCamera = Camera.main;
        textUI = GetComponentInChildren<TMP_Text>();
        Image = GetComponentInChildren<Image>();
        textUI.text = text;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
