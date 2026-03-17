using UnityEngine;
using DG.Tweening;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("UI References")]


    public GameObject blackScreen;

  

    [Header("UI Animation")]
    public RectTransform inventoryPanel;
    public float animDuration = 0.4f;

    public ItemSlot[] itemSlots;
    public ItemSlot[] skillSlots;

    private bool _menuActivated;

  
    private GameObject player;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }
    private void Start()
    {
        player = GameObject.FindWithTag("Player");


        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].InitializeSlot();
        }
        for (int i = 0; i < skillSlots.Length; i++)
        {
            skillSlots[i].InitializeSlot();
        }


        if (itemSlots.Length > 0)
        {
            itemSlots[0].ClearDescription();
        }
        if (skillSlots.Length > 0) skillSlots[0].ClearDescription();


        blackScreen.SetActive(false);
        _menuActivated = false;

        inventoryPanel.gameObject.SetActive(false);

        inventoryPanel.anchoredPosition = new Vector2(inventoryPanel.anchoredPosition.x, -1000f);

        if (playerMovement == null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
        }
    }
    private void Update()
    {
      
        if (Input.GetButtonDown("Inventory"))
        {
            _menuActivated = !_menuActivated;

            inventoryPanel.DOKill();

            blackScreen.SetActive(_menuActivated);

            if (playerMovement != null)
            {
                if (_menuActivated)
                {
                    inventoryPanel.gameObject.SetActive(true);

                    inventoryPanel.anchoredPosition = new Vector2(inventoryPanel.anchoredPosition.x, -1000f);

                    inventoryPanel.DOAnchorPosY(0f, animDuration).SetEase(Ease.OutBack).SetUpdate(true);
                    playerMovement.DisablePlayerInput();
                }
                else
                {
                    inventoryPanel.DOAnchorPosY(-1000f, animDuration).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() =>
                    {
                        inventoryPanel.gameObject.SetActive(false);
                    });

                    playerMovement.EnablePlayerInput();
                }
            }
        }
        
    }
    public int AddItem(ItemData itemData, int amount)
    {
        ItemSlot[] targetSlots = itemData.isSkill ? skillSlots : itemSlots;

        int leftoverAmount = amount;

        for (int i = 0; i < targetSlots.Length; i++)
        {
            if (!targetSlots[i].IsFull() && targetSlots[i].GetItemData() == itemData)
            {
                leftoverAmount = targetSlots[i].AddItem(itemData, leftoverAmount);
                if (leftoverAmount <= 0) return 0;
            }
        }

        if (leftoverAmount > 0)
        {
            for (int i = 0; i < targetSlots.Length; i++)
            {
                if (targetSlots[i].IsEmpty())
                {
                    leftoverAmount = targetSlots[i].AddItem(itemData, leftoverAmount);
                    if (leftoverAmount <= 0) return 0;
                }
            }
        }

        return leftoverAmount;

    }
    public void DeselectAllSlots()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].Deselect();
        }
        for (int i = 0; i < skillSlots.Length; i++)
        {
            skillSlots[i].Deselect();
        }
    }

}
