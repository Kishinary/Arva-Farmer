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
    public float movespeed = 6f;
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
    public WeaponParent weaponParent;
    public IWeapon currentWeapon;

    [Header("Talking")]
    public bool isTalking = false;

    //slow effect by using skills
    private float originalSpeed;
    private Coroutine activeSlowCoroutine;
    void Start()
    {
        originalSpeed = movespeed;
    }

    // Call this whenever you switch tools
    public void SetWeapon(IWeapon newWeapon)
    {
        currentWeapon = newWeapon;
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        weaponParent = GetComponentInChildren<WeaponParent>();
        pointerPosition = GetComponent<PlayerInput>().actions.FindActionMap("Player").FindAction("pointerPosition");


        
        
        playerInput = GetComponent<PlayerInput>();
        DontDestroyOnLoad(this.gameObject);
       
    }
    //HAM LAM SLOW PLAYER DO SU DUNG SKILLS
    
    public void ApplySlow(float multiplier, float duration)
    {
        if (activeSlowCoroutine != null)
        {
            
            StopCoroutine(activeSlowCoroutine);
        }
        
        activeSlowCoroutine = StartCoroutine(SlowRoutine(multiplier, duration));
    }
    private IEnumerator SlowRoutine(float multiplier, float duration)
    {
        movespeed = multiplier * originalSpeed;
      
        yield return new WaitForSeconds(duration);

        movespeed = originalSpeed;

        activeSlowCoroutine = null;
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

    


    void Update()
    {
        if (mainCamera == null) mainCamera = Camera.main;
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
        if (context.performed && !isTalking && currentWeapon != null)
        {
            if (currentWeapon.NormalAttack()) {
                CineCamera.instance.TriggerPreset(currentWeapon.GetNormalShake());
            }
        }
    }
    public void OnSpecialMove(InputAction.CallbackContext context)
    {
        if (context.performed && !isTalking && currentWeapon != null)
        {
            if (currentWeapon.SpecialAttack()) CineCamera.instance.TriggerPreset(currentWeapon.GetSpecialShake());
        }
    }

    public Vector2 GetPointerInput()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) return Vector2.zero;
        }
        Vector3 mousePos = pointerPosition.ReadValue<Vector2>();
        mousePos.z = Camera.main.nearClipPlane;

        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    void UpdateMouseDirection()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) return; 
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
        animator.SetBool("IsMoving", false); 
        Debug.Log("Input Disabled");
    }
    public void EnablePlayerInput()
    {
        movespeed = originalSpeed;
        playerInput.ActivateInput();
        Debug.Log("Input Enabled");
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.5f);

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
