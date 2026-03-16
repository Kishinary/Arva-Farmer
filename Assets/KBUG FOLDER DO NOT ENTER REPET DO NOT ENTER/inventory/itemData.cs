using UnityEngine;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Core Data")]
    public string id; 
    public string itemName;
    public Sprite itemIcon;

    [TextArea(3, 5)]
    public string itemDescription;

    [Header("Inventory Rules")]
    public int maxStackSize = 64; 
    public GameObject itemPrefab; 

    [Header("Abilities (Optional)")]
    public Ability abilityScriptableObject; 
}