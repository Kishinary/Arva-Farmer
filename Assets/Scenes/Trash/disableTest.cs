using UnityEngine;

public class disableTest : MonoBehaviour
{
    public GameObject boss;
    public GameObject blackScreen;
    public GameObject[] trash;
    public GameObject player;
    private PlayerHealth health;

    // Update is called once per frame

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        health = player.GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (boss == null || health.CurrentHealth == 0)
        {
            blackScreen.SetActive(true);
            for (int i = 0; i < trash.Length; i++)
            {

                Destroy(trash[i]);
            }
        }
    }
}
