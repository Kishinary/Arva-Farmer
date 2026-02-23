using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    
    [Header("UI Reference")]
    public Transform healthBarFill; // Kéo đối tượng HealthFill vào đây

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Hàm để gọi khi Enemy bị trúng đòn
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthBar();

        if (currentHealth <= 0) Die();
    }

    void UpdateHealthBar()
    {
        // Tính toán tỉ lệ phần trăm máu còn lại
        float healthPercentage = currentHealth / maxHealth;

        // Thay đổi Scale trục X của thanh Fill
        healthBarFill.localScale = new Vector3(healthPercentage, 1, 1);
    }

    void Die()
    {
        Debug.Log("Enemy đã chết!");
        Destroy(gameObject);
    }
}