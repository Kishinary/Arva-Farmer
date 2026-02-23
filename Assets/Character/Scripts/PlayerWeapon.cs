using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerWeapon : MonoBehaviour
{
    public float attackRadius = 1f;
    public float attackDistance = 1.2f;
    public float attackDuration = 0.15f;
    public int damage = 10;

    float timer;
    bool attacking;

    Vector2 attackDir;

    Rigidbody2D rb;
    Animator animator;

    public float attackRate = 3f;
    private float nextAttackTime = 0f;
    public Vector2 attackDirection;
    public float attackType = 1f;

    public bool attacked = false;
    public bool isattacking = false;

    private Camera mainCamera;
    public Vector2 snappedDir = Vector2.down;

    private void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && Time.time >= nextAttackTime)
        {
            isattacking = true;
            rb.linearVelocity = Vector2.zero;

            UpdateMouseDirection();

            animator.SetFloat("AttackX", snappedDir.x);
            animator.SetFloat("AttackY", snappedDir.y);

            nextAttackTime = Time.time + animator.GetCurrentAnimatorStateInfo(0).length - 0.2f;
            WhichAttack();
        }
    }

    private void WhichAttack()
    {
        if (attackType == 1f)
        {
            if (attacked)
            {
                animator.SetTrigger("Attack2");
                attacked = false;
                DoDamage();
            }
            else
            {
                animator.SetTrigger("Attack");
                DoDamage();
                StartCoroutine(SwordCooldown());
            }
        }
        else if (attackType == 2)
        {
            animator.SetTrigger("AttackSpear");
            if (attacked == true)
            {
                animator.SetTrigger("AttackSpear2");
                attacked = false;
            }
        }
    }

    IEnumerator SwordCooldown()
    {
        attacked = true;
        yield return new WaitForSeconds(6f);
        attacked = false;
    }

    void UpdateMouseDirection()
    {
        if (Mouse.current == null) return;
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorld.z = 0f;

        Vector2 dir = mouseWorld - transform.position;
        dir.Normalize();

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            snappedDir = new Vector2(Mathf.Sign(dir.x), 0);
        }
        else
        {
            snappedDir = new Vector2(0, Mathf.Sign(dir.y));
        }
    }

    void DoDamage()
    {
        Vector2 hitPos = (Vector2)transform.position + snappedDir * attackDistance;

        Collider2D[] hits = Physics2D.OverlapCircleAll(hitPos, attackRadius);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                EnemyStats enemy = hit.GetComponent<EnemyStats>();

                if (enemy != null)
                    enemy.TakeDamage(transform.position, damage);
            }
        }
    }
}
