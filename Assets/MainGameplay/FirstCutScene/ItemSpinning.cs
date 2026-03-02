using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    [Header("Settings")]
    public float rotationSpeed = 100f;
    public float bobSpeed = 2f;
    public float bobAmount = 0.5f;

    private Vector3 startPos;

    void Start()
    {
        // Store the starting position to bob around it
        startPos = transform.position;
    }

    void Update()
    {
        // 1. Spinning Effect
        // We rotate around the Y axis to get that "flat flip" look
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        // 2. Bobbing Effect (Sine Wave)
        // Mathf.Sin creates a smooth up-and-down motion over time
        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobAmount;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}