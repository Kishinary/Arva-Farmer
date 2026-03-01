using UnityEngine;
using UnityEngine.UIElements;

public class HealthBarController : MonoBehaviour
{
    private VisualElement _mainBar;
    private VisualElement _ghostBar;
    private Label _hpLabel;

    void OnEnable() {
        var root = GetComponent<UIDocument>().rootVisualElement;
        
        // Tìm các phần tử theo tên đã đặt trong UXML
        _mainBar = root.Q<VisualElement>("MainBar");
        _ghostBar = root.Q<VisualElement>("GhostBar");
        _hpLabel = root.Q<Label>("HpLabel");

        // Đăng ký nhận thông tin mỗi khi máu thay đổi
        PlayerHealth.OnHealthChanged += UpdateUI;
    }

    void OnDisable() {
        PlayerHealth.OnHealthChanged -= UpdateUI;
    }

    private void UpdateUI(float current, float max) {
        float pct = (current / max) * 100f;
        
        // Dùng Length.Percent để ép thanh máu luôn là hình chữ nhật
        _mainBar.style.width = new Length(pct, LengthUnit.Percent);
        _ghostBar.style.width = new Length(pct, LengthUnit.Percent);
        
        _hpLabel.text = $"{Mathf.CeilToInt(current)} / {max}";
    }
}