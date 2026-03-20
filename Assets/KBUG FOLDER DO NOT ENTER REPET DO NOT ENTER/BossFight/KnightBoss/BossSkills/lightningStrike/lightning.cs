using UnityEngine;

public class lightningAttack : MonoBehaviour
{
    public float damage = 15f;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            playerHealth.TakeDamage(damage);

            
        }
    }


}
