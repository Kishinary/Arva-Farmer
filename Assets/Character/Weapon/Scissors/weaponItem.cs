using DG.Tweening;
using UnityEngine;

public class weaponItem : MonoBehaviour
{
    public GameObject weaponPrefabToEquip;


    [HideInInspector]
    public GameObject player;
    private PlayerMovement playerMovement;

    [SerializeField] private float floatSpeed = 3f;
    private float floatHeight = 0.15f;

    private Vector3 _startPos;

    private bool isPlayerInRange = false;
    private Collider2D playerColliderRef;

    private inventoryScript inventoryUI;

    public float dropRadius = 1.5f;
    public float jumpPower = 1.5f;
    public float dropDuration = 0.5f;
    private bool isDropping = false;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerMovement = player.GetComponent<PlayerMovement>();
        inventoryUI = FindFirstObjectByType<inventoryScript>();

        _startPos = transform.position;
        PerformDropAnimation();
    }

    private void Update()
    {
        if (!isDropping)
        {
            float newY = _startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
            transform.position = new Vector3(_startPos.x, newY, _startPos.z);
        }
        if (!isDropping && isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            PickUpAndEquip(playerColliderRef);
        }
    }
    private void PerformDropAnimation()
    {
        isDropping = true;
        Vector2 randomCircle = Random.insideUnitCircle * dropRadius;
        Vector3 targetPos = transform.position + new Vector3(randomCircle.x, randomCircle.y, 0f);
        transform.DOJump(targetPos, jumpPower, 1, dropDuration)
            .SetEase(Ease.OutQuad) 
            .OnComplete(() =>
            {
                isDropping = false;
                _startPos = transform.position; 
            });
    }
    private void PickUpAndEquip(Collider2D collision)
    {
        Transform weaponParent = collision.transform.Find("WeaponParent");

        if (weaponParent == null)
        {
            Debug.LogError("Cant Find transform WeaponParent");
            return;
        }

        foreach (Transform child in weaponParent)
        {
            weaponDropConfig dropConfig = child.GetComponent<weaponDropConfig>();

            Vector3 dropPosition = collision.transform.position;
            Instantiate(dropConfig.droppedItemPrefab, dropPosition, Quaternion.identity);

            child.SetParent(null);
            Destroy(child.gameObject);
        }

        GameObject newWeapon = Instantiate(weaponPrefabToEquip, weaponParent);

     
        newWeapon.transform.localRotation = Quaternion.identity;

        IWeapon ieWeapon = newWeapon.GetComponent<IWeapon>();

        if (ieWeapon != null)
        {
            playerMovement.SetWeapon(ieWeapon);

            if (inventoryUI != null)
            {
                inventoryUI.changeWeaponSprite();
            }
        }
        else
        {
            Debug.Log("No prefab weapon prefab detected");
         }

        Destroy(gameObject);
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
            playerColliderRef = collision; 
           

        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            playerColliderRef = null;

        }
    }



}
