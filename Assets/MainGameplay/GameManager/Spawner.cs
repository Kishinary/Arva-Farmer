using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Spawner : MonoBehaviour
{
    public GameObject Toucher;
    public GameObject Archer;
    public GameObject Summoner;
    public GameObject Mage;
    public GameObject Charger;

    public Vector2 randomP;

    public List<GameObject> SpawnLoc = new();
    public List<GameObject> Fireflies = new();

    public Transform PlayerSpawn;
    public List<GameObject> Enemies = new List<GameObject>();

    public bool enemies = false;
    public bool goable = false;
    private string NextScene;
    public GameObject Notifi;


    private bool startCounting = false;

    [Header("Racket")]
    public BugRacket Player;
    public bool spawnable = true;
    private List<GameObject> activeFireflies = new List<GameObject>();
    void Start()
    {
        int rand = Random.Range(0, 10); 
        StartCoroutine(SpawnStarter(rand));
        Player = FindFirstObjectByType<BugRacket>();
        if (Player.GetComponent<BugRacket>())
        {
            spawnable = true;
        }
    }

    private void Update()
    {
        EnemyStats enemyManager = FindFirstObjectByType<EnemyStats>();
        if (!enemyManager)
        {
            NextScene = FindFirstObjectByType<DungeonSpawner>().NextScene;
            if (NextScene != null && goable == false && startCounting) {
                goable = true;
                Instantiate(Notifi);
            }
            return;
        }
        FireflyPopulation();

    }

    private void FireflyPopulation()
    {
        activeFireflies.RemoveAll(f => f == null);
        if (Player)
        {
            if (activeFireflies.Count + Player.storedBugObject.Count < 8 && Fireflies.Count > 0)
            {
                while (activeFireflies.Count + Player.storedBugObject.Count < 8)
                {
                    float roll = Random.Range(0f, 100f);
                    GameObject prefab;

                    // 2. Assign prefab based on the roll
                    if (roll < 50f)
                    {
                        // 50% chance
                        prefab = Fireflies[0];
                    }
                    else if (roll < 80f)
                    {
                        // 30% chance (50 + 30 = 80)
                        prefab = Fireflies[1];
                    }
                    else
                    {
                        // 20% chance (the remaining 80 to 100)
                        prefab = Fireflies[2];
                    }

                    // Get a random spawn location from your list
                    Vector2 spawnPos = RandomSpawn();

                    GameObject newFirefly = Instantiate(prefab, new Vector3(spawnPos.x, spawnPos.y, 0), Quaternion.identity);
                    activeFireflies.Add(newFirefly);
                }
            }
        }
        else
        {
            if (activeFireflies.Count < 7 && Fireflies.Count > 0)
            {
                while (activeFireflies.Count < 7)
                {
                    float roll = Random.Range(0f, 100f);
                    GameObject prefab;

                    // 2. Assign prefab based on the roll
                    if (roll < 50f)
                    {
                        // 50% chance
                        prefab = Fireflies[0];
                    }
                    else if (roll < 80f)
                    {
                        // 30% chance (50 + 30 = 80)
                        prefab = Fireflies[1];
                    }
                    else
                    {
                        // 20% chance (the remaining 80 to 100)
                        prefab = Fireflies[2];
                    }

                    // Get a random spawn location from your list
                    Vector2 spawnPos = RandomSpawn();

                    GameObject newFirefly = Instantiate(prefab, new Vector3(spawnPos.x, spawnPos.y, 0), Quaternion.identity);
                    activeFireflies.Add(newFirefly);
                }
            }
        }

    }
    IEnumerator SpawnStarter(float rand)
    {
        yield return new WaitForSeconds(1.5f);
        Instantiating(rand);
        startCounting = true;   
    }
    void Instantiating(float rand)
    {
        switch (rand)
        {
            case 0:
                AutoSummon(Summoner);
                AutoSummon(Toucher, 4);
                break;
            case 1:
                AutoSummon(Archer, 3);
                AutoSummon(Mage,2);
                break;
            case 2:
                AutoSummon(Charger, 2);
                AutoSummon(Toucher, 3);
                break;
            case 3:
                AutoSummon(Summoner, 2);
                AutoSummon(Archer);
                AutoSummon(Mage);
                break;
            case 4:
                AutoSummon(Charger);
                AutoSummon(Toucher);
                AutoSummon(Archer);
                AutoSummon(Mage);
                AutoSummon(Summoner);
                break;
            case 5:
                AutoSummon(Charger, 3);
                AutoSummon(Toucher, 3);
                break;
            case 6:
                AutoSummon(Archer);
                AutoSummon(Mage, 3);
                AutoSummon(Toucher, 2);
                break;
            case 7:
                AutoSummon(Charger, 5);
                break;
            case 8:
                AutoSummon(Charger);
                AutoSummon(Archer, 5);
                break;
            case 9:
                AutoSummon(Summoner, 3);
                AutoSummon(Archer,2);
                break;

        }
    }


    void AutoSummon(GameObject summonedObject, float amount = 1)
    {
        randomP = new Vector2(Random.Range(0,10), Random.Range(0,10));
        for (int i = 0; i < amount; i++)
        {
            Vector2 spawner = RandomSpawn();
            Instantiate(summonedObject, new Vector3(spawner.x, spawner.y, 0), Quaternion.identity);
        }
    }

    Vector2 RandomSpawn()
    {
        Vector2 Spawnlocation = new Vector2(0, 0);
        Spawnlocation = SpawnLoc[Random.Range(0, SpawnLoc.Count)].transform.position;
        return Spawnlocation;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (goable == true)
        {
            SceneManager.LoadScene(NextScene);
        }
    }
}
