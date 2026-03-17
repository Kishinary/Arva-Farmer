using UnityEngine;

public class SilkTether : MonoBehaviour
{
    public LineRenderer line;
    private Transform player;
    private Transform weapon;
    private float sagAmount;
    void Start()
    {
        line = GetComponent<LineRenderer>();
        player = GameObject.FindWithTag("Player").transform;
        weapon = this.transform;
    }
    void Update()
    {
        if (line.enabled)
        {
            sagAmount = Random.Range(-10, 10) * 0.1f;
            line.positionCount = 3;
            line.SetPosition(0, player.position); // Start at player

            Vector3 midPoint = Vector3.Lerp(player.position, weapon.position, 0.5f);
            midPoint.y -= sagAmount;

            line.SetPosition(1, midPoint); // The "Loop" point
            line.SetPosition(2, weapon.position); // End at weapon
        }
    }
}