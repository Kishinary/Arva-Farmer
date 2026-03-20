using UnityEngine;

public class CharacterSpawn : MonoBehaviour
{
    public Transform playerSpawn;
    void Start()
    {
        var Player = GameObject.FindWithTag("Player");
        Player.transform.position = playerSpawn.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
