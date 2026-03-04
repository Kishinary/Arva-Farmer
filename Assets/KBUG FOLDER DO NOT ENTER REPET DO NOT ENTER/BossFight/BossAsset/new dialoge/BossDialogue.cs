using System.Collections;
using UnityEngine;
using TMPro;

public class BossDialogue : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI textComponent;

    [TextArea(3, 10)]
    public string[] lines;
    public float textSpeed;

    private int index;

    [Header("Camera & Flow")]
    public CameraZoom m_camera;
    public Camera cam;
    public GameObject NextDialogue;

    [Header("State Checkers")]
    private bool isChatting;
    private bool notSpawnyet = true;

    [Header("Sound Effect")]
    public AudioSource audioSource;
    public AudioClip typingSound;

    void Start()
    {
        m_camera = GetComponentInParent<CameraZoom>();
        // Tối ưu: Chỉ tìm kiếm 1 lần trong Start
        if (Camera.main != null) cam = Camera.main;

        textComponent = GetComponentInChildren<TextMeshProUGUI>();
        textComponent.text = string.Empty;
    }

    void Update()
    {
        // Gộp chung logic xử lý Input chuột trái vào một nơi để dễ quản lý
        if (Input.GetMouseButtonDown(0))
        {
            if (notSpawnyet)
            {
                // Click lần đầu tiên để bắt đầu hội thoại
                StartDialogue();
            }
            else
            {
                // Kiểm tra xem TMP đã hiển thị hết số lượng ký tự chưa
                if (textComponent.maxVisibleCharacters >= textComponent.textInfo.characterCount)
                {
                    // Đã gõ xong -> Chuyển câu tiếp theo
                    NextLine();
                }
                else
                {
                    // Đang gõ dở -> Bấm để Skip (Hiển thị toàn bộ chữ ngay lập tức)
                    SkipTyping();
                }
            }
        }
    }

    void StartDialogue()
    {
        notSpawnyet = false;
        isChatting = true;
        index = 0;

        //Player.DisablePlayerInput();
        //m_camera.ZoomIn();

        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        // 1. Gán toàn bộ văn bản và ép TMP bóc tách các thẻ Rich Text / Emoji
        textComponent.text = lines[index];
        textComponent.ForceMeshUpdate();

        // 2. Lấy tổng số ký tự thực tế
        int totalCharacters = textComponent.textInfo.characterCount;

        // 3. Giấu toàn bộ chữ đi
        textComponent.maxVisibleCharacters = 0;

        // 4. Bật âm thanh loop
        if (audioSource != null && typingSound != null)
        {
            audioSource.clip = typingSound;
            audioSource.loop = true;
            audioSource.Play();
        }

        // 5. Vòng lặp hiện dần từng ký tự
        for (int i = 0; i <= totalCharacters; i++)
        {
            textComponent.maxVisibleCharacters = i;
            yield return new WaitForSeconds(textSpeed);
        }

        // 6. Tắt âm thanh khi gõ xong
        StopAudio();
    }

    private void SkipTyping()
    {
        // Dừng Coroutine gõ chữ lại
        StopAllCoroutines();

        // Bắt buộc TẮT ÂM THANH để tránh bug kẹt tiếng
        StopAudio();

        // Ép số lượng ký tự hiển thị bằng với tổng số ký tự (Hiện full câu)
        textComponent.maxVisibleCharacters = textComponent.textInfo.characterCount;
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            StartCoroutine(TypeLine());
        }
        else
        {
            // Kết thúc hội thoại
            gameObject.SetActive(false);
            isChatting = false;

            //m_camera.ZoomOut();
            //Player.EnablePlayerInput();

            if (NextDialogue != null)
            {
                NextDialogue.SetActive(true);
            }
        }
    }

    private void StopAudio()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}