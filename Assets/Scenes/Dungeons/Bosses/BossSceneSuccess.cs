using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossSceneSuccess : MonoBehaviour
{
    public bool goable = false;
    public bool going = false;

    public GameObject YesGameObject;
    public GameObject nahh;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(StartCheck());
    }

    // Update is called once per frame
    void Update()
    {
        if (goable)
        {
            if (!GameObject.FindWithTag("Enemy"))
            {
                going = true;
                goable = false;
                if (SceneManager.GetActiveScene().name == "Boss2")
                {
                    SceneController.Instance.NextScene("Lobby", true);

                }
                else
                {
                    Instantiate(nahh, YesGameObject.transform.position, Quaternion.identity);
                    Destroy(YesGameObject);
                }
            }
        }
    }

    IEnumerator StartCheck()
    {
        yield return new WaitForSeconds(5);
        goable = true;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (going && collision.gameObject.GetComponent<PlayerMovement>())
        {
           SceneController.Instance.NextScene("Dungeon2-1", true);
        }
    }




}
