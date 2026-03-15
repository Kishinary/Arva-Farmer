using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;






[CreateAssetMenu(menuName = "Ability/stunSkills")]
public class stunSkills : Ability
{

    public GameObject stunIndicator;
    private GameObject stunIndicatorInThis;




    public GameObject stunIndicatorFake;
    private GameObject stunFakeInThis;
    private stunFake stunFake;


    public float stunRange;
    private stunIndicator stunChangeRange;

    public float slowMultipleSpeed,timeSlow;



    private GameObject player;


    public override void BeginAim(GameObject parent, Transform playerTransform, Rigidbody2D rb)
    {
        
        stunIndicatorInThis = Instantiate(stunIndicator, playerTransform, rb);

        stunChangeRange = stunIndicatorInThis.GetComponent<stunIndicator>();
        stunChangeRange.radiusDistances = stunRange;

        

    }
    public override void DuringAim(GameObject parent, Transform playerTransform, Rigidbody2D rb)
    {

    }


    public override void Activate(GameObject parent, Transform playerTransform, Rigidbody2D rb)
    {
        
        Destroy(stunIndicatorInThis);

        stunFakeInThis = Instantiate(stunIndicatorFake, playerTransform, rb);
        stunFake = stunFakeInThis.GetComponent<stunFake>();
        stunFake.radiusDistances = stunRange;

        stunFake.timeSlow = this.timeSlow;
        stunFake.slowSpeed = this.slowMultipleSpeed;

        

    }
    public override void BeginCoolDown(GameObject parent)
    {
        
        
       
    }
}


