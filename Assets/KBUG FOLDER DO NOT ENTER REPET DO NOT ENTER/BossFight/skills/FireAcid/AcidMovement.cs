using UnityEngine;
using System;
using System.Collections;
public class AcidMovement : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float bulletSpeed = 6f;
    public float bulletCount = 4;
    public IEnumerator ExecuteEightWayShoot(Vector2 firePosition, Action onComplete)
    {
        yield return new WaitForSeconds(1f);
        float angleStep = 360f / bulletCount;
        float currentAngle = UnityEngine.Random.Range(0f, 45f);
        for (int i = 0; i < bulletCount; i++)
        {
            Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);
            GameObject bullet = Instantiate(bulletPrefab, firePosition, rotation);

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = bullet.transform.up * bulletSpeed;
            }
            currentAngle += angleStep;
        }
        yield return new WaitForSeconds(0f);

        onComplete?.Invoke();
    }



}
