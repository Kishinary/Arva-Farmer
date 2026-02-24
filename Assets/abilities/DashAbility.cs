using JetBrains.Annotations;
using UnityEngine;



[CreateAssetMenu(fileName = "DashAbility")]
public class DashAbility : Ability
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float dashVelocity;

    public override void Activate(GameObject parent)
    {
        Debug.Log("Ability Activated: " + name);
        PlayerMovement movement = parent.GetComponent<PlayerMovement>();
        Rigidbody2D rigidbody = parent.GetComponent<Rigidbody2D>();

        rigidbody.velocity = movement.moveInput.normalized * dashVelocity; 
    }
    
}
