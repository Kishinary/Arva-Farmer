using System.Collections;
using UnityEngine;

public class MoneyDrop : MonoBehaviour
{
    [Header("Magnet Settings")]
    public int amount = 10;
    public float waitTime = 2f;
    public float baseSpeed = 2f;
    public float acceleration = 25f; // Increased for a snappier feel

    [Header("Pop Effect")]
    public float popHeight = 1.5f;
    public float popDuration = 0.3f;

    [Header("Visual Juice (Bob & Spin)")]
    public float rotationSpeed = 200f;
    public float bobSpeed = 2f;
    public float bobAmount = 0.2f;

    [Header("Effects")]
    public GameObject collectVFXPrefab;

    private Transform playerTransform;
    private bool isMagnetizing = false;
    private bool isBobbing = false;
    private Vector3 anchorPosition;
    private float currentSpeed;

    void Start()
    {
        currentSpeed = baseSpeed;

        // Find the player by tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        // Start the sequence
        StartCoroutine(PopSequence());
    }

    private IEnumerator PopSequence()
    {
        Vector3 spawnPos = transform.position;
        float elapsed = 0;

        // 1. Pop Up/Down
        while (elapsed < popDuration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / popDuration;
            float heightOffset = Mathf.Sin(percent * Mathf.PI) * popHeight;
            transform.position = spawnPos + new Vector3(0, heightOffset, 0);
            yield return null;
        }

        // 2. Settle on the ground
        anchorPosition = spawnPos;
        transform.position = anchorPosition;
        isBobbing = true;

        // 3. Wait 2 seconds
        yield return new WaitForSeconds(waitTime);

        // 4. Trigger Magnet
        isBobbing = false;
        isMagnetizing = true;
    }

    void Update()
    {
        // SPIN (Always happens)
        // Note: Using Vector3.forward if this is a 2D game is usually better!
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        // BOB (Only while waiting)
        if (isBobbing)
        {
            float newY = anchorPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobAmount;
            transform.position = new Vector3(anchorPosition.x, newY, transform.position.z);
        }

        // MAGNET (Flight logic)
        if (isMagnetizing && playerTransform != null)
        {
            // Move toward player position
            Vector3 targetPos = playerTransform.position;
            float distance = Vector3.Distance(transform.position, targetPos);

            // Accelerate based on time and closeness
            currentSpeed += acceleration * Time.deltaTime;

            // Move using MoveTowards for more reliability
            transform.position = Vector3.MoveTowards(transform.position, targetPos, currentSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    void Collect()
    {
        if (collectVFXPrefab != null)
        {
            Instantiate(collectVFXPrefab, transform.position, Quaternion.identity);
        }

        CurrencyManager.Instance.AddMoney(amount);
        Destroy(gameObject);
    }
}