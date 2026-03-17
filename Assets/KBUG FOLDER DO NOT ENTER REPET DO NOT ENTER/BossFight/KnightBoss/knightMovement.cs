using JetBrains.Annotations;
using System;
using UnityEngine;



public abstract class KnightBaseState
{
    protected knightMovement boss;

    public KnightBaseState(knightMovement bossController)
    {
        this.boss = bossController;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void ExecuteComboAttack(BossCombo combo, int hitIndex);
}

public class KnightPhase1State : KnightBaseState
{
    public KnightPhase1State(knightMovement boss) : base(boss) { }
    public override void EnterState()
    {
        boss.moveSpeed = 2f;

    }
    public override void UpdateState()
    {
        ChasePlayer();
       

        boss.dartAttack();
    }

    public override void ExecuteComboAttack(BossCombo combo, int hitIndex)
    {
        
    }

    public override void ExitState()
    {
        boss.SetMovementDirection(Vector2.zero);
    }

    private void ChasePlayer()
    {
        Vector2 direction = ((Vector2)boss.player.transform.position - (Vector2)boss.transform.position).normalized;
        boss.SetMovementDirection(direction);
    }
}

/*public class KnightPhase2State : BossBaseState
{

}*/


public class knightMovement : MonoBehaviour
{
    [Header("Combo Controller")]
    public BossCatching bossCatching;

    [HideInInspector]
    public GameObject player;

    [Header("Abilities")]


    [Header("Stats")]
    public float moveSpeed;

    [Header("Phase Thresholds")]
    [Range(0f, 1f)]
    public float phase2Threshold = 0.5f;

    [Header("Abilities Modules")]
    public KnightDartAttack dartAttackModule;

    // FSM manager
    private KnightBaseState currentState;

    // States
    private KnightPhase1State phase1State;
    //private KnightPhase2State phase2State;

    private bool isPhase2 = false;

    // Health
    public EnemyStats enemyStats;
    public bool isActionLocked = false;

    /* [Header("Effect")]

     [Header("Cooldowns")]
 */

    //movement using rigidbody
    
    private Vector2 currentDirection;//movement cache
    [SerializeField] private Rigidbody2D rb;


    private void Awake()
    {
        if(rb == null) rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        player = GameObject.FindWithTag("Player");

    
        phase1State = new KnightPhase1State(this);
        //phase2State = new KnightPhase2State(this);

        //start phase 1
        if (bossCatching != null)
        {
            bossCatching.OnBossAttackTriggered += HandleComboAttack;
        }
        ChangeState(phase1State);
    }

    private void HandleComboAttack(BossCombo combo, int hitIndex)
    {
        currentState?.ExecuteComboAttack(combo, hitIndex);
    }

    private void OnDestroy()
    {

        if (bossCatching != null)
        {
            bossCatching.OnBossAttackTriggered -= HandleComboAttack;
        }
    }

    private void Update()
    {
        currentState?.UpdateState();
        
        

        if (!isPhase2 && enemyStats.health <= enemyStats.maxHealth * phase2Threshold)
        {
           
            isPhase2 = true;
        }
    }
     
    private void FixedUpdate()
    {
        if (!isActionLocked)
        {
            
            rb.linearVelocity = currentDirection * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero; 
        }
        
    }
    public void ChangeState(KnightBaseState newState)
    {
        currentState?.ExitState();
        currentState = newState;
        currentState?.EnterState();
    }

   
    public event Action<Vector2> OnMove;

    public void SetMovementDirection(Vector2 direction)
    {
        currentDirection = direction;
        NotifyWalk(direction);
    }

    public void NotifyWalk(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            OnMove?.Invoke(direction);
        }
    }



    public void dartAttack()
    {
        if (dartAttackModule != null)
        {
            dartAttackModule.ExecuteAttack();
        }
    }




}
