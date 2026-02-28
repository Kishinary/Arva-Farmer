using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField]
    private string itemName;

    [SerializeField]
    private int quantity;

    [SerializeField]
    private Sprite sprite;

    [TextArea]
    [SerializeField] 
    private string itemDescription;

    [SerializeField]
    private Ability abilityScriptableObject;



    private inventoryManager inventoryManager;

    

    void Start()
    {

        GameObject canvasObj = GameObject.Find("inventory_canvas");
        if (canvasObj != null)
        {
            inventoryManager = canvasObj.GetComponent<inventoryManager>();
        }else
        {
                       Debug.LogError("inventoryManager not found on inventory_canvas!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && inventoryManager != null)
        {
           

            int leftOverItems = inventoryManager.AddItem(itemName, quantity, sprite, itemDescription);
            
            if (leftOverItems <= 0)
            {
                AbilityHolder abilityHolder = collision.GetComponent<AbilityHolder>();
                if (abilityHolder != null && abilityScriptableObject != null)
                {
                    
                    EquipAbilityToPlayer(abilityHolder);
                }


                GetComponent<Collider2D>().enabled = false;
                Destroy(gameObject);
            }
            else
            {
                quantity = leftOverItems;
            }
        }
    }
    private void EquipAbilityToPlayer(AbilityHolder holder)
    {
        bool isEquipped = false;
        
        for (int i = 0; i < holder.abilities.Length; i++)
        {
            
            if (holder.abilities[i].ability == null)
            {
                holder.abilities[i].ability = abilityScriptableObject;

                isEquipped = true;
                
                break;
            }
        }
        if (!isEquipped)
        {
            Debug.LogWarning("Cant equip more!");
            
        }
    }

}
