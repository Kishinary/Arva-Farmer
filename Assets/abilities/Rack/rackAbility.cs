using UnityEngine;



[CreateAssetMenu(menuName = "Ability/Rack")]
public class rackAbility : Ability
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Activate(GameObject parent, Transform playerTransform, Rigidbody2D rb)
    {
        Debug.Log("Rack activated at position: " + playerTransform.position);
        // Implement the logic for the Rack ability here
    }
    public override void BeginCoolDown(GameObject parent)
    {
        // Implement the logic for beginning the cooldown of the Rack ability here
    }
}
