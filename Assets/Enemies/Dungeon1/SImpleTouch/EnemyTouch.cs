using System.Collections;
using UnityEngine;

public class EnemyTouch : MonoBehaviour
{
    private float speed = 4f;
    private Transform playerTransform;

    Vector2 snappedDir = Vector2.down;
    Vector2 moveDir;

    private Animator animator;

    public bool isAttacking;

    float randomTimer;
    Vector2 randomOffset;

    void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        animator = GetComponent<Animator>();

        ChooseRandomOffset();
    }

    void Update()
    {
        if (playerTransform == null) return;

        float dist = Vector2.Distance(transform.position, playerTransform.position);

        if (!isAttacking)
        {
            randomTimer -= Time.deltaTime;

            if (randomTimer <= 0)
            {
                ChooseRandomOffset();
            }

            Vector2 dirToPlayer = (playerTransform.position - transform.position).normalized;

            // Mix chase direction with random offset
            moveDir = (dirToPlayer + randomOffset * 0.6f).normalized;

            transform.position += (Vector3)moveDir * speed * Time.deltaTime;

            if (dist <= 1 && !isAttacking)
            {
                Attack();
            }
        }

        Vector2 dir = moveDir;

        float snapX = Mathf.Abs(dir.x) > 0.1f ? Mathf.Sign(dir.x) : 0;
        float snapY = Mathf.Abs(dir.y) > 0.1f ? Mathf.Sign(dir.y) : 0;

        snappedDir = new Vector2(snapX, snapY);

        animator.SetFloat("MoveX", snappedDir.x);
        animator.SetFloat("MoveY", snappedDir.y);
    }

    void ChooseRandomOffset()
    {
        randomTimer = Random.Range(1f, 2f);

        randomOffset = Random.insideUnitCircle.normalized;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player") && isAttacking)
        {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(10);
        }
    }

    void Attack()
    {
        isAttacking = true;

        Vector2 dir = (playerTransform.position - transform.position).normalized;

        animator.SetFloat("AttackX", dir.x);
        animator.SetFloat("AttackY", dir.y);

        animator.SetTrigger("Attack");
    }

    void OnEndAttack()
    {
        isAttacking = false;
    }
}