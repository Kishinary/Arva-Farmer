using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalSceneTrans : MonoBehaviour
{
    public string SceneString;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SceneController.Instance.NextScene(SceneString, true);
        }
    }
}
