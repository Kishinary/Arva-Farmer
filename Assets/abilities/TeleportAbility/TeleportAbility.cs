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
    [SerializeField] private float checkRadius = 0.2f;
    [SerializeField] private LayerMask obstacleLayer;
    private Vector2 currentPosition;

    

    private Vector2 CalculateTargetPosition(Vector2 currentPosition)
    {
       
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = -Camera.main.transform.position.z;

        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        Vector2 directionToMouse = mouseWorldPosition - currentPosition;
        Vector2 teleportVector = Vector2.ClampMagnitude(directionToMouse, maxTeleportDistance);

        float distance = teleportVector.magnitude;
        Vector2 direction = teleportVector.normalized;

        RaycastHit2D hit = Physics2D.CircleCast(currentPosition, checkRadius, direction, distance, obstacleLayer);



        if (hit.collider != null)
        {
            
            float safeDistance = distance * hit.fraction;
            
            return currentPosition + (direction * safeDistance);
        }
        else
        {
            return currentPosition + teleportVector;
        }


    }
    public void Aim(Rigidbody2D rb, GameObject indicatorTarget)
    {
       
        Vector2 targetPosition = CalculateTargetPosition(rb.position);
        
        if (indicatorTarget != null)
        {
         
            indicatorTarget.transform.position = targetPosition;
        }else
        {
            Debug.Log("No indicatorTarget");
        }
    }


    public override void Activate(GameObject parent, Transform playerTransform, Rigidbody2D rb)
    {
        Vector2 targetPosition = CalculateTargetPosition(rb.position);
        
        rb.position = targetPosition;
       

    }
    public override void BeginCoolDown(GameObject parent)
    {
        
        
       
    }
}


