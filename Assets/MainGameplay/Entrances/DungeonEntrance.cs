using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class DungeonEntrance : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        SceneManager.LoadScene("Dungeon1-1");

    }
    private void Start()
    {

    }

    private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
        }
    }

}
