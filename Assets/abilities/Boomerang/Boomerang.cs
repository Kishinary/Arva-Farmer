using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;






[CreateAssetMenu(menuName = "Ability/Boomerang")]
public class Boomerang : Ability
{



    private GameObject player;
    [Header("BoomerangPrefab")]
    private GameObject flyingBoomerangPrefab;
    private GameObject flyingBoomerangPrefabLeft;
    private GameObject flyingBoomerangPrefabRight;


    public override void Activate(GameObject parent, Transform playerTransform, Rigidbody2D rb)
    {
        
        player = GameObject.FindGameObjectWithTag("Player");
       
        flyingBoomerangPrefab = player.GetComponent<AbilityHolder>().flyingBoomerangPrefab;
        flyingBoomerangPrefabLeft = player.GetComponent<AbilityHolder>().flyingBoomerangPrefabLeft;
        flyingBoomerangPrefabRight = player.GetComponent<AbilityHolder>().flyingBoomerangPrefabRight;




        GameObject boomerang_straight = Instantiate(flyingBoomerangPrefab, playerTransform.position, Quaternion.identity);
        GameObject boomerang_left = Instantiate(flyingBoomerangPrefabLeft, playerTransform.position, Quaternion.identity);
        GameObject boomerang_right = Instantiate(flyingBoomerangPrefabRight, playerTransform.position, Quaternion.identity);


        boomerang_straight.SetActive(true);
        boomerang_left.SetActive(true);
        boomerang_right.SetActive(true);

        Debug.Log("Boomerang activated at position: " + playerTransform.position);




    }

    
    public override void BeginCoolDown(GameObject parent)
    {
        
        
       
    }
}


