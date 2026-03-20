using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class BossEntrance : MonoBehaviour, IInteractable
{
    public void Interact()
    {
    }
    private void Start()
    {

    }

    private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            SceneController.Instance.NextScene("Dungeon2-4", true);

        }
    }

}