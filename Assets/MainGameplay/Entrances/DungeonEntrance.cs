using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class DungeonEntrance : MonoBehaviour, IInteractable
{
    public GameObject Guidance;

    public void Interact()
    {
        SceneManager.LoadScene("MainDungeon");
    }

    private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Guidance.SetActive(true);
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
