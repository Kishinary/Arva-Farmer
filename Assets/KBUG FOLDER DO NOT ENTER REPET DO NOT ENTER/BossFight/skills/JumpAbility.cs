using UnityEngine;
using System.Collections;
using System;


public class JumpAbility : MonoBehaviour
{
    
    public IEnumerator ExecuteJump(Transform visual, Vector2 target, float duration, float height, Action onComplete)
    {
        Vector2 startPos = transform.position;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.position = Vector2.Lerp(startPos, target, t);
            //di chuyen visual
            float h = Mathf.Sin(t * Mathf.PI) * height;
            visual.localPosition = new Vector3(0, h, 0);

            elapsed += Time.deltaTime;

            yield return null;


        }
        transform.position = target;
        visual.localPosition = Vector3.zero;
        if(onComplete != null)
        {
            onComplete.Invoke();
        }

    }



}
