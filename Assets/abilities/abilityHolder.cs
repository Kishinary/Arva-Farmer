using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AbilityHolder : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Ability ability;
    float cooldownTime;
    float activeTime;

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
                    ability.Activate(gameObject);
                    state = AbilityState.active;
                    activeTime = ability.activeTime;
                }
                break;
            case AbilityState.active:
                if (activeTime > 0)
                {
                    if(activeTime > 0)
                    {
                        activeTime -= Time.deltaTime;
                    }
                    else
                    {
                        state = AbilityState.cooldown;
                        cooldownTime = ability.cooldownTime;
                    }
                }
                break;
            case AbilityState.cooldown:
                if (activeTime > 0)
                {
                    if (cooldownTime > 0)
                    {
                        cooldownTime -= Time.deltaTime;
                    }
                    else
                    {
                        state = AbilityState.ready;
                    }
                }
                break;
        }
    }
}
