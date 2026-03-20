using UnityEngine;

public class lightningWarning : MonoBehaviour
{
    public float blinkSpeed = 5f;
    public float minAlpha = 0.2f;
    public float maxAlpha = 0.4f;
    public float lifeTime = 1f;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        
    }
    private void Update()
    {

        Color currentColor = spriteRenderer.color;

        float t = Mathf.PingPong(Time.time * blinkSpeed, 1f);

        currentColor.a = Mathf.Lerp(minAlpha, maxAlpha, t);


        spriteRenderer.color = currentColor;
    }
}
