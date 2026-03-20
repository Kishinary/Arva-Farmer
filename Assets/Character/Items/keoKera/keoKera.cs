using UnityEngine;


[CreateAssetMenu(fileName = "New Heal Potion", menuName = "Abilities/Consumables/keoKera")]
public class keoKera : Ability
{

    public float healAmount = 20f;
    public override void Activate(GameObject parent, Transform playerTransform, Rigidbody2D rb)
    {
        GameObject player = GameObject.FindWithTag("Player");
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        playerHealth.Heal(healAmount);
    }
}
