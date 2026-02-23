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

    public void AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (itemSlot[i].isFull == false)
            {
                itemSlot[i].AddItem(itemName, quantity, itemSprite, itemDescription);
                return;
            }
            else if (itemSlot[i].itemName == itemName)
            {
                itemSlot[i].quantity += quantity;
                itemSlot[i].quantityText.text = itemSlot[i].quantity.ToString();
                return;
            }
        }
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
