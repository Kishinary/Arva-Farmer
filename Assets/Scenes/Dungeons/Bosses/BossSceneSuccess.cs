using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossSceneSuccess : MonoBehaviour
{
    public bool goable = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (goable)
        {
            SceneManager.LoadScene("Lobby");
        }
    }

    IEnumerator StartCheck()
    {
        yield return new WaitForSeconds(5);
        goable = true;
    }
}
