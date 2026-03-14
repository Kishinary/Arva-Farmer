using UnityEngine;

public class animation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private BossMovement bossMovement;

    private Animator animator;
    /// <summary>
    ///TONGUE ATTACK == BITE ATTACK
    /// </summary>

    private readonly int moveXHash = Animator.StringToHash("MoveX");
    private readonly int moveYHash = Animator.StringToHash("MoveY");
    private readonly int isJumpingHash = Animator.StringToHash("isJumpAttacking");
    private readonly int jumpSpeedHash = Animator.StringToHash("jumpSpeed");

    public readonly int BiteAttackHash = Animator.StringToHash("isTongueAttack");
    public readonly int BiteAttackSpeedHash = Animator.StringToHash("TongueAttackSpeed");

    [SerializeField] private float defaultJumpClipLength = 1.0f;

    private void Awake()
    {
        
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        bossMovement.OnMove += UpdateMovementAnimation;

        bossMovement.OnJumpAttackStart += HandleJumpAttackStart;
        bossMovement.OnJumpAttackEnd += HandleJumpAttackEnd;
        
        bossMovement.OnBiteAttackStart += HandleBiteAttackStart;
        bossMovement.OnBiteAttackEnd += HandleBiteAttackEnd;


    }

    private void OnDisable()
    {
        
        bossMovement.OnMove -= UpdateMovementAnimation;
        bossMovement.OnJumpAttackStart -= HandleJumpAttackStart;
        bossMovement.OnJumpAttackEnd -= HandleJumpAttackEnd;

        bossMovement.OnBiteAttackStart -= HandleBiteAttackStart;
        bossMovement.OnBiteAttackEnd -= HandleBiteAttackEnd;
    }

    private void UpdateMovementAnimation(Vector2 direction)
    {
        // Dùng Hash thay cho String
        animator.SetFloat(moveXHash, direction.x);
        animator.SetFloat(moveYHash, direction.y);
    }
    private void HandleJumpAttackStart(Vector2 jumpDirection, float jumpDuration)
    {
        animator.SetBool(isJumpingHash, true);

        
        animator.SetFloat(moveXHash, jumpDirection.x);
        animator.SetFloat(moveYHash, jumpDirection.y);

        float requiredSpeed = defaultJumpClipLength / jumpDuration;
        animator.SetFloat(jumpSpeedHash, requiredSpeed);
    }
    private void HandleJumpAttackEnd()
    {
        animator.SetBool(isJumpingHash, false);
        animator.SetFloat(jumpSpeedHash, 1f); // Reset về tốc độ mặc định
    }

    private void HandleBiteAttackStart()
    {
        
        float requiredSpeed = defaultJumpClipLength / 2f; // Giả sử thời gian của Bite là 2 s
        
        animator.SetBool(BiteAttackHash, true);
        animator.SetFloat(BiteAttackSpeedHash, requiredSpeed);
    }
    private void HandleBiteAttackEnd()
    {
      
        animator.SetBool(BiteAttackHash, false);
        animator.SetFloat(BiteAttackSpeedHash, 1f); // Reset về tốc độ mặc định
    }


}
