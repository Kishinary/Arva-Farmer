using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rake : MonoBehaviour, IWeapon
{
    private Animator animator;

    [Header("Trap Settings")]
    public Transform TrapPoint;
    public float catchRadius = 1.2f;

    [Header("Release Settings")]
    public float releaseRadius = 3f;
    public float pullSpeed = 4f;

    [Header("Storage")]
    public int maxtraps = 5;
    public GameObject BearTrap;
    public GameObject RakeTrap;
    public GameObject Holetrap;
    public GameObject SpikeTrap;


    [Header("Cooldowns")]
    public float catchCooldown = 1.5f;
    public float TrapCooldown = 1.3f;

    private float nextCatchTime;
    private float nextReleaseTime;


    [Header("Damage")]

    [Header("Particles")]
    public ParticleSystem TrapParticle;
    public ParticleSystem releaseParticle;

    private WeaponParent weaponParent;
    private PlayerMovement playerMove;
    public bool isPulling = false;
    public string GetNormalShake() => "BugRacket";
    public string GetSpecialShake() => "BugRacket";

    void Start()
    {
        animator = GetComponent<Animator>();
        weaponParent = GetComponentInParent<WeaponParent>();
        playerMove = GetComponentInParent<PlayerMovement>();
    }

    private Coroutine attackRoutine;
    public bool NormalAttack()
    {
        if (Time.time >= nextCatchTime)
        {
            if (attackRoutine != null) StopCoroutine(attackRoutine);
            animator.SetTrigger("Swing");
            attackRoutine = StartCoroutine(ComicalRakeAttack());
            nextCatchTime = Time.time + catchCooldown;
            return true;
        }
        return false;

    }

    public bool SpecialAttack()
    {
        if (Time.time >= nextReleaseTime)
        {
            animator.SetTrigger("Release");
            nextReleaseTime = Time.time + TrapCooldown;
            return true;
        }
        return false;
    }

    public void ApplyRelease()
    {
        Quaternion particleRotation = weaponParent.transform.rotation;

        if (weaponParent.transform.localScale.y == -1)
        {
            particleRotation *= Quaternion.Euler(releaseParticle.transform.eulerAngles.x * -1, 0, 0);
        }

        Instantiate(releaseParticle, transform.position, particleRotation);

        ReleaseTrap();
    }


    public void ReleaseTrap()
    {
        float k = Random.Range(0, 4);
        if (k == 0)
        {
            Instantiate(BearTrap, TrapPoint.position, Quaternion.identity);
        }
        else if (k == 1)
        {
            Instantiate(RakeTrap, TrapPoint.position, Quaternion.identity);
        }
        else if (k == 2)
        {
            Instantiate(Holetrap, TrapPoint.position, Quaternion.identity);
        }
        else
        {
            Instantiate(SpikeTrap, TrapPoint.position, Quaternion.identity);
        }
    }

    IEnumerator ComicalRakeAttack()
    {
        // 1. Slam Down (Instant Massive Scale)
        transform.localScale = new Vector3(4f, 4f, 1f);

        yield return new WaitForSeconds(0.1f);

        isPulling = true; 

        float duration = 0.5f;
        float elapsed = 0f;
        Vector3 massiveScale = transform.localScale;
        Vector3 tinyScale = new Vector3(0.5f, 0.5f, 1f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(massiveScale, tinyScale, elapsed / duration);
            yield return null;
        }

        isPulling = false;
        transform.localScale = Vector3.one;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isPulling && collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyStats>().TakeDamage(transform.position, 4);
            Rigidbody2D enemyRb = collision.GetComponent<Rigidbody2D>();
            var PlayerPull = (collision.transform.position - playerMove.transform.position).normalized * 2;
            Vector2 knockbackDir = -PlayerPull;
            enemyRb.linearVelocity = Vector2.zero;
            enemyRb.AddForce(knockbackDir * 2.5f, ForceMode2D.Impulse);
        }
    }


    IEnumerator PullEnemy(Transform enemyTransform)
    {
        float pullDuration = 0.3f;
        float elapsed = 0f;

        while (elapsed < pullDuration)
        {
            if (enemyTransform == null) yield break; 

            elapsed += Time.deltaTime;
            // Move the enemy toward the player
            enemyTransform.position = Vector2.MoveTowards(
                enemyTransform.position,
                playerMove.gameObject.transform.position,
                pullSpeed * Time.deltaTime
            );
            yield return null;
        }
    }
}