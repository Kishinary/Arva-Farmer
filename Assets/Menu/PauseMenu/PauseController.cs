using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    // Singleton giúp Menu sống sót qua các Scene mà không bị nhân bản
    public static PauseMenuController Instance;

    private UIDocument _uiDocument;
    private VisualElement _background;
    private Button _btnResume;
    private Button _btnQuit;

    private bool _isPaused = false;

    [Header("Ghi đúng tên Scene Main Menu vào đây")]
    public string mainMenuSceneName = "MainMenu";

    void Awake()
    {
        // Đảm bảo chỉ có 1 Pause Menu duy nhất tồn tại
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Lệnh giữ GameObject không bị hủy khi qua Scene khác
    }

    void OnEnable()
    {
        _uiDocument = GetComponent<UIDocument>();
        if (_uiDocument == null) return;

        var root = _uiDocument.rootVisualElement;
        if (root == null) return;

        // Tìm các phần tử UI dựa theo Tên (name) và Lớp (class) bạn đã đặt trong UXML
        _background = root.Q<VisualElement>(className: "background");
        _btnResume = root.Q<Button>("btn-resume");
        _btnQuit = root.Q<Button>("btn-quit");

        // Gán chức năng cho các nút bấm
        if (_btnResume != null) _btnResume.clicked += ResumeGame;
        if (_btnQuit != null) _btnQuit.clicked += QuitToMenu;

        // QUAN TRỌNG: Ẩn menu đi ngay khi game vừa bắt đầu
        if (_background != null) _background.style.display = DisplayStyle.None;
    }

    void Update()
    {
        // 1. Nếu đang ở màn hình Main Menu thì vô hiệu hóa nút Esc (không cho bật Pause Menu)
        if (SceneManager.GetActiveScene().name == mainMenuSceneName)
        {
            if (_isPaused) ResumeGame();
            return;
        }

        // 2. Nhấn phím Esc để Bật/Tắt Pause Menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isPaused) ResumeGame();
            else PauseGame();
        }
    }

    private void PauseGame()
    {
        _isPaused = true;
        Time.timeScale = 0f; // Dừng toàn bộ thời gian và vật lý trong game
        if (_background != null) _background.style.display = DisplayStyle.Flex; // Hiện giao diện lên
    }

    private void ResumeGame()
    {
        _isPaused = false;
        Time.timeScale = 1f; // Đưa thời gian chạy bình thường trở lại
        if (_background != null) _background.style.display = DisplayStyle.None; // Ẩn giao diện đi
    }

    private void QuitToMenu()
    {
        ResumeGame(); // QUAN TRỌNG: Phải mở khóa thời gian (Time.timeScale = 1) trước khi load scene mới
        SceneManager.LoadScene(mainMenuSceneName); // Chuyển về Scene Main Menu
    }
}