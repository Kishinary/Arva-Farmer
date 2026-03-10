using UnityEngine;
using UnityEngine.SceneManagement;

public class AnotherOne : MonoBehaviour
{
    void Start()
    { 
    }
    void Update()
    {
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) 
        {
            SceneController.Instance.NextScene("Dungeon1-1", true);

        }
    }
}
