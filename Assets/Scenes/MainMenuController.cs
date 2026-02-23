using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private void OnEnable()
    {
        // 1. Lấy UIDocument gắn trên cùng Object với Script này
        var uiDocument = GetComponent<UIDocument>();

        // 2. Tìm cái Label dựa trên cái tên "play-label" bạn vừa đặt
        var playButton = uiDocument.rootVisualElement.Q<Label>("play-label");

        if (playButton != null)
        {
            // 3. Đăng ký sự kiện khi người dùng nhấn chuột vào Label
            playButton.RegisterCallback<ClickEvent>(evt => LoadSampleScene());
        }
    }

    private void LoadSampleScene()
    {
        // 4. Chuyển sang SampleScene (Nhớ add scene vào Build Settings nhé!)
        SceneManager.LoadScene("SampleScene");
    }
}