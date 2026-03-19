using System.Collections.Generic;
using UnityEngine;

public class ShopPedestal : MonoBehaviour, IInteractable
{
    public Transform playerTransform; 
    public float dropDistance = 2f;
    public GameObject Orb;
    bool interactable = true;

    public List<GameObject> otherItem = new();
    public GameObject DestroyEffect;

    private void Start()
    {}
    public void Interact()
    {
        if (!interactable) return;
        // 2. Define the positions
        // Start is the shop's position
        Vector3 startPos = transform.position;

        // End is a point between the shop and the player
        Vector3 endPos = transform.position - new Vector3(0, 3, 0);

        // 3. Call the function
        Orb.GetComponent<ItemDropAnimation>().LaunchItem(startPos, endPos);
        for (int i = 0; i < otherItem.Count; i++)
        {
            Destroy(otherItem[i]);
            Instantiate(DestroyEffect, otherItem[i].transform.position, Quaternion.identity);
        }
        interactable = false;

    }
}