using UnityEngine;

public class ArcherEnemy : MonoBehaviour
{
    private Transform player;

    [Header("Combat")]
    public float attackRange = 6f;
    public float fireRate = 1.5f;
    float fireCooldown;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float retreatSpeed = 3f;
    public float retreatTime = 1.5f;

    float retreatTimer;
    bool isRetreating = false;

    [Header("Projectile")]
    public GameObject arrowPrefab;
    public Transform firePoint;
    public float arrowSpeed = 10f;

    int shotsFired = 0;
    int shotsBeforeRetreat;

    void Start()
    {
        ChooseShotsBeforeRetreat();
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        FacePlayer();

        float distance = Vector2.Distance(transform.position, player.position);

        if (isRetreating)
        {
            Retreat();
            return;
        }

        if (distance > attackRange)
        {
            MoveTowardsPlayer();
        }
        else
        {
            Attack();
        }
    }

    void MoveTowardsPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)dir * moveSpeed * Time.deltaTime;
    }

    void Attack()
    {
        fireCooldown -= Time.deltaTime;

        if (fireCooldown <= 0)
        {
            ShootArrow();
            fireCooldown = fireRate;

            shotsFired++;

            if (shotsFired >= shotsBeforeRetreat)
            {
                StartRetreat();
            }
        }
    }

    void ShootArrow()
    {
        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);

        Vector2 dir = (player.position - firePoint.position).normalized;

        arrow.GetComponent<Rigidbody2D>().linearVelocity = dir * arrowSpeed;
    }

    void StartRetreat()
    {
        isRetreating = true;
        retreatTimer = retreatTime;
        shotsFired = 0;
        ChooseShotsBeforeRetreat();
    }

    void Retreat()
    {
        retreatTimer -= Time.deltaTime;

        Vector2 dir = (transform.position - player.position).normalized;
        transform.position += (Vector3)dir * retreatSpeed * Time.deltaTime;

        if (retreatTimer <= 0)
        {
            isRetreating = false;
        }
    }

    void ChooseShotsBeforeRetreat()
    {
        shotsBeforeRetreat = Random.Range(1, 3); // 1 or 2 shots
    }

    void FacePlayer()
    {
        if (player.position.x > transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);
    }
}