using UnityEngine;

public class dartFollowing : MonoBehaviour
{

    public float speed = 20.0f;
    public float lifeTime = 5f;
    public float damage = 10f;


    //1/4 castTime

    private Rigidbody2D rb;
    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    public void Start()
    {

        rb.linearVelocity = transform.right * speed;
        Destroy(gameObject, lifeTime);
    }



    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerHealth>(out PlayerHealth hitPlayer))
        {
            hitPlayer.TakeDamage(damage);
            Destroy(gameObject);
        }else if (collision.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }



}
