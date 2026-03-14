using UnityEngine;

public class tractorIndicatorBlink : MonoBehaviour
{

    public float blinkSpeed = 5f;

    private SpriteRenderer spriteRenderer;

    private GameObject player;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindWithTag("Player");
    }
    private void Update()
    {

        Color currentColor = spriteRenderer.color;


        currentColor.a = Mathf.PingPong(Time.time * blinkSpeed, 0.8f) + 0.2f;


        spriteRenderer.color = currentColor;



        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;



        Vector3 direction = mouseWorldPos - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float snappedAngle = Mathf.Round(angle / 45f) * 45f;
       


        transform.rotation = Quaternion.Euler(0f, 0f, snappedAngle);

        transform.position = player.transform.position;
    }
}
