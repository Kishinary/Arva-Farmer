using System.Collections;
using UnityEngine;

public class CreepShoot : MonoBehaviour
{
    Transform player;

    [Header("References")]
    public Transform firePoint;
    public GameObject projectilePrefab;
    public Animator animator;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float preferredDistance = 4f;
    public float strafeChangeTime = 5f;
    public float retreatDistance = 2.5f;

    [Header("Attack")]
    public float attackInterval = 4f;
    public float projectileSpeed = 8f;

    float attackTimer;
    float strafeTimer;

    bool isAttacking;

    Vector2 moveDir;
    Vector2 strafeDir;

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        animator = GetComponent<Animator>();
        attackTimer = attackInterval;
        strafeTimer = strafeChangeTime;
        PickNewStrafeDirection();
    }

    void Update()
    {
        if (player == null) return;

        attackTimer -= Time.deltaTime;

        if (!isAttacking)
        {
            HandleMovement();
        }

        if (attackTimer <= 0 && !isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }

        UpdateAnimator();
    }

    void HandleMovement()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        Vector2 toPlayer = (player.position - transform.position).normalized;

        strafeTimer -= Time.deltaTime;

        if (strafeTimer <= 0)
        {
            PickNewStrafeDirection();
            strafeTimer = strafeChangeTime;
        }

        if (distance < retreatDistance)
        {
            // Too close → retreat
            moveDir = -toPlayer;
        }
        else if (distance > preferredDistance)
        {
            // Too far → approach
            moveDir = toPlayer;
        }
        else
        {
            // Good range → strafe
            moveDir = strafeDir;
        }
        float currentSpeed = moveSpeed * GetComponent<EnemyStats>().speedMultiplier;
        transform.position += (Vector3)(moveDir * currentSpeed * Time.deltaTime);
    }

    IEnumerator AttackRoutine()
    {
        animator.SetTrigger("Attack");
        isAttacking = true;
        // Stop movement
        moveDir = Vector2.zero;

        // 0.3 second pause before attack
        yield return new WaitForSeconds(0.3f);

        Vector2 attackDir = (player.position - transform.position).normalized;

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

    void PickNewStrafeDirection()
    {
        // Perpendicular to player direction
        Vector2 toPlayer = (player.position - transform.position).normalized;

        // Random left or right strafe
        if (Random.value > 0.5f)
            strafeDir = new Vector2(-toPlayer.y, toPlayer.x);
        else
            strafeDir = new Vector2(toPlayer.y, -toPlayer.x);
    }

    void UpdateAnimator()
    {
        animator.SetFloat("MoveX", moveDir.x);
        animator.SetFloat("MoveY", moveDir.y);
    }
}