using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public Transform Playerpawn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var Player = GameObject.FindWithTag("Player");
        Player.transform.position = Playerpawn.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
