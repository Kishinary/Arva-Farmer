using NUnit.Framework;
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

    public List<GameObject> SpawnLoc = new List<GameObject>();

    public Transform PlayerSpawn;
    public List<GameObject> Enemies = new List<GameObject>();

    public bool enemies = false;
    public bool goable = false;
    private string NextScene;
    public GameObject Notifi;

    private bool startCounting = false;
    void Start()
    {
        float rand = Random.Range(0, 9);
        StartCoroutine(SpawnStarter(rand));
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
