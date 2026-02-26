using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;


public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    //===============ITEM DATA===============//
    public string itemName;
    public int quantity;
    public Sprite itemSprite;
    public bool isFull;
    public string itemDescription;
    [SerializeField]
    private int maxNumberOfItems;


   


    //===============ITEM SLOT===============//
    
    public TMP_Text quantityText;

    [SerializeField]
    private Image itemImage;


    //===============ITEM DESCRIPTION SLOT===============//
    public Image itemDescriptionImage;
    public TMP_Text itemDescriptionNameText;
    public TMP_Text itemDescriptionText;

    //===============nothing===============//
    public GameObject selectedShader;

    public bool thisItemSelected;

    private inventoryManager inventoryManager;

    private void Start()
    {
        inventoryManager = GameObject.Find("inventory_canvas").GetComponent<inventoryManager>();
    }




    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        //check if the itemslot is full 
        if (isFull)
        {
            
            return quantity;
        }
        //Update Name
        this.itemName = itemName;
        


        //Update Sprite
        this.itemSprite = itemSprite;
        itemImage.sprite = this.itemSprite;



        //Update description
        this.itemDescription = itemDescription;
        


        //Update quantity
        this.quantity += quantity;
        if (this.quantity >= maxNumberOfItems)
        {
            quantityText.text = maxNumberOfItems.ToString();
            quantityText.enabled = true;
            isFull = true;




            //Return the LEFTOVER
            int extraItems = this.quantity - maxNumberOfItems;
            this.quantity = maxNumberOfItems;
            return extraItems;
        }
        //Update Quantity Text
        quantityText.text = this.quantity.ToString();
        quantityText.enabled = true;
        



        return 0;

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            //Debug.Log("Left Clicked");
            OnLeftClick();
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            //Debug.Log("Right Clicked");
            OnRightClick();
        }
    }



    public void OnLeftClick()
    {
        inventoryManager.DeselectAllSlots();
        selectedShader.SetActive(true);
        thisItemSelected = true;
        itemDescriptionNameText.text = itemName;
        itemDescriptionText.text = itemDescription;
        itemDescriptionImage.sprite = itemSprite;
    }
    public void OnRightClick()
    {
        Debug.Log("Right Clicked");
    }
}
