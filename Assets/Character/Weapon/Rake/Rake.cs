using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rake : MonoBehaviour, IWeapon
{
    private Animator animator;

    [Header("Trap Settings")]
    public LayerMask bugLayer;
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


    [Header("Cooldowns")]
    public float catchCooldown = 2f;
    public float TrapCooldown = 2f;

    private float nextCatchTime;
    private float nextReleaseTime;


    [Header("Damage")]

    [Header("Particles")]
    public ParticleSystem TrapParticle;
    public ParticleSystem releaseParticle;

    private WeaponParent weaponParent;
    private PlayerMovement playerMove;
    private bool isPulling = false;
    public string GetNormalShake() => "BugRacket";
    public string GetSpecialShake() => "BugRacket";

    void Start()
    {
        animator = GetComponent<Animator>();
        weaponParent = GetComponentInParent<WeaponParent>();
        playerMove = GetComponentInParent<PlayerMovement>();
    }

    public void NormalAttack() 
    {
        if (Time.time >= nextCatchTime)
        {
            StartCoroutine(ComicalRakeAttack());
            nextCatchTime = Time.time + catchCooldown;
        }
    }

    public void SpecialAttack()
    {
        if (Time.time >= nextReleaseTime)
        {
            animator.SetTrigger("Release");
            nextReleaseTime = Time.time + TrapCooldown;
        }
    }

    public void ApplyTrap()
    {
        Quaternion particleRotation = weaponParent.transform.rotation;

        if (weaponParent.transform.localScale.y == -1)
        {
            particleRotation *= Quaternion.Euler(TrapParticle.transform.eulerAngles.x * -1, 0, 0);
        }

        Instantiate(TrapParticle, transform.position, particleRotation);

        ReleaseTrap();
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
        float k = Random.Range(0, 8);
        if (k <= 2)
        {
            Instantiate(BearTrap, TrapPoint.position, Quaternion.identity);
        }
        else if (k <= 5)
        {
            Instantiate(RakeTrap, TrapPoint.position, Quaternion.identity);
        }
        else
        {
            Instantiate(Holetrap, TrapPoint.position, Quaternion.identity);
        }
    }

    public void Pull()
    {

    }

    IEnumerator ComicalRakeAttack()
    {
        // 1. Slam Down (Instant Massive Scale)
        transform.localScale = new Vector3(4f, 4f, 1f);

        yield return new WaitForSeconds(0.1f);

        isPulling = true; 

        float duration = 0.4f;
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
        // Only pull if the rake is in its 'retracting' state
        if (isPulling && collision.CompareTag("Enemy"))
        {
            // Move enemy toward the player
            collision.transform.position = Vector2.MoveTowards(
                collision.transform.position,
                playerMove.transform.position,
                pullSpeed * Time.deltaTime
            );
        }
    }


    IEnumerator PullEnemy(Transform enemyTransform)
    {
        float pullDuration = 0.3f;
        float elapsed = 0f;

        while (elapsed < pullDuration)
        {
            if (enemyTransform == null) yield break; // In case enemy dies mid-pull

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