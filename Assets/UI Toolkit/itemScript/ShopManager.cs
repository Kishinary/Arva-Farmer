using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }
    
    public RectTransform shopPanel;
    public float animDuration = 0.4f;
    public ShopSlotUI[] shopSlots;

    private Transform currentNPC;

    private GameObject player;
    private CurrencyManager currencyManager;

    public GameObject descriptionPanel;
    public Image descIcon;
    public TextMeshProUGUI descName;
    public TextMeshProUGUI descText;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        player = GameObject.FindWithTag("Player");
        currencyManager = player.GetComponent<CurrencyManager>();
    }

    private void Start()
    {
        shopPanel.gameObject.SetActive(false);
        shopPanel.anchoredPosition = new Vector2(shopPanel.anchoredPosition.x, 1000f);
    }
    public void OpenShop(ShopItem[] itemsToSell, Transform npcTransform)
    {
        currentNPC = npcTransform;


        for (int i = 0; i < shopSlots.Length; i++)
        {
            if (shopSlots[i] == null)
            {
                continue; 
            }
            if (i < itemsToSell.Length)
            {
                shopSlots[i].gameObject.SetActive(true);
                shopSlots[i].SetupSlot(itemsToSell[i]);
            }
            else
            {
                shopSlots[i].HideSlot();
            }
        }
        shopPanel.gameObject.SetActive(true);
        shopPanel.DOKill();
        shopPanel.DOAnchorPosY(0f, animDuration).SetEase(Ease.OutBack).SetUpdate(true);

    }
    public void CloseShop()
    {
        shopPanel.DOKill();
        shopPanel.DOAnchorPosY(1000f, animDuration).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() =>
        {
            shopPanel.gameObject.SetActive(false);
            currentNPC = null;
        });
        
    }
    public void BuyItem(ShopItem shopItem)
    {
        if (currencyManager.currentMoney >= shopItem.price)
        {
            currencyManager.currentMoney -= shopItem.price;
            if (shopItem.dropPrefab != null && currentNPC != null)
            {
                float dropDistance = Random.Range(1.0f, 2.0f);
                Vector2 randomDirection = Random.insideUnitCircle.normalized;
                Vector3 spawnOffset = new Vector3(randomDirection.x, randomDirection.y, 0f) * dropDistance;
                Vector3 spawnPos = currentNPC.position + spawnOffset;

                Instantiate(shopItem.dropPrefab, spawnPos, Quaternion.identity);
            }
        }
        else
        {
            Debug.LogWarning("Not enough");
        }
    }

    public void ShowDescription(ShopItem item)
    {
        descriptionPanel.SetActive(true); 
        descIcon.sprite = item.itemIcon;
        descName.text = item.itemName;
        descText.text = item.description;
    }

    public void ClearDescription()
    {
        descriptionPanel.SetActive(false);
    }


}
