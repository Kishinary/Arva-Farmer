using UnityEngine;

public class DirtDamage : MonoBehaviour
{
    public float SpecialAttackCooldown = 15f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyStats>().TakeDamage(transform.position, SpecialAttackCooldown);
            Debug.Log("Hitted");
        }
    }
}
