using UnityEngine;
using System.Collections;

public class ItemDropAnimation : MonoBehaviour
{
    public GameObject yellowOrbPrefab;
    public GameObject finalItemPrefab;
    public float arcHeight = 3f;
    public float duration = 0.8f;

    public void LaunchItem(Vector3 startPos, Vector3 endPos)
    {
        StartCoroutine(AnimateDrop(startPos, endPos));
    }

    private IEnumerator AnimateDrop(Vector3 start, Vector3 end)
    {
        // 1. Instantiate the Orb
        GameObject orb = Instantiate(yellowOrbPrefab, start, Quaternion.identity);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / duration;

            Vector3 currentPos = Vector3.Lerp(start, end, percent);

            float yOffset = arcHeight * (4 * percent * (1 - percent));
            currentPos.y += yOffset;

            orb.transform.position = currentPos;
            yield return null;
        }

        Destroy(orb);
        GameObject item = Instantiate(finalItemPrefab, end, Quaternion.identity);

        item.transform.localScale = Vector3.zero;
        float popTime = 0.2f;
        float popElapsed = 0f;
        while (popElapsed < popTime)
        {
            popElapsed += Time.deltaTime;
            item.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, popElapsed / popTime);
            yield return null;
        }
        Destroy(this.gameObject);
    }
}