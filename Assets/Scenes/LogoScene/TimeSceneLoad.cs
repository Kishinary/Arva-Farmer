using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TimedSceneLoad : MonoBehaviour
{
    [SerializeField] private float delay = 3f; // Tổng thời gian logo hiện (ví dụ 5 giây)
    [SerializeField] private string sceneName = "MainMenu";

    void Start()
    {
        StartCoroutine(WaitAndLoad());
    }

    IEnumerator WaitAndLoad()
    {
        // Đợi đúng bằng thời gian Animation chạy
        yield return new WaitForSeconds(delay);
        
        // Chuyển cảnh
        SceneManager.LoadScene(sceneName);
    }
}