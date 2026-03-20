using UnityEngine;

public class donutCollider : MonoBehaviour
{
    public float outerRadius = 1f;
    public float thickness = 0.1f;
    public int edgeCount = 32;

    void Start()
    {
        GenerateDonut();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        
    }

    public void GenerateDonut()
    {
        PolygonCollider2D polyCollider = GetComponent<PolygonCollider2D>();

        polyCollider.pathCount = 2;

        float innerRadius = outerRadius - thickness;

        if (innerRadius <= 0)
        {
            
            return;
        }

        Vector2[] outerPoints = new Vector2[edgeCount];
        Vector2[] innerPoints = new Vector2[edgeCount];

        for (int i = 0; i < edgeCount; i++)
        {
            float angle = (i * Mathf.PI * 2f) / edgeCount;
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            outerPoints[i] = new Vector2(cos * outerRadius, sin * outerRadius);

            innerPoints[i] = new Vector2(cos * innerRadius, sin * innerRadius);
        }

         
        polyCollider.SetPath(0, outerPoints);
        polyCollider.SetPath(1, innerPoints);
    }
}
