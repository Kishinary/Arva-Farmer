using UnityEngine;
using System.Collections;
public class blackShadow : MonoBehaviour
{
    private SpriteRenderer sr;
    public float activeTime = 0.5f;
    public Color trailColor = new Color(1f, 1f, 1f, 0.5f);
    public void SetupSnapshot(Sprite currentSprite, Vector3 pos, Quaternion rot, Vector3 scale)
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();

        sr.sprite = currentSprite;
        transform.position = pos;
        transform.rotation = rot;
        transform.localScale = scale;

        sr.color = trailColor;

        StartCoroutine(FadeOutRoutine());
    }

    private IEnumerator FadeOutRoutine()
    {
        float elapsedTime = 0f;
        Color startColor = sr.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f); // Alpha = 0

        while (elapsedTime < activeTime)
        {
            float t = elapsedTime / activeTime;
            sr.color = Color.Lerp(startColor, endColor, t);

            elapsedTime += Time.deltaTime;
            yield return null; 
        }

        Destroy(gameObject);
    }




}
