using Unity.Cinemachine;
using UnityEngine;

public class Angler : MonoBehaviour, IInteractable
{
    public GameObject Dialogue;
    public bool interacted;

    [Header("Player")]
    private GameObject Player;
    private CinemachineCamera cine;
    void Start()
    {
        Player = GameObject.FindWithTag("Player");
        cine = FindFirstObjectByType<CinemachineCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (interacted)
        //{
         //   Player.GetComponent<PlayerMovement>().DisablePlayerInput();
        //}
        //else
        //{
          //  Player.GetComponent<PlayerMovement>().EnablePlayerInput();
        //}
    }

    public void Interact()
    {
        interacted = true;
        Dialogue.SetActive(true);
        cine.GetComponent<CameraZoom>();
    }
}
