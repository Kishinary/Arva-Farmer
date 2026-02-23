using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Bắt buộc phải có để điều khiển Slider

public class EnemyStats : MonoBehaviour
{
    [Header("Health Settings")]
    public float health = 20;
    private float maxHealth;
    public bool invincible = false;

    [Header("Knockback Settings")]
    public float knockbackForce = 8f;

    [Header("UI References")]
    public Slider healthSlider; // Kéo Slider từ Hierarchy vào đây

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        maxHealth = health;

        // Khởi tạo thanh máu
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = health;
        }
    }

    void Update()
    {
        // Giữ thanh máu luôn nằm ngang ngay cả khi Enemy quay mặt (Flip)
        if (healthSlider != null)
        {
            healthSlider.transform.parent.rotation = Quaternion.identity;
        }

        // PHÍM TEST: Bấm Space để tự trừ 2 máu
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(Vector2.zero, 2);
        }
    }

    public void TakeDamage(Vector2 hitSource, float damage)
    {
        if (invincible || health <= 0) return;

        health -= damage;

        // Cập nhật giá trị Slider
        if (healthSlider != null)
        {
            healthSlider.value = health;
        }

        // Xử lý Knockback
        if (hitSource != Vector2.zero)
        {
            Vector2 knockbackDir = (transform.position - (Vector3)hitSource).normalized;
            rb.linearVelocity = Vector2.zero; // Reset vận tốc cũ
            rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
        }

        if (health <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(Invincibility());
        }
    }

    IEnumerator Invincibility()
    {
        invincible = true;
        
        // Hiệu ứng nhấp nháy đơn giản (tùy chọn)
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = new Color(1, 1, 1, 0.5f);
        
        yield return new WaitForSeconds(0.5f);
        
        if (sr != null) sr.color = Color.white;
        invincible = false;
    }

    void Die()
    {
        Debug.Log("Enemy Died!");
        // Có thể thêm hiệu ứng nổ ở đây
        Destroy(gameObject);
    }
}