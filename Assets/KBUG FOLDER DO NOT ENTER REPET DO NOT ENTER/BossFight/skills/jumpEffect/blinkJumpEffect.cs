using UnityEngine;

public class blinkJumpEffect : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float blinkTime = 0.8f;
    public float blinkSpeed = 10f;

    private SpriteRenderer _spriteRenderer;
    private Color _color;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _color = _spriteRenderer.color;
        Destroy(gameObject,blinkTime);
    }

    // Update is called once per frame
    void Update()
    {
        _color.a = Mathf.Abs(Mathf.Cos(Time.time * blinkSpeed) * 0.5f);

        _spriteRenderer.color = _color;
    }
}
