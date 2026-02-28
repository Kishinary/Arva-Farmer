using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStats : MonoBehaviour
{
    [Header("Health Settings")]
    public float health = 20;
    private float maxHealth;
    public bool invincible = false;
    [SerializeField] private float invincibilityDuration = 0.05f;

    [Header("Flash Settings")]
    [SerializeField] private Material flashMaterial; 
    [SerializeField] private float flashDuration = 0.15f;

    [Header("Knockback Settings")]
    public float knockbackForce = 8f;

    [Header("UI References")]
    public Slider healthSlider;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Material originalMaterial;
    private Coroutine flashRoutine;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        
        maxHealth = health;
        originalMaterial = sr.material; // Lưu Material gốc chuẩn

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = health;
        }
    }

    void Update()
    {
        // Giữ thanh máu không bị xoay theo Enemy
        if (healthSlider != null && healthSlider.transform.parent != null)
        {
            healthSlider.transform.parent.rotation = Quaternion.identity;
        }

        // Test phím Space
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(Vector2.zero, 2);
        }
    }

    public void TakeDamage(Vector2 hitSource, float damage)
    {
        if (invincible || health <= 0) return;

        health -= damage;
        
        // Luôn ưu tiên hiệu ứng Flash White khi trúng đòn
        TriggerFlash();

        if (healthSlider != null)
        {
            healthSlider.value = health;
        }

        // Xử lý Knockback
        if (hitSource != Vector2.zero)
        {
            Vector2 knockbackDir = (transform.position - (Vector3)hitSource).normalized;
            rb.linearVelocity = Vector2.zero; 
            rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
        }

        if (health <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityRoutine());
        }
    }

    private void TriggerFlash()
    {
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        // Bước 1: Hiện màu trắng tinh
        sr.material = flashMaterial; 
        sr.color = Color.white; // Đảm bảo Alpha luôn là 1 khi Flash

        yield return new WaitForSeconds(flashDuration);

        // Bước 2: Trả về Material gốc ngay lập tức
        sr.material = originalMaterial; 
        flashRoutine = null;
    }

    private IEnumerator InvincibilityRoutine()
    {
        invincible = true;

        // Hiệu ứng hình ảnh khi bất tử: Nhấp nháy nhẹ (tùy chọn)
        // Thay vì chỉnh Alpha 0.5 cố định, ta có thể cho quái nhấp nháy 
        float timer = 0;
        while (timer < invincibilityDuration)
        {
            // Nếu không muốn nhấp nháy, bạn có thể xóa đoạn switch color này
            // sr.enabled = !sr.enabled; // Cách nhấp nháy cổ điển (ẩn/hiện)
            yield return new WaitForSeconds(0.05f);
            timer += 0.05f;
        }

        sr.enabled = true; // Đảm bảo cuối cùng Sprite luôn hiện
        invincible = false;
    }

    void Die()
    {
        // Có thể thêm hiệu ứng nổ tại đây
        Destroy(gameObject);
    }
}