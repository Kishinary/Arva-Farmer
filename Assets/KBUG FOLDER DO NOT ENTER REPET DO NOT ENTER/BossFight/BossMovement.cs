using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.XR;
using static UnityEngine.LightAnchor;



public abstract class BossBaseState
    {
        protected BossMovement boss;
        public BossBaseState(BossMovement bossController)
        {
            this.boss = bossController;
        }
        public abstract void EnterState();
        public abstract void UpdateState();
        public abstract void ExitState();



    public abstract void ExecuteComboAttack(BossCombo combo, int hitIndex);
}

public class BossPhase1State : BossBaseState
{
    public BossPhase1State(BossMovement boss) : base(boss) { }

    



    public override void EnterState()
    {
        //Debug.Log("Entering Phase 1");
        // Initialize phase 1 behavior
        boss.moveSpeed = 0.5f;
    }
    public override void UpdateState()
    {
        if (boss.isActionLocked == false)
        {
            ChasePlayer();
        }
    }
    public override void ExecuteComboAttack(BossCombo combo, int hitIndex)
    {
        if (boss.isActionLocked) return;

        BandType bandType = combo.bandType;

        float duration = 0f;
        if (combo.beatCount == 1 && hitIndex == 0)
        {
            duration = 1f + combo.beatTimes[hitIndex] - combo.beatTimes[hitIndex];
        }

        if (bandType == BandType.Kick)
        {

            /*if (hitIndex == 0 && combo.beatCount >= 2)
            {
                Debug.Log($"[TUNG CHIÊU] Nhịp {combo.bandType} thuộc Combo số {combo.comboID} ({combo.beatCount} nhịp).");
            }*/
            PerformAcidShoot();

            if (combo.beatCount == 1)
            {
                randomPerformRythym();
            }
            else if (combo.beatCount == 2)
            {
                PerformAcidShoot();
            }



        }
        else if (bandType == BandType.Bass)
        {
            /*if (hitIndex == 0 && combo.beatCount >= 2)
            {
                Debug.Log($"[TUNG CHIÊU] Nhịp {combo.bandType} thuộc Combo số {combo.comboID} ({combo.beatCount} nhịp).");
            }*/
            PerformAcidShoot();
            if (combo.beatCount == 1)
            {
                randomPerformRythym();
            }
            else if (combo.beatCount == 2)
            {
                PerformAcidShoot();
            }



        }
        else if (bandType == BandType.LowMid)
        {
            /*if (hitIndex == 0 && combo.beatCount >= 2)
            {
                Debug.Log($"[TUNG CHIÊU] Nhịp {combo.bandType} thuộc Combo số {combo.comboID} ({combo.beatCount} nhịp).");
            }*/
            PerformAcidShoot();
            if (combo.beatCount == 1)
            {
                randomPerformRythym();
            }
            else if (combo.beatCount == 2)
            {
                PerformAcidShoot();
            }

        }
        else if (bandType == BandType.HighMid)
        {
            /*if (hitIndex == 0 && combo.beatCount >= 2)
            {
                Debug.Log($"[TUNG CHIÊU] Nhịp {combo.bandType} thuộc Combo số {combo.comboID} ({combo.beatCount} nhịp).");
            }*/
            PerformAcidShoot();
            if (combo.beatCount == 1)
            {
                randomPerformRythym();
            }
            else if (combo.beatCount == 2)
            {
                PerformAcidShoot();
            }


        }
        else if (bandType == BandType.Treble)
        {
            /*if (hitIndex == 0 && combo.beatCount >= 2)
            {
                Debug.Log($"[TUNG CHIÊU] Nhịp {combo.bandType} thuộc Combo số {combo.comboID} ({combo.beatCount} nhịp).");
            }*/
            if (combo.beatCount == 1)
            {
                randomPerformRythym();
            }
            else if (combo.beatCount == 2)
            {
                PerformAcidShoot();
            }
            PerformAcidShoot();


        }
        if (combo.beatCount == 1 && hitIndex == 0)
        {
            int randomSkill = UnityEngine.Random.Range(1, 5);

            bool canJump = Time.time >= boss.lastJumpTime + boss.jumpCooldown;

            if (randomSkill == 1 && boss.isActionLocked == false && canJump)
            {
                boss.lastJumpTime = Time.time;
                PerformJump(duration);
            }else if(randomSkill == 1 && boss.isActionLocked == true)
            {
                randomSkill = UnityEngine.Random.Range(2, 5);
            }
            if (randomSkill == 2)
            {
                
            }
            if (randomSkill == 3)
            {
                
            }
            if (randomSkill == 4)
            {
                
            }

        }
    }
    private void randomPerformRythym()
    {
        int randomRanged = UnityEngine.Random.Range(1, 100);
        if (randomRanged <= 60)
        {
            PerformAcidShoot();
        }
        else
        {
            PerformRockLaserBeam();
        }
    }

    private void PerformRockLaserBeam()
    {
        
        boss.StartCoroutine(boss.rockLaserBeam.RockSummon(

            () =>
            {
                
            }
            ));
    }
    private void PerformAcidShoot()
    {
        

        //boss.NotifyEightWayShootStart();

        boss.StartCoroutine(boss.acidMovement.ExecuteEightWayShoot(

            boss.transform.position,() =>
            {
                
            }
            ));
    }


    // Implement phase 1 behavior (e.g., basic attacks, movement patterns)
    private void PerformJump(float jumpDuration)
    {
        boss.isActionLocked = true; // Lock actions during jump



        Vector2 targetPos = (Vector2)boss.player.transform.position;
        Vector2 currentPos = (Vector2)boss.transform.position;
        Vector2 jumpDirection = (targetPos - currentPos).normalized;

        GameObject.Instantiate(boss.jumpWarning, targetPos, Quaternion.identity);

        boss.NotifyJumpAttackStart(jumpDirection, jumpDuration);




        //NOTICE: JUMP HEIGHT =  JUMP DURATION IN ANIMATION, IF YOU WANT TO CHANGE JUMP HEIGHT, CHANGE JUMP DURATION IN ANIMATION TOO
        boss.StartCoroutine(boss.jumpAbility.ExecuteJump(
            boss.Visual,
            (Vector2)boss.player.transform.position,
            jumpDuration,
            5f, () => {
                //Debug.Log("Jump completed!");
                //Debug.Log((Vector2)boss.player.transform.position);
                boss.NotifyJumpAttackEnd();
                boss.isActionLocked = false;

                GameObject.Instantiate(boss.dustSplashPrefab, boss.transform.position, Quaternion.identity);




            }

            )
         );
    }




    private void ChasePlayer()
    {
        Vector2 direction = ((Vector2)boss.player.transform.position - (Vector2)boss.transform.position).normalized;
        boss.transform.position += (Vector3)(direction * boss.moveSpeed * Time.deltaTime);
        //Debug.Log("Chasing player. Current position: " + boss.player.transform.position);
        boss.NotifyMovement(direction);// Notify animation system of movement direction

    }




    public override void ExitState()
        {
        boss.isActionLocked = false;
        boss.NotifyJumpAttackEnd();
        boss.NotifyBiteAttackEnd();


        // Clean up phase 1 state if necessary

    }
}
public class BossPhase2State : BossBaseState
{


    public float timer = 2f;

    

    public BossPhase2State(BossMovement boss) : base(boss) { }
    public override void EnterState()
    {
        boss.enemyStats.health = boss.enemyStats.maxHealth; // Reset health to threshold for phase 2
        boss.moveSpeed = 2f; // Increase speed for phase 2
        GameObject newAura = GameObject.Instantiate(boss.auraBoss, boss.spriteTransform.position, Quaternion.identity);
        newAura.transform.SetParent(boss.spriteTransform); // Make the aura a child of the boss so it moves with it
        newAura.transform.SetParent(boss.spriteTransform, false);
        newAura.transform.localPosition = Vector3.zero;

    }
    public override void UpdateState()
    {

        if (!boss.isActionLocked)
        {
            ChasePlayer();
        }

    }
    public override void ExecuteComboAttack(BossCombo combo, int hitIndex)
    {
        if (boss.isActionLocked) return;

        float duration = 0.5f;
        


        if (combo.bandType == BandType.Kick)
        {
            
            if (Time.time >= boss.lastJumpTime + boss.jumpCooldown)
            {
                boss.lastJumpTime = Time.time;
                PerformJump(duration);
            }
            else 
            {
                PerformAcidShoot();
            }
        }
        
        else if (combo.bandType == BandType.Bass)
        {
            if (Time.time >= boss.lastBiteTime + boss.biteCooldown)
            {
                boss.lastBiteTime = Time.time;
                PerformBite();
            }
        }
       
        else
        {
            int randomRanged = UnityEngine.Random.Range(1, 100);
            if (randomRanged <= 60)
            {
                PerformAcidShoot();
            }
            else
            {
                PerformRockLaserBeam();
            }
        }
    }
    public override void ExitState()
    {
        boss.isActionLocked = false;
        boss.NotifyJumpAttackEnd();
        boss.NotifyBiteAttackEnd();
    }

    private void PerformRockLaserBeam()
    {

        boss.StartCoroutine(boss.rockLaserBeam.RockSummon(

            () =>
            {

            }
            ));
    }
    private void PerformAcidShoot()
    {


        //boss.NotifyEightWayShootStart();

        boss.StartCoroutine(boss.acidMovement.ExecuteEightWayShoot(

            boss.transform.position, () =>
            {

            }
            ));
    }
    private void PerformJump(float jumpDuration)
    {
        boss.isActionLocked = true; // Lock actions during jump



        Vector2 targetPos = (Vector2)boss.player.transform.position;
        Vector2 currentPos = (Vector2)boss.transform.position;
        Vector2 jumpDirection = (targetPos - currentPos).normalized;

        GameObject.Instantiate(boss.jumpWarning, targetPos, Quaternion.identity);

        boss.NotifyJumpAttackStart(jumpDirection, jumpDuration);




        //NOTICE: JUMP HEIGHT =  JUMP DURATION IN ANIMATION, IF YOU WANT TO CHANGE JUMP HEIGHT, CHANGE JUMP DURATION IN ANIMATION TOO
        boss.StartCoroutine(boss.jumpAbility.ExecuteJump(
            boss.Visual,
            (Vector2)boss.player.transform.position,
            jumpDuration,
            5f, () => {
                //Debug.Log("Jump completed!");
                //Debug.Log((Vector2)boss.player.transform.position);
                boss.NotifyJumpAttackEnd();
                boss.isActionLocked = false;

                GameObject.Instantiate(boss.dustSplashPrefab, boss.transform.position, Quaternion.identity);




            }

            )
         );
    }
    private void PerformBite()
    {
        boss.isActionLocked = true;
        
        Vector2 targetPos = (Vector2)boss.player.transform.position;

        boss.NotifyBiteAttackStart();

        boss.StartCoroutine(boss.biteAbility.ExecuteBite(
            targetPos,
            0.5f, 
            () => {
                boss.isActionLocked = false;
                boss.NotifyBiteAttackEnd();
            }
        ));
    }
   


    private void ChasePlayer()
    {
        Vector2 direction = ((Vector2)boss.player.transform.position - (Vector2)boss.transform.position).normalized;
        boss.transform.position += (Vector3)(direction * boss.moveSpeed * Time.deltaTime);
        //Debug.Log("Chasing player. Current position: " + boss.transform.position);
        boss.NotifyMovement(direction);// Notify animation system of movement direction

    }

}

public class BossMovement : MonoBehaviour
        {

    [Header("Combo Controller")]
    public BossCatching bossCatching;

    [HideInInspector]
    public GameObject player; // Reference to the player for targeting
                              //jump ability of boss
    [Header("Abilities")]
    public JumpAbility jumpAbility;
    public BiteAbility biteAbility;
    public AcidMovement acidMovement;
    public RockLaserConnectToBoss rockLaserBeam;


    public Transform Visual; // Visual representation of the boss for jump effect

    [Header("Stats")]
    public float moveSpeed;

    [Header("Phase Thresholds")]
    [Range(0f, 1f)]
    public float phase2Threshold = 0.5f; // Boss enters phase 2 at 50% HP

    //FSM manager
    private BossBaseState currentState;

    // States
    private BossPhase1State phase1State;
    private BossPhase2State phase2State;

    private bool isPhase2 = false;
            
    //health
    public EnemyStats enemyStats;


    public bool isActionLocked = false;

    // Effects
    [Header("Effect")]
    public GameObject dustSplashPrefab;
    public GameObject auraBoss;
    public Transform spriteTransform;
    public GameObject jumpWarning;

    [Header("Cooldowns")]
    public float jumpCooldown = 4f;
    public float biteCooldown = 4f;

    [HideInInspector]
    public float lastJumpTime = -999f;
    [HideInInspector]
    public float lastBiteTime = -999f;




    void Start()
            {
        player = GameObject.FindWithTag("Player");


        if(bossCatching != null)
        {
            bossCatching.OnBossAttackTriggered += HandleComboAttack;
        }

        // Initialize states
        phase1State = new BossPhase1State(this);
        phase2State = new BossPhase2State(this);

        ChangeState(phase1State);
        
        }
    private void HandleComboAttack(BossCombo combo, int hitIndex)
    {
        // Gọi hàm của state hiện tại
        currentState?.ExecuteComboAttack(combo, hitIndex);
    }
    private void OnDestroy()
    {
        // Hủy đăng ký Event để tránh lỗi
        if (bossCatching != null)
        {
            bossCatching.OnBossAttackTriggered -= HandleComboAttack;
        }
    }

    // Update is called once per frame
    void Update()
        {
        // Delegate logic cho state hiện tại xử lý
        currentState?.UpdateState();

        //health check để chuyển phase
        if(!isPhase2 && enemyStats.health <= enemyStats.maxHealth * phase2Threshold)
        {
            
            ChangeState(phase2State);
            Debug.Log("Boss entered Phase 2!");
            isPhase2 = true;
        }

    }


    public void ChangeState(BossBaseState newState)
        {
            //Debug.Log("Changing state to: " + newState.GetType().Name);
            if (currentState != null)
            {
                currentState.ExitState();
            }
            else
            {
                //Debug.Log("No current state to exit. 1");
            }
            currentState = newState;
            if (currentState != null)
            {
                currentState.EnterState();
            }
            else
            {
                //Debug.Log("No current state to exit. 2");
            }


        }

    //animator event
    public event Action onIdle;
    public event Action<Vector2> OnMove;
    public event Action<Vector2,float> OnJumpAttackStart;
    public event Action OnJumpAttackEnd;

    


    public void NotifyMovement(Vector2 direction)
        {
            //Debug.Log("Notifying movement: " + direction);
            OnMove?.Invoke(direction);
        }
    //jump attack
    public void NotifyJumpAttackStart(Vector2 direction, float jumpDuration)
    {
        //Debug.Log("Notifying jump attack start: " + direction);
        // Có thể thêm logic để thông báo cho animation hoặc các hệ thống khác biết rằng boss đang bắt đầu một đòn tấn công nhảy
        OnJumpAttackStart?.Invoke(direction, jumpDuration);
    }
    public void NotifyJumpAttackEnd()
    {
        //Debug.Log("Notifying jump attack end");
        // Có thể thêm logic để thông báo cho animation hoặc các hệ thống khác biết rằng boss đã kết thúc đòn tấn công nhảy
        OnJumpAttackEnd?.Invoke();
    }


    public event Action OnBiteAttackStart;
    public event Action OnBiteAttackEnd;
    public void NotifyBiteAttackStart()
    {
        OnBiteAttackStart?.Invoke();
    }
    public void NotifyBiteAttackEnd()
    {
        OnBiteAttackEnd?.Invoke();
    }



}
