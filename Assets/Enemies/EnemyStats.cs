using System.Collections;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public float health = 20;
    public float knockbackForce = 8f;

    public bool invincible = false;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(Vector2 hitSource, float damage)
    {
        if (invincible) return;
        
        health -= damage;

        // Calculate knockback direction
        Vector2 knockbackDir = (transform.position - (Vector3)hitSource).normalized;

        // Reset velocity so knockback feels responsive
        rb.linearVelocity = Vector2.zero;

        // Apply knockback
        rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);

        if (health <= 0)
        {
            Destroy(gameObject);
        }
        StartCoroutine(Invincibility());

    }
    IEnumerator Invincibility()
    {
        invincible = true;
        yield return new WaitForSeconds(0.5f);
        invincible = false;
    }
}
