using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class DungeonEntrance : MonoBehaviour
{
    public GameObject Guidance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Guidance.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E)) {
                SceneManager.LoadScene("MainDungeon");
            }
        }
    }
    private void OnTriggerExit2D(UnityEngine.Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Guidance.SetActive(false);
        }
    }
}
