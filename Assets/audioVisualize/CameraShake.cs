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

    /// <summary>
    /// Hàm này phơi bày ra ngoài (public) để GameController có thể gọi vào.
    /// </summary>
    public void Shake(float rawIntensity)
    {
        // Chuyển đổi cường độ âm thanh thành lực rung vật lý
        currentShakeIntensity = rawIntensity * shakeMultiplier;
    }

    private void Update()
    {
        // Chỉ tính toán rung nếu cường độ đủ lớn (Tối ưu CPU)
        if (currentShakeIntensity > 0.01f)
        {
            // Tạo vector ngẫu nhiên và nhân với lực rung hiện tại
            Vector3 randomOffset = Random.insideUnitSphere * currentShakeIntensity;

            // Trong game 2D Top-down, ta không muốn Camera bị rung thụt thò theo trục Z
            randomOffset.z = 0f;

            // Cập nhật vị trí
            camTransform.localPosition = originalPosition + randomOffset;

            // Dập tắt lực rung dần theo thời gian (Lerp)
            currentShakeIntensity = Mathf.Lerp(currentShakeIntensity, 0f, Time.deltaTime * dampingSpeed);
        }
        else
        {
            // Reset vị trí về chuẩn xác lúc ban đầu khi hết rung
            currentShakeIntensity = 0f;
            camTransform.localPosition = originalPosition;
        }
    }
}//UNUSED