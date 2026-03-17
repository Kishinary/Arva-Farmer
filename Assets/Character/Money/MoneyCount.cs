using UnityEngine;
using TMPro;
using System.Collections;

public class AnimatedWallet : MonoBehaviour
{
    public TextMeshProUGUI walletText;

    [Header("Animation Settings")]
    public float ticksPerSecond = 50f; // Constant speed: how many numbers per second
    public float shakeAmount = 5f;

    private int displayedMoney = 0;
    private Coroutine countCoroutine;
    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.localPosition;
        if (CurrencyManager.Instance != null)
        {
            displayedMoney = CurrencyManager.Instance.currentMoney;
            walletText.text = displayedMoney.ToString();
        }
    }

    void Update()
    {
        // Detect if the manager's balance has changed
        if (CurrencyManager.Instance != null && CurrencyManager.Instance.currentMoney != displayedMoney)
        {
            if (countCoroutine != null) StopCoroutine(countCoroutine);
            countCoroutine = StartCoroutine(UpdateWalletAnimation(CurrencyManager.Instance.currentMoney));
        }
    }

    IEnumerator UpdateWalletAnimation(int targetAmount)
    {
        // While we haven't reached the target number...
        while (displayedMoney != targetAmount)
        {
            // 1. Constant Speed Update: Use MoveTowards so it never slows down at the end
            // This moves the number by a set amount every frame
            float step = ticksPerSecond * Time.deltaTime;
            displayedMoney = (int)Mathf.MoveTowards(displayedMoney, targetAmount, Mathf.Max(1f, step));

            walletText.text = displayedMoney.ToString();

            // 2. Shake Effect: Jitter the UI position while the number is changing
            transform.localPosition = originalPosition + (Vector3)Random.insideUnitCircle * shakeAmount;

            yield return null;
        }

        // Snap back to original position when done
        transform.localPosition = originalPosition;
    }
}