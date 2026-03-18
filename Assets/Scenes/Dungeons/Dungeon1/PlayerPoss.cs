using UnityEngine;

public class PlayerPoss : MonoBehaviour
{
    public Transform PlayerPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Transform Player = GameObject.FindWithTag("Player").transform;
        if (Player) Player.position = PlayerPos.position;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
