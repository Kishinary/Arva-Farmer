using UnityEngine;

public class blinkPlusRotationEffect : MonoBehaviour
{
    public float blinkSpeed = 5f;
    public float minAlpha = 0.2f;
    public float maxAlpha = 0.4f;
    public float lifeTime = 1f;
    public Vector3 rotationSpeed = new Vector3(0f, 0f, 500f);
    public float scaleSpeed = 2f;

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

        transform.Rotate(rotationSpeed * Time.deltaTime, Space.Self);
        transform.localScale += Vector3.one * scaleSpeed * Time.deltaTime;

        spriteRenderer.color = currentColor;
    }
}
