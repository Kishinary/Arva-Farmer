using UnityEngine;

public class inventoryManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject InventoryMenu;
    private bool menuActivated;
    public ItemSlot[] itemSlot;

    void Start()
    {
        InventoryMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetButtonDown("Inventory"))
        {
            

            //turn on || off inventory menu
            if (menuActivated)
            {
                
                InventoryMenu.SetActive(false);
                menuActivated = false;
                // turn off inventory
            }
            else
            {
                
                
                InventoryMenu.SetActive(true);
                menuActivated = true;
                // turn on inventory
            }


        }
       
        
    }

    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
           
            if (itemSlot[i].isFull == false && itemSlot[i].itemName == itemName || itemSlot[i].quantity == 0)
            {
                int leftOverItems = itemSlot[i].AddItem(itemName, quantity, itemSprite, itemDescription);
                
                if (leftOverItems > 0)
                {


                    leftOverItems = AddItem(itemName, leftOverItems, itemSprite, itemDescription);
                   


                }
                
                return leftOverItems;
                



            }
        }
       
        return quantity;
    }


    public void DeselectAllSlots()
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            itemSlot[i].selectedShader.SetActive(false);
            itemSlot[i].thisItemSelected = false;
        }
    }












}
