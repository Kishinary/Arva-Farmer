using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public class Item : MonoBehaviour
{
    [Header("Item Config")]
    [SerializeField] private ItemData itemData; 
    [SerializeField] private int quantity = 1;

    [SerializeField] private float floatSpeed = 3f;
    private float floatHeight = 0.15f;

    private Vector3 _startPos;

  

    private void Start()
    {
        _startPos = transform.position;
    }

    private void Update()
    {
        float newY = _startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;

       
        transform.position = new Vector3(_startPos.x, newY, _startPos.z);

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            

            int leftOver = InventoryManager.Instance.AddItem(itemData, quantity);
            
            if (leftOver <= 0)
            {

                if (itemData.abilityScriptableObject != null)
                {
                    AbilityHolder abilityHolder = collision.GetComponent<AbilityHolder>();
                    if (abilityHolder != null)
                    {
                        EquipAbilityToPlayer(abilityHolder);
                    }
                }
                Destroy(gameObject);
            }
            else
            {
                
                quantity = leftOver;
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
                holder.abilities[i].ability = itemData.abilityScriptableObject;
                isEquipped = true;
                
                break;
            }
        }

        if (!isEquipped)
        {
            Debug.LogWarning("CantEquipt, full!");
        }
    }

}
