using System.Collections;
using UnityEngine;
using UnityEngine.UI;         // Dành cho quái nhỏ (Slider)
using UnityEngine.UIElements; // Dành cho Boss (UXML)

public class EnemyStats : MonoBehaviour
{
    [Header("Health Settings")]
    public float health = 20;
    public float maxHealth;

    public bool invincible = false;
    [SerializeField] private float invincibilityDuration = 0.2f;

    [Header("Flash Settings")]
    [SerializeField] private Material flashMaterial; 
    [SerializeField] private float flashDuration = 0.15f;

    [Header("Knockback Settings")]
    public float knockbackForce = 0.5f;

    [Header("UI - Quái Nhỏ (Canvas Slider)")]
    public UnityEngine.UI.Slider healthSlider;

    [Header("UI - Boss (UI Toolkit)")]
    public UIDocument healthBarDocument;
    private VisualElement _healthFill;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Material originalMaterial;
    private Coroutine flashRoutine;

    [Header("Particle")]
    public ParticleSystem DeathPar;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        maxHealth = health;
        originalMaterial = sr.material;

        // 1. Khởi tạo UI cho Quái Nhỏ (Nếu có gắn Slider)
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = health;
        }

        // 2. Khởi tạo UI cho Boss (Nếu có gắn UIDocument)
        if (healthBarDocument != null && healthBarDocument.rootVisualElement != null)
        {
            var root = healthBarDocument.rootVisualElement;
            _healthFill = root.Q<VisualElement>("HealthFill"); 
            UpdateUXMLHealthBar();
        }
    }

    void Update()
    {
        // Giữ thanh máu Slider không bị xoay
        if (healthSlider != null && healthSlider.transform.parent != null)
        {
            healthSlider.transform.parent.rotation = Quaternion.identity;
        }

        // Giữ thanh máu Boss không bị xoay (Nếu đặt trong World Space)
        if (healthBarDocument != null)
        {
            healthBarDocument.transform.rotation = Quaternion.identity;
        }
    }

    public void TakeDamage(Vector2 hitSource, float damage)
    {
        if (invincible || health <= 0) return;

        health -= damage;
        
        TriggerFlash();

        // 3. Cập nhật UI ngay khi nhận sát thương
        UpdateHealthUI();

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

    // Hàm tổng hợp để cập nhật UI
    private void UpdateHealthUI()
    {
        // Nếu là quái nhỏ (có Slider) -> Cập nhật Slider
        if (healthSlider != null)
        {
            healthSlider.value = health;
        }

        // Nếu là Boss (có UXML) -> Cập nhật UXML
        UpdateUXMLHealthBar();
    }

    // Hàm cập nhật riêng cho UXML
    private void UpdateUXMLHealthBar()
    {
        if (_healthFill == null) return;

        float percentage = (health / maxHealth) * 100f;
        percentage = Mathf.Clamp(percentage, 0, 100); 

        _healthFill.style.width = Length.Percent(percentage);
    }

    private void TriggerFlash()
    {
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        sr.material = flashMaterial; 
        sr.color = Color.white; 

        yield return new WaitForSeconds(flashDuration);

        sr.material = originalMaterial; 
        flashRoutine = null;
    }

    private IEnumerator InvincibilityRoutine()
    {
        invincible = true;

        float timer = 0;
        float blinkInterval = 0.05f; 

        while (timer < invincibilityDuration)
        {
            sr.enabled = !sr.enabled; 
            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }

        sr.enabled = true; 
        invincible = false;
    }

    void Die()
    {
        if (DeathPar != null)
        {
            Instantiate(DeathPar, transform.position, Quaternion.identity);
        }
        
        // Ẩn thanh máu Boss khi chết (tùy chọn)
        if (healthBarDocument != null)
        {
            healthBarDocument.gameObject.SetActive(false);
        }

        Destroy(gameObject);
    }
}