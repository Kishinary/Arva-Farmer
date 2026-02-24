using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    // Singleton để truy cập từ mọi nơi nếu cần
    public static PauseMenuController Instance { get; private set; }

    private VisualElement _pauseMenuContainer;
    private bool _isPaused = false;
    
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    void Awake()
    {
        // Logic Singleton: Nếu đã có một bản GlobalUI rồi thì xóa bản mới đi
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // Giữ GameObject này không bị xóa khi đổi Scene
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SetupUI();
        // Đăng ký sự kiện mỗi khi Scene thay đổi để cập nhật lại UI nếu cần
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetupUI(); // Thiết lập lại tham chiếu UI khi sang Scene mới
        ResumeGame(); // Đảm bảo không bị Pause khi vừa vào Scene mới
    }

    private void SetupUI()
    {
        var uiDoc = GetComponent<UIDocument>();
        if (uiDoc == null) return;

        var root = uiDoc.rootVisualElement;
        _pauseMenuContainer = root.Q<VisualElement>("PauseMenuContainer");

        root.Q<VisualElement>("ResumeRow")?.RegisterCallback<ClickEvent>(evt => ResumeGame());
        root.Q<VisualElement>("QuitRow")?.RegisterCallback<ClickEvent>(evt => QuitToMainMenu());

        if (_pauseMenuContainer != null)
            _pauseMenuContainer.style.display = DisplayStyle.None;
    }

    void Update()
    {
        // Kiểm tra xem Scene hiện tại có phải là MainMenu không
        // Thường thì chúng ta không muốn hiện Pause Menu ở màn hình chính
        if (SceneManager.GetActiveScene().name == mainMenuSceneName) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isPaused) ResumeGame(); else PauseGame();
        }
    }

    public void PauseGame()
    {
        if (_pauseMenuContainer == null) return;
        _isPaused = true;
        Time.timeScale = 0f;
        _pauseMenuContainer.style.display = DisplayStyle.Flex;
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        if (_pauseMenuContainer == null) return;
        _isPaused = false;
        Time.timeScale = 1f;
        _pauseMenuContainer.style.display = DisplayStyle.None;
        // UnityEngine.Cursor.visible = false; 
    }

    private void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        _isPaused = false;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}