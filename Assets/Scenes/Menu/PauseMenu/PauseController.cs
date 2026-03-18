using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenuController : MonoBehaviour
{
    public static PauseMenuController Instance;

    private UIDocument _uiDocument;
    private VisualElement _background;
    private Button _btnResume;
    private Button _btnQuit;
    private bool _isPaused = false;

    [Header("Cấu hình")]
    public string mainMenuSceneName = "MainMenu";

    void Awake()
    {
        // 1. Singleton: Ngăn chặn nhân bản chính Menu
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Đăng ký sự kiện load Scene để tìm lại UI tham chiếu
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Mỗi khi qua cảnh mới, phải làm 3 việc:
        SetupUI();          // Tìm lại các nút bấm mới
        EnsureEventSystem(); // Đảm bảo chuột bấm được
        CheckIfInMainMenu(); // Tự ẩn nếu lỡ đang ở Main Menu
    }

    private void SetupUI()
    {
        _uiDocument = GetComponent<UIDocument>();
        if (_uiDocument == null) return;

        var root = _uiDocument.rootVisualElement;
        
        // Tìm UI (ưu tiên tìm theo Name, sau đó là Class)
        _background = root.Q<VisualElement>("background") ?? root.Q<VisualElement>(className: "background");
        _btnResume = root.Q<Button>("btn-resume");
        _btnQuit = root.Q<Button>("btn-quit");

        // Gỡ sự kiện cũ và gán sự kiện mới (tránh bị trùng lặp khi quay lại Scene)
        if (_btnResume != null)
        {
            _btnResume.clicked -= ResumeGame;
            _btnResume.clicked += ResumeGame;
        }
        if (_btnQuit != null)
        {
            _btnQuit.clicked -= QuitToMenu;
            _btnQuit.clicked += QuitToMenu;
        }

        // Luôn mặc định ẩn khi mới load Scene
        if (_background != null) _background.style.display = DisplayStyle.None;
    }

    private void EnsureEventSystem()
    {
        // Nếu Scene mới thiếu EventSystem, chuột sẽ không bấm được. Code này tự tạo nó.
        if (FindFirstObjectByType<EventSystem>() == null)
        {
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }
    }

    private void CheckIfInMainMenu()
    {
        if (SceneManager.GetActiveScene().name == mainMenuSceneName)
        {
            ResumeGame(); // Reset thời gian và ẩn UI nếu đang ở Menu chính
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == mainMenuSceneName) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        _isPaused = true;
        Time.timeScale = 0f;
        if (_background != null) _background.style.display = DisplayStyle.Flex;

        // HIỆN CHUỘT
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        _isPaused = false;
        Time.timeScale = 1f;
        if (_background != null) _background.style.display = DisplayStyle.None;

        // ẨN CHUỘT (Chỉ bật nếu bạn chơi game FPS/TPS)
        // Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Locked;
    }

    private void QuitToMenu()
    {
        ResumeGame(); // Quan trọng: Đưa Time.timeScale về 1 trước khi chuyển scene

        // Xử lý xóa nhân vật để tránh bị nhân bản khi quay lại game
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Destroy(player); 
        }

        // Reset dữ liệu máu nếu cần (từ file PlayerHealth của bạn)
        PlayerHealth.IsFirstLoad = true;

        SceneManager.LoadScene(mainMenuSceneName);
    }
}