using JetBrains.Annotations;
using System;
using System.Collections;
using UnityEngine;
using static knightMovement;



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
    public virtual void ExecuteComboAttack(BossCombo combo, int hitIndex)
    {
        if (boss.isActionLocked) return;

        switch (combo.bandType)
        {
            case BandType.Kick: HandleKickCombo(combo, hitIndex); break;
            case BandType.Bass: HandleBassCombo(combo, hitIndex); break;
            case BandType.LowMid: HandleLowMidCombo(combo, hitIndex); break;
            case BandType.HighMid: HandleHighMidCombo(combo, hitIndex); break;
            case BandType.Treble: HandleTrebleCombo(combo, hitIndex); break;
        }
    }

    protected virtual void HandleKickCombo(BossCombo combo, int hitIndex) { }
    protected virtual void HandleBassCombo(BossCombo combo, int hitIndex) { }
    protected virtual void HandleLowMidCombo(BossCombo combo, int hitIndex) { }
    protected virtual void HandleHighMidCombo(BossCombo combo, int hitIndex) { }
    protected virtual void HandleTrebleCombo(BossCombo combo, int hitIndex) { }
}


public class KnightPhase1State : KnightBaseState
{
    public KnightPhase1State(knightMovement boss) : base(boss) { }
    public override void EnterState()
    {
        boss.moveSpeed = 4f;
        boss.ResetCloseAttackCooldown();

    }
    public override void UpdateState()
    {
        if (boss.isActionLocked) return;

        boss.UpdateCombatCooldowns(); 

        if (boss.CanExecuteCloseAttack())
        {
            boss.StartCloseAttackSequence(); 
        }
        else if (!boss.isExecutingCloseAttack)
        {
            boss.ChasePlayer();
        }

    }

    protected override void HandleKickCombo(BossCombo combo, int hitIndex)
    {
        if (combo.beatCount == 1)
        {
            switch (boss.currentLongRangeStance)
            {
                case BossStance.LightningFocus:
                    boss.lightningStrikePerform();
                    break;


            }

            boss.RegisterAttack();

        }
        else if(combo.beatCount == 2)
        {
            switch (boss.currentLongRangeStance)
            {
                case BossStance.LightningFocus:
                    boss.lightningStrikePerform();
                    break;


            }

            boss.RegisterAttack();
        }
    }
    protected override void HandleBassCombo(BossCombo combo, int hitIndex)
    {
        if (combo.beatCount == 1)
        {
            switch (boss.currentLongRangeStance)
            {
                case BossStance.LightningFocus:
                    boss.lightningStrikePerform();
                    break;


            }

            boss.RegisterAttack();
        }
    }
    protected override void HandleLowMidCombo(BossCombo combo, int hitIndex)
    {
        if (combo.beatCount == 1)
        {
            
        }
       
    }
    protected override void HandleHighMidCombo(BossCombo combo, int hitIndex)
    {
        if (combo.beatCount == 1)
        {
        }
        
    }
    protected override void HandleTrebleCombo(BossCombo combo, int hitIndex)
    {

        

        if (combo.beatCount == 1)
        {
            switch (boss.currentLongRangeStance)
            {
                case BossStance.LightningFocus:
                    boss.lightningStrikePerform();
                    break;


            }

            boss.RegisterAttack();


            


        }
        if (boss.isExecutingCloseAttack)
        {
            boss.ProgressCloseAttackStep();
        }


    }

 
    

    

    

    public override void ExitState()
    {

        boss.SetMovementDirection(Vector2.zero);
        if (boss.isExecutingCloseAttack)
        {
            boss.EndCloseAttackSequence(); 
        }
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
    [Header("SkillsPrefab")]
    public GameObject lightningStrike;
    public GameObject fireCirclePrefab;

    [Header("Close Attack Management")]
    public float minCloseAttackCooldown = 5f;
    public float maxCloseAttackCooldown = 10f;

    public int maxCloseAttackSteps = 5;
    public bool isExecutingCloseAttack { get; private set; }
    public int currentCloseAttackStep { get; private set; }


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
        RollNextStance();
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

    public void ChasePlayer()
    {
        Vector2 direction = ((Vector2)player.transform.position - (Vector2)transform.position).normalized;
        SetMovementDirection(direction);
    }


    public int minAttacksPerStance = 8;
    public int maxAttacksPerStance = 20;

    private int targetAttackCount;
    private int currentAttackCount;

    private float closeAttackTimer = 0f;

    public void UpdateCombatCooldowns()
    {
        if (closeAttackTimer > 0)
        {
            closeAttackTimer -= Time.deltaTime;
        }
    }


    public enum BossStance
    {
        LightningFocus,
        
       
    }
    public BossStance currentLongRangeStance = BossStance.LightningFocus;
    public void RollNextStance()
    {
        int numberOfStances = System.Enum.GetNames(typeof(BossStance)).Length;
        if (numberOfStances > 1)
        {
            BossStance newStance = currentLongRangeStance;
            while (newStance == currentLongRangeStance)
            {
                newStance = (BossStance)UnityEngine.Random.Range(0, numberOfStances);
            }
            currentLongRangeStance = newStance;
        }

       
        targetAttackCount = UnityEngine.Random.Range(minAttacksPerStance, maxAttacksPerStance + 1);

        currentAttackCount = 0;
    }
    public void RegisterAttack()
    {
        currentAttackCount++;
        
        if (currentAttackCount >= targetAttackCount)
        {
            RollNextStance();
        }
    }

    public void dartAttack()
    {
        if (dartAttackModule != null)
        {
            dartAttackModule.ExecuteAttack();
        }
    }

    
  

    //lightningSkills
    
    private float maxLightningRadius = 5f;
    private float safeLightningRadius = 1f;
    public void lightningStrikePerform()
    {
        Vector2 playerPos = player.transform.position;
        Vector2 spawnPos = MathUtility.GetRandomPositionAround(playerPos, maxLightningRadius, safeLightningRadius);
        GameObject newStrike = Instantiate(lightningStrike, spawnPos, Quaternion.identity);
    }


    public void ResetCloseAttackCooldown()
    {
        
        closeAttackTimer = UnityEngine.Random.Range(minCloseAttackCooldown, maxCloseAttackCooldown);
        Debug.Log($"New cooldown {closeAttackTimer:F2} seconds!");
    }
    public enum CloseAttackType
    {
        SuperBoost,
        //HeavyCleave
    }
    private CloseAttackType currentCloseAttackType = CloseAttackType.SuperBoost;
    public bool CanExecuteCloseAttack()
    {
        return !isExecutingCloseAttack && closeAttackTimer <= 0f;
    }
    public void StartCloseAttackSequence()
    {
        isExecutingCloseAttack = true;
        currentCloseAttackStep = 0;

        SetMovementDirection(Vector2.zero); 
        Debug.Log("Start Close Attack");
    }
    public void ProgressCloseAttackStep()
    {
        currentCloseAttackStep++;

        ExecuteCloseAttackStrike(currentCloseAttackStep);

        if (currentCloseAttackStep >= maxCloseAttackSteps)
        {
            EndCloseAttackSequence();
        }
    }
    public void EndCloseAttackSequence()
    {
        isExecutingCloseAttack = false;
        currentCloseAttackStep = 0;
        ResetCloseAttackCooldown(); 

        Debug.Log($"Attack end!");
    }
    public void ExecuteCloseAttackStrike(int step)
    {
        Vector2 directionToPlayer = ((Vector2)player.transform.position - (Vector2)transform.position).normalized;

        switch (currentCloseAttackType)
        {
            case CloseAttackType.SuperBoost:
                PerformSuperBoostCombo(step, directionToPlayer);
                break;
            /*case CloseAttackType.HeavyCleave:
                Debug.Log("HeavyCleave");
                break;*/
        }
    }

    private void PerformSuperBoostCombo(int step, Vector2 direction)
    {
        switch (step)
        {
            case 1: Debug.Log("test 1"); break;
            case 2: Debug.Log("test 2"); break;
            case 3: Debug.Log("test 3"); break;
            case 4:
                Debug.Log("test 4");
                break;

            case 5:
                Debug.Log("test 5");
                RandomizeNextCloseAttack();
                break;
        }
    }
    private void RandomizeNextCloseAttack()
    {
        Array values = Enum.GetValues(typeof(CloseAttackType));
        currentCloseAttackType = (CloseAttackType)values.GetValue(UnityEngine.Random.Range(0, values.Length));
    }


}
