using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSpawn : MonoBehaviour
{
    public Transform playerSpawn;
    public GameObject Playser;
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            Instantiate(Playser, transform.position, Quaternion.identity);
        }
        var Player = GameObject.FindWithTag("Player");
        Player.transform.position = playerSpawn.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
