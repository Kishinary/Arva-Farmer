using UnityEngine;

public class Orbiter : MonoBehaviour
{
    public Transform player;
    public float radius = 10f;
    public float rotationSpeed = 120f;

    float angle;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        angle = Random.Range(0f, 360f);
    }

    void Update()
    {
        if (player == null) return;

        angle += rotationSpeed * Time.deltaTime;

        float rad = angle * Mathf.Deg2Rad;

        Vector3 pos = new Vector3(
            player.position.x + Mathf.Cos(rad) * radius,
            player.position.y + Mathf.Sin(rad) * radius,
            0
        );

        transform.position = pos - new Vector3(0f, 0.2f, 0);
    }
}