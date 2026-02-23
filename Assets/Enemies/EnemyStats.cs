using System.Collections;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public float EnemyHealth;
    public float hpMultiplier;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(float Damage)
    {
        EnemyHealth -= Damage;
        if (EnemyHealth <= 0)
        {
            Destroy(this.gameObject);
        }
    }

}
