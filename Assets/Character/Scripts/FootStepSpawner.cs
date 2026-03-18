using UnityEngine;

public class FootStepSpawner : MonoBehaviour
{
    public GameObject footstepPrefab;
    public float distanceThreshold = 0.5f;

    private Vector3 lastSpawnPosition;

    void Start()
    {
        lastSpawnPosition = transform.position;
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, lastSpawnPosition);

        if (distance >= distanceThreshold)
        {
            SpawnFootstep();
            lastSpawnPosition = transform.position;
        }
    }

    void SpawnFootstep()
    {
        Instantiate(footstepPrefab, transform.position, Quaternion.identity);
    }
}