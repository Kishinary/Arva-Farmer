using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class HPBarController : MonoBehaviour
{
    public static HPBarController Instance { get; private set; }

    private VisualElement _hpBarFill;
    private VisualElement _hpBarGhost;
    private Label _hpText;

    [Header("Ghost Effect")]
    [SerializeField] private float ghostShrinkDelay = 0.5f;
    [SerializeField] private float ghostShrinkSpeed = 4f;

    private float _currentGhostPercent = 100f;
    private Coroutine _ghostCoroutine;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable() { PlayerHealth.OnHealthChanged += UpdateHPBar; }
    void OnDisable() { PlayerHealth.OnHealthChanged -= UpdateHPBar; }

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _hpBarFill = root.Q<VisualElement>("hp-bar-fill");
        _hpBarGhost = root.Q<VisualElement>("hp-bar-ghost");
        _hpText = root.Q<Label>("hp-text");

        if (PlayerHealth.Instance != null)
            UpdateHPBar(PlayerHealth.Instance.CurrentHealth, PlayerHealth.Instance.MaxHealth);
    }

    private void UpdateHPBar(float current, float max)
    {
        if (_hpBarFill == null || _hpBarGhost == null) return;

        float targetPercent = (max > 0) ? (current / max) * 100f : 0f;

        // 1. Cập nhật thanh đỏ ngay lập tức
        _hpBarFill.style.width = Length.Percent(targetPercent);
        _hpText.text = $"{Mathf.CeilToInt(current)}/{Mathf.CeilToInt(max)}";

        // 2. Xử lý thanh trắng
        if (targetPercent < _currentGhostPercent)
        {
            // Bị mất máu -> Chờ rồi mới co thanh trắng
            if (_ghostCoroutine != null) StopCoroutine(_ghostCoroutine);
            _ghostCoroutine = StartCoroutine(ShrinkGhost(targetPercent));
        }
        else
        {
            // Hồi máu -> Tăng thanh trắng ngay lập tức để đè lên fill
            if (_ghostCoroutine != null) StopCoroutine(_ghostCoroutine);
            _currentGhostPercent = targetPercent;
            _hpBarGhost.style.width = Length.Percent(_currentGhostPercent);
        }
    }

    private IEnumerator ShrinkGhost(float target)
    {
        yield return new WaitForSeconds(ghostShrinkDelay);

        while (_currentGhostPercent > target)
        {
            _currentGhostPercent = Mathf.MoveTowards(_currentGhostPercent, target, ghostShrinkSpeed * Time.deltaTime * 30f);
            _hpBarGhost.style.width = Length.Percent(_currentGhostPercent);
            yield return null;
        }
        _currentGhostPercent = target;
    }
}