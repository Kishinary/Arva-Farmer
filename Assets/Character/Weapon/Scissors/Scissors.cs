using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Scissors : MonoBehaviour, IWeapon
{
    public float nextAttackTime;
    public float attackCooldown = 4f;
    private Animator animator;

    [Header("Components")]
    private PlayerMovement PlayerMove;

    [Header("Thrust Settings")]
    public float thrustDistance = 2f;
    public float attackDuration = 0.15f;
    public float returnDuration = 0.1f;

    [Header("Normal Attack")]
    private float nextNormalAttackTime;
    private float NormalattackCooldown = 0.25f;

    [Header("Combat")]
    public float damage = 10f;
    public LayerMask enemyLayer;
    public float hitRadius = 1f;

    private Vector3 originalLocalPos;
    private HashSet<GameObject> alreadyHit = new HashSet<GameObject>();

    [Header("Effect")]
    private LineRenderer line;

    public string GetNormalShake() => "Scissors";
    public string GetSpecialShake() => "Scissors";

    void Start()
    {
        animator = GetComponent<Animator>();
        PlayerMove = GetComponentInParent<PlayerMovement>();
        originalLocalPos = transform.localPosition;
        line = GetComponentInChildren<LineRenderer>();
    }

    public float GetFinalDamage() => damage * PlayerMove.Damagepercentage;

    public bool SpecialAttack()
    {
        if (Time.time >= nextAttackTime)
        {
            animator.SetTrigger("Thrust");
            nextAttackTime = Time.time + attackCooldown;
            // 1f damage, 1f distance (Full Lunge)
            StartCoroutine(ThrustRoutine(1f, 1f));
            return true;
        }
        return false;
    }

    public bool NormalAttack()
    {
        if (Time.time >= nextNormalAttackTime)
        {
            animator.SetTrigger("Thrust");
            nextNormalAttackTime = Time.time + NormalattackCooldown;
            // 2f damage, 0.15f distance (Short Poke)
            StartCoroutine(ThrustRoutine(2f, 0.15f));
            return true;
        }
        return false;
    }

    IEnumerator ThrustRoutine(float damageMult, float distMult)
    {
        alreadyHit.Clear();
        if (line != null) line.enabled = true;

        float targetDist = thrustDistance * distMult;
        float timer = 0f;

        // --- FORWARD PHASE ---
        while (timer < attackDuration)
        {
            timer += Time.deltaTime;
            float percent = timer / attackDuration;

            // Apply the distance multiplier here
            transform.localPosition = originalLocalPos + Vector3.right * (targetDist * percent);

            CheckForHits(damageMult);
            yield return null;
        }

        // --- RETURN PHASE ---
        timer = 0f;
        while (timer < returnDuration)
        {
            timer += Time.deltaTime;
            float percent = timer / returnDuration;

            // Smoothly return from the actual distance reached
            transform.localPosition = Vector3.Lerp(originalLocalPos + Vector3.right * targetDist, originalLocalPos, percent);
            yield return null;
        }

        transform.localPosition = originalLocalPos;
        if (line != null) line.enabled = false;
    }

    private void CheckForHits(float multiplier)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, hitRadius, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            if (!alreadyHit.Contains(hit.gameObject))
            {
                EnemyStats stats = hit.GetComponent<EnemyStats>();
                if (stats != null)
                {
                    stats.TakeDamage(transform.position, GetFinalDamage() * multiplier);
                    alreadyHit.Add(hit.gameObject);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hitRadius);
    }
}