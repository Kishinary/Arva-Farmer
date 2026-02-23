using UnityEngine;
using UnityEngine.UIElements;

public class HealthBarController : MonoBehaviour
{
    private VisualElement _healthFill;

    void OnEnable()
    {
        // Lấy root của UXML
        var root = GetComponent<UIDocument>().rootVisualElement;
        
        // Tìm VisualElement có tên là "HealthFill" (phải khớp tên trong UI Builder)
        _healthFill = root.Q<VisualElement>("HealthFill");

        // Đăng ký lắng nghe sự kiện thay đổi máu
        PlayerHealth.OnHealthChanged += UpdateUI;
    }

    void OnDisable()
    {
        // Hủy đăng ký khi không dùng nữa để tránh lỗi bộ nhớ
        PlayerHealth.OnHealthChanged -= UpdateUI;
    }

    private void UpdateUI(float currentHealth, float maxHealth)
    {
        if (_healthFill == null) return;

        // Tính toán phần trăm
        float pct = (currentHealth / maxHealth) * 100f;

        // Cập nhật độ rộng (Width) của thanh máu
        _healthFill.style.width = Length.Percent(pct);
    }
}