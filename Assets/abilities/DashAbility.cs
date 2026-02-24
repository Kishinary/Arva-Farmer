using JetBrains.Annotations;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;



[CreateAssetMenu(fileName = "DashAbility")]
public class DashAbility : Ability
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float dashVelocity;

    public override void Activate(GameObject parent)
    {
        PlayerMovement movement = parent.GetComponent<PlayerMovement>();
        /*
        Debug.Log("Dashing is not added in movement script ----- KBUG -----");
        */
        Debug.Log(movement.norMovespeed);




    }
    public override void BeginCoolDown(GameObject parent)
    {
        
        
       
    }
}
