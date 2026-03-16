using NUnit.Framework;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class DungeonSpawner : MonoBehaviour
{
    public static DungeonSpawner instance { get; private set; }

    public SoundManager SoundManager;
    public List<GameObject> DungeonMap = new List<GameObject>();

    public GameObject player;

    public string NextScene;
    private string currentScene;

    [Header("Difficulty Changer")]
    public float HealthMultiplier;
    public float DamageMultiplier;
    void Start()
    {

        currentScene = SceneManager.GetActiveScene().name;
        SceneMultiplier(currentScene);

        player = FindFirstObjectByType<PlayerMovement>().gameObject;
        Instantiate(SoundManager);

        int randomIndex = Random.Range(0, DungeonMap.Count);
        GameObject ChoosenDungeonPrefab = DungeonMap[randomIndex];

        GameObject spawnedDungeon = Instantiate(ChoosenDungeonPrefab);

        Spawner spawner = spawnedDungeon.GetComponentInChildren<Spawner>();

        if (spawner != null && spawner.PlayerSpawn != null)
        {
            player.transform.position = spawner.PlayerSpawn.position;
        }
    }

    void Update()
    {
        
    }
    public void SceneMultiplier(string scene)
    {
        // Default values
        HealthMultiplier = 1f;
        DamageMultiplier = 1f;

        // Extract the level number (e.g., from "Dungeon1-3" get 3)
        // Or manually set them like you started:
        if (scene == "Dungeon1-1") { NextScene = "Dungeon1-2"; HealthMultiplier = 1.0f; DamageMultiplier = 1.0f; }
        else if (scene == "Dungeon1-2") { NextScene = "Dungeon1-3"; HealthMultiplier = 1.1f; DamageMultiplier = 1.05f; }
        else if (scene == "Dungeon1-3") { NextScene = "Dungeon1-4"; HealthMultiplier = 1.21f; DamageMultiplier = 1.1f; }
        else if (scene == "Dungeon1-4") { NextScene = "Dungeon1-5"; HealthMultiplier = 1.33f; DamageMultiplier = 1.15f; }
        else if (scene == "Dungeon1-5") { NextScene = "Shop"; HealthMultiplier = 1.5f; DamageMultiplier = 1.2f; }
        else if (scene == "Shop") { NextScene = "Boss1"; HealthMultiplier = 1f; DamageMultiplier = 1.2f; }
        else if (scene == "Boss1") { NextScene = "Dungeon2-1"; HealthMultiplier = 1f; DamageMultiplier = 1.2f; }
        else if (scene == "Dungeon2-1") { NextScene = "Dungeon2-2"; HealthMultiplier = 1.1f; DamageMultiplier = 1.2f; }
        else if (scene == "Dungeon2-2") { NextScene = "Dungeon2-3"; HealthMultiplier = 1.2f; DamageMultiplier = 1.2f; }
        else if (scene == "Dungeon2-3") { NextScene = "Dungeon2-4"; HealthMultiplier = 1.3f; DamageMultiplier = 1.2f; }
        else if (scene == "Dungeon2-4") { NextScene = "Dungeon2-5"; HealthMultiplier = 1.4f; DamageMultiplier = 1.2f; }
        else if (scene == "Dungeon2-5") { NextScene = "Shop2"; HealthMultiplier = 1.5f; DamageMultiplier = 1.2f; }
        else if (scene == "Shop2") { NextScene = "Boss2"; HealthMultiplier = 1f; DamageMultiplier = 1.2f; }
        else if (scene == "Boss2") { NextScene = "Dungeon2-6"; HealthMultiplier = 1.5f; DamageMultiplier = 1.2f; }
        else if (scene == "Dungeon2-6") { NextScene = "Dungeon2-7"; HealthMultiplier = 1.6f; DamageMultiplier = 1.2f; }
        else if (scene == "Dungeon2-7") { NextScene = "Dungeon2-8"; HealthMultiplier = 1.75f; DamageMultiplier = 1.2f; }
        else if (scene == "Dungeon2-8") { NextScene = "Dungeon2-9"; HealthMultiplier = 1.75f; DamageMultiplier = 1.2f; }
        else if (scene == "Dungeon2-9") { NextScene = "Dungeon2-10"; HealthMultiplier = 1.75f; DamageMultiplier = 1.2f; }
        else if (scene == "Dungeon2-10") { NextScene = "Shop3"; HealthMultiplier = 1f; DamageMultiplier = 1.2f; }
        else if (scene == "Shop3") { NextScene = "Boss3"; HealthMultiplier = 1f; DamageMultiplier = 1.2f; }
        else if (scene == "Boss3") { NextScene = "Lobby"; HealthMultiplier = 1f; DamageMultiplier = 1.2f; }
    }
}


