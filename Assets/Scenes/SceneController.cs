using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance; // Singleton để gọi từ mọi nơi
    public Animator transitionAnim;

    private void Awake()
    {
        // Kiểm tra nếu đã có bản thực thi nào chưa, nếu có thì xóa bản này
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Giữ script này không bị mất khi đổi scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void NextScene(string sceneName)
    {
        StartCoroutine(LoadScene(sceneName));
    }

    IEnumerator LoadScene(string sceneName)
    {
        // Chạy animation che màn hình
        transitionAnim.SetTrigger("Start");

        // Đợi 1 giây (bằng thời gian animation)
        yield return new WaitForSeconds(1f);

        // Load scene mới
        SceneManager.LoadScene(sceneName);
    }
}