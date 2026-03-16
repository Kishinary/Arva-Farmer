using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements; 
using TMPro;
using System.Runtime.InteropServices; // Thêm namespace này để dùng TextMeshPro

public class EnemyStats : MonoBehaviour
{
    [Header("Health Settings")]
    public float health;
    public float maxHealth;
    public float realMaxHealth;
    public bool invincible = false;
    [SerializeField] private float invincibilityDuration = 0.02f;

    [Header("Damage PopUp (Mới)")]
    [SerializeField] private GameObject damageTextPrefab; // Kéo Prefab DamageText (UI) vào đây
    [SerializeField] private Vector3 damageTextOffset = new Vector3(0, 1f, 0); // Vị trí hiện số so với quái

    [Header("Flash Settings")]
    [SerializeField] private Material flashMaterial; 
    [SerializeField] private float flashDuration = 0.15f;

    [Header("Knockback Settings")]
    public float knockbackForce = 0.5f;

    [Header("UI - Quái Nhỏ")]
    public UnityEngine.UI.Slider healthSlider;

    [Header("UI - Boss (UXML)")]
    public UIDocument healthBarDocument;
    [SerializeField] private float ghostDelay = 0.3f; 
    
    private VisualElement _healthFill;
    private VisualElement _ghostFill;
    private Coroutine _ghostCoroutine; 

    private Rigidbody2D rb;
    
    [Header("Components")]
    [SerializeField] private SpriteRenderer sr; 
    private Material originalMaterial;
    private Coroutine flashRoutine;

    [Header("Particle")]
    public ParticleSystem DeathPar;

    [Header("Hitbox")]
    private Collider2D hitbox;

    [Header("Movement")]
    public float speedMultiplier = 1;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        hitbox = GetComponent<Collider2D>();

        realMaxHealth = health * (DungeonSpawner.instance == null ? 1f : DungeonSpawner.instance.HealthMultiplier);
        health = realMaxHealth;
        originalMaterial = sr.material;
    }

    void Start()
    {
        if (healthBarDocument != null)
        {
            if (healthSlider != null) 
            {
                if (healthSlider.transform.parent != null && healthSlider.transform.parent != transform)
                    healthSlider.transform.parent.gameObject.SetActive(false);
                else
                    healthSlider.gameObject.SetActive(false);
            }

            if (healthBarDocument.rootVisualElement != null)
            {
                var root = healthBarDocument.rootVisualElement;
                _healthFill = root.Q<VisualElement>("HealthFill");
                _ghostFill = root.Q<VisualElement>("GhostFill");
                UpdateBossUI(health, health);
            }
        }
        else if (healthSlider != null)
        {
            healthSlider.maxValue = realMaxHealth;
            healthSlider.value = health;
        }
    }

    void Update()
    {
        if (healthSlider != null && healthSlider.gameObject.activeInHierarchy && healthSlider.transform.parent != null)
        {
            healthSlider.transform.parent.rotation = Quaternion.identity;
        }
    }

    public void TakeDamage(Vector2 hitSource, float damage)
    {
        if (invincible || health <= 0) return;

        health -= damage;

        // --- XỬ LÝ HIỆN SỐ SÁT THƯƠNG ---
        SpawnDamageText(damage);

        TriggerFlash();
        StartCoroutine(ResetHitBox());

        if (healthBarDocument != null)
        {
            UpdateBossUI(health, health + damage); 
        }
        else if (healthSlider != null)
        {
            healthSlider.value = health;
        }

        if (hitSource != Vector2.zero)
        {
            Vector2 knockbackDir = (transform.position - (Vector3)hitSource).normalized;
            rb.linearVelocity = Vector2.zero; 
            rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
        }

        if (health <= 0) Die();
        else StartCoroutine(InvincibilityRoutine());
    }

    private void SpawnDamageText(float damageAmount)
    {
        if (damageTextPrefab == null) return;

        Vector3 randomOffset = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.1f, 0.1f), 0);
        Vector3 spawnPosition = transform.position + damageTextOffset + randomOffset;

        GameObject popup = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity);

        DamagePopUp popUpScript = popup.GetComponent<DamagePopUp>();
        if (popUpScript != null)
        {
            popUpScript.Setup((int)damageAmount);
        }
    }

    private void UpdateBossUI(float currentHealth, float previousHealth)
    {
        if (_healthFill == null || _ghostFill == null) return;
        float targetPercentage = Mathf.Clamp((currentHealth / maxHealth) * 100f, 0, 100);
        _healthFill.style.width = Length.Percent(targetPercentage);
        if (_ghostCoroutine != null) StopCoroutine(_ghostCoroutine);
        _ghostCoroutine = StartCoroutine(GhostBarRoutine(targetPercentage));
    }

    private IEnumerator GhostBarRoutine(float targetPercentage)
    {
        yield return new WaitForSeconds(ghostDelay);
        if (_ghostFill != null) _ghostFill.style.width = Length.Percent(targetPercentage);
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
        yield return new WaitForSeconds(invincibilityDuration);
        invincible = false;
    }

    void Die()
    {
        if (DeathPar != null) Instantiate(DeathPar, transform.position, Quaternion.identity);
        if (healthBarDocument != null) healthBarDocument.gameObject.SetActive(false);

        if (transform.parent != null) Destroy(transform.parent.gameObject);
        else Destroy(gameObject);
    }

    IEnumerator ResetHitBox()
    {
        hitbox.enabled = false;
        yield return new WaitForSeconds(0.02f);
        hitbox.enabled = true;
    }

    IEnumerator Slowed(float speed, float time)
    {
        this.speedMultiplier = speed;

        yield return new WaitForSeconds(time);
        this.speedMultiplier = 1f;
    }

    public void TriggerSLowed(float speed, float time)
    {
        StartCoroutine(Slowed(speed, time));
    }
    public void TriggerPit(float time)
    {
        StartCoroutine(PitfallSequence(time));
    }
    IEnumerator PitfallSequence(float time)
    {
        this.GetComponent<SpriteRenderer>().enabled = false;
        this.GetComponent<Collider2D>().enabled = false;

        yield return new WaitForSeconds(time);

        this.GetComponent<SpriteRenderer>().enabled = true;
        this.GetComponent<Collider2D>().enabled = true;
    }
}