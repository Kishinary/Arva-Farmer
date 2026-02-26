using UnityEngine;

public class WeaponParent : MonoBehaviour
{
    [Header("Mouse Tracker")]
    public Vector2 PointerPosition { get; set; }
    private Vector2 attackDirection;
    public Transform weaponPosition;

    public bool isSwinging = false;
    void Start()
    {
        weaponPosition = GetComponentInChildren<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Spinner();
        PointerPosition = gameObject.GetComponentInParent<PlayerMovement>().GetPointerInput();
    }
    void Spinner()
    {
        if (isSwinging) return;

        Vector2 direction = (PointerPosition - (Vector2)transform.position).normalized;

        transform.right = direction;

        Vector2 scale = transform.localScale;

        if (direction.x < 0)
        {
            scale.y = -1;
        }
        else if (direction.x > 0)
        {
            scale.y = 1;
        }
        transform.localScale = scale;
    }
}
