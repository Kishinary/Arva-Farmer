using UnityEngine;

public class warningSlashAttack : MonoBehaviour
{
    public float lifeTime = 3f;

    private SpriteRenderer spriteRenderer;
    public float blinkSpeed = 5f;
    public float minAlpha = 0.2f;
    public float maxAlpha = 0.4f;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Destroy(gameObject,lifeTime);
    }
    private void Update()
    {
        Color currentColor = spriteRenderer.color;

        float t = Mathf.PingPong(Time.time * blinkSpeed, 1f);

        currentColor.a = Mathf.Lerp(minAlpha, maxAlpha, t);


        spriteRenderer.color = currentColor;
    }

}
