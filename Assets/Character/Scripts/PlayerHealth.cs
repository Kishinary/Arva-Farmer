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
    if (_currentHealth <= 0 || damage <= 0) return;

    // 1. Trừ máu và cập nhật UI
    _currentHealth = Mathf.Max(_currentHealth - damage, 0);
    OnHealthChanged?.Invoke(_currentHealth, maxHealth);

    // 2. Xử lý hiệu ứng nháy (Flash)
    if (flashMaterial != null && spriteRenderer != null)
    {
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashRoutine());
    }

    // 3. Kiểm tra nếu hết máu thì chạy chuỗi hành động chết
    if (_currentHealth <= 0)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "Dungeon2-4"){
            StartCoroutine(DeathSequence());
            SceneController.Instance.NextScene("Dungeon2-4", true);
            Time.timeScale = 1f;
            _currentHealth = maxHealth;
            OnHealthChanged?.Invoke(_currentHealth, maxHealth);
        }else{
        StartCoroutine(DeathSequence());
        }
    }
    }

// Hàm xử lý riêng cho việc chết và chuyển cảnh
    IEnumerator DeathSequence()
    {
    // Chạy Anim chết ngay lập tức
    
    // Đóng băng thời gian
    Time.timeScale = 0f;
    CineZoom.instance.ZoomIn(transform.position, 1f);
    anim.SetTrigger("Die");
    yield return new WaitForSecondsRealtime(1f);
    if (SceneManager.GetActiveScene().name == "Boss" || SceneManager.GetActiveScene().name == "Boss2")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    else
        {
            Destroy(gameObject);
            SceneController.Instance.NextScene("Lobby", true);
        }
    // Đợi 2 giây thời gian thực (vì timeScale đã = 0)
    Time.timeScale = 1f;
    _currentHealth = maxHealth;
    OnHealthChanged?.Invoke(_currentHealth, maxHealth);
    // Reset lại trạng thái trước khi chuyển cảnh
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
}