using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float movespeed = 1f;
    public float norMovespeed = 7f;
    private float AttackingMovespeed = 3f;
    public Vector2 moveInput;


    [Header("Animation")]
    Rigidbody2D rb;
    Animator animator;
    public float LastX;
    public float LastY;


    [Header("Combat")]
    private float attacktimer = 0f;    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        rb.linearVelocity = moveInput * movespeed;
        attacktimer += Time.deltaTime;
    }
    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        animator.SetFloat("InputX", moveInput.x);
        animator.SetFloat("InputY", moveInput.y);

        if (moveInput.x != 0 || moveInput.y != 0)
        {
            animator.SetBool("IsMoving", true);
        }
        else
        {
            animator.SetBool("IsMoving", false);
            animator.SetFloat("InputX", LastX);
            animator.SetFloat("InputY", LastY);
        }

        if (context.canceled)
        {
            LastX = moveInput.x;
            LastY = moveInput.y;
            animator.SetFloat("LastX",LastX);
            animator.SetFloat("LastY", LastY);
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            attacktimer = 0f;
            animator.SetTrigger("Attack");
            movespeed = AttackingMovespeed;
        }
        
    }

}