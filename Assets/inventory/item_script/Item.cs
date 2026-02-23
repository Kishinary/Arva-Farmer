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

    private inventoryManager inventoryManager;

    void Start()
    {       

        inventoryManager = GameObject.Find("inventory_canvas").GetComponent<inventoryManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            //Debug.Log("Collided with player, adding item to inventory");
            int leftOverItems = inventoryManager.AddItem(itemName, quantity, sprite, itemDescription);
            
            if (leftOverItems <= 0)
            {
                GetComponent<Collider2D>().enabled = false;
                Destroy(gameObject);
            }
            else
            {
                quantity = leftOverItems;
            }
        }
    }


}
