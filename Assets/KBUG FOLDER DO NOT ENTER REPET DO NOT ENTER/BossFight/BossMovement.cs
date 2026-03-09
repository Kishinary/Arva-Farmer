using System;
using System.Runtime.CompilerServices;
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
}

public class BossPhase1State : BossBaseState
{
    public BossPhase1State(BossMovement boss) : base(boss) { }

    public float timer = 2f;



    public override void EnterState()
    {
        //Debug.Log("Entering Phase 1");
        // Initialize phase 1 behavior
        boss.moveSpeed = 0.5f;
    }
    public override void UpdateState()
    {


        //jump ability of boss
        if (boss.isActionLocked) return;

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = 3f;
            PerformJump();

            return;

        }
        ChasePlayer();




    }

    


        // Implement phase 1 behavior (e.g., basic attacks, movement patterns)
   private void PerformJump()
        {
        boss.isActionLocked = true; // Lock actions during jump
        float jumpDuration = 1f;

        //hàm ChasePlayer failed, boss sẽ thực hiện jump attack để tiếp cận player
        //lấy direction từ boss đến player thay thế
        Vector2 targetPos = (Vector2)boss.player.transform.position;
        Vector2 currentPos = (Vector2)boss.transform.position;
        Vector2 jumpDirection = (targetPos - currentPos).normalized;

        boss.NotifyJumpAttackStart(jumpDirection, jumpDuration);


        //NOTICE: JUMP HEIGHT =  JUMP DURATION IN ANIMATION, IF YOU WANT TO CHANGE JUMP HEIGHT, CHANGE JUMP DURATION IN ANIMATION TOO
        boss.StartCoroutine(boss.jumpAbility.ExecuteJump(
            boss.Visual,
            (Vector2)boss.player.transform.position,
            jumpDuration,
            5f, () => {// Callback sau khi nhảy xong, có thể thêm hiệu ứng hoặc logic khác ở đây
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
        //Debug.Log("Chasing player. Current position: " + boss.transform.position);
        boss.NotifyMovement(direction);// Notify animation system of movement direction
        
    }




    public override void ExitState()
        {
            //Debug.Log("Exiting Phase 1");
            boss.isActionLocked = false;
            boss.NotifyJumpAttackEnd(); // Ensure any jump attack state is reset
            

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
        boss.moveSpeed = 5f; // Increase speed for phase 2
        GameObject newAura = GameObject.Instantiate(boss.auraBoss, boss.spriteTransform.position, Quaternion.identity);
        newAura.transform.SetParent(boss.spriteTransform); // Make the aura a child of the boss so it moves with it
        newAura.transform.SetParent(boss.spriteTransform, false);
        newAura.transform.localPosition = Vector3.zero;

    }
    public override void UpdateState()
    {



        if (boss.isActionLocked) return;

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = UnityEngine.Random.Range(2.5f, 4f);
            int randomSkillID = UnityEngine.Random.Range(0,2);
            //int randomSkillID = 1;


            switch (randomSkillID)
            {
                case 0:
                    Debug.Log("Boss decides to Jump!");
                    PerformJump();
                    break;
                case 1:
                    Debug.Log("Boss decides to Bite!");
                    PerformBite();
                    break;
            }

            return;
        }
        ChasePlayer();
    }
    public override void ExitState()
    {
        //Debug.Log("Exiting Phase 2");
        // Clean up phase 2 state if necessary
    }


    private void PerformJump()
    {
        boss.isActionLocked = true; // Lock actions during jump
        float jumpDuration = 1f;

        //hàm ChasePlayer failed, boss sẽ thực hiện jump attack để tiếp cận player
        //lấy direction từ boss đến player thay thế
        Vector2 targetPos = (Vector2)boss.player.transform.position;
        Vector2 currentPos = (Vector2)boss.transform.position;
        Vector2 jumpDirection = (targetPos - currentPos).normalized;

        boss.NotifyJumpAttackStart(jumpDirection, jumpDuration);


        //NOTICE: JUMP HEIGHT =  JUMP DURATION IN ANIMATION, IF YOU WANT TO CHANGE JUMP HEIGHT, CHANGE JUMP DURATION IN ANIMATION TOO
        boss.StartCoroutine(boss.jumpAbility.ExecuteJump(
            boss.Visual,
            (Vector2)boss.player.transform.position,
            jumpDuration,
            5f, () => {// Callback sau khi nhảy xong, có thể thêm hiệu ứng hoặc logic khác ở đây
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
            0.5f, // Thời gian lao tới (0.5s sẽ rất nhanh và nguy hiểm)
            () => {
                // Callback khi lao đến nơi
                boss.isActionLocked = false;
                // Thực hiện logic trừ máu hoặc trigger Animation "Bite_Impact" ở đây
                Debug.Log("Boss chomped the player!");
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
            public GameObject player; // Reference to the player for targeting
                                      //jump ability of boss
            public JumpAbility jumpAbility;
            public BiteAbility biteAbility;


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
    public GameObject dustSplashPrefab;
    public GameObject auraBoss;
    public Transform spriteTransform;


    void Start()
            {
        player = GameObject.FindWithTag("Player");

            // Initialize states
            phase1State = new BossPhase1State(this);
            phase2State = new BossPhase2State(this);

            ChangeState(phase1State);
        
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
