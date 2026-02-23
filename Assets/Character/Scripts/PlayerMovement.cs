using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float movespeed = 1f;
    public float norMovespeed = 7f;
    //private float AttackingMovespeed = 0f;
    public Vector2 moveInput;


    [Header("Animation")]
    Rigidbody2D rb;
    Animator animator;
    public float LastX;
    public float LastY;


    [Header("Combat")]
    public float attackRate = 3f;
    private float nextAttackTime = 0f;
    public Vector2 attackDirection;
    public float attackType = 1f;

    public bool attacked = false;
    public bool isattacking = false;

    private Camera mainCamera;
    public Vector2 snappedDir = Vector2.down;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();mainCamera = Camera.main;
    }

    void Update()
    {
        if (!isattacking)
        {
         rb.linearVelocity = moveInput * movespeed;
        }
    }
    public void Move(InputAction.CallbackContext context)
    {
        animator.SetBool("IsMoving", true);
        if (context.canceled)
        {
            LastX = moveInput.x;
            LastY = moveInput.y;
            animator.SetFloat("LastX", LastX);
            animator.SetFloat("LastY", LastY);
            animator.SetBool("IsMoving", false);
        }

        moveInput = context.ReadValue<Vector2>();
        animator.SetFloat("InputX", moveInput.x);
        animator.SetFloat("InputY", moveInput.y);
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

            attackDirection = moveInput;
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
                rb.linearVelocity = snappedDir.normalized * 1.5f;
                animator.SetTrigger("Attack2");
                attacked = false;
            }
            else
            {
                rb.linearVelocity = snappedDir.normalized * 1f;
                //rb.AddForce(snappedDir.normalized * 4f, ForceMode2D.Impulse);
                animator.SetTrigger("Attack");
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
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(
            Mouse.current.position.ReadValue()
        );
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
}
