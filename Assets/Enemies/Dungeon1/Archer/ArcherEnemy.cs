using System.Collections;
using UnityEngine;

public class BirdEnemy : MonoBehaviour
{
    Transform player;

    [Header("References")]
    public Transform shadow;
    public Transform firePoint;
    public GameObject projectilePrefab;
    public Animator animator;
    public GameObject orbiter;

    [Header("Movement")]
    public float orbitSpeed = 20f;

    [Header("Attack")]
    public float attackInterval = 5f;
    public float projectileSpeed = 8f;

    float attackTimer;
    bool isAttacking;

    Vector2 moveDir;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player").transform;

        attackTimer = attackInterval;
    }

    void Update()
    {
        if (player == null) return;

        attackTimer -= Time.deltaTime;

        if (!isAttacking)
        {
            Orbiter();
        }

        if (attackTimer <= 0 && !isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }

        UpdateAnimator();
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;

        moveDir = Vector2.zero;

        Vector2 attackDir = (player.position - transform.position).normalized;

        animator.SetFloat("AttackX", attackDir.x);
        animator.SetFloat("AttackY", attackDir.y);

        yield return new WaitForSeconds(0.5f);

        animator.SetTrigger("Attack");

        ShootProjectile(attackDir);

        attackTimer = attackInterval;

        yield return new WaitForSeconds(0.2f);

        isAttacking = false;
    }

    void ShootProjectile(Vector2 dir)
    {
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        proj.GetComponent<Rigidbody2D>().linearVelocity = dir * projectileSpeed;
    }

    void UpdateAnimator()
    {
        animator.SetFloat("MoveX", moveDir.x);
        animator.SetFloat("MoveY", moveDir.y);
    }

    void Orbiter()
    {
        shadow.transform.position = Vector2.MoveTowards(shadow.transform.position, orbiter.transform.position, orbitSpeed * Time.deltaTime);
    }
}