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
    Vector2 snappedDir;

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
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        proj.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        proj.GetComponent<Rigidbody2D>().linearVelocity = dir * projectileSpeed;
    }

    void UpdateAnimator()
    {
        moveDir = (player.position - transform.position).normalized;

        Vector2 dir = moveDir;

        float snapX = Mathf.Abs(dir.x) > 0.1f ? Mathf.Sign(dir.x) : 0;
        float snapY = Mathf.Abs(dir.y) > 0.1f ? Mathf.Sign(dir.y) : 0;

        snappedDir = new Vector2(snapX, snapY);

        animator.SetFloat("MoveX", snappedDir.x);
        animator.SetFloat("MoveY", snappedDir.y);

    }

    void Orbiter()
    {
        shadow.transform.position = Vector2.MoveTowards(shadow.transform.position, orbiter.transform.position, orbitSpeed * Time.deltaTime);
        transform.position = Vector2.MoveTowards(transform.position, shadow.transform.position + new Vector3(0, 1.1f, 0), orbitSpeed * Time.deltaTime); 
    }
}