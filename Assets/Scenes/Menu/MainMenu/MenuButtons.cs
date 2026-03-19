using UnityEngine;
using UnityEngine.SceneManagement;

public class SlideUI : MonoBehaviour
{
    public RectTransform mainMenu;
    public RectTransform credits;
    public float speed = 4f;

    private Vector2 mainTarget;
    private Vector2 creditsTarget;

    void Start()
    {
        mainTarget = mainMenu.anchoredPosition = Vector2.zero;
        creditsTarget = credits.anchoredPosition = new Vector2(1920, 0);
    }

    void Update()
    {
        mainMenu.anchoredPosition = Vector2.Lerp(
            mainMenu.anchoredPosition, mainTarget, Time.deltaTime * speed);

        credits.anchoredPosition = Vector2.Lerp(
            credits.anchoredPosition, creditsTarget, Time.deltaTime * speed);
    }

    public void ShowCredits()
    {
        mainTarget = new Vector2(-1920, 0);
        creditsTarget = Vector2.zero;
    }

    public void Back()
    {
        mainTarget = Vector2.zero;
        creditsTarget = new Vector2(1920, 0);
    }


    public void Quite()
    {
        Application.Quit();
    }

    public void GotoLobby()
    {
        SceneManager.LoadScene("Lobby");
    }
}