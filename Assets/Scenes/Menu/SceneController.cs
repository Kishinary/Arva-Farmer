using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance; 
    public Animator transitionAnim;

    private void Awake()
    {
       
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void NextScene(string sceneName, bool useTransition) 
    {
    if (useTransition) {
        StartCoroutine(LoadSceneWithTransition(sceneName));
    } else {
        SceneManager.LoadScene(sceneName);
    }
    }

    IEnumerator LoadSceneWithTransition(string sceneName)
    {
   
    transitionAnim.SetTrigger("End");
    yield return new WaitForSeconds(1f);
    AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
    while (!operation.isDone) {
        yield return null;
    }
    
    transitionAnim.SetTrigger("Start");
    }
}