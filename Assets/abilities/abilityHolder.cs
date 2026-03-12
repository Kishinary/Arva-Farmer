using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class AbilityHolder : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    
    [Header("Boomerang Spread Settings")]
    public GameObject flyingBoomerangPrefabLeft;
    public GameObject flyingBoomerangPrefabRight;
    public GameObject flyingBoomerangPrefab;


    [System.Serializable]
    public class AbilitySlot
    {
        public Ability ability;
        public KeyCode key;

        // HideInInspector để ẩn các biến chạy ngầm khỏi giao diện Unity cho đỡ rối
        [HideInInspector] public AbilityState state = AbilityState.ready;
        [HideInInspector] public float cooldownTime;
        [HideInInspector] public float activeTime;
    }
    public enum AbilityState
    {
        ready,
        active,
        cooldown
    }
    [Header("Abilities Setup")]
    public AbilitySlot[] abilities;

    [Header("References")]
    [SerializeField] private GameObject player;


    private Transform playerTransform;
    private Rigidbody2D rb;




    //teleport
    [Header("TeleportReferences")]
    public TeleportAbility teleportAbility;



    private PlayerMovement movementScript;






    private void Awake()
    {
        playerTransform = player.transform;
        rb = player.GetComponent<Rigidbody2D>();
        movementScript = player.GetComponent<PlayerMovement>();
    }
    //Teleport
    
    private float teleportCooldownTimer = 0f;
    public GameObject indicatorTarget;
    public GameObject circleIndicatorTarget;
    public GameObject teleportEffect;

    private float moveSpeedOrigin;


    void Update()
    {
        for (int i = 0; i < abilities.Length; i++)
        {
            ProcessAbilityLogic(abilities[i]);
        }
        //teleport


        if (teleportCooldownTimer > 0)
        {
            teleportCooldownTimer -= Time.deltaTime;
        }
        else
        {
            // Khi Cooldown đã về 0 (Sẵn sàng)

            // Khi ĐANG GIỮ phím
            if (Input.GetKey(KeyCode.Space))
            {
                // Bật lại indicator (nếu bạn đã ẩn nó đi lúc trước)
                

                if (indicatorTarget != null && !indicatorTarget.activeSelf)
                {
                    circleIndicatorTarget.SetActive(true);
                    indicatorTarget.SetActive(true);
                }

                teleportAbility.Aim(rb, indicatorTarget);
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                TeleportHandler();
            }
        }


    }

    
    private void TeleportHandler()
    {
        GameObject teleportDust = Instantiate(teleportEffect, rb.position, Quaternion.identity);
        teleportDust.transform.position = rb.position;

        SpriteRenderer playerSprite = player.GetComponent<SpriteRenderer>();

        teleportAbility.Activate(gameObject, transform, rb);
        StartCoroutine(TeleportRecoveryRoutine(playerSprite, rb));

        
        


        teleportAbility.BeginCoolDown(gameObject);

        teleportCooldownTimer = teleportAbility.cooldownTime;


        if (indicatorTarget != null)
        {
            circleIndicatorTarget.SetActive(false);
            indicatorTarget.SetActive(false);
        }
    }
    private IEnumerator TeleportRecoveryRoutine(SpriteRenderer sprite, Rigidbody2D rb)
    {

        float freezeDuration = 0.5f; // Thời gian bị khóa di chuyển (giây)
        float blinkInterval = 0.1f;  // Tốc độ nhấp nháy (giây)
        float timer = 0f;


        Color originalColor = sprite.color;
        Color flashColor = new Color(2f, 2f, 2f, 2f);

        moveSpeedOrigin = movementScript.movespeed;
        movementScript.movespeed = 0;

        // 2. VÒNG LẶP NHẤP NHÁY
        bool isFlashing = false;
        while (timer < freezeDuration)
        {
            // Đảo qua lại giữa màu gốc và màu chớp trắng
            sprite.color = isFlashing ? originalColor : flashColor;
            isFlashing = !isFlashing;

            // Đợi một khoảng thời gian ngắn rồi tiếp tục vòng lặp
            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }


        movementScript.movespeed = moveSpeedOrigin;
        //return color afterward
        sprite.color = originalColor;


    }




    private void ProcessAbilityLogic(AbilitySlot slot)
    {
        // Bỏ qua nếu chưa gán kỹ năng vào ô này
        if (slot.ability == null) return;

        switch (slot.state)
        {
            case AbilityState.ready:
                if (Input.GetKeyDown(slot.key))
                {
                    slot.ability.Activate(gameObject, playerTransform, rb);
                    slot.state = AbilityState.active;
                    slot.activeTime = slot.ability.activeTime;
                }
                break;

            case AbilityState.active:
                if (slot.activeTime > 0)
                {
                    slot.activeTime -= Time.deltaTime;
                }
                else
                {
                    slot.ability.BeginCoolDown(gameObject);
                    slot.state = AbilityState.cooldown;
                    slot.cooldownTime = slot.ability.cooldownTime;
                }
                break;

            case AbilityState.cooldown:
                if (slot.cooldownTime > 0)
                {
                    slot.cooldownTime -= Time.deltaTime;
                }
                else
                {
                    slot.state = AbilityState.ready;
                }
                break;
        }
    }



}
/*
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class AbilityHolder : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Ability ability;
    float cooldownTime;
    float activeTime;


    [SerializeField] private GameObject player; // nhet player vao day

    private Transform playerTransform;

    private Rigidbody2D rb;
    enum AbilityState
    {
        ready,
        active,
        cooldown
    }
    AbilityState state = AbilityState.ready;

    public KeyCode key;

    private void Awake()
    {
        rb = player.GetComponent<Rigidbody2D>();
    }


    void Update()
    {
        switch (state)
        {
            case AbilityState.ready:
                if (Input.GetKeyDown(key))
                {
                    
                    playerTransform = player.transform;
                    
                    ability.Activate(gameObject, playerTransform,rb);
                    state = AbilityState.active;
                    activeTime = ability.activeTime;
                    
                }
                break;
            case AbilityState.active:
                
                
                    if(activeTime > 0)
                    {
                        
                        activeTime -= Time.deltaTime;
                    }
                    else
                    {   
                        
                        ability.BeginCoolDown(gameObject);
                        state = AbilityState.cooldown;
                        cooldownTime = ability.cooldownTime;
                    }
                
                break;
            case AbilityState.cooldown:
               
                    if (cooldownTime > 0)
                    {
                        
                        cooldownTime -= Time.deltaTime;
                    }
                    else
                    {   
                        
                    state = AbilityState.ready;
                    }
                
                break;
        }
    }
}

 * 
*/