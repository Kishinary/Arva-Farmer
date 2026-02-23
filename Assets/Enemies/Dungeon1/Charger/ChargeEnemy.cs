using UnityEngine;
using System.Collections;

public class ChargeEnemy : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float chargeSpeed = 10f;

    public float chargeRange = 4f;
    public float chargeDuration = 0.4f;
    public float chargeCooldown = 2f;

    private Transform player;
    private Rigidbody2D rb;

    private bool isCharging = false;
    private bool canCharge = true;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (!isCharging)
        {
            // Move toward player
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                moveSpeed * Time.deltaTime
            );

            if (dist <= chargeRange && canCharge)
            {
                StartCoroutine(Charge());
            }
        }
    }

    IEnumerator Charge()
    {
        canCharge = false;
        isCharging = true;

        Vector2 dir = (player.position - transform.position).normalized;

        float timer = 0;

        while (timer < chargeDuration)
        {
            rb.linearVelocity = dir * chargeSpeed;
            timer += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        isCharging = false;

        yield return new WaitForSeconds(chargeCooldown);
        canCharge = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            collision.gameObject
                .GetComponent<PlayerHealth>()
                .TakeDamage(10);
        }
    }
}