using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private Label playButton;
    private Label quitButton;

    private void OnEnable()
    {
        // 1. Lấy UIDocument gắn trên cùng Object với Script này
        var uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        // 2. Tìm cái Label dựa trên tên "play-label" và "quit-label"
        playButton = root.Q<Label>("play-label");
        quitButton = root.Q<Label>("quit-label");

        // 3. Đăng ký sự kiện cho nút Play (Giữ nguyên feature cũ)
        if (playButton != null)
        {
            playButton.RegisterCallback<ClickEvent>(OnPlayClicked);
        }

        // 4. Đăng ký sự kiện cho nút Quit (Feature mới)
        if (quitButton != null)
        {
            quitButton.RegisterCallback<ClickEvent>(OnQuitClicked);
        }
    }

    private void OnPlayClicked(ClickEvent evt)
    {
        // Chuyển sang SampleScene
        SceneManager.LoadScene("SampleScene");
    }

    private void OnQuitClicked(ClickEvent evt)
    {
        Debug.Log("Thoát chương trình...");

        #if UNITY_EDITOR
            // Nếu đang trong Editor, dừng chế độ Play
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // Nếu là bản build, đóng ứng dụng
            Application.Quit();
        #endif
    }

    private void OnDisable()
    {
        // Hủy đăng ký sự kiện để tránh lỗi bộ nhớ (Best Practice)
        if (playButton != null)
            playButton.UnregisterCallback<ClickEvent>(OnPlayClicked);
            
        if (quitButton != null)
            quitButton.UnregisterCallback<ClickEvent>(OnQuitClicked);
    }
}