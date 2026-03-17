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

    }
    private void OnDisable()
    {
        knightMovement.OnMove -= HandleMoveAnimation;
    }



    private void HandleMoveAnimation(Vector2 direction)
    {
        Debug.Log("dáoidasuiodjasodi");


        animator.SetFloat("posX", direction.x);
        animator.SetFloat("posY", direction.y);
    }



}
