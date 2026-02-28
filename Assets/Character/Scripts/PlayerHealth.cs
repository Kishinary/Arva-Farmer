using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float MaxHealth = 100f;
    
    // Lưu trữ máu xuyên Scene
    public static float SavedHealth = 100f; 
    public static bool IsFirstLoad = true;

    private float _currentHealth;
    public float Health {
        get => _currentHealth;
        private set {
            _currentHealth = Mathf.Clamp(value, 0, MaxHealth);
            SavedHealth = _currentHealth;
            OnHealthChanged?.Invoke(_currentHealth, MaxHealth);
        }
    }

    // Sự kiện để UI tự động cập nhật
    public static event Action<float, float> OnHealthChanged;

    void Awake() {
        // Tự động lấy lại máu đã lưu hoặc đặt bằng Max nếu là lần đầu
        Health = IsFirstLoad ? MaxHealth : SavedHealth;
        IsFirstLoad = false;
    }

    void Start() {
        OnHealthChanged?.Invoke(Health, MaxHealth);
    }

    public void TakeDamage(float damage) {
        Health -= damage;
        if (Health <= 0) Die();
    }

    public void Heal(float amount) {
        Health += amount;
    }

    private void Die() {
        IsFirstLoad = true; // Chết thì reset về Max cho lần sau
        Debug.Log("Player Died!");
    }
}