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

    [Header("Spawn Settings")]
    public List<GameObject> SpawnLoc = new List<GameObject>();
    public Transform PlayerSpawn;

    [Header("UI & Victory Logic")]
    [Tooltip("Kéo Prefab WinningCanvas vào đây")]
    public GameObject winningCanvasPrefab; 
    
    public bool goable = false; // Trạng thái đã dọn sạch phòng
    private bool startCounting = false; // Chỉ bắt đầu đếm sau khi quái đã spawn
    private string NextScene;

    void Start()
    {
        // 1. Chặn script chạy ở các Scene không phải màn chơi chính
        if (IsRestrictedScene()) return;

        // 2. Random đội hình quái (0-9)
        int rand = Random.Range(0, 10);
        StartCoroutine(SpawnStarter(rand));
    }

    private void Update()
    {
        // Kiểm tra Scene hợp lệ
        if (IsRestrictedScene()) return;

        // Chỉ kiểm tra quái sau khi kết thúc 1.5s chờ ở Coroutine
        if (!startCounting) return;

        // Tìm quái vật hiện có trong Scene
        EnemyStats enemyManager = FindFirstObjectByType<EnemyStats>();

        // Logic xuất hiện Winning Canvas: Hết quái + Chưa từng hiện bảng
        if (enemyManager == null && !goable)
        {
            DungeonSpawner dSpawner = FindFirstObjectByType<DungeonSpawner>();
            if (dSpawner != null)
            {
                NextScene = dSpawner.NextScene;
                
                if (!string.IsNullOrEmpty(NextScene))
                {
                    goable = true; // Đánh dấu đã xong để không Instantiate liên tục
                    ShowWinningNotification();
                }
            }
        }
    }

    // Hàm lọc các Scene không muốn chạy logic thắng/thua
    bool IsRestrictedScene()
    {
        string currentName = SceneManager.GetActiveScene().name.ToLower();
        return currentName.Contains("mainmenu") || 
               currentName.Contains("logoscene") || 
               currentName.Contains("lobby");
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
        yield return new WaitForSeconds(1.5f);
        
        Instantiating((int)rand);
        
        // Sau khi gọi hàm tạo quái xong, mới cho phép Update quét quái
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
        }
    }

    Vector2 RandomSpawnPosition()
    {
        if (SpawnLoc.Count == 0) return Vector2.zero;
        return SpawnLoc[Random.Range(0, SpawnLoc.Count)].transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Khi đã thắng (goable = true) và Player chạm vào Portal/Vùng chuyển cảnh
        if (goable && collision.CompareTag("Player"))
        {
            SceneManager.LoadScene(NextScene);
        }
    }
}