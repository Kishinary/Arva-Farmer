using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public float lifetime = 2f; // Thời gian sống tối đa (giây) trước khi tự bốc hơi
    public int damage = 10;

    private void Awake()
    {
        Destroy(gameObject, lifetime);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
        else if (other.CompareTag("Wall") || other.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }

}
