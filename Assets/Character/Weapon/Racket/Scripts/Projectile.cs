using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Properties")]
    public float speed = 100f;
    public float lifeTime = 0.3f;
    public float damage = 15f;

    [Header("Other Stuff")]
    public string bug;
    public GameObject explosion;
    public LayerMask enemy;
    void Start()
    {
        Destroy(gameObject, lifeTime);

        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0f;

        Vector3 dir = mouse - transform.position;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Update()
    {
        transform.position += transform.right * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D hit)
    {
        if (hit.CompareTag("Enemy"))
        {
            if (bug == "Bettle")
            {
                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, 3, enemy);
                foreach (Collider2D enemy in hitEnemies)
                {
                    enemy.GetComponent<EnemyStats>().TakeDamage(transform.position, 30);
                }
                Instantiate(explosion, transform.position, Quaternion.identity);
                Destroy(this.gameObject);
            }
            else if (bug == "Butterfly")
            {
                EnemyStats enemy = hit.GetComponent<EnemyStats>();

                if (enemy != null)
                    enemy.TakeDamage(transform.position, damage);
                Destroy(this.gameObject);
            }
            else
            {
                EnemyStats enemy = hit.GetComponent<EnemyStats>();

                if (enemy != null)
                    enemy.TakeDamage(transform.position, damage + 10);
            }

        }

    }
}
