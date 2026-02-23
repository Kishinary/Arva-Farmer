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
    public bool attacked = false;
    private Camera mainCamera;
    private PlayerWeapon WeaponManager;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
        WeaponManager = GetComponent<PlayerWeapon>();
    }

    void Update()
    {
        //if (!WeaponManager.isattacking)
        //{
         rb.linearVelocity = moveInput * movespeed;
        //}
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
    public void OnInteract(InputAction.CallbackContext context)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.35f);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Interactable"))
            {
                IInteractable interactable = hit.GetComponent<IInteractable>();

                if (interactable != null)
                {
                    interactable.Interact();
                    return;
                }
            }
        }
    }
}
