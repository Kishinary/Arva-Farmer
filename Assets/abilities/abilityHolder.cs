using UnityEngine;
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


    void Update()
    {
        switch (state)
        {
            case AbilityState.ready:
                if (Input.GetKeyDown(key))
                {
                    playerTransform = player.transform;
                    rb = player.GetComponent<Rigidbody2D>();
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
