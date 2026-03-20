using UnityEngine;
using System.Collections;

public class PlayerActionForcer : MonoBehaviour
{
    private PlayerMovement player;
    private Rigidbody2D rb;
    private Animator anim;

    private void Awake()
    {
        player = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    public void ForceMove(Vector2 direction, float duration)
    {
        StartCoroutine(ForceMoveRoutine(direction.normalized, duration));
    }

    private IEnumerator ForceMoveRoutine(Vector2 dir, float duration)
    {
        player.DisablePlayerInput();

        // 2. Set the walking animation to the forced direction
        anim.SetBool("IsMoving", true);
        anim.SetFloat("InputX", dir.x);
        anim.SetFloat("InputY", dir.y);

        float elapsed = 0;
        while (elapsed < duration)
        {
            rb.linearVelocity = dir * player.movespeed;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 4. Reset and Give control back
        rb.linearVelocity = Vector2.zero;
        anim.SetBool("IsMoving", false);
        player.EnablePlayerInput();
    }
}