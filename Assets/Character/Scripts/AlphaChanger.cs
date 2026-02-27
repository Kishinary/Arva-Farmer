using UnityEngine;

public class AlphaChanger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Decorations"))
        {
            float timer = 0;
            timer += Time.deltaTime;
            Color TempColor = collision.GetComponent<Renderer>().material.color;
            TempColor.a = Mathf.Lerp(1, 0.5f, timer / 0.1f);
            collision.GetComponent<Renderer>().material.color = TempColor;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Decorations"))
        {
            float timer = 0;
            timer += Time.deltaTime;
            Color TempColor = collision.GetComponent<Renderer>().material.color;
            TempColor.a = Mathf.Lerp(0.5f, 1f, timer / 0.1f);
            collision.GetComponent<Renderer>().material.color = TempColor;
        }
    }
}
