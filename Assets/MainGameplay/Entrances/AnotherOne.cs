using UnityEngine;
using UnityEngine.SceneManagement;

public class AnotherOne : MonoBehaviour
{
    public Vector2 playerspawn;
    void Start()
    {
        GameObject.FindWithTag("Player").transform.position = playerspawn;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        SceneManager.LoadScene("Dungeon1-1");
    }
}
