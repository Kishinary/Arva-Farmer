using UnityEngine;
using System.Collections;

public class ChargeEnemy : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float chargeSpeed = 12f;

    public float chargeRange = 5f;
    public float chargeDuration = 0.4f;
    public float chargeCooldown = 2f;

    private Transform player;
    private Rigidbody2D rb;

    private bool isCharging = false;
    private bool canCharge = true;

    [Header("Animation")]
    private Animator animator;
    Vector2 snappedDir = Vector2.down;
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (!isCharging)
        {
            float curSpeed = moveSpeed * GetComponent<EnemyStats>().speedMultiplier;
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                curSpeed * Time.deltaTime
            );

            if (dist <= chargeRange && canCharge)
            {
                StartCoroutine(Charge());
            }
        }

        Vector2 dir = (player.position - transform.position).normalized;

        float snapX = Mathf.Abs(dir.x) > 0.1f ? Mathf.Sign(dir.x) : 0;
        float snapY = Mathf.Abs(dir.y) > 0.1f ? Mathf.Sign(dir.y) : 0;

        snappedDir = new Vector2(snapX, snapY);
        animator.SetFloat("MoveX", snappedDir.x);
        animator.SetFloat("MoveY", snappedDir.y);
    }

    IEnumerator Charge()
    {
        canCharge = false;
        isCharging = true;

        Vector2 dir = (player.position - transform.position).normalized;
        animator.SetFloat("AttackX", dir.x);
        animator.SetFloat("AttackY", dir.y);
        animator.SetTrigger("Attack");

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
        if (collision.transform.CompareTag("Player") && isCharging)
        {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(10);
        }
    }
}