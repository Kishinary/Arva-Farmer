using System; // Cần thiết để dùng Action
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float Health;
    public float MaxHealth = 100f;
    public bool invincible = false;

    // Sự kiện này sẽ báo cho UI biết: "Này, máu vừa đổi đấy, vẽ lại đi!"
    public static event Action<float, float> OnHealthChanged;

    void Start()
    {
        Health = MaxHealth;
        // Gửi thông báo ban đầu để UI hiển thị 100%
        OnHealthChanged?.Invoke(Health, MaxHealth);
    }

    public void TakeDamage(float Damage)
    {
        if (invincible) return;

        Health -= Damage;
        Health = Mathf.Max(Health, 0); // Đảm bảo máu không âm

        // Phát tín hiệu thay đổi máu
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
        // Xử lý chết ở đây (Play hiệu ứng, chuyển scene...)
        Destroy(this.gameObject);
    }

    IEnumerator Invincibility()
    {
        invincible = true;
        yield return new WaitForSeconds(0.5f);
        invincible = false;
    }
}