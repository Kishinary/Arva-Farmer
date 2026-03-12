using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Hệ số nhân để phóng đại cường độ rung từ âm thanh")]
    public float shakeMultiplier = 1.5f;
    [Tooltip("Tốc độ dập tắt lực rung (Càng lớn dừng càng nhanh)")]
    public float dampingSpeed = 10f;

    // Cache (Lưu trữ) các giá trị
    private Transform camTransform;
    private Vector3 originalPosition;

    // Biến trạng thái
    private float currentShakeIntensity = 0f;

    private void Awake()
    {
        camTransform = transform;
        originalPosition = camTransform.localPosition;
    }

    public void Shake(float rawIntensity)
    {
        currentShakeIntensity = rawIntensity * shakeMultiplier;
    }

    private void Update()
    {
        if (currentShakeIntensity > 0.01f)
        {
            Vector3 randomOffset = Random.insideUnitSphere * currentShakeIntensity;

            randomOffset.z = 0f;

            camTransform.localPosition = originalPosition + randomOffset;

            currentShakeIntensity = Mathf.Lerp(currentShakeIntensity, 0f, Time.deltaTime * dampingSpeed);
        }
        else
        {
            currentShakeIntensity = 0f;
            camTransform.localPosition = originalPosition;
        }
    }
}