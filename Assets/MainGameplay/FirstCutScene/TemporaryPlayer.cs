using System.Collections;
using System.Threading;
using UnityEngine;

public class TemporaryPlayer : MonoBehaviour
{
    public Rigidbody2D rb;
    public Transform TargetPosition;
    public Animator animator;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        
    }
    public IEnumerator MoveUp()
    {
        animator.SetBool("Walk", true);

        while (Vector2.Distance(transform.position, TargetPosition.position) > 0.05f)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                TargetPosition.position,
                3f * Time.deltaTime
            );

            yield return null;
        }

        animator.SetBool("Walk", false);
    }
}
