using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class stunFake : MonoBehaviour
{
    private GameObject player;

    [HideInInspector]
    public float radiusDistances;


    private EnemyStats enemyStats;

    public float slowSpeed;
    public float timeSlow;

    public GameObject stunEffect;

    void Start()
    {

        Destroy(gameObject, 0.5f);

        player = GameObject.FindWithTag("Player");
        transform.position = player.transform.position;
        transform.localScale = new Vector3(radiusDistances * transform.localScale.x, radiusDistances * transform.localScale.y, 1);
        
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {


            enemyStats = collision.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                
                enemyStats.TriggerSLowed(slowSpeed, timeSlow);
                Instantiate(stunEffect, collision.transform.position, Quaternion.identity);

            }
        }
    }
    
}
