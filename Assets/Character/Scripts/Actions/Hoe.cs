using UnityEngine;
using System.Collections;

public class Hoe : MonoBehaviour
{
    public GameObject spikePrefab;

    public float spikeDistance = 1.5f;
    public float spikeSpacing = 1f;
    public int maxSpikes = 6;

    float holdTimer;
    bool holding;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            holdTimer = 0;
            holding = true;
        }

        if (holding)
        {
            holdTimer += Time.deltaTime;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (holdTimer < 0.25f)
            {
                SingleSpike();
            }
            else
            {
                StartCoroutine(SpikeLine());
            }

            holding = false;
        }
    }

    

    void SingleSpike()
    {
        Vector2 dir = MouseDirection();

        Vector2 spawnPos = (Vector2)transform.position + dir * spikeDistance;

        Instantiate(spikePrefab, spawnPos, Quaternion.identity);
    }

    IEnumerator SpikeLine()
    {
        Vector2 dir = MouseDirection();

        for (int i = 1; i <= maxSpikes; i++)
        {
            Vector2 pos = (Vector2)transform.position + dir * spikeSpacing * i;

            Instantiate(spikePrefab, pos, Quaternion.identity);

            yield return new WaitForSeconds(0.05f); // smooth eruption
        }
    }

    Vector2 MouseDirection()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return (mouse - transform.position).normalized;
    }
}