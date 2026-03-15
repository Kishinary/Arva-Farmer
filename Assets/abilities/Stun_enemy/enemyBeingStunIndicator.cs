using UnityEngine;

public class enemyBeingStunIndicator : MonoBehaviour
{
    [HideInInspector]
    public GameObject enemy;

    private GameObject CurrentStunIndicator;
    private Collider2D stunAreaCollider; 

    public void setEnemy(GameObject enemy)
    {
        this.enemy = enemy;
    }

    public void setCurrentStunIndicator(GameObject CurrentStunIndicator, float radiusDistances)
    {
        this.CurrentStunIndicator = CurrentStunIndicator;

        stunAreaCollider = CurrentStunIndicator.GetComponent<Collider2D>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (enemy != null && stunAreaCollider != null)
        {
          
            Vector2 enemyPos = enemy.transform.position;

            if (stunAreaCollider.OverlapPoint(enemyPos))
            {
                
                transform.position = enemy.transform.position;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            
            Destroy(gameObject);
        }
    }
}