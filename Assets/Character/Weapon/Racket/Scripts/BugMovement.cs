using UnityEngine;

public class BugMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 0.5f;     
    public float range = 0.8f;         
    public float hoverSpeed = 2f;      
    public float hoverAmount = 0.2f;   

    private Vector2 startPosition;
    private float randomOffset;

    void Start()
    {
        startPosition = transform.position;

        randomOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        float x = Mathf.Sin(Time.time * moveSpeed + randomOffset) * range;
        float y = Mathf.Cos(Time.time * (moveSpeed * 0.8f) + randomOffset) * (range * 0.5f);

        float bob = Mathf.Sin(Time.time * hoverSpeed + randomOffset) * hoverAmount;

        transform.position = startPosition + new Vector2(x, y + bob);
    }
}