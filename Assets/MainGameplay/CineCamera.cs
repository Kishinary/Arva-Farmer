using UnityEngine;
using Unity.Cinemachine;
public class CineCamera : MonoBehaviour
{
    private GameObject Player;
    private CinemachineCamera m_Camera;
    private void Start()
    {
        m_Camera = GetComponent<CinemachineCamera>();
        Player = GameObject.FindWithTag("Player");
        m_Camera.Follow = Player.transform;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
