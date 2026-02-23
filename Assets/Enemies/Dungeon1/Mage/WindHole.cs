using UnityEngine;

public class WindHole : MonoBehaviour
{
    public float pullForce = 10f;
    public float duration = 2f;

    private bool active = false;

    public void StartWarning()
    {
        // Change color / animation to warning
    }

    public void Activate()
    {
        active = true;
        Invoke("DestroyHole", duration);
    }

    void DestroyHole()
    {
        Destroy(gameObject);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!active) return;

        if (other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

            Vector2 dir = (transform.position - other.transform.position).normalized;
            rb.AddForce(dir * pullForce);
        }
    }
}