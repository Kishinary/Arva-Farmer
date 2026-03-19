using UnityEngine;


[CreateAssetMenu(fileName = "New Heal Potion", menuName = "Abilities/Consumables/BaMia")]
public class BaMia : Ability
{

    public float healAmount = 20f;
    public float speedIncreasePercent = 0.1f;
    public float duration = 3f;
    public override void Activate(GameObject parent, Transform playerTransform, Rigidbody2D rb)
    {
        GameObject player = GameObject.FindWithTag("Player");
        
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        playerHealth.Heal(healAmount);
        playerMovement.ApplySpeedPlayer(1f + speedIncreasePercent, duration); 
    }
}
