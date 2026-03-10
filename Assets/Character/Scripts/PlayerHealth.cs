using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;
    public float MaxHealth = 100f;
    
    public static float SavedHealth = 100f; 
    public static bool IsFirstLoad = true;
    private float _currentHealth;

    public float multiplier = 1f;

    public float Health {
        get => _currentHealth;
        private set {
            _currentHealth = Mathf.Clamp(value, 0, MaxHealth);
            SavedHealth = _currentHealth;
            // Gửi: Máu hiện tại, Máu tối đa
            OnHealthChanged?.Invoke(_currentHealth, MaxHealth);
        }
    }

    public static event Action<float, float> OnHealthChanged;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
            return;
        }

        Health = IsFirstLoad ? MaxHealth : SavedHealth;
        IsFirstLoad = false;
        SceneMultiplier(SceneManager.GetActiveScene().name);
    }

    public void TakeDamage(float damage) {
        Health -= damage * multiplier;
        if (Health <= 0) Die();
    }

    private void Die() {
        IsFirstLoad = true;
        SavedHealth = MaxHealth;
        Debug.Log("Player Died!");
    }

    private void SceneMultiplier(string scene) {
        if (scene.Contains("1-2")) multiplier = 1.25f;
        else if (scene.Contains("1-5")) multiplier = 2f;
        else multiplier = 1f;
    }
}