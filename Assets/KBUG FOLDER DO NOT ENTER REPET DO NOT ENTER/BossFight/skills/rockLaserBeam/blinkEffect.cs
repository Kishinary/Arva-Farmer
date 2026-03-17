using UnityEngine;

public class blinkEffect : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        if (lifeTime > 0f)
        {
            Destroy(gameObject, lifeTime);
        }
    }
    private void Update()
    {

        Color currentColor = spriteRenderer.color;

        float t = Mathf.PingPong(Time.time * blinkSpeed, 1f);

        currentColor.a = Mathf.Lerp(minAlpha, maxAlpha, t);


        spriteRenderer.color = currentColor;
    }

}
