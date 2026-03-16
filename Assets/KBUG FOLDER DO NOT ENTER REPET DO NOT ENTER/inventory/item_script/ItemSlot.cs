using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    [Header("Slot Data")]
    private ItemData itemData;
    private int quantity;

    [Header("Slot UI")]
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Image itemImage;
    [SerializeField] private GameObject selectedShader;

    [Header("Description UI")]
    [SerializeField] private Image itemDescriptionImage;
    [SerializeField] private TMP_Text itemDescriptionNameText;
    [SerializeField] private TMP_Text itemDescriptionText;

    private bool thisItemSelected;

    
    public void InitializeSlot()
    {
        EmptySlot();
    }

    public bool IsEmpty() => itemData == null;
    public bool IsFull() => itemData != null && quantity >= itemData.maxStackSize;
    public ItemData GetItemData() => itemData;

    public int AddItem(ItemData newData, int amountToAdd)
    {
        if (IsEmpty())
        {
            itemData = newData;
            itemImage.sprite = itemData.itemIcon;
            itemImage.enabled = true;
        }
        quantity += amountToAdd;
        if (quantity >= itemData.maxStackSize)
        {
            int extraItems = quantity - itemData.maxStackSize;
            quantity = itemData.maxStackSize;

            UpdateUI();
            return extraItems;
        }
        UpdateUI();
        return 0;


    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            
            OnLeftClick();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
          
            OnRightClick();
        }
    }

    public void OnLeftClick()
    {
        if (IsEmpty()) return;
        InventoryManager.Instance.DeselectAllSlots();

        selectedShader.SetActive(true);
        thisItemSelected = true;
        itemDescriptionNameText.text = itemData.itemName;
        itemDescriptionText.text = itemData.itemDescription;
        itemDescriptionImage.sprite = itemData.itemIcon;
        itemDescriptionImage.enabled = true;

    }
    public void OnRightClick()
    {
        if (!IsEmpty() && itemData.itemPrefab != null)
        {
            if (quantity == 1 && itemData.abilityScriptableObject != null)
            {
                RemoveAbilityFromPlayer(itemData.abilityScriptableObject);
            }

            quantity--;
            UpdateUI();
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                
                Vector3 dropOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
                Instantiate(itemData.itemPrefab, player.transform.position + dropOffset, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("No player detected");
            }

            if (quantity <= 0)
            {
                EmptySlot();
            }
        }
    }

    private void RemoveAbilityFromPlayer(Ability abilityToRemove)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        AbilityHolder holder = player.GetComponent<AbilityHolder>();
        for (int i = 0; i < holder.abilities.Length; i++)
        {
            if (holder.abilities[i].ability == abilityToRemove)
            {
                holder.abilities[i].ability = null;
                holder.abilities[i].state = AbilityHolder.AbilityState.ready;
                holder.abilities[i].cooldownTime = 0;
                holder.abilities[i].activeTime = 0;
                break;
            }
        }
    }
    public void Deselect()
    {
        selectedShader.SetActive(false);
        thisItemSelected = false;
    }
    private void EmptySlot()
    {
        itemData = null;
        quantity = 0;

        itemImage.sprite = null;
        itemImage.enabled = false;
        quantityText.enabled = false;

        if (thisItemSelected)
        {
            ClearDescription();
        }
    }
    private void UpdateUI()
    {
        quantityText.text = quantity.ToString();
        quantityText.enabled = true;
    }
    public void ClearDescription()
    {
        itemDescriptionNameText.text = "";
        itemDescriptionText.text = "";
        itemDescriptionImage.sprite = null;
        itemDescriptionImage.enabled = false; 
    }


}
