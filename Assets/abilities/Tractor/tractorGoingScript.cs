using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class tractorGoingScript : MonoBehaviour
{
    [HideInInspector]
    public GameObject player;
    public float speed = 1f;

    public float lifeTime = 1f;

    public Vector3 direction= Vector3.forward;

    private Vector2 snappedDirection;

    private PlayerMovement playerMovement;

    public float slowTime = 0.3f;
    public float makePlayerSlow = 0.1f;

    [HideInInspector]
    public Animator animator;

    [HideInInspector]
    public float damageAmount = 1f;

    [SerializeField] private LayerMask obstacleLayer;

    void Start()
    {
        
        Destroy(gameObject, lifeTime);
        
        player = GameObject.FindWithTag("Player");
        playerMovement = player.GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();


        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        

        direction = mouseWorldPos - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float snappedAngle = Mathf.Round(angle / 45f) * 45f;
        float snappedAngleRad = snappedAngle * Mathf.Deg2Rad;
        snappedDirection = new Vector2(Mathf.Cos(snappedAngleRad), Mathf.Sin(snappedAngleRad));

        snappedDirection.x = Mathf.Round(snappedDirection.x);
        snappedDirection.y = Mathf.Round(snappedDirection.y);

       

    

        playerMovement.ApplySlow(makePlayerSlow, slowTime);


        

        animator.SetFloat("positionX", snappedDirection.x);
        animator.SetFloat("positionY", snappedDirection.y);
    }



    // Update is called once per frame
    void Update()
    {
        
        transform.position += (Vector3)snappedDirection * speed * Time.deltaTime;
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject enemy = collision.gameObject;

       
        if (enemy.CompareTag("Enemy"))
        {
            enemy.GetComponent<EnemyStats>().TakeDamage(Vector2.zero, damageAmount);
            
        }
        if (collision.name == "Wall")
        {
            
            Destroy(gameObject);
        }

    }




}
