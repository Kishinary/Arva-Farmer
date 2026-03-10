using System.Collections;
using UnityEngine;

public class Shovel : MonoBehaviour
{
    private Animator animator;

    [Header("Attack Settings")]
    public LayerMask enemyLayer;
    public Transform attackPoint;
    public float normalAttackRadius = 0.5f;
    public float specialAttackRadius = 2.5f;
    [Range(0, 360)] public float specialAttackAngle = 90f;

    [Header("Cooldowns")]
    public float attackCooldown = 1f;
    public float normalAttackCooldown = 0.5f;
    private float nextAttackTime = 0f;
    private float nextNormalAttackTime = 0f;

    [Header("Damage")]
    public int normalAttackDamage = 10;
    public int specialAttackDamage = 25;

    [Header("Particles")]
    public ParticleSystem dirtParticle;
    public ParticleSystem SmashParticle;

    private WeaponParent weaponParent;
    private PlayerMovement playerMove;

    void Start()
    {
        animator = GetComponent<Animator>();
        weaponParent = GetComponentInParent<WeaponParent>();
        playerMove = GetComponentInParent<PlayerMovement>();
    }

    public void NormalAttack()
    {
        if (Time.time >= nextNormalAttackTime)
        {
            animator.SetTrigger("Smash");
            playerMove.movespeed = 3f;
            nextNormalAttackTime = Time.time + normalAttackCooldown;
        }
    }

    public void Attack() // Special Attack
    {
        if (Time.time >= nextAttackTime)
        {
            animator.SetTrigger("Dig");
            nextAttackTime = Time.time + attackCooldown;
        }
    }


    public void ApplyParticleForNormal()
    {
        Quaternion particleRotation = weaponParent.transform.rotation;
        if (weaponParent.transform.localScale.y == -1)
        {
            particleRotation *= Quaternion.Euler(SmashParticle.transform.eulerAngles.x * -1, 0, 0);
        }

        Instantiate(SmashParticle, transform.position, particleRotation);
        PerformNormalHitCheck();
        playerMove.movespeed = 6f;

    }

    public void ApplyParticle() 
    {
        Quaternion particleRotation = weaponParent.transform.rotation;
        if (weaponParent.transform.localScale.y == -1)
        {
            particleRotation *= Quaternion.Euler(dirtParticle.transform.eulerAngles.x * -1 - 10f, 0, 0);
        }

        Instantiate(dirtParticle, transform.position, weaponParent.transform.rotation);
        PerformConeHitCheck(); 
    }

    private void PerformNormalHitCheck()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, normalAttackRadius, enemyLayer);
        foreach (Collider2D enemy in hitEnemies)
        {
            ApplyDamage(enemy, normalAttackDamage, "ShovelNormal");
        }
    }

    private void PerformConeHitCheck()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, specialAttackRadius, enemyLayer);
        foreach (Collider2D enemy in enemiesInRange)
        {
            Vector2 dirToEnemy = (enemy.transform.position - transform.position).normalized;
            float angleToEnemy = Vector2.Angle(transform.right, dirToEnemy);

            if (angleToEnemy < specialAttackAngle / 2f)
            {
                ApplyDamage(enemy, specialAttackDamage, "ShovelSpecial");
            }
        }
    }

    private void ApplyDamage(Collider2D enemy, int damage, string shakePreset)
    {
        enemy.GetComponent<EnemyStats>().TakeDamage(playerMove.transform.position, damage);
        CineCamera.instance.TriggerPreset(shakePreset);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, normalAttackRadius);

        Gizmos.color = Color.blue;
        DrawWireCone(transform.position, transform.right, specialAttackAngle, specialAttackRadius);
    }

    private void DrawWireCone(Vector3 origin, Vector3 direction, float angle, float range)
    {
        Vector3 leftRayRotation = Quaternion.AngleAxis(-angle / 2, Vector3.forward) * direction;
        Vector3 rightRayRotation = Quaternion.AngleAxis(angle / 2, Vector3.forward) * direction;
        Gizmos.DrawRay(origin, leftRayRotation * range);
        Gizmos.DrawRay(origin, rightRayRotation * range);
    }
}