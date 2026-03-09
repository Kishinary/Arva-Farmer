using UnityEngine;
using TMPro;

public class DamagePopUp : MonoBehaviour
{
    private TextMeshPro textMesh;
    
    [Header("Cài đặt Di chuyển")]
    [SerializeField] private float moveYSpeed = 1.5f; // Tốc độ bay lên

    [Header("Cài đặt Màu sắc & Thời gian")]
    [SerializeField] private float colorChangeDuration = 0.5f; // Thời gian chuyển từ Trắng sang Đỏ
    [SerializeField] private float fadeOutDuration = 0.8f;     // Thời gian mờ đi sau khi đã đỏ

    private Color startColor = Color.white; // Màu lúc mới hiện (Trắng)
    private Color targetColor = Color.red;  // Màu sẽ chuyển thành (Đỏ)
    private Color currentColor;

    private float timer;
    private int phase = 0; // 0: Đang đổi màu, 1: Đang mờ đi

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    public void Setup(int damageAmount)
    {
        textMesh.SetText(damageAmount.ToString());
        
        // Khởi tạo trạng thái ban đầu là màu trắng rõ nét
        currentColor = startColor;
        textMesh.color = currentColor;
        
        timer = 0f;
        phase = 0; // Bắt đầu ở giai đoạn 0 (đổi màu)
    }

    private void Update()
    {
        // 1. Chữ luôn bay từ từ lên trên
        transform.position += new Vector3(0, moveYSpeed) * Time.deltaTime;

        // 2. Giai đoạn 0: Chuyển màu từ Trắng sang Đỏ
        if (phase == 0)
        {
            timer += Time.deltaTime;
            float colorFraction = timer / colorChangeDuration; // Tỷ lệ phần trăm thời gian đã trôi qua

            // Hàm Lerp giúp chuyển màu mượt mà
            currentColor = Color.Lerp(startColor, targetColor, colorFraction);
            textMesh.color = currentColor;

            // Nếu đã chuyển màu xong -> Chuyển sang giai đoạn mờ đi
            if (timer >= colorChangeDuration)
            {
                phase = 1;
                timer = 0f; // Reset timer cho giai đoạn sau
            }
        }
        // 3. Giai đoạn 1: Giữ màu Đỏ và làm mờ dần (Fade out)
        else if (phase == 1)
        {
            timer += Time.deltaTime;
            float fadeFraction = timer / fadeOutDuration;

            // Giảm dần giá trị Alpha (độ trong suốt) từ 1 về 0
            currentColor.a = Mathf.Lerp(1f, 0f, fadeFraction);
            textMesh.color = currentColor;

            // Xóa object khi đã mờ hoàn toàn
            if (currentColor.a <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }
}