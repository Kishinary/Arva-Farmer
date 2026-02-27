using UnityEngine;
using System.Collections;

public class Watercan : MonoBehaviour
{
    [Header("Laser Settings")]
    public float damage = 50f;
    public float maxLaserDistance = 10f;
    public float fadeDuration = 0.5f; // How long it takes to disappear
    public float initialWidth = 0.2f;
    public LayerMask hitLayers;

    [Header("References")]
    public LineRenderer lineRenderer;
    public Transform firePoint;
    public GameObject impactEffect;

    private bool isShooting = false;

    [Header("Components")]
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 2;
    }

    void Update()
    {
    }

    public void Attack()
    {
        if (!isShooting)
        {
            animator.SetTrigger("Shoot");
            StartCoroutine(ShootLaserPulse());
        }
    }
    IEnumerator ShootLaserPulse()
    {
        isShooting = true;
        lineRenderer.enabled = true;

        Vector3 startPoint = firePoint.position;

        RaycastHit2D hit = Physics2D.Raycast(startPoint, firePoint.right, maxLaserDistance, hitLayers);
        Vector3 endPoint = hit ? (Vector3)hit.point : startPoint + (firePoint.right * maxLaserDistance);

        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);

        if (hit && hit.collider.TryGetComponent(out EnemyStats health))
        {
            health.TakeDamage(transform.position, damage);
        }

        if (hit && impactEffect != null)
            Instantiate(impactEffect, endPoint, Quaternion.identity);

        // 4. Animate the Fade (Width and Color only)
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float pct = 1 - (elapsed / fadeDuration);

            lineRenderer.startWidth = initialWidth * pct;
            lineRenderer.endWidth = initialWidth * pct;

            // Note: We are NOT calling SetPosition(0, firePoint.position) here anymore.
            // This keeps the laser "frozen" in the air where it was fired.

            yield return null;
        }

        lineRenderer.enabled = false;
        isShooting = false;
    }
}