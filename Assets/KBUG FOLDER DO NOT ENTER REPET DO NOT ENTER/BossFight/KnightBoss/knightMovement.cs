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

    }

    public override void ExecuteComboAttack(BossCombo combo, int hitIndex)
    {
        
    }

    public override void ExitState()
    {

    }

    private void ChasePlayer()
    {
        Vector2 direction = ((Vector2)boss.player.transform.position - (Vector2)boss.transform.position).normalized;
        boss.transform.position += (Vector3)(direction * boss.moveSpeed * Time.deltaTime);
        boss.NotifyMovement(direction);

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

    void Start()
    {
        player = GameObject.FindWithTag("Player");

    
        phase1State = new KnightPhase1State(this);
        //phase2State = new KnightPhase2State(this);

        //start phase 1
        ChangeState(phase1State);
    }

    private void HandleComboAttack(BossCombo combo, int hitIndex)
    {
        currentState?.ExecuteComboAttack(combo, hitIndex);
    }

    private void OnDestroy()
    {
        
            bossCatching.OnBossAttackTriggered -= HandleComboAttack;
    }

    private void Update()
    {
        currentState?.UpdateState();

        if (!isPhase2 && enemyStats.health <= enemyStats.maxHealth * phase2Threshold)
        {
            //ChangeState(phase2State);
            isPhase2 = true;
        }
    }
    public void ChangeState(KnightBaseState newState)
    {
        currentState.ExitState();
        currentState = newState;
        currentState.EnterState();
    }

    public event Action onIdle;
    public event Action<Vector2> OnMove;

    public void NotifyMovement(Vector2 direction)
    {
        OnMove?.Invoke(direction);
    }




}
