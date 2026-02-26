using UnityEngine;

public class Slash : MonoBehaviour
{
    public float speed = 12f;
    public float lifeTime = 0.3f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += transform.right * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D hit)
    {
        if (hit.CompareTag("Enemy"))
        {
            EnemyStats enemy = hit.GetComponent<EnemyStats>();

            if (enemy != null)
                enemy.TakeDamage(transform.position, 10);
        }
    }
}
