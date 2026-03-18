using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject Toucher;
    public GameObject Archer;
    public GameObject Summoner;
    public GameObject Mage;
    public GameObject Charger;

    public Vector2 randomP;

    public List<GameObject> SpawnLoc = new();
    public List<GameObject> Fireflies = new();

    public Transform PlayerSpawn;

    [Header("UI & Victory Logic")]
    public GameObject winningCanvasPrefab; 
    
    public bool goable = false; 
    private bool startCounting = false;
    private string NextScene;

    [Header("Racket")]
    private BugRacket Player;
    public bool spawnable = true;
    private List<GameObject> activeFireflies = new List<GameObject>();

    public GameObject Notifi;

    [Header("Spawn")]
    public GameObject spawnAni;
    void Start()
    {
        // 1. Chặn script chạy ở các Scene không phải màn chơi chính
        if (IsRestrictedScene()) return;

        // 2. Random đội hình quái (0-9)
        int rand = Random.Range(0, 10);
        StartCoroutine(SpawnStarter(rand));
        Player = FindFirstObjectByType<BugRacket>();
        if (Player)
        {
            spawnable = true;
        }
    }

    private void Update()
    {
        FireflyPopulation();

        // Combined Logic: Only run if we haven't already finished (goable is false)
        if (!goable && startCounting)
        {
            SceneChecker();
        }
    }

    private void SceneChecker()
    {
        // 1. Safety Check
        if (IsRestrictedScene()) return;

        // 2. Check if enemies still exist
        EnemyStats enemyManager = FindFirstObjectByType<EnemyStats>();

        // 3. If NO enemies are left
        if (enemyManager == null)
        {
            DungeonSpawner dSpawner = FindFirstObjectByType<DungeonSpawner>();

            // 4. Check if the Dungeon Spawner has prepared the next level
            if (dSpawner != null && !string.IsNullOrEmpty(dSpawner.NextScene))
            {
                NextScene = dSpawner.NextScene;
                goable = true; // Mark as finished

                // Trigger your UI
                Instantiate(Notifi);
                ShowWinningNotification();
            }
        }
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


    void ShowWinningNotification()
    {
        if (winningCanvasPrefab != null)
        {
            // Tạo bảng thông báo
            GameObject notice = Instantiate(winningCanvasPrefab);
            
            // Nếu bạn dùng Animation hoặc Script WinningFade dài 3s, 
            // ta hủy object sau 3.1s để dọn dẹp bộ nhớ
            Destroy(notice, 3.1f);
        }
    }

    IEnumerator SpawnStarter(float rand)
    {
        // Đợi 1.5s để Scene ổn định và tránh lỗi "vừa vào đã hiện"
        yield return new WaitForSeconds(2f);
        
        Instantiating((int)rand);

        // Sau khi gọi hàm tạo quái xong, mới cho phép Update quét quái
        yield return new WaitForSeconds(1.5f);

        startCounting = true;
    }

    // --- Giữ nguyên logic đội hình quái của bạn ---
    void Instantiating(int rand)
    {
        switch (rand)
        {
            case 0: AutoSummon(Summoner); AutoSummon(Toucher, 4); break;
            case 1: AutoSummon(Archer, 3); AutoSummon(Mage, 2); break;
            case 2: AutoSummon(Charger, 2); AutoSummon(Toucher, 3); break;
            case 3: AutoSummon(Summoner, 2); AutoSummon(Archer); AutoSummon(Mage); break;
            case 4: AutoSummon(Charger); AutoSummon(Toucher); AutoSummon(Archer); AutoSummon(Mage); AutoSummon(Summoner); break;
            case 5: AutoSummon(Charger, 3); AutoSummon(Toucher, 3); break;
            case 6: AutoSummon(Archer); AutoSummon(Mage, 3); AutoSummon(Toucher, 2); break;
            case 7: AutoSummon(Charger, 5); break;
            case 8: AutoSummon(Charger); AutoSummon(Archer, 5); break;
            case 9: AutoSummon(Summoner, 3); AutoSummon(Archer, 2); break;
        }
    }

    void AutoSummon(GameObject summonedObject, float amount = 1)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector2 spawnPos = RandomSpawnPosition();
            Instantiate(summonedObject, new Vector3(spawnPos.x, spawnPos.y, 0), Quaternion.identity);
            Instantiate(spawnAni, new Vector3(spawnPos.x, spawnPos.y -0.5f, 0), Quaternion.identity);
        }
    }

    Vector2 RandomSpawnPosition()
    {
        if (SpawnLoc.Count == 0) return Vector2.zero;
        return SpawnLoc[Random.Range(0, SpawnLoc.Count)].transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (goable && collision.CompareTag("Player"))
        {
            SceneManager.LoadScene(NextScene);
        }
    }
    bool IsRestrictedScene()
    {
        string currentName = SceneManager.GetActiveScene().name.ToLower();
        return currentName.Contains("mainmenu") ||
               currentName.Contains("logoscene") ||
               currentName.Contains("lobby");
    }
}