using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rake : MonoBehaviour, IWeapon
{
    private Animator animator;

    [Header("Rectangle Hitbox Settings")]
    public float hitBoxLength = 3f;   // How far the rake reaches (X axis)
    public float hitBoxWidth = 1.2f;  // How wide the rake is (Y axis)
    public float hitOffset = 1.5f;    // Distance from player to box center
    public LayerMask enemyLayer;

    [Header("Storage & Traps")]
    public GameObject BearTrap;
    public GameObject RakeTrap;
    public GameObject Holetrap;
    public GameObject SpikeTrap;

    [Header("Cooldowns")]
    public float catchCooldown = 1.5f;
    public float TrapCooldown = 1.3f;

    private float nextCatchTime;
    private float nextReleaseTime;
    float damage = 10f;

    private WeaponParent weaponParent;
    private PlayerMovement playerMove;
    private bool isPulling = false;

    public Transform TrapPoint;
    public ParticleSystem releaseParticle;


    // Prevents double-hitting the same enemy during one pull
    private HashSet<GameObject> alreadyHit = new HashSet<GameObject>();

    public string GetNormalShake() => "BugRacket";
    public string GetSpecialShake() => "BugRacket";

    void Start()
    {
        animator = GetComponent<Animator>();
        weaponParent = GetComponentInParent<WeaponParent>();
        playerMove = GetComponentInParent<PlayerMovement>();
    }

    public float GetFinalDamage() => damage * playerMove.Damagepercentage;

    public bool NormalAttack()
    {
        if (Time.time >= nextCatchTime)
        {
            animator.SetTrigger("Swing");
            StartCoroutine(ComicalRakeAttack());
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

    IEnumerator ComicalRakeAttack()
    {
        alreadyHit.Clear();
        isPulling = true;

        // Visual "Slam" effect from your original code
        transform.localScale = new Vector3(4f, 4f, 1f);
        yield return new WaitForSeconds(0.1f);

        float duration = 0.5f;
        float elapsed = 0f;
        Vector3 massiveScale = transform.localScale;
        Vector3 tinyScale = new Vector3(0.5f, 0.5f, 1f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(massiveScale, tinyScale, elapsed / duration);

            // CUSTOM RECTANGLE CHECK
            CheckRectangleHitbox();

            yield return null;
        }

        isPulling = false;
        transform.localScale = Vector3.one;
    }

    private void CheckRectangleHitbox()
    {
        // Calculate the center of the box in front of the weapon
        Vector2 center = transform.position + transform.right * hitOffset + new Vector3(0, 1f, 0) ;
        Vector2 size = new Vector2(hitBoxLength, hitBoxWidth);
        float angle = transform.eulerAngles.z;

        // Find all enemies in the rectangle
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, angle, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy") && !alreadyHit.Contains(hit.gameObject))
            {
                // Deal Damage
                hit.GetComponent<EnemyStats>().TakeDamage(transform.position, GetFinalDamage() * 0.5f);

                // Pulling Logic
                Rigidbody2D enemyRb = hit.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    Vector2 pullDir = (playerMove.transform.position - hit.transform.position).normalized;
                    enemyRb.linearVelocity = Vector2.zero;
                    enemyRb.AddForce(pullDir * 5f, ForceMode2D.Impulse);
                }

                alreadyHit.Add(hit.gameObject); // mark as hit
            }
        }
    }

    // Visualize the rectangle in the Editor so you can customize it easily
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(
            (Vector2)transform.position + (Vector2)transform.right * hitOffset + new Vector2(0,1f),
            transform.rotation,
            new Vector3(hitBoxLength, hitBoxWidth, 1));

        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
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
}