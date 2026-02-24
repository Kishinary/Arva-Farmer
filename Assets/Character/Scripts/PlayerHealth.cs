using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // Biến static giúp lưu giá trị máu trong bộ nhớ ngay cả khi chuyển Scene
    public static float SavedHealth = 100f; 
    public static bool IsFirstLoad = true; // Để biết có nên reset máu về Max hay không

    public float Health;
    public float MaxHealth = 100f;
    public bool invincible = false;

    public static event Action<float, float> OnHealthChanged;

    void Awake()
    {
        // 1. Nếu là lần đầu tiên chạy game, đặt máu bằng MaxHealth
        if (IsFirstLoad)
        {
            Health = MaxHealth;
            SavedHealth = MaxHealth;
            IsFirstLoad = false;
        }
        else
        {
            // 2. Nếu chuyển từ Scene khác sang, lấy lại lượng máu đã lưu
            Health = SavedHealth;
        }
    }

    void Start()
    {
        // Gửi thông báo để UI cập nhật ngay khi vào Scene mới
        OnHealthChanged?.Invoke(Health, MaxHealth);
    }

    public void TakeDamage(float Damage)
    {
        if (invincible) return;

        Health -= Damage;
        Health = Mathf.Max(Health, 0);

        // Cập nhật vào biến tĩnh để lưu trữ cho Scene sau
        SavedHealth = Health;

        OnHealthChanged?.Invoke(Health, MaxHealth);

        if (Health <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(Invincibility());
        }
    }

    private void Die()
    {
        // Reset lại dữ liệu khi chết để lần sau chơi lại có đủ máu
        IsFirstLoad = true;
        Destroy(this.gameObject);
    }

    IEnumerator Invincibility()
    {
        invincible = true;
        yield return new WaitForSeconds(0.5f);
        invincible = false;
    }
}