using UnityEngine;

public class knighBossAnim : MonoBehaviour
{
    [SerializeField] private knightMovement knightMovement;

    private GameObject player;
    private Animator animator;



    private Vector2 direction;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        animator = GetComponent<Animator>();
        
    }


    private void OnEnable()
    {
        knightMovement.OnMove += HandleMoveAnimation;

        knightMovement.OnSlashAttack += HandleSlashAttack;
        knightMovement.EndSlashAttack += endSlashAttack;

        knightMovement.OnPiercingDash += HandlePiercingDash;
        knightMovement.EndPiercingDash += endPiercingDash;


    }
    private void OnDisable()
    {
        knightMovement.OnMove -= HandleMoveAnimation;

        knightMovement.OnSlashAttack -= HandleSlashAttack;
        knightMovement.EndSlashAttack -= endSlashAttack;

        knightMovement.OnPiercingDash -= HandlePiercingDash;
        knightMovement.EndPiercingDash -= endPiercingDash;
    }


    private void HandleSlashAttack(Vector2 direction)
    {
        animator.SetBool("slashAttack", true);
        animator.SetFloat("posX", direction.x);
        animator.SetFloat("posY", direction.y);
    }
    private void endSlashAttack() { animator.SetBool("slashAttack", false); }

    private void HandlePiercingDash(Vector2 direction)
    {
        animator.SetFloat("posX", direction.x);
        animator.SetFloat("posY", direction.y);
        animator.SetBool("isPiercingDash", true);
    }
    private void endPiercingDash() { animator.SetBool("isPiercingDash", false); }

    private void HandleMoveAnimation(Vector2 direction)
    {
 


        animator.SetFloat("posX", direction.x);
        animator.SetFloat("posY", direction.y);
    }



}
