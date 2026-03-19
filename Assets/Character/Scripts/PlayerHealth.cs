using System;
using System.Collections; // Bắt buộc phải có để dùng IEnumerator
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance { get; private set; }

    [Header("Stats")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Flash Effect")]
    [SerializeField] private Material flashMaterial;
    [SerializeField] private float flashDuration = 0.1f;

    [Header("Debug")]
    public static bool IsFirstLoad = true; 

    public static event Action<float, float> OnHealthChanged;
    public static event Action OnDied;

    public float _currentHealth;
    public float MaxHealth => maxHealth;
    public float CurrentHealth => _currentHealth;

    private SpriteRenderer spriteRenderer;
    private Material originalMaterial;
    private Coroutine flashRoutine;

    private Animator anim;
    void Awake()
    {
        anim = GetComponent<Animator>();
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _currentHealth = maxHealth;
        
        // Nên lấy SpriteRenderer và Material gốc ở Awake để đảm bảo an toàn
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalMaterial = spriteRenderer.material;
        }
    }

    void Start()
    {
        OnHealthChanged?.Invoke(_currentHealth, maxHealth);
    }

    void Update()
    {
    }

    public void TakeDamage(float damage)
    {
        if (_currentHealth <= 0 || damage <= 0){
            StartCoroutine(Diesequence());
        }

        // Xử lý hiệu ứng nháy
        if (flashMaterial != null && spriteRenderer != null)
        {
            if (flashRoutine != null) StopCoroutine(flashRoutine);
            flashRoutine = StartCoroutine(FlashRoutine());
        }
    
        _currentHealth = Mathf.Max(_currentHealth - damage, 0);
        OnHealthChanged?.Invoke(_currentHealth, maxHealth);

        if (_currentHealth <= 0) anim.SetTrigger("Die");
    }

    private IEnumerator FlashRoutine()
    {
        spriteRenderer.material = flashMaterial;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.material = originalMaterial;
        flashRoutine = null;
    }

    public void Heal(float amount)
    {
        if (_currentHealth <= 0 || amount <= 0) return;
        _currentHealth = Mathf.Min(_currentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(_currentHealth, maxHealth);
    }

    private IEnumerator Diesequence ()
    {
        anim.SetTrigger("Die");
        yield return new WaitForSeconds(0.25f);
        SceneController.Instance.NextScene("Lobby", true);
        _currentHealth = MaxHealth;
    }
}