using System.Collections;
using TMPro;
using UnityEngine;
public class rockDropScript : MonoBehaviour
{
    [HideInInspector]
    public Vector2 direction;

    public float lifeTime = 3f;
    public float speed = 5f;


    public Transform visualTransform;
    public float maxHeight = 3f;
    public float curveOffset = 2f;
    public float flightDuration = 1.5f;

    private Vector2 TargetPosition;

    private GameObject player;

    [Header("ParentPosition")]
    public GameObject Parent;

    [Header("TargetIndicator")]
    public GameObject indicator;
    private GameObject targetIndicator;

    public GameObject DamageArea;

    private void Start()
    {
        if(Parent == null)
        {
            Debug.Log("No Parent in rock script");
        }
        player = GameObject.FindWithTag("Player");


        Vector2 ParentPosition = Parent.transform.position;
        Vector2 playerPositions = player.transform.position;

        //Vector2 extraPosition = new Vector2(1f, 0f);
        Vector2 extraPosition = new Vector2();

        
        string rockName = gameObject.name;
        if(rockName == "rock1") extraPosition = new Vector2(1f, 0f);
        else if (rockName == "rock2") extraPosition = new Vector2(0f, 1f);
        else if (rockName == "rock3") extraPosition = new Vector2(-1f, 0f);
        else if (rockName == "rock4") extraPosition = new Vector2(0f, -1f);



        TargetPosition = playerPositions + extraPosition;

        
        targetIndicator = Instantiate(indicator, TargetPosition, Quaternion.identity);

        StartCoroutine(RockMovement(ParentPosition, TargetPosition));
    }
    private IEnumerator RockMovement(Vector2 p0, Vector2 p2) {

        Vector2 direction = p2 - p0;
        Vector2 midPoint = (p0 + p2) / 2f;

        Vector2 perpendicular = new Vector2(-direction.y, direction.x).normalized;

        Vector2 p1 = midPoint + perpendicular * curveOffset;

        float elapsed = 0f;

        while (elapsed < flightDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / flightDuration;

            float u = 1f - t;
            Vector2 currentGroundPos = (u * u * p0) + (2f * u * t * p1) + (t * t * p2);
            transform.position = currentGroundPos;

            if (visualTransform != null)
            {
                // sin(0) = 0, sin(pi/2) = 1, sin(pi) = 0
                float currentHeight = Mathf.Sin(t * Mathf.PI) * maxHeight;
                visualTransform.localPosition = new Vector3(0f, currentHeight, 0f);
            }
            yield return null;
        }

        transform.position = p2;
        if (visualTransform != null) visualTransform.localPosition = Vector3.zero;

        Explode();

    }
    private void Explode()
    {
        Instantiate(DamageArea, TargetPosition, Quaternion.identity);
        Destroy(targetIndicator);
        Destroy(gameObject);

        

        // Instantiate(explosionParticle, transform.position, Quaternion.identity);
        // Destroy(gameObject); (Hoặc đưa về Object Pool)
    }

}
