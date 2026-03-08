using UnityEngine;
using System.Collections;

public class EnemySummoner : MonoBehaviour
{
    private Transform player;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float keepDistance = 5f;

    [Header("Floating")]
    public float bobAmplitude = 0.2f;
    public float bobFrequency = 2f;

    [Header("Summoning")]
    public GameObject minionPrefab;
    public int summonCount = 2;
    public float summonCooldown = 4f;
    public float castTime = 1.5f;
    public float summonRadius = 2f;

    [Header("References")]
    public Animator animator;

    private bool isCasting = false;
    private Rigidbody2D rb;

    private Vector2 basePosition;
    private Vector2 moveDir;

    [Header("Particles")]
    public ParticleSystem summonParticle;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        basePosition = transform.position;

        StartCoroutine(SummonLoop());
    }

    void Update()
    {
        if (player == null) return;

        HandleMovement();
        HandleBobbing();
        UpdateAnimator();
    }

    void HandleMovement()
    {
        if (isCasting)
        {
            rb.linearVelocity = Vector2.zero;
            moveDir = Vector2.zero;
            return;
        }

        float dist = Vector2.Distance(transform.position, player.position);
        Vector2 dir = (player.position - transform.position).normalized;

        if (dist < keepDistance)
        {
            moveDir = -dir;
            rb.linearVelocity = moveDir * moveSpeed;
        }
        else
        {
            moveDir = Vector2.zero;
            rb.linearVelocity = Vector2.zero;
        }
    }

    void HandleBobbing()
    {
        float bobOffset = Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        transform.position = new Vector2(transform.position.x, basePosition.y + bobOffset);
        basePosition = new Vector2(transform.position.x, basePosition.y);
    }

    void UpdateAnimator()
    {
        animator.SetFloat("MoveX", moveDir.x);
        animator.SetFloat("MoveY", moveDir.y);
    }

    IEnumerator SummonLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(summonCooldown);

            if (this == null) yield break;

            if (!isCasting)
                StartCoroutine(Summon());
        }
    }

    IEnumerator Summon()
    {
        isCasting = true;

        rb.linearVelocity = Vector2.zero;
        moveDir = Vector2.zero;

        yield return new WaitForSeconds(castTime);

        for (int i = 0; i < summonCount; i++)
        {
            Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * summonRadius;
            Instantiate(minionPrefab, spawnPos, Quaternion.identity);
            Instantiate(summonParticle, spawnPos, Quaternion.identity);
        }

        isCasting = false;
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }
}