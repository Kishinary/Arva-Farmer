using UnityEngine;


[CreateAssetMenu(fileName = "New Heal Potion", menuName = "Abilities/Consumables/YenSao")]
public class YenSao : Ability
{

    
    public float speedIncreasePercent = 100f;
    public float duration = 300f;

    public override void Activate(GameObject parent, Transform playerTransform, Rigidbody2D rb)
    {
        GameObject player = GameObject.FindWithTag("Player");
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        playerMovement.ApplySpeedPlayer(1f + speedIncreasePercent, duration);
    }


}
