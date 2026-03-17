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


    public GameObject tractorSkill;


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
        aiming,
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
        player = GameObject.FindGameObjectWithTag("Player");

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

    //checking if teleport
    private bool isAimingTeleport = false;

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
            isAimingTeleport = false;
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!IsAnyAbilityInUse())
            {
                isAimingTeleport = true;
            }
        }
       

        if (Input.GetKey(KeyCode.Space))
            {
                
                

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
            isAimingTeleport = false;
        }
        


    }

    private bool IsAnyAbilityInUse(AbilitySlot currentSlotToCheck = null)
    {
        if (isAimingTeleport) return true;

        foreach (var slot in abilities)
        {
            if (slot == currentSlotToCheck) continue;
            if (slot.state == AbilityState.aiming || slot.state == AbilityState.active)
            {
                return true;
            }

        }
        return false;
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

        float freezeDuration = 0.25f; 
        float blinkInterval = 0.3f;  
        float timer = 0f;


        Color originalColor = sprite.color;
        Color flashColor = new Color(1.8f, 1.8f, 1.8f, 1.8f);



        movementScript.ApplySlow(0f,freezeDuration);

       
        bool isFlashing = false;
        while (timer < freezeDuration)
        {
            sprite.color = isFlashing ? originalColor : flashColor;
            isFlashing = !isFlashing;

            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }


        sprite.color = originalColor;


    }




    private void ProcessAbilityLogic(AbilitySlot slot)
    {
        // Bỏ qua nếu chưa gán kỹ năng vào ô này
        if (slot.ability == null) return;

        switch (slot.state)
        {
            case AbilityState.ready:
                if (Input.GetKeyDown(slot.key) && !IsAnyAbilityInUse())
                {
                    slot.ability.BeginAim(gameObject, playerTransform, rb);
                    slot.state = AbilityState.aiming;
                  
                }
                break;
            case AbilityState.aiming:
                if (Input.GetKey(slot.key))
                {
                    slot.ability.DuringAim(gameObject, playerTransform, rb);
                }
                if (Input.GetKeyUp(slot.key))
                {
                    slot.ability.Activate(gameObject, playerTransform, rb);
                    slot.activeTime = slot.ability.activeTime;
                    slot.state = AbilityState.active;
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
