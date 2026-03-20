using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnFF : MonoBehaviour
{
    [Header("Racket")]
    private BugRacket Player;
    public bool spawnable = true;
    private List<GameObject> activeFireflies = new List<GameObject>();
    public List<GameObject> Fireflies = new List<GameObject>();
    public List<GameObject> SpawnLoc = new List<GameObject>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = GameObject.FindWithTag("Player").GetComponent<BugRacket>();
    }

    // Update is called once per frame
    void Update()
    {
        FireflyPopulation();
    }
    Vector2 RandomSpawnPosition()
    {
        if (SpawnLoc.Count == 0) return Vector2.zero;
        return SpawnLoc[Random.Range(0, SpawnLoc.Count)].transform.position;
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
                    Vector2 spawnPos = RandomSpawnPosition();

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
                    Vector2 spawnPos = RandomSpawnPosition();

                    GameObject newFirefly = Instantiate(prefab, new Vector3(spawnPos.x, spawnPos.y, 0), Quaternion.identity);
                    activeFireflies.Add(newFirefly);
                }
            }
        }
    }
}
