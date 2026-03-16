using UnityEngine;
using System.Collections;

public class Scissors : MonoBehaviour, IWeapon
{
    public float nextAttackTime;
    public float attackCooldown = 4f;
    private Animator animator;

    public bool isNormaling;

    [Header("Components")]
    private PlayerMovement PlayerMove;

    [Header("Thrust Settings")]
    public float thrustDistance = 2f;     
    public float attackDuration = 0.15f;    
    public float returnDuration = 0.1f;    
    public AnimationCurve thrustCurve;    


    [Header("Normal Attack")]
    private float nextNormalAttackTime;
    private float NormalattackCooldown = 0.25f;

    [Header("Combat")]
    public float damage = 5f;
    public LayerMask enemyLayer;
    public Transform hitPoint;
    public float hitRadius = 0.5f;

    private Vector3 originalLocalPos;
    private bool isAttacking = false;

    public string GetNormalShake() => "Scissors";
    public string GetSpecialShake() => "Scissors";

    void Start()
    {
        animator = GetComponent<Animator>();
        PlayerMove = GetComponentInParent<PlayerMovement>();
        originalLocalPos = transform.localPosition;

    }

    // Update is called once per frame
    void Update()
    {
    }

    public bool SpecialAttack()
    {
        if (Time.time >= nextAttackTime)
        {
            animator.SetTrigger("Thrust");
            PlayerMove.movespeed = 7f;
            nextAttackTime = Time.time + attackCooldown;
            StartCoroutine(ThrustRoutine());
            return true;
        }
        return false;
    }


    public bool NormalAttack()
    {
        if (Time.time >= nextNormalAttackTime)
        {
            animator.SetTrigger("Thrust");
            PlayerMove.movespeed = 7f;
            nextAttackTime = Time.time + NormalattackCooldown;
            StartCoroutine(Thrust());
            return true;
        }
        return false;
    }



    IEnumerator ThrustRoutine()
    {
        isAttacking = true;

        float timer = 0f;
        while (timer < attackDuration)
        {
            timer += Time.deltaTime;
            float percent = timer / attackDuration;

            transform.localPosition = originalLocalPos + Vector3.right * (thrustDistance * percent);

            yield return null;
        }
        timer = 0f;
        while (timer < returnDuration)
        {
            timer += Time.deltaTime;
            float percent = timer / returnDuration;

            // Return from the extended position back to original
            transform.localPosition = Vector3.Lerp(originalLocalPos + Vector3.right * thrustDistance, originalLocalPos, percent);

            yield return null;
        }

        transform.localPosition = originalLocalPos;
        yield return new WaitForSeconds(0.3f);
        isAttacking = false;
    }


    private void OnDrawGizmosSelected()
    {
        if (hitPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(hitPoint.position, hitRadius);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (isAttacking) { 
                collision.GetComponent<EnemyStats>().TakeDamage(transform.position, 4f);
            }
            if (isNormaling) {
                collision.GetComponent<EnemyStats>().TakeDamage(transform.position, 10f);
            }

        }
    }

    IEnumerator Thrust()
    {
        isNormaling = true;

        float timer = 0f;
        while (timer < attackDuration)
        {
            timer += Time.deltaTime;
            float percent = timer / attackDuration;

            transform.localPosition = originalLocalPos + Vector3.right * (thrustDistance * percent * 0.15f);

            yield return null;
        }
        timer = 0f;
        while (timer < returnDuration)
        {
            timer += Time.deltaTime;
            float percent = timer / returnDuration;

            // Return from the extended position back to original
            transform.localPosition = Vector3.Lerp(originalLocalPos + Vector3.right * thrustDistance * 0.1f, originalLocalPos, percent);

            yield return null;
        }

        transform.localPosition = originalLocalPos;
        yield return new WaitForSeconds(0.3f);
        isNormaling = false;
    }
}

