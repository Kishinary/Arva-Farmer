using UnityEngine;

public class ShopNPC : MonoBehaviour
{
    [Header("Shop Inventory")]
    public ShopItem[] itemsForSale;

    private bool isPlayerInRange = false;

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Q))
        {
            if (ShopManager.Instance.shopPanel.gameObject.activeSelf)
            {
                ShopManager.Instance.CloseShop();
            }
            else
            {
                ShopManager.Instance.OpenShop(itemsForSale, transform);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            ShopManager.Instance.CloseShop();
        }
    }


}
