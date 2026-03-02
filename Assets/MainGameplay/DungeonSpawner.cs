using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class DungeonSpawner : MonoBehaviour
{
    public SoundManager SoundManager;
    public List<GameObject> DungeonMap = new List<GameObject>();

    public GameObject player;

    public string NextScene;
    private string currentScene;
    void Start()
    {
        player = FindFirstObjectByType<PlayerMovement>().gameObject;
        Instantiate(SoundManager);
        int randomIndex = Random.Range(0, DungeonMap.Count);
        GameObject ChoosenDungeon = DungeonMap[randomIndex];
        Instantiate(ChoosenDungeon);
        Spawner spawner = ChoosenDungeon.GetComponentInChildren<Spawner>();
        player.transform.position = spawner.PlayerSpawn.position;
        currentScene = SceneManager.GetActiveScene().name;
        SceneMultiplier(currentScene);
    }

    void Update()
    {
        
    }
    public void SceneMultiplier(string scene)
    {
        if (scene == "Dungeon1-1")
        {
            NextScene = "Dungeon1-2";
        }
        if (scene == "Dungeon1-2")
        {
            NextScene = "Dungeon1-3";
        }
        if (scene == "Dungeon1-3")
        {
            NextScene = "Dungeon1-4";
        }
        if (scene == "Dungeon1-4")
        {
            NextScene = "Dungeon1-5";
        }
        if (scene == "Dungeon1-5")
        {
            NextScene = "Shop";
        }

    }

}

