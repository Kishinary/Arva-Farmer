using UnityEngine;
using System.Collections.Generic; // Required for Dictionary

public class Traps : MonoBehaviour
{
    public float damage;
    public string Traptype;
    public float stunDuration = 2f;
    public float spikeDamageInterval = 0.5f; // New variable for interval

    // Tracks: Key = Enemy Instance ID, Value = Next time they can take damage
    private Dictionary<int, float> damageTimers = new Dictionary<int, float>();

    private void Start()
    {
        Destroy(this.gameObject, 7);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyStats stats = collision.GetComponent<EnemyStats>();
            if (stats == null) return;

            if (Traptype == "BearTrap")
            {
                stats.TakeDamage(transform.position, damage * 3);
                Destroy(this.gameObject);
            }
            else if (Traptype == "HoleTrap")
            {
                stats.TriggerPit(1f);
                stats.TriggerSLowed(0, 2f);
                Destroy(this.gameObject);
            }
            else if (Traptype == "RakeTrap")
            {
                stats.TakeDamage(transform.position, damage/2);
                stats.TriggerSLowed(0, 2f);
            }
            // Note: SpikeTrap initial hit is handled by OnTriggerStay logic below
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Traptype == "SpikeTrap" && collision.CompareTag("Enemy"))
        {
            int enemyID = collision.GetInstanceID();

            // Check if the enemy is in our timer dictionary
            if (!damageTimers.ContainsKey(enemyID) || Time.time >= damageTimers[enemyID])
            {
                // Deal Damage
                collision.GetComponent<EnemyStats>().TakeDamage(transform.position, damage);

                // Set the next allowed damage time (Current Time + 0.5s)
                damageTimers[enemyID] = Time.time + spikeDamageInterval;
            }
        }
    }
}