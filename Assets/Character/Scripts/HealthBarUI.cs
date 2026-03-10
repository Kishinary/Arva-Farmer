using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class HealthBarUI : MonoBehaviour
{
    public VisualTreeAsset heartTemplate; // Kéo file HeartTemplate.uxml vào đây
    public float lerpSpeed = 8f;

    private VisualElement _container;
    private List<VisualElement> _masks = new List<VisualElement>();
    private List<VisualElement> _fills = new List<VisualElement>();
    
    private float _visualHealth;

    void OnEnable() {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _container = root.Q<VisualElement>("HealthBar"); // Đảm bảo UXML chính có cái này

        PlayerHealth.OnHealthChanged += HandleUpdate;
        
        if (PlayerHealth.Instance != null) {
            _visualHealth = PlayerHealth.Instance.Health;
            SetupHearts(PlayerHealth.Instance.MaxHealth);
        }
    }

    void OnDisable() {
        PlayerHealth.OnHealthChanged -= HandleUpdate;
    }

    void SetupHearts(float maxHealth) {
        _container.Clear();
        _masks.Clear();
        _fills.Clear();

        // 1 trái tim = 20 máu (Ví dụ)
        int heartCount = Mathf.CeilToInt(maxHealth / 20f);

        for (int i = 0; i < heartCount; i++) {
            var heart = heartTemplate.Instantiate();
            var mask = heart.Q<VisualElement>(className: "heart-mask");
            var fill = heart.Q<VisualElement>(className: "heart-fill");
            
            _container.Add(heart);
            _masks.Add(mask);
            _fills.Add(fill);
        }
    }

    private void HandleUpdate(float current, float max) {
        // Có thể thêm hiệu ứng chớp trắng ở đây
    }
    void Awake() {
    // Giữ object này không bị xóa khi load scene mới
    DontDestroyOnLoad(gameObject);
    }
    void Update() {
        if (PlayerHealth.Instance == null) return;

        // Làm mượt máu
        _visualHealth = Mathf.Lerp(_visualHealth, PlayerHealth.Instance.Health, Time.deltaTime * lerpSpeed);

        float healthPerHeart = PlayerHealth.Instance.MaxHealth / _masks.Count;

        for (int i = 0; i < _masks.Count; i++) {
            float fillAmount = Mathf.Clamp01((_visualHealth / healthPerHeart) - i);
            _masks[i].style.height = Length.Percent(fillAmount * 100f);
        }
    }
}