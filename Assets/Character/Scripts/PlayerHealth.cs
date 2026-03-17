using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance { get; private set; }

    [Header("Stats")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Debug")]
    [SerializeField] private bool enableSpaceBarTest = false;

    // --- THÊM DÒNG NÀY ĐỂ HẾT LỖI ---
    public static bool IsFirstLoad = true; 
    // --------------------------------

    public static event Action<float, float> OnHealthChanged;
    public static event Action OnDied;

    public float _currentHealth;
    public float MaxHealth => maxHealth;
    public float CurrentHealth => _currentHealth;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _currentHealth = maxHealth;
    }

    void Start()
    {
        OnHealthChanged?.Invoke(_currentHealth, maxHealth);
    }

    void Update()
    {
        if (enableSpaceBarTest && Input.GetKeyDown(KeyCode.Space))
            TakeDamage(10f);
    }

    public void TakeDamage(float damage)
    {
        if (_currentHealth <= 0 || damage <= 0) return;

        _currentHealth = Mathf.Max(_currentHealth - damage, 0);
        OnHealthChanged?.Invoke(_currentHealth, maxHealth);

        if (_currentHealth <= 0) OnDied?.Invoke();
    }

    public void Heal(float amount)
    {
        if (_currentHealth <= 0 || amount <= 0) return;

        _currentHealth = Mathf.Min(_currentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(_currentHealth, maxHealth);
    }
}