using UnityEngine;

public class SolidDirtWall : MonoBehaviour
{
    [Header("Settings")]
    public float damage = 20f;
    public float knockbackStrength = 30f; // Higher for solid hits
    public Vector2 wallSize = new Vector2(2f, 3f);
    public LayerMask playerLayer;

    private bool hasDealtSpawnDamage = false;

    private void Start()
    {
        // 1. INSTANT CHECK: Catch the player if they are inside the solid block on spawn
        Collider2D hit = Physics2D.OverlapBox(transform.position, wallSize, transform.eulerAngles.z, playerLayer);

        if (hit != null)
        {
            ApplyHeavyHit(hit.gameObject);
            hasDealtSpawnDamage = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 2. REGULAR COLLISION: Catch players who walk into the wall later
        if (collision.gameObject.CompareTag("Player") && !hasDealtSpawnDamage)
        {
            ApplyHeavyHit(collision.gameObject);
        }
    }

    private void ApplyHeavyHit(GameObject player)
    {
        var playerMove = player.GetComponent<PlayerMovement>();
        if (playerMove != null)
        {
            // Calculate direction from the CENTER of the wall to the player
            Vector2 knockDir = (player.transform.position - transform.position).normalized;

            // Fallback if exactly at center
            if (knockDir == Vector2.zero) knockDir = transform.up;

            // Trigger the knockback logic in your PlayerMovement script
            playerMove.ApplyKnockback(knockDir * knockbackStrength, 0.25f);

            // Optional: damage the player here
            // player.GetComponent<PlayerStats>()?.TakeDamage(damage);
        }
    }

    // Visual aid to make sure the box matches your wall in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(Vector3.zero, wallSize);
    }
}