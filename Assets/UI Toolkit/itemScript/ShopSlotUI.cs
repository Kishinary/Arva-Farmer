using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ShopSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    public Button buyButton;

    private ItemData _currentItem;

    private ShopItem currentItem;

    
    public void OnPointerEnter(PointerEventData eventData)
    {
        ShopManager.Instance.ShowDescription(currentItem);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        ShopManager.Instance.ClearDescription();
    }
    
    public void SetupSlot(ShopItem itemSetup)
    {
        currentItem = itemSetup;
        iconImage.sprite = currentItem.itemIcon;
        nameText.text = currentItem.itemName;
        priceText.text = currentItem.price.ToString() + " G";

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(OnBuyClicked);
    }
    private void OnBuyClicked()
    {
        ShopManager.Instance.BuyItem(currentItem);
    }
    public void HideSlot()
    {
        gameObject.SetActive(false);
    }


}
