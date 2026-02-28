using UnityEngine;
using System.Collections;

public class Scissors : MonoBehaviour
{
    public float nextAttackTime;
    public float attackCooldown = 1f;
    private Animator animator;

    public bool isNormaling;

    [Header("Components")]
    private PlayerMovement PlayerMove;

    [Header("Thrust Settings")]
    public float thrustDistance = 2f;     
    public float attackDuration = 0.1f;    
    public float returnDuration = 0.2f;    
    public AnimationCurve thrustCurve;     // Use this for "snappy" movement

    [Header("Combat")]
    public float damage = 25f;
    public LayerMask enemyLayer;
    public Transform hitPoint;
    public float hitRadius = 0.5f;

    private Vector3 originalLocalPos;
    private bool isAttacking = false;

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

    public void Attack()
    {
        if (Time.time >= nextAttackTime)
        {
            animator.SetTrigger("Thrust");
            PlayerMove.movespeed = 7f;
            nextAttackTime = Time.time + attackCooldown;
            StartCoroutine(ThrustRoutine());
        }
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isAttacking) { 
            if (collision.CompareTag("Enemy"))
            {
                collision.GetComponent<EnemyStats>().TakeDamage(transform.position, 5f);
            }
        }
    }
}

