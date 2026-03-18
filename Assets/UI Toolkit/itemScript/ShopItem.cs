using System;
using UnityEngine;
[Serializable]
public struct ShopItem
{
    public string itemName;
    public Sprite itemIcon;
    public GameObject dropPrefab;
    public int price;
    [TextArea(3, 5)]
    public string description;
}