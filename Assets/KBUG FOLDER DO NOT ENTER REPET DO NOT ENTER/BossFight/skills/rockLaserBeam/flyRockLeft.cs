using Unity.VisualScripting;
using UnityEngine;

public class flyRockLeft : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float speed = 10f;
    public float lifeTime = 1f; 
    private Rigidbody2D rb;

    public GameObject BlowEffect;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Vector2 moveDirection = Vector2.left;
        rb.linearVelocity = moveDirection * speed;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Instantiate(BlowEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else if (other.name == "Wall")
        {
            Destroy(gameObject);
        }


    }

}
