using Unity.Cinemachine;
using UnityEngine;

public class NPCDialogue : MonoBehaviour, IInteractable
{
    public GameObject Dialogue;
    public bool interacted;
    void Start()
    {
    }
    public void Interact()
    {
        interacted = true;
        Dialogue.SetActive(true);
    }
}
