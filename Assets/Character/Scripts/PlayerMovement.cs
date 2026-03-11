using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{   
    [Header("Movement")]
    public float movespeed = 10f;
    public Vector2 moveInput;
    public Camera mainCamera;

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
    public PickaxeSlash Pickaxe;
    public Watercan watercan;
    public Scissors scissors;
    public int PlayerWeapon = 1;


    [Header("Talking")]
    public bool isTalking = false;
    void Start()
    {
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        weaponParent = GetComponentInChildren<WeaponParent>();
        pointerPosition = GetComponent<PlayerInput>().actions.FindActionMap("Player").FindAction("pointerPosition");


        
        
        playerInput = GetComponent<PlayerInput>();
        DontDestroyOnLoad(this.gameObject);
        
        
        shovel = GetComponentInChildren<Shovel>();
        Pickaxe = GetComponentInChildren<PickaxeSlash>();
        watercan = GetComponentInChildren<Watercan>();
        scissors = GetComponentInChildren<Scissors>();
    }

    private void FixedUpdate()
    {
            rb.linearVelocity = moveInput * movespeed;
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        mainCamera = Camera.main;
    }

    public GameObject rockLaserBeam;


    void Update()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (Pickaxe != null) {
            Pickaxe.PointerPosition = GetPointerInput();
        }
        pointerInput = GetPointerInput();
        UpdateMouseDirection();


        animator.SetFloat("InputX", snappedDir.x);
        animator.SetFloat("InputY", snappedDir.y);
        animator.SetFloat("LastX", snappedDir.x);
        animator.SetFloat("LastY", snappedDir.y);

       
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        animator.SetBool("IsMoving", true);
        if (context.canceled)
        {
            animator.SetBool("IsMoving", false);
        }

        moveInput = context.ReadValue<Vector2>();
    }

    public void OnAttackNewWeapon(InputAction.CallbackContext context)
    {
        if (context.performed && !isTalking) {
            if (shovel != null)
            {
                shovel.NormalAttack();
                CineCamera.instance.TriggerPreset("ShovelNormal");
            }
            else if (Pickaxe != null)
            {
                Pickaxe.Attack();
                CineCamera.instance.TriggerPreset("Slash");
            }
            else if (watercan != null)
            {
                watercan.Attack();
                CineCamera.instance.TriggerPreset("Watercan");
            }
            else if (scissors != null)
            {
                scissors.NormalAttack();
                CineCamera.instance.TriggerPreset("Scissors");
            }
        }
    }
    public void OnSpecialMove(InputAction.CallbackContext context)
    {
        if (context.performed && !isTalking)
        { 
            if (shovel != null)
            {
                shovel.Attack();
                CineCamera.instance.TriggerPreset("ShovelSpecial");
            }
            if (scissors != null)
            {
                scissors.Attack();
                CineCamera.instance.TriggerPreset("Scissors");
            }
        }
    }

    public Vector2 GetPointerInput()
    {
        // If the camera was destroyed by a scene change, find the new one
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) return Vector2.zero; // Still no camera found
        }
        Vector3 mousePos = pointerPosition.ReadValue<Vector2>();
        mousePos.z = Camera.main.nearClipPlane;

        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    void UpdateMouseDirection()
    {
        // If the camera was destroyed by a scene change, find the new one
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) return; // Still no camera found
        }
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
        movespeed = 0;
        playerInput.DeactivateInput();
    }
    public void EnablePlayerInput()
    {
        movespeed = 10f;
        playerInput.ActivateInput();
    }





    //public void OnInteract(InputAction.CallbackContext context)
    //{
      //  Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.35f);

        //foreach (Collider2D hit in hits)
        //{
          //  if (hit.CompareTag("Interactable"))
            //{
              //  IInteractable interactable = hit.GetComponent<IInteractable>();

                //if (interactable != null)
                //{
                  //  interactable.Interact();
                    //return;
                //}
            //}
        //}
    //}
}
