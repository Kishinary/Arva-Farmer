using UnityEngine;
using UnityEngine.Pool;

public class Axe : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //flying Boomerang staight forward, no spread
    public enum AxeState { Outward, Returning, Caught }

    [Header("Movement Settings")]
    [SerializeField] private float throwSpeed = 15f;
    [SerializeField] private float returnAcceleration = 20f;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private float rotationSpeed = 700f;

    [Header("Detection")]
    [SerializeField] private float catchRange = 0.5f;
    [SerializeField] private LayerMask obstacleLayer;



    [Header("Damage Settings")]
    [SerializeField] private float damageAmount = 10f;
    private GameObject enemy;


   

   


    private AxeState currentState;
    private float currentSpeed;
    private Vector3 startPosition;
    private Vector2 direction;

    [SerializeField] private GameObject player;
    private Vector2 currentPosition;//player position at the moment of throwing, used for return direction calculation





    void Awake()
    {
        currentPosition = player.transform.position;
        
        SetupThrow();


        


    }


    void SetupThrow()
    {
        startPosition = transform.position;
        currentSpeed = throwSpeed;
        currentState = AxeState.Outward;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        direction = (mouseWorldPos - transform.position).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case AxeState.Outward:
                HandleOutwardMovement();
                break;
            case AxeState.Returning:
                HandleReturnMovement();
                break;
        }

        // Luôn xoay rìu khi đang bay
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
    private void HandleOutwardMovement()
    {
        transform.position += (Vector3)direction * currentSpeed * Time.deltaTime;
        currentSpeed -= throwSpeed * Time.deltaTime;
        if (Vector2.Distance(startPosition, transform.position) >= maxDistance || currentSpeed <= 0)
        {
            currentState = AxeState.Returning;
        }
    }
    private void HandleReturnMovement()
    {
        
        Vector2 returnDir = (player.transform.position - transform.position).normalized;
        currentSpeed += returnAcceleration * Time.deltaTime;
        transform.position += (Vector3)returnDir * currentSpeed * Time.deltaTime;
        if (Vector2.Distance(transform.position, player.transform.position) < catchRange)
        {
            CatchAxe();
        }

    }
    private void CatchAxe()
    {
        currentState = AxeState.Caught;
        
        Destroy(gameObject);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

        enemy = collision.gameObject;

        if (enemy.CompareTag("Enemy"))
        {
            enemy.GetComponent<EnemyStats>().TakeDamage(Vector2.zero, damageAmount);
        }


    }
   



    //handle spawn effect

}
