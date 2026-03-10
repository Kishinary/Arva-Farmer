using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class HealthBarUI : MonoBehaviour
{
    public VisualTreeAsset heartTemplate;
    public float whiteDrainSpeed = 5f; 

    private VisualElement _container;
    private List<VisualElement> _redMasks = new List<VisualElement>();
    private List<VisualElement> _whiteMasks = new List<VisualElement>();
    private float _visualWhiteHealth;

    void OnEnable() {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _container = root.Q<VisualElement>("HealthBar");

        if (_container != null && PlayerHealth.Instance != null) {
            _visualWhiteHealth = PlayerHealth.Instance.Health;
            SetupHearts();
        }
    }

    void SetupHearts() {
        _container.Clear();
        _redMasks.Clear();
        _whiteMasks.Clear();

        int heartCount = Mathf.CeilToInt(PlayerHealth.Instance.MaxHealth / 20f);

        for (int i = 0; i < heartCount; i++) {
            var heart = heartTemplate.Instantiate();
            var wMask = heart.Q<VisualElement>("WhiteMask");
            var rMask = heart.Q<VisualElement>("RedMask");

            if (wMask != null && rMask != null) {
                _container.Add(heart);
                _whiteMasks.Add(wMask);
                _redMasks.Add(rMask);
            }
        }
    }
    void Awake(){
        DontDestroyOnLoad(gameObject);
    }
    void Update() {
        if (PlayerHealth.Instance == null || _redMasks.Count == 0) return;

        float actualHP = PlayerHealth.Instance.Health;
        // Hiệu ứng Lerp để lớp trắng tụt chậm sau lớp đỏ
        _visualWhiteHealth = Mathf.Lerp(_visualWhiteHealth, actualHP, Time.deltaTime * whiteDrainSpeed);

        float hpPerHeart = PlayerHealth.Instance.MaxHealth / _redMasks.Count;

        for (int i = 0; i < _redMasks.Count; i++) {
            float redFill = Mathf.Clamp01((actualHP / hpPerHeart) - i);
            float whiteFill = Mathf.Clamp01((_visualWhiteHealth / hpPerHeart) - i);

            _redMasks[i].style.height = Length.Percent(redFill * 100f);
            _whiteMasks[i].style.height = Length.Percent(whiteFill * 100f);
        }
    }
}