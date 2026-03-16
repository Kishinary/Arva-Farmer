using UnityEngine;
using System.Collections;

public class Traps : MonoBehaviour
{
    public string Traptype;
    public float stunDuration = 2f;

    private void Start()
    {
        Destroy(this.gameObject, 7);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyStats stats = collision.GetComponent<EnemyStats>();

            if (Traptype == "BearTrap")
            {
                stats.TakeDamage(transform.position, 30);
                Destroy(this.gameObject);  
            }
            else if (Traptype == "SpikeTrap")
            {
                stats.TakeDamage(transform.position, 10);
            }
            else if (Traptype == "HoleTrap")
            {
                collision.GetComponent<EnemyStats>().TriggerPit(1f);
                collision.GetComponent<EnemyStats>().TriggerSLowed(0, 2f);
                Destroy(this.gameObject);
            }
            else if (Traptype == "RakeTrap")
            {
                stats.TakeDamage(transform.position, 5);
                collision.GetComponent<EnemyStats>().TriggerSLowed(0, 2f);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Traptype == "SpikeTrap" && collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyStats>().TakeDamage(transform.position, 1f);
        }
    }
}