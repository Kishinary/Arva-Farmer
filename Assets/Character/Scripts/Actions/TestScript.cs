using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour
{
    public Transform player;
    public Vector2 pointerPosition;

    public float overshoot = 20f;
    public float swingTime = 0.075f;
    public float attackCooldown = 0.2f;

    private bool attacking = false;
    private float lastAttack;
    private bool backSide = false;

    void Update()
    {
        Aim();
    }

    void Aim()
    {
        if (attacking) return;

        Vector2 dir = (pointerPosition - (Vector2)player.position).normalized;

        if (backSide)
            dir = -dir;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void Attack()
    {
        if (Time.time < lastAttack + attackCooldown) return;
        if (attacking) return;

        StartCoroutine(Swing());
        lastAttack = Time.time;
    }

    IEnumerator Swing()
    {
        attacking = true;

        Vector2 dir = (pointerPosition - (Vector2)player.position).normalized;
        float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        backSide = !backSide;

        float target = backSide ? baseAngle + 180f : baseAngle;

        transform.rotation = Quaternion.Euler(0, 0, target);

        float over = target + overshoot;

        float t = 0;

        // overshoot
        while (t < swingTime)
        {
            t += Time.deltaTime;

            dir = (pointerPosition - (Vector2)player.position).normalized;
            baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            target = backSide ? baseAngle + 180f : baseAngle;
            over = target + overshoot;

            float angle = Mathf.Lerp(target, over, t / swingTime);

            transform.rotation = Quaternion.Euler(0, 0, angle);

            yield return null;
        }

        t = 0;

        // settle
        while (t < swingTime)
        {
            t += Time.deltaTime;

            dir = (pointerPosition - (Vector2)player.position).normalized;
            baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            target = backSide ? baseAngle + 180f : baseAngle;
            over = target + overshoot;

            float angle = Mathf.Lerp(over, target, t / swingTime);

            transform.rotation = Quaternion.Euler(0, 0, angle);

            yield return null;
        }

        attacking = false;
    }
}
