using UnityEngine;

public static class MathUtility
{

    public static Vector2 GetRandomPositionAround(Vector2 origin, float maxRadius, float minRadius = 0f)
    {
        if (minRadius <= 0f)
        {
            return origin + Random.insideUnitCircle * maxRadius;
        }
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(minRadius, maxRadius);
        return origin + (randomDirection * randomDistance);
    }



}
