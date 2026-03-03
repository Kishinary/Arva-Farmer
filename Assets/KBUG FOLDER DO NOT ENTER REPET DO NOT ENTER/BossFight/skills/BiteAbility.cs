using JetBrains.Annotations;
using UnityEngine;
using System.Collections;
using System;

public class BiteAbility : MonoBehaviour
{
    public AnimationCurve dashCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public IEnumerator ExecuteBite(Vector2 target, float duration, Action onComplete)
    {
    

    Vector2 startPos = transform.position;
    float elapsed = 0f;
        while (elapsed < duration)
        { 
            float t = elapsed / duration;
            float curveT = dashCurve.Evaluate(t);
            transform.position = Vector2.Lerp(startPos, target, curveT);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = target;
        onComplete?.Invoke();



    }
}
