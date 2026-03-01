using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Frame Settings")]
    [Tooltip("Tâm của khung sinh thành so với Transform này")]
    public Vector2 centerOffset = Vector2.zero;

    public GameObject enemyPrefab;

    [Tooltip("Kích thước chiều rộng (X) và chiều cao (Y) của khung")]
    public Vector2 areaSize = new Vector2(100f, 50f);



    public void SpawnEnemyAtPosition(Vector3 position)
    {
        
        Initiate();
    }

    public Vector2 GetRandomPositionInFrame()
    {
        // Tính toán tọa độ tâm thực tế trên World Space
        Vector2 worldCenter = (Vector2)transform.position + centerOffset;

        // Tính ranh giới (Bounds) một nửa
        float halfWidth = areaSize.x / 2f;
        float halfHeight = areaSize.y / 2f;

        // Sinh số ngẫu nhiên cho trục X và Y
        float randomX = Random.Range(worldCenter.x - halfWidth, worldCenter.x + halfWidth);
        float randomY = Random.Range(worldCenter.y - halfHeight, worldCenter.y + halfHeight);

        return new Vector2(randomX, randomY);
    }
    private void Initiate()
    {
       
        Vector2 worldCenter = (Vector2)transform.position + centerOffset;
        GameObject enemyInstance = Instantiate(enemyPrefab, worldCenter + GetRandomPositionInFrame(), Quaternion.identity);
        Debug.Log($"Spawned enemy at: {worldCenter}");
    }
}
