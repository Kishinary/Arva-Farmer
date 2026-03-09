using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements; 

public class EnemyStats : MonoBehaviour
{
    [Header("Health Settings")]
    public float health = 20;
    public float maxHealth;

    public bool invincible = false;
    [SerializeField] private float invincibilityDuration = 0.02f;

    [Header("Flash Settings")]
    [SerializeField] private Material flashMaterial; 
    [SerializeField] private float flashDuration = 0.15f;

    [Header("Knockback Settings")]
    public float knockbackForce = 0.5f;

    [Header("UI - Quái Nhỏ")]
    public UnityEngine.UI.Slider healthSlider;

    [Header("UI - Boss (UXML)")]
    public UIDocument healthBarDocument;
    [SerializeField] private float ghostDelay = 0.3f; // Thời gian chờ trước khi thanh trắng tụt
    
    private VisualElement _healthFill;
    private VisualElement _ghostFill;
    private Coroutine _ghostCoroutine; // Lưu trữ coroutine của thanh trắng

    private Rigidbody2D rb;
    
    [Header("Components")]
    [SerializeField] private SpriteRenderer sr; 
    private Material originalMaterial;
    private Coroutine flashRoutine;

    [Header("Particle")]
    public ParticleSystem DeathPar;

    [Header("Hitbox")]
    private Collider2D hitbox;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        hitbox = GetComponent<Collider2D>();

        maxHealth = health;
        originalMaterial = sr.material;
    }

    void Start()
    {
        // 1. Nếu là Boss (Có UIDocument)
        if (healthBarDocument != null)
        {
            // Tắt hoàn toàn Slider cũ (Bao gồm cả Canvas cha của nó nếu có)
            if (healthSlider != null) 
            {
                if (healthSlider.transform.parent != null && healthSlider.transform.parent != transform)
                    healthSlider.transform.parent.gameObject.SetActive(false);
                else
                    healthSlider.gameObject.SetActive(false);
            }

            // Khởi tạo các thanh UXML
            if (healthBarDocument.rootVisualElement != null)
            {
                var root = healthBarDocument.rootVisualElement;
                _healthFill = root.Q<VisualElement>("HealthFill");
                _ghostFill = root.Q<VisualElement>("GhostFill");
                
                // Set máu ban đầu đầy 100%
                UpdateBossUI(health, health);
            }
        }
        else if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = health;
        }
    }

    void Update()
    {
        // Giữ Slider quái nhỏ không xoay
        if (healthSlider != null && healthSlider.gameObject.activeInHierarchy && healthSlider.transform.parent != null)
        {
            healthSlider.transform.parent.rotation = Quaternion.identity;
        }
    }

    public void TakeDamage(Vector2 hitSource, float damage)
    {
        if (invincible || health <= 0) return;

        health -= damage;
        TriggerFlash();
        StartCoroutine(ResetHitBox());

        // Cập nhật UI
        if (healthBarDocument != null)
        {
            UpdateBossUI(health, health + damage); // Truyền vào máu mới và máu cũ (để tính delay)
        }
        else if (healthSlider != null)
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

    private void UpdateBossUI(float currentHealth, float previousHealth)
    {
        if (_healthFill == null || _ghostFill == null) return;

        float targetPercentage = Mathf.Clamp((currentHealth / maxHealth) * 100f, 0, 100);

        // 1. Thanh Đỏ: Giật xuống ngay lập tức
        _healthFill.style.width = Length.Percent(targetPercentage);

        // 2. Thanh Trắng: Chờ một chút rồi mới chạy theo
        if (_ghostCoroutine != null) StopCoroutine(_ghostCoroutine);
        _ghostCoroutine = StartCoroutine(GhostBarRoutine(targetPercentage));
    }

    private IEnumerator GhostBarRoutine(float targetPercentage)
    {
        // Chờ khoảng thời gian delay
        yield return new WaitForSeconds(ghostDelay);
        
        // Cập nhật thanh trắng. Hiệu ứng trượt mượt mà đã được USS lo (transition: width)
        if (_ghostFill != null)
        {
            _ghostFill.style.width = Length.Percent(targetPercentage);
        }
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
        
        while (timer < invincibilityDuration)
        {
            yield return new WaitForSeconds(invincibilityDuration);
            invincible = false;
            yield return new WaitForSeconds(0.05f - invincibilityDuration);
            timer += Time.deltaTime;
        }

        sr.enabled = true; 
    }

    void Die()
    {
        if (DeathPar != null)
        {
            Instantiate(DeathPar, transform.position, Quaternion.identity);
        }

        if (healthBarDocument != null)
        {
            healthBarDocument.gameObject.SetActive(false);
        }

        if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator ResetHitBox()
    {
        hitbox.enabled = false;
        yield return new WaitForSeconds(0.02f);
        hitbox.enabled = true;
    }
} 