using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float Health;
    public float MaxHealth = 100f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Health = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(float Damage)
    {
        Health -= Damage;
        if (Health <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
