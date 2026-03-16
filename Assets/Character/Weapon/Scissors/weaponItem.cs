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



    private inventoryScript inventoryUI;
    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerMovement = player.GetComponent<PlayerMovement>();
        inventoryUI = FindFirstObjectByType<inventoryScript>();

        _startPos = transform.position;

    }

    private void Update()
    {
        float newY = _startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(_startPos.x, newY, _startPos.z);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Transform weaponParent = collision.transform.Find("WeaponParent");
            if(weaponParent == null)
            {
                Debug.Log("thang dau buoi re rach, weapon parent dau dit me may");
            }
            foreach (Transform child in weaponParent)
            {
                child.SetParent(null);
                Destroy(child.gameObject);
            }
            GameObject newWeapon = Instantiate(weaponPrefabToEquip, weaponParent);
            
            newWeapon.transform.localRotation = Quaternion.identity;

            
            IWeapon ieWeapon = newWeapon.GetComponent<IWeapon>();
            playerMovement.SetWeapon(ieWeapon);

            if (ieWeapon != null)
            {
                inventoryUI.changeWeaponSprite();
            }
            else { Debug.Log("wtf"); }
            Destroy(gameObject);


        }
    }



}
