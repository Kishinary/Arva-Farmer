using UnityEngine;
using System.Collections;

public class EnemySummoner : MonoBehaviour
{
    private Transform player;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float keepDistance = 5f;

    [Header("Summoning")]
    public GameObject minionPrefab;
    public int summonCount = 2;
    public float summonCooldown = 4f;
    public float castTime = 1.5f;

    public float summonRadius = 2f;

    private bool isCasting = false;
    private Rigidbody2D rb;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();

        StartCoroutine(SummonLoop());
    }

    void Update()
    {
        if (isCasting || player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        Vector2 dir = (player.position - transform.position).normalized;

        if (dist < keepDistance)
        {
            // move away from player
            rb.linearVelocity = -dir * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    IEnumerator SummonLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(summonCooldown);

            if (!isCasting)
                StartCoroutine(Summon());
        }
    }

    IEnumerator Summon()
    {
        isCasting = true;

        rb.linearVelocity = Vector2.zero;

        // casting delay
        yield return new WaitForSeconds(castTime);

        for (int i = 0; i < summonCount; i++)
        {
            Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * summonRadius;

            Instantiate(minionPrefab, spawnPos, Quaternion.identity);
        }

        isCasting = false;
    }
}
