using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class FollowingMovement : MonoBehaviour
{
    public Transform target; // The target to follow
    public float moveSpeed = 5f; // Speed of the enemy movement
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform; // Find the player by tag

    }

    // Update is called once per frame
    void Update()
    {
        if (target.position != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance <= 20)
            {
                Vector3 direction = (target.position - transform.position).normalized;
                float speed = moveSpeed; // Adjust speed as needed

                transform.position += direction * speed * Time.deltaTime;
                if (direction.x > 0)
                {
                    // Facing right
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
                else if (direction.x < 0)
                {
                    // Facing left
                    transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }

            }
    

        }
    }
    
}
