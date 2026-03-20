using System.Collections;
using UnityEngine;

public class bulletManager : MonoBehaviour
{

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject indicator;

    [SerializeField] private float bulletSpeed = 5f;

    [SerializeField] private float offsetAngle = 0f;
    private void Start()
    {

        StartCoroutine(bulletPerform());
        offsetAngle = Random.Range(0f, 360f);
        Destroy(gameObject,3f);
    }
    private IEnumerator bulletPerform()
    {
        Instantiate(indicator, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(1f);
        Fire8WayBurst();
    }
    private void Fire8WayBurst()
    {
        for (int i = 0; i < 2; i++)
        {
            float angleRad = ((i * 45f) + offsetAngle) * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
            GameObject newBullet = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

            bulletMovement bulletComponent = newBullet.GetComponent<bulletMovement>();
            if (bulletComponent != null)
            {
                bulletComponent.Launch(direction, bulletSpeed);
            }

        }
    }



}
