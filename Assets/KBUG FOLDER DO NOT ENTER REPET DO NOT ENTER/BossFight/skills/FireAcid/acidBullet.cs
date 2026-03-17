using UnityEngine;




public class acidBullet : MonoBehaviour
{
    public float lifetime = 2f; // Thời gian sống tối đa (giây) trước khi tự bốc hơi
    public float damage = 5f;



    public GameObject BlowEffect;


    private void Awake()
    {
        Destroy(gameObject, lifetime);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            playerHealth.TakeDamage(damage);

            Instantiate(BlowEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
        else if (other.CompareTag("Wall") || other.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }

}
