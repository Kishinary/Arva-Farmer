using UnityEngine;

public class knightBossOwnDamge : MonoBehaviour
{
    public float damage = 20f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            playerHealth.TakeDamage(damage);
        }
    }
}
