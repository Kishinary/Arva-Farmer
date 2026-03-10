using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance { get; private set; }
    public float MaxHealth = 100f;
    private float _currentHealth;

    // Thêm các biến này để fix lỗi PauseController
    public static bool IsFirstLoad = true; 

    public static event Action<float, float> OnHealthChanged;

    public float Health {
        get => _currentHealth;
        private set {
            _currentHealth = Mathf.Clamp(value, 0, MaxHealth);
            OnHealthChanged?.Invoke(_currentHealth, MaxHealth);
        }
    }

    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        _currentHealth = MaxHealth;
    }

    void Update() {
        // Sửa SPACE thành Space để fix lỗi đỏ
        if (Input.GetKeyDown(KeyCode.Space)) {
            TakeDamage(20f);
        }
    }

    public void TakeDamage(float damage) {
        Health -= damage;
    }
}