using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Required for HashSet

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
    public float hitRadius = 1f; // Updated to 1 as requested

    private Vector3 originalLocalPos;

    // This list tracks who we already hit during the current thrust
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
            StartCoroutine(ThrustRoutine(1f)); // Damage multiplier 1x
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
            StartCoroutine(ThrustRoutine(2f)); // Damage multiplier 2x
            return true;
        }
        return false;
    }

    // Unified Coroutine for both attacks
    IEnumerator ThrustRoutine(float damageMult)
    {
        alreadyHit.Clear(); // Reset hit list at start of attack
        if (line != null) line.enabled = true;

        float timer = 0f;
        while (timer < attackDuration)
        {
            timer += Time.deltaTime;
            float percent = timer / attackDuration;

            // Move Scissors forward
            transform.localPosition = originalLocalPos + Vector3.right * (thrustDistance * percent);

            // HITBOX CHECK: Scan for enemies around the scissors every frame
            CheckForHits(damageMult);

            yield return null;
        }

        // Return logic
        timer = 0f;
        while (timer < returnDuration)
        {
            timer += Time.deltaTime;
            float percent = timer / returnDuration;
            transform.localPosition = Vector3.Lerp(originalLocalPos + Vector3.right * thrustDistance, originalLocalPos, percent);
            yield return null;
        }

        transform.localPosition = originalLocalPos;
        if (line != null) line.enabled = false;
    }

    private void CheckForHits(float multiplier)
    {
        // Snapshot of everything within 1 unit of the scissors
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, hitRadius, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            // Only damage if we haven't hit this specific enemy in this thrust yet
            if (!alreadyHit.Contains(hit.gameObject))
            {
                EnemyStats stats = hit.GetComponent<EnemyStats>();
                if (stats != null)
                {
                    stats.TakeDamage(transform.position, GetFinalDamage() * multiplier);
                    alreadyHit.Add(hit.gameObject); // Mark as hit
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