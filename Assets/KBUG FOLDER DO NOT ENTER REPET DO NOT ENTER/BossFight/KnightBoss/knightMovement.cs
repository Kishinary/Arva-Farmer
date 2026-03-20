using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
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
        boss.moveSpeed = 2f;
        boss.ResetCloseAttackCooldown();

    }
    public override void UpdateState()
    {
        if (boss.isActionLocked || boss.isDashing) { return;  }

        boss.UpdateCombatCooldowns();

        if (!boss.isExecutingCloseAttack)
        {
            float distanceToPlayer = Vector2.Distance(boss.transform.position, boss.player.transform.position);

            if (boss.CanExecuteCloseAttack())
            {
                boss.StartCloseAttackSequence();
            }
            else
            {
                boss.ChasePlayer();
            }
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
        }else if(combo.beatCount == 3) { }


        if (combo.beatCount >= 4 && hitIndex == 0) {
            boss.spikeBoomPerform(combo.beatCount);
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
        if (combo.beatCount >= 4 && hitIndex == 0)
        {
            boss.spikeBoomPerform(combo.beatCount);
        }
    }
    protected override void HandleLowMidCombo(BossCombo combo, int hitIndex)
    {
        if (combo.beatCount == 1)
        {
            
        }
        if (combo.beatCount >= 4 && hitIndex == 0)
        {
            boss.spikeBoomPerform(combo.beatCount);
        }

    }
    protected override void HandleHighMidCombo(BossCombo combo, int hitIndex)
    {
        if (combo.beatCount == 1)
        {
        }
        if (combo.beatCount >= 4 && hitIndex == 0)
        {
            boss.spikeBoomPerform(combo.beatCount);
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
    public float phase2Threshold = 0f;


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
    public GameObject spikeBoomController;

    public Transform[] spikeBoomPosition;

    public GameObject slashHitBox;
    public GameObject indicatorSplashHitBox;

    [Header("Close Attack Management")]
    public float closeAttackRange = 3f;
    public float minCloseAttackCooldown = 5f;
    public float maxCloseAttackCooldown = 10f;

    public bool isExecutingCloseAttack { get; private set; }

    [Header("Dash Settings")]
    public float dashSpeed = 40f;
    public bool isDashing = false;

    public TrailRenderer dashTrail;

    public GameObject ghostTrailPrefab;
    public float ghostSpawnInterval = 0.05f;
    [Header("Piercing Dash Settings")]
    public float piercingDashSpeed = 30f;
    public float maxDashDuration = 20f;
    public LayerMask obstacleLayer;

    [Header("Settings")]
    public GameObject spriteComponent;
    private SpriteRenderer spriteRenderer;
    private void Awake()
    {
        if(rb == null) rb = GetComponent<Rigidbody2D>();
        spriteRenderer = spriteComponent.GetComponent<SpriteRenderer>();
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
        if (isDashing) return;

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
    public event Action<Vector2> OnSlashAttack;
    public event Action EndSlashAttack;

    public event Action<Vector2> OnPiercingDash;
    public event Action EndPiercingDash;

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

    public void SetSlashAttackDirection(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            OnSlashAttack?.Invoke(direction);
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

   

    
  

    //lightningSkills
    
    private float maxLightningRadius = 5f;
    private float safeLightningRadius = 1f;
    public void lightningStrikePerform()
    {
        Vector2 playerPos = player.transform.position;
        Vector2 spawnPos = MathUtility.GetRandomPositionAround(playerPos, maxLightningRadius, safeLightningRadius);
        GameObject newStrike = Instantiate(lightningStrike, spawnPos, Quaternion.identity);
    }
    //spikeBoom
    //spikeBoomPosition
    public void spikeBoomPerform(int numberOfSpikes)
    {
        if (numberOfSpikes > 3)
        {
            int actualSpikesToSpawn = Mathf.Min(numberOfSpikes, spikeBoomPosition.Length);
            List<Transform> availablePoints = new List<Transform>(spikeBoomPosition);
            for (int i = 0; i < actualSpikesToSpawn; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, availablePoints.Count);
                Transform selectedPoint = availablePoints[randomIndex];

                Instantiate(spikeBoomController, selectedPoint.position, Quaternion.identity);


                availablePoints.RemoveAt(randomIndex);

            }
        }
    }


    public void ResetCloseAttackCooldown()
    {
        
        closeAttackTimer = UnityEngine.Random.Range(minCloseAttackCooldown, maxCloseAttackCooldown);
        Debug.Log($"New cooldown {closeAttackTimer:F2} seconds!");
    }
    public enum CloseAttackType
    {
        SuperBoost,
        PiercingDash,

    }
    private CloseAttackType currentCloseAttackType = CloseAttackType.SuperBoost;
    public bool CanExecuteCloseAttack()
    {
        return !isExecutingCloseAttack && closeAttackTimer <= 0f;
    }
    public void StartCloseAttackSequence()
    {
        isExecutingCloseAttack = true;
        isActionLocked = true;
        SetMovementDirection(Vector2.zero); 
        StartCoroutine(CloseAttackRoutine());
    }
    


    public void EndCloseAttackSequence()
    {
        isExecutingCloseAttack = false;
        isActionLocked = false;
        ResetCloseAttackCooldown(); 

        Debug.Log($"Attack end!");
    }
    

    
    private IEnumerator slashAttack(Vector2 direction, float duration) //superboose = slashAttack
    {

        Vector3 indicatorAdd = new Vector3(1, 3, 0);
        Instantiate(indicatorSplashHitBox, transform.position + indicatorAdd, Quaternion.identity);

        float blinkSpeed = 5f;
        float minAlpha = 0.5f;
        float maxAlpha = 1f;
        float timeBlink = 0.5f;

        Color originColor = spriteRenderer.color;
        Color currentColor = originColor;
        while (timeBlink > 0)
        {


            float t = Mathf.PingPong(Time.time * blinkSpeed, 1f);
            currentColor.a = Mathf.Lerp(minAlpha, maxAlpha, t);
            spriteRenderer.color = currentColor;

            timeBlink -= Time.deltaTime;

            yield return null;
        }
        spriteRenderer.color = originColor;
        //yield return new WaitForSeconds(duration);

        SetSlashAttackDirection(direction);

        float hitboxOffsetDistance = 2f;
        Vector2 bossPos = transform.position;
        Vector2 spawnPosition = bossPos + (direction * hitboxOffsetDistance);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion spawnRotation = Quaternion.Euler(0f, 0f, angle);

        Instantiate(slashHitBox, spawnPosition, spawnRotation);
        


        yield return new WaitForSeconds(duration);
        EndSlashAttack?.Invoke();
    }

   

    public void StartDashToPlayer(float distanceOffset = 1.5f)
    {
        StartCoroutine(DashRoutine(distanceOffset));
    }
    private IEnumerator DashRoutine(float distanceOffset)
    {
       
        isDashing = true;


        Vector2 playerPos = player.transform.position;
        Vector2 bossPos = transform.position;
        Vector2 directionToPlayer = (playerPos - bossPos).normalized;

        float fixedDashTime = 0.2f;

        rb.linearVelocity = directionToPlayer * dashSpeed;

        yield return new WaitForSeconds(fixedDashTime);

        rb.linearVelocity = Vector2.zero;

        isDashing = false;
       

    }
    
    
    private IEnumerator CloseAttackRoutine()
    {

        Vector2 directionToPlayer = ((Vector2)player.transform.position - (Vector2)transform.position).normalized;

        switch (currentCloseAttackType)
        {
            /*case CloseAttackType.SuperBoost:
                yield return StartCoroutine(DashRoutine(1.5f));
                yield return StartCoroutine(slashAttack(directionToPlayer, 0.7f));
                break;*/
            case CloseAttackType.PiercingDash:
                yield return StartCoroutine(PiercingDashRoutine(directionToPlayer));
                break;




        }

        RandomizeNextCloseAttack();

        EndCloseAttackSequence();
    }
    private void RandomizeNextCloseAttack()
    {
        Array values = Enum.GetValues(typeof(CloseAttackType));
        currentCloseAttackType = (CloseAttackType)values.GetValue(UnityEngine.Random.Range(0, values.Length));
    }
    public GameObject PiercingDashWarning;
    private IEnumerator PiercingDashRoutine(Vector2 direction)
    {
        isActionLocked = true;
        Vector2 oldDirection = direction;

        OnPiercingDash?.Invoke(direction);

        float angle = Mathf.Atan2(oldDirection.y, oldDirection.x) * Mathf.Rad2Deg;
        Quaternion warningRotation = Quaternion.Euler(0f, 0f, angle);

        Instantiate(PiercingDashWarning,transform.position, warningRotation);
        yield return new WaitForSeconds(1f);
        isDashing = true;

        float timer = 0f;
        bool hitObstacle = false;
        rb.linearVelocity = oldDirection * piercingDashSpeed;


        while (timer < maxDashDuration && !hitObstacle)
        {
            float rayDistance = (piercingDashSpeed * Time.fixedDeltaTime) + 0.1f;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, oldDirection, rayDistance, obstacleLayer);

            if (hit.collider != null)
            {
                Debug.Log("Hit Obstacle: " + hit.collider.gameObject.name);
                hitObstacle = true;
            }

            timer += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        rb.linearVelocity = Vector2.zero;
        isActionLocked = false;
        isDashing = false;

        EndPiercingDash?.Invoke();

        yield return new WaitForSeconds(0.2f);

    }
}
