using UnityEngine;
using System;
using System.Collections;
public class RockLaserConnectToBoss : MonoBehaviour
{
    public GameObject rockPrefab;
    public IEnumerator RockSummon(Action onComplete)
    {
        Instantiate(rockPrefab, Vector2.zero, Quaternion.identity);

        yield return new WaitForSeconds(0.5f);
        onComplete?.Invoke();
    }



}
