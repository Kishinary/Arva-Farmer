using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.XR;
using static UnityEngine.LightAnchor;





public class BossmovementInCutScene : MonoBehaviour
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
  


    public bool isActionLocked = false;

    // Effects

    public event Action OnJumpAttackEnd;
    public event Action<Vector2, float> OnJumpAttackStart;

    public event Action onIdle;
    public event Action<Vector2> OnMove;



    public GameObject dustSplashPrefab;

    void Start()
    {



        

    }

   
    private void PerformJump(Transform point)
    {
        isActionLocked = true; // Lock actions during jump
        float jumpDuration = 1f;

        //hàm ChasePlayer failed, boss sẽ thực hiện jump attack để tiếp cận player
        //lấy direction từ boss đến player thay thế
        Vector2 targetPos = (Vector2)player.transform.position;
        Vector2 currentPos = (Vector2)transform.position;
        Vector2 jumpDirection = (targetPos - currentPos).normalized;

        NotifyJumpAttackStart(jumpDirection, jumpDuration);


        //NOTICE: JUMP HEIGHT =  JUMP DURATION IN ANIMATION, IF YOU WANT TO CHANGE JUMP HEIGHT, CHANGE JUMP DURATION IN ANIMATION TOO
        StartCoroutine(jumpAbility.ExecuteJump(
            Visual,
            point.position,
            jumpDuration,
            5f, () => {// Callback sau khi nhảy xong, có thể thêm hiệu ứng hoặc logic khác ở đây
                //Debug.Log("Jump completed!");
                //Debug.Log((Vector2)boss.player.transform.position);
                NotifyJumpAttackEnd();
                isActionLocked = false;

                GameObject.Instantiate(dustSplashPrefab, transform.position, Quaternion.identity);


            }

            )
         );

    }
  
    public float timer = 0.5f;
    // Update is called once per frame
    public Transform point1;
    public Transform point2;

    private int pointNumber = 1;
    void Update()
    {
        if (isActionLocked) return;
    }

    public void jumpToPoint1()
    {
       

        PerformJump(point1);
    }
    public void jumpToPoint2()
    {
        PerformJump(point2);
    }
    public void faceToCamera()
    {
        
        onIdle?.Invoke();
    }



    //animator event



    //jump attack




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


}
