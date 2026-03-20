using UnityEngine;

public class bulletMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private float speed;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }
    public void Launch(Vector2 direction, float projSpeed)
    {
        rb.linearVelocity = direction.normalized * projSpeed;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            

            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            playerHealth.TakeDamage(10f);

           
            Despawn();
        }

    }
    private void OnBecameInvisible()
    {
        Despawn();
    }
    private void Despawn()
    {
        Destroy(gameObject);
    }


}
