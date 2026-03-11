using UnityEngine;
using System.Collections;

public class flyRock : MonoBehaviour
{
    public GameObject rockToLeft;
    public GameObject rockToRight;
    public float lifeTime;


    private void Start()
    {
        lifeTime = 5f;
        StartCoroutine(SummonAndSplit());
        Destroy(gameObject, lifeTime);
    }
    private IEnumerator SummonAndSplit()
    {
        float randomX = Random.Range(-2.5f, 2f);
        float randomY = Random.Range(-4f, 8.5f);

        Vector3 spawnPosition = new Vector3( randomX, randomY, 0f);


        //effect here
        yield return new WaitForSeconds(0.3f);
        GameObject leftRock = Instantiate(rockToLeft, spawnPosition, Quaternion.identity);
        leftRock.SetActive(true);

        GameObject rightRock = Instantiate(rockToRight, spawnPosition, Quaternion.identity);
        rightRock.SetActive(true);

    }




}
