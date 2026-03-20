using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BigGuy : MonoBehaviour
{
    Transform player;
    Rigidbody2D rb;

    [Header("References")]
    public Transform firePoint;
    public GameObject dirtWallPrefab;
    public Animator animator;
    public ParticleSystem explosion;

    [Header("Movement (Heavy Style)")]
    public float moveSpeed = 2f;
    public float preferredDistance = 5f;
    public float retreatDistance = 3f;
    public float acceleration = 50f;

    [Header("Attack Settings")]
    public float attackInterval = 4f;
    public float wallSpawnOffset = 4.0f;
    public float projectileSpeed = 8f;

    // Logic to track the previous wall
    private GameObject lastSpawnedWall;

    float attackTimer;
    bool isAttacking;
    Vector2 moveDir;
    Vector2 snappedDir;

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // Unity 6 specific damping
        rb.linearDamping = 4;
        attackTimer = attackInterval;
    }

    void Update()
    {
        if (player == null || isAttacking) return;

        UpdateAnimator();

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    void FixedUpdate()
    {
        // Smoothly stop if attacking or player is lost
        if (player == null || isAttacking)
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, 0.1f);
            return;
        }

        HandleHeavyMovement();
    }

    void HandleHeavyMovement()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        Vector2 toPlayer = (player.position - transform.position).normalized;

        // Determine direction based on preferred distances
        if (distance < retreatDistance)
        {
            moveDir = -toPlayer;
        }
        else if (distance > preferredDistance)
        {
            moveDir = toPlayer;
        }
        else
        {
            moveDir = Vector2.zero;
        }

        // Apply force-based movement for that "heavy" feel
        rb.AddForce(moveDir * acceleration, ForceMode2D.Force);

        // Cap the max speed
        if (rb.linearVelocity.magnitude > moveSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        attackTimer = attackInterval;

        // 1. Wind up
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.5f);

        // 2. Aim and Spawn
        Vector2 attackDir = (player.position - transform.position).normalized;
        SpawnDirtWall(attackDir);

        // 3. Recovery
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }

    void SpawnDirtWall(Vector2 dir)
    {
        // DELETE THE PREVIOUS WALL IF IT EXISTS
        if (lastSpawnedWall != null)
        {
            Destroy(lastSpawnedWall);
        }

        // Calculate spawn position and rotation
        Vector3 spawnPos = transform.position + (Vector3)(dir * wallSpawnOffset);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // Create the new wall and store it in our reference variable
        lastSpawnedWall = Instantiate(dirtWallPrefab, spawnPos, Quaternion.Euler(0, 0, angle));

        // Feedback effects
        if (explosion != null) Instantiate(explosion, spawnPos, Quaternion.identity);
        CineCamera.instance.TriggerPreset("Watercan");
    }

    void UpdateAnimator()
    {
        Vector2 dir = (player.position - transform.position).normalized;

        // Snapping logic for 4-way or 8-way movement animations
        float snapX = Mathf.Abs(dir.x) > 0.1f ? Mathf.Sign(dir.x) : 0;
        float snapY = Mathf.Abs(dir.y) > 0.1f ? Mathf.Sign(dir.y) : 0;

        snappedDir = new Vector2(snapX, snapY);
        animator.SetFloat("MoveX", snappedDir.x);
        animator.SetFloat("MoveY", snappedDir.y);

        // Tells the animator if the BigGuy is physically moving
        animator.SetBool("IsMoving", rb.linearVelocity.magnitude > 0.1f);
    }
}