using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float movespeed = 1f;
    public float norMovespeed = 7f;
    private float AttackingMovespeed = 0f;
    public Vector2 moveInput;


    [Header("Animation")]
    Rigidbody2D rb;
    Animator animator;
    public float LastX;
    public float LastY;


    [Header("Combat")]
    public float attackRate = 1f;
    private float nextAttackTime = 0f;
    public Vector2 attackDirection;
    public float attackType = 1f;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        rb.linearVelocity = moveInput * movespeed;
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
            animator.SetFloat("AttackX", moveInput.x);
            animator.SetFloat("AttackY", moveInput .y);
            attackDirection = moveInput;
            nextAttackTime = Time.time + 1f / attackRate;
            animator.SetTrigger("Attack");
            movespeed = AttackingMovespeed;
            if (attackType == 1f)
            {
                rb.AddForce(transform.right * moveInput * 0.5f, ForceMode2D.Impulse);
            }
        }
        
    }

}