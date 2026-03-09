using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;






[CreateAssetMenu(menuName = "Ability/DashAbility")]
public class TeleportAbility : Ability
{
    

    [SerializeField] private float maxTeleportDistance = 5f;
    [SerializeField] private float checkRadius = 0.5f;
    [SerializeField] private LayerMask obstacleLayer;
    public ParticleSystem burst;
    private Vector2 currentPosition;
    


    public override void Activate(GameObject parent, Transform playerTransform, Rigidbody2D rb)
    {
        Instantiate(burst, playerTransform.position, Quaternion.identity);
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = -Camera.main.transform.position.z;

        GameObject indicatorTarget = GameObject.FindWithTag("indicatorTarget");

        


      
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        Debug.Log("Vị trí thế giới của Chuột: " + mouseWorldPosition);

        currentPosition = rb.position;

        Vector2 directionToMouse = mouseWorldPosition - currentPosition;

        Vector2 teleportVector = Vector2.ClampMagnitude(directionToMouse, maxTeleportDistance);

        float distance = teleportVector.magnitude;
        Vector2 direction = teleportVector.normalized;

        RaycastHit2D hit = Physics2D.CircleCast(currentPosition, checkRadius, direction, distance, obstacleLayer);



        Vector2 targetPosition;

        if (hit.collider != null)
        {
            // Obstacle detected, teleport to the point just before the obstacle
            float safeDistance = distance * hit.fraction;
            targetPosition = currentPosition + (direction * safeDistance);
        } else
        {
            // Clear to teleport full distance
            targetPosition = currentPosition + teleportVector;
        }
        rb.position = targetPosition;

       

    }
    public override void BeginCoolDown(GameObject parent)
    {
       
    }
}


