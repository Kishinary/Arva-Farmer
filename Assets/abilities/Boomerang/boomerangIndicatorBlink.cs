using UnityEngine;

public class boomerangIndicatorBlink : MonoBehaviour
{
    public float blinkSpeed = 5f;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {

        Color currentColor = spriteRenderer.color;


        currentColor.a = Mathf.PingPong(Time.time * blinkSpeed, 0.5f) + 0.2f;


        spriteRenderer.color = currentColor;
    }
}
