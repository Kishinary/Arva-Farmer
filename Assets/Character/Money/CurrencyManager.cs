using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance; // Singleton pattern to access from anywhere
    public int currentMoney;

    private void Awake()
    {
        // Ensure only one manager exists and it survives scene changes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        Debug.Log("Total Money: " + currentMoney);
    }

    public bool TrySpendMoney(int cost)
    {
        if (currentMoney >= cost)
        {
            currentMoney -= cost;
            Debug.Log("Spent: " + cost + " | Remaining: " + currentMoney);
            return true; // Purchase successful
        }

        Debug.Log("Not enough money!");
        return false; // Purchase failed
    }
}