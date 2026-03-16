using Unity.VisualScripting;
using UnityEngine;

public class stunIndicator : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private GameObject enemySmallerStunIndicator;
    private enemyBeingStunIndicator enemyBeingStunIndicatorScript;

    public float radiusDistances;



    void Start()
    {
        transform.localScale = new Vector3(radiusDistances * transform.localScale.x, radiusDistances * transform.localScale.y, 1);
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position;
    }

    
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            enemyBeingStunIndicator existingStun = collision.GetComponentInChildren<enemyBeingStunIndicator>();

            if (existingStun == null)
            {
                GameObject newStunIndicator = Instantiate(
                    enemySmallerStunIndicator,
                    collision.transform.position,
                    Quaternion.identity,
                    collision.transform 
                );

               
                enemyBeingStunIndicator script = newStunIndicator.GetComponent<enemyBeingStunIndicator>();
                script.setEnemy(collision.gameObject);
                script.setCurrentStunIndicator(gameObject, radiusDistances);
            }
        }
    }

}
