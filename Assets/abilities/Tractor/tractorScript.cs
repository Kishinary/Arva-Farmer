using UnityEngine;




[CreateAssetMenu(menuName = "Ability/tractorScript")]
public class tractorScript : Ability
{

    public GameObject tractorPrefab;


    public GameObject indicatorPrefab;
    private GameObject currentIndicator;

    public float tractorDamage;

    public override void BeginAim(GameObject parent, Transform playerTransform, Rigidbody2D rb)
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        currentIndicator = GameObject.Instantiate(indicatorPrefab, playerTransform.position, Quaternion.identity);



        Vector3 direction = mouseWorldPos - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float snappedAngle = Mathf.Round(angle / 45f) * 45f;
        float snappedAngleRad = snappedAngle * Mathf.Deg2Rad;
        Vector2 snappedDirection = new Vector2(Mathf.Cos(snappedAngleRad), Mathf.Sin(snappedAngleRad));

        snappedDirection.x = Mathf.Round(snappedDirection.x);
        snappedDirection.y = Mathf.Round(snappedDirection.y);

        snappedDirection = snappedDirection.normalized;

        currentIndicator.transform.rotation = Quaternion.Euler(0f, 0f, snappedAngle);

    }

    public override void DuringAim(GameObject parent, Transform playerTransform, Rigidbody2D rb)
    {
        

    }
    public override void Activate(GameObject parent, Transform playerTransform, Rigidbody2D rb)
    {
        Destroy(currentIndicator);

        if (tractorPrefab != null)
        {

            GameObject tractorInstance = Instantiate(tractorPrefab, playerTransform.position, Quaternion.identity);
            tractorGoingScript damageAmount = tractorInstance.GetComponent<tractorGoingScript>();
            damageAmount.damageAmount = tractorDamage;


        }
    }

    public override void BeginCoolDown(GameObject parent)
    {

    }
}
