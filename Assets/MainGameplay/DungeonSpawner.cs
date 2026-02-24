using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DungeonSpawner : MonoBehaviour
{
    public List<GameObject> DungeonMap = new List<GameObject>();
    void Start()
    {
        int randomIndex = Random.Range(0, DungeonMap.Count);
        GameObject ChoosenDungeon = DungeonMap[randomIndex];
        Instantiate(ChoosenDungeon);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
