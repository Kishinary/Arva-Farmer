using UnityEngine;
using System.Collections;

public class MageEnemy : MonoBehaviour
{
    public GameObject windHolePrefab;

    public float moveSpeed = 2f;
    public float strafeSpeed = 2f;

    public float approachDistance = 7f;
    public float retreatDistance = 3f;

    public float castDelay = 2f;
    public float attackCooldown = 3f;

    public float teleportDistance = 5f;

    private Transform player;
    private Rigidbody2D rb;

    private bool isCasting = false;

    private int hitCount = 0;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();

        StartCoroutine(AttackLoop());
    }

    void Update()
    {
        if (player == null || isCasting) return;

        float dist = Vector2.Distance(transform.position, player.position);

        Vector2 dirToPlayer = (player.position - transform.position).normalized;

        if (dist > approachDistance)
        {
            // Move closer
            rb.linearVelocity = dirToPlayer * moveSpeed;
        }
        else if (dist < retreatDistance)
        {
            // Run away
            rb.linearVelocity = -dirToPlayer * moveSpeed;
        }
        else
        {
            // Strafe around player
            Vector2 strafeDir = Vector2.Perpendicular(dirToPlayer);
            rb.linearVelocity = strafeDir * strafeSpeed;
        }
    }

    IEnumerator AttackLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackCooldown);

            if (!isCasting)
            {
                StartCoroutine(CastWindHole());
            }
        }
    }

    IEnumerator CastWindHole()
    {
        isCasting = true;

        rb.linearVelocity = Vector2.zero;

        Vector3 targetPos = player.position;

        GameObject hole = Instantiate(windHolePrefab, targetPos, Quaternion.identity);

        yield return new WaitForSeconds(castDelay);

        hole.GetComponent<WindHole>().Activate();

        isCasting = false;
    }

    public void TakeDamage(int dmg)
    {
        hitCount++;

        if (hitCount >= 2)
        {
            Teleport();
            hitCount = 0;
        }
    }

    void Teleport()
    {
        Vector2 dir = (transform.position - player.position).normalized;
        transform.position += (Vector3)(dir * teleportDistance);
    }
}