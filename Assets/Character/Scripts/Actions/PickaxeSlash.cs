using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class PickaxeSlash : MonoBehaviour
{
    [Header("Mouse Tracker")]
    public Vector2 PointerPosition { get; set; }
    private Vector2 attackDirection;
    public Transform weaponPosition;

    [Header("Attack Details")]
    public int hitcombo = 1;
    public bool attacking;
    public float overshootAngle = 15f;
    public float settleTime = 0.25f;
    public float attackDelay = 0.2f;

    private bool isSwinging = false;
    private float lastAttackTime = 0f;

    [Header("Weapon")]
    public GameObject weapon;
    public GameObject slash;

    [Header("Sound")]
    private SoundManager soundManager;
    void Update()
    {
        Spinner();
    }
    private void Start()
    {
        soundManager = GameObject.FindWithTag("Audio").GetComponent<SoundManager>();
        weaponPosition = GetComponentInChildren<Transform>();

    }

    public void Attack()
    {
        if (Time.time < lastAttackTime + attackDelay) return;
        if (isSwinging) return;
        
        float angle = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;

        Collider2D[] hits = Physics2D.OverlapBoxAll((Vector2)weaponPosition.position + attackDirection * 0.8f, new Vector2(0.8f, 4), angle);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                EnemyStats enemy = hit.GetComponent<EnemyStats>();

                if (enemy != null)
                    enemy.TakeDamage(transform.position, 10);
            }
        }
        //soundManager.PlaySFX(soundManager.SwingSound);
        StartCoroutine(Swing());
        lastAttackTime = Time.time;
    }

    IEnumerator Swing()
    {
        isSwinging = true;
        if (hitcombo == 1)
        {
            float baseAngle = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;

            float flipAngle = baseAngle - 180f;
            float overshoot = flipAngle - overshootAngle;
            SummonSlash();
            // INSTANT FLIP
            transform.rotation = Quaternion.Euler(0, 0, flipAngle);

            float timer = 0;

            while (timer < settleTime)
            {
                timer += Time.deltaTime;
                float t = timer / settleTime;

                float angle = Mathf.Lerp(baseAngle, overshoot, t);
                transform.rotation = Quaternion.Euler(0, 0, angle);

                yield return null;
            }

            timer = 0;

            while (timer < settleTime)
            {
                timer += Time.deltaTime;
                float t = timer / settleTime;

                float angle = Mathf.Lerp(overshoot, flipAngle, t);
                transform.rotation = Quaternion.Euler(0, 0, angle);

                yield return null;
            }
            hitcombo = 2;
        }
        else
        {
            float baseAngle = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;

            float flipAngle = baseAngle + 180f;
            float overshoot = flipAngle + overshootAngle;
            SummonSlash();
            // INSTANT FLIP
            transform.rotation = Quaternion.Euler(0, 0, flipAngle);

            float timer = 0;

            while (timer < settleTime)
            {
                timer += Time.deltaTime;
                float t = timer / settleTime;

                float angle = Mathf.Lerp(baseAngle, overshoot, t);
                transform.rotation = Quaternion.Euler(0, 0, angle);

                yield return null;
            }

            timer = 0;

            while (timer < settleTime)
            {
                timer += Time.deltaTime;
                float t = timer / settleTime;

                float angle = Mathf.Lerp(overshoot, flipAngle, t);
                transform.rotation = Quaternion.Euler(0, 0, angle);

                yield return null;
            }
            hitcombo = 1;
        }
            isSwinging = false;
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
        if (hitcombo == 2)
        {
            transform.right = -direction;
            scale.y *= -1;
        }
        transform.localScale = scale;
    }

    void SummonSlash()
    {
        Vector3 direction = (PointerPosition - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Instantiate(slash,transform.position + direction * 0.5f, Quaternion.Euler(0, 0, angle));
    }
}