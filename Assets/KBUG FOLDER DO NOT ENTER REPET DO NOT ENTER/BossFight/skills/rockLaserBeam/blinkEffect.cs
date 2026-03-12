using UnityEngine;

public class blinkEffect : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float blinkSpeed = 5f;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        
        Color currentColor = spriteRenderer.color;

 
        currentColor.a = Mathf.PingPong(Time.time * blinkSpeed, 0.5f);

     
        spriteRenderer.color = currentColor;
    }

}
