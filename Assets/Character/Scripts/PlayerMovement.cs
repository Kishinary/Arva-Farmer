using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float movespeed = 10f;
    public Vector2 moveInput;
    private Camera mainCamera;

    [Header("Animation")]
    Rigidbody2D rb;
    Animator animator;
    public float LastX;
    public float LastY;
    public Vector2 snappedDir = Vector2.down;
    public PlayerInput playerInput;


    [Header("Combat")]
    public InputAction pointerPosition;
    Vector2 pointerInput;


    [Header("Combat-related Components")]
    public Shovel shovel;
    public WeaponParent weaponParent;

    public int PlayerWeapon;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        weaponParent = GetComponentInChildren<WeaponParent>();
        pointerPosition = GetComponent<PlayerInput>().actions.FindActionMap("Player").FindAction("pointerPosition");


        mainCamera = Camera.main;



        if (PlayerWeapon == 1)
        {
            shovel = GetComponentInChildren<Shovel>();
        }
    }
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

    }

    private void FixedUpdate()
    {
            rb.linearVelocity = moveInput * movespeed;
        //if (!WeaponManager.isattacking)
        //{
        //}
    }




    void Update()
    {
        pointerInput = GetPointerInput();
        //weaponParent.PointerPosition = pointerInput;
        UpdateMouseDirection();
        animator.SetFloat("InputX", snappedDir.x);
        animator.SetFloat("InputY", snappedDir.y);
        animator.SetFloat("LastX", snappedDir.x);
        animator.SetFloat("LastY", snappedDir.y);
    }
    public void Move(InputAction.CallbackContext context)
    {
        animator.SetBool("IsMoving", true);
        if (context.canceled)
        {
            animator.SetBool("IsMoving", false);
        }

        moveInput = context.ReadValue<Vector2>();
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


    public void OnAttackNewWeapon(InputAction.CallbackContext context)
    {
        if (context.performed) {
            if (PlayerWeapon == 1)
            {
                //shovel.attack();
                return;
            }
            else if (PlayerWeapon == 2) {
                return;
            }
            else
            {
                return;
            }
        }
        
    }
    public Vector2 GetPointerInput()
    {
        Vector3 mousePos = pointerPosition.ReadValue<Vector2>();
        mousePos.z = Camera.main.nearClipPlane;

        return Camera.main.ScreenToWorldPoint(mousePos);
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

    public void DisablePlayerInput()
    {
        if (playerInput != null)
        {
            playerInput.DeactivateInput();
        }
        else
        {
            Debug.LogWarning("PlayerInput is missing from the script!");
        }
        pointerInput = Vector3.zero;
    }
    public void EnablePlayerInput()
    {
        playerInput.ActivateInput();
        pointerInput = GetPointerInput();
    }
}
