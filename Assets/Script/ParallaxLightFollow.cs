using UnityEngine;

// Yêu cầu phải có SpriteRenderer để tự động đọc kích thước ảnh
[RequireComponent(typeof(SpriteRenderer))]
public class ParallaxLightFollow : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("Kéo thả Player của bạn vào đây")]
    public Transform player;

    [Header("Parallax Settings")]
    [Tooltip("Tỉ lệ di chuyển so với Player. 0 = đứng im, 1 = bằng Player")]
    [Range(0f, 1f)]
    public float followSpeedRatio = 0.5f;

    [Header("Auto Drift (Tùy chọn)")]
    [Tooltip("Tốc độ trôi tự động của bóng/mây khi Player đứng im")]
    public Vector2 autoDriftSpeed = new Vector2(0.2f, 0f);

    [Header("Looping Settings")]
    [Tooltip("Bật để mây tự động lặp lại khi Player đi quá xa")]
    public bool infiniteLoop = true;

    private Vector3 lastPlayerPosition;
    private float spriteWidth;
    private float spriteHeight;

    void Start()
    {
        if (player != null)
        {
            lastPlayerPosition = player.position;
        }
        else
        {
            Debug.LogWarning("Bạn chưa gán Player cho script ParallaxLightFollow!");
        }

        // Tự động lấy kích thước Chiều Rộng và Chiều Cao của bức ảnh mây
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite != null)
        {
            spriteWidth = sr.bounds.size.x;
            spriteHeight = sr.bounds.size.y;
        }
    }

    void LateUpdate()
    {
        if (player == null) return;

        // 1. Tính quãng đường và di chuyển Parallax + Drift (Giống code cũ)
        Vector3 playerDeltaMove = player.position - lastPlayerPosition;
        transform.position += playerDeltaMove * followSpeedRatio;
        transform.position += (Vector3)(autoDriftSpeed * Time.deltaTime);
        lastPlayerPosition = player.position;

        // 2. Xử lý Lặp vô tận (Infinite Looping)
        if (infiniteLoop && spriteWidth > 0 && spriteHeight > 0)
        {
            // Tính khoảng cách hiện tại giữa Player và Bóng mây
            float distanceX = player.position.x - transform.position.x;
            float distanceY = player.position.y - transform.position.y;

            // Nếu khoảng cách trục X lớn hơn chiều rộng bức ảnh -> Bê nó lên trước
            if (Mathf.Abs(distanceX) >= spriteWidth)
            {
                // Dịch chuyển tức thời một đoạn bằng đúng 1 lần chiều rộng để không bị giật khấc
                transform.position += new Vector3(Mathf.Sign(distanceX) * spriteWidth, 0, 0);
            }

            // Tương tự cho trục Y (đi lên/đi xuống)
            if (Mathf.Abs(distanceY) >= spriteHeight)
            {
                transform.position += new Vector3(0, Mathf.Sign(distanceY) * spriteHeight, 0);
            }
        }
    }
}