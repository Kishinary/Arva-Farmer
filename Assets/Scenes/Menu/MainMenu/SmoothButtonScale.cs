using UnityEngine;
using UnityEngine.EventSystems;

public class SmoothButtonScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector3 hoverScale = new Vector3(1.2f, 1.2f, 1f);
    public float smoothTime = 0.15f; // Adjust for "snappiness"

    private Vector3 originalScale;
    private Vector3 targetScale;

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    void Update()
    {
        // Smoothly interpolates the scale every frame
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime / smoothTime);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;
    }
}