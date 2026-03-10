using System;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{   
    public static PlayerHealth Instance;
    public float MaxHealth = 100f;
    
    // Lưu trữ máu xuyên Scene
    public static float SavedHealth = 100f; 
    public static bool IsFirstLoad = true;
    private float _currentHealth;

    public float multiplier = 1;
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
        string CurrentScene = SceneManager.GetActiveScene().name;
        
    }

    void Start() {
        OnHealthChanged?.Invoke(Health, MaxHealth);
    }

    public void TakeDamage(float damage) {
        Health -= damage * multiplier;
        CineCamera.instance.TriggerPreset("TakeDamage");
        if (Health <= 0) Die();
    }

    public void Heal(float amount) {
        Health += amount;
    }

    private void Die() {
        IsFirstLoad = true; // Chết thì reset về Max cho lần sau
        Debug.Log("Player Died!");
    }


    private void SceneMultiplier(string scene) {
        if (scene == "Dungeon1-1")
        {
            multiplier = 1f;
        }
        if (scene == "Dungeon1-2")
        {
            multiplier = 1.25f;
        }
        if (scene == "Dungeon1-3")
        {
            multiplier = 1.5f;
        }
        if (scene == "Dungeon1-4")
        {
            multiplier = 1.75f;
        }
        if (scene == "Dungeon1-5")
        {
            multiplier = 2;
        }

    }
}