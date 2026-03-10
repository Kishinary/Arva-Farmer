using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private Label playButton;
    private Label quitButton;

    private void OnEnable()
    {
        
        var uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

       
        playButton = root.Q<Label>("play-label");
        quitButton = root.Q<Label>("quit-label");

        
        if (playButton != null)
        {
            playButton.RegisterCallback<ClickEvent>(OnPlayClicked);
        }

        
        if (quitButton != null)
        {
            quitButton.RegisterCallback<ClickEvent>(OnQuitClicked);
        }
    }

    private void OnPlayClicked(ClickEvent evt)
    {
        
        SceneManager.LoadScene("Lobby");
    }

    private void OnQuitClicked(ClickEvent evt)
    {
        Debug.Log("Thoát chương trình...");

        #if UNITY_EDITOR
            
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            
            Application.Quit();
        #endif
    }

    private void OnDisable()
    {
      
        if (playButton != null)
            playButton.UnregisterCallback<ClickEvent>(OnPlayClicked);
            
        if (quitButton != null)
            quitButton.UnregisterCallback<ClickEvent>(OnQuitClicked);
    }
}