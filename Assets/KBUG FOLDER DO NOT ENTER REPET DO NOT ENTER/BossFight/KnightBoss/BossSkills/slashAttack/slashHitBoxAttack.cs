using UnityEngine;

public class slashHitBoxAttack : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float lifeTime = 0.5f;
    public float damge = 30f;
    public void Awake()
    {
        Destroy(gameObject,lifeTime);
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            playerHealth.TakeDamage(damge);
        }
    }


}
