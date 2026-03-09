using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;




[RequireComponent(typeof(AudioSource))]
public class GameController : MonoBehaviour
{
    [Header("Effects")]
    public CameraShaker cameraShaker;

    [Header("References")]
    public SongAnalyzer analyzer;
    private AudioSource audioSource;
    public AudioClip songToAnalyze;

    [Header("Beat Tracking")]
    private List<SpectralEvent> spectralData;
    private int lastProcessedIndex = -1;
    private int currentIndex = 0;
    private bool isPlaying = false;


    [Header("Debug Visualizer")]
    public bool showVisualizer = true; // Bật/tắt thanh EQ trên màn hình
    public float visualizerMultiplier = 100f; // Khuếch đại cột sóng cho dễ nhìn
    private FrequencyBands displayBands; // Biến lưu trữ để tạo hiệu ứng thanh EQ nảy mượt mà


    //Effect
    public GameObject bassParticlePrefab;
    public float particleScaleMultiplier = 2f;


    private async void Start()
    {
        audioSource = GetComponent<AudioSource>();



        if (songToAnalyze != null && analyzer != null)
        {
            Debug.Log("[GameController] Bắt đầu quá trình phân tích bài hát...");

            // Đợi SongAnalyzer phân tích xong
            await analyzer.AnalyzeAudioAsync(songToAnalyze);


            // Lấy danh sách beat đã phân tích lưu vào biến cục bộ
            spectralData = analyzer.spectralTimeline;

            // 3. Chuẩn bị phát nhạc
            audioSource.clip = songToAnalyze;
            audioSource.Play();
            isPlaying = true;

            Debug.Log($"[GameController] Nhạc lên! Tổng số nhịp tìm được: {spectralData.Count}");
            // Bắt đầu phát nhạc và sinh quái vật tại đây...
        }
        else
        {
            Debug.LogError("[GameController] Thiếu AudioClip hoặc SongAnalyzer!");
        }
    }

    private void Update()
    {
        if (!isPlaying || spectralData == null || currentIndex >= spectralData.Count) return;

        float currentAudioTime = audioSource.time;
        int sampleRate = audioSource.clip.frequency;
        int windowSize = 1024; // Phải khớp với N bên SongAnalyzer

        // 1. TÍNH TOÁN INDEX HIỆN TẠI (O(1) Lookup)
        int currentFrameIndex = Mathf.FloorToInt((currentAudioTime * sampleRate) / windowSize);

        // Đảm bảo không vượt quá mảng
        if (currentFrameIndex >= spectralData.Count) return;

        // 2. CẬP NHẬT VISUALIZER LIÊN TỤC (UI)
        FrequencyBands currentBands = spectralData[currentFrameIndex].Bands;
        float lerpSpeed = Time.deltaTime * 15f; // Tăng tốc độ mượt lên một chút

        // Lerp TĂNG VÀ GIẢM mượt mà dựa trên dữ liệu thực tế từng frame
        displayBands.Kick = Mathf.Lerp(displayBands.Kick, currentBands.Kick, lerpSpeed);
        displayBands.Bass = Mathf.Lerp(displayBands.Bass, currentBands.Bass, lerpSpeed);
        displayBands.LowMid = Mathf.Lerp(displayBands.LowMid, currentBands.LowMid, lerpSpeed);
        displayBands.HighMid = Mathf.Lerp(displayBands.HighMid, currentBands.HighMid, lerpSpeed);
        displayBands.Treble = Mathf.Lerp(displayBands.Treble, currentBands.Treble, lerpSpeed);
        
        
        for (int i = lastProcessedIndex + 1; i <= currentFrameIndex; i++)
        {
            if (spectralData[i].IsBeat)
            {
                OnBeatHit(spectralData[i]);
                
            }
        }

        lastProcessedIndex = currentFrameIndex;




    }
    private void OnBeatHit(SpectralEvent spectralInfo)
    {
        if (spectralInfo.Bands.Kick > 0.20f)
        {


            GameObject particle = Instantiate(bassParticlePrefab, Vector3.zero, Quaternion.identity);
            float dynamicScale = spectralInfo.Bands.Kick * particleScaleMultiplier;

            particle.transform.localScale = new Vector3(dynamicScale, dynamicScale, dynamicScale);


        }

    }
    private void OnGUI()
    {
        if (!showVisualizer || !isPlaying) return;

        // Định dạng cột
        float barWidth = 40f;
        float spacing = 10f;
        float startX = 20f;
        float screenBottom = Screen.height - 20f;

        // Hàm cục bộ (Local function) để vẽ từng cột cho gọn code
        void DrawBar(int index, string label, float value, Color color)
        {
            float barHeight = value * visualizerMultiplier;
            // Giới hạn chiều cao cột không vọt ra khỏi màn hình
            barHeight = Mathf.Clamp(barHeight, 2f, Screen.height / 2f);

            Rect rect = new Rect(startX + index * (barWidth + spacing), screenBottom - barHeight, barWidth, barHeight);

            // Vẽ cột màu
            GUI.color = color;
            GUI.DrawTexture(rect, Texture2D.whiteTexture);

            // Vẽ nhãn (Label) ở dưới cột
            GUI.color = Color.white;
            GUI.Label(new Rect(rect.x, screenBottom, barWidth + 20, 20), label);
        }

        // Vẽ 5 dải tần với 5 màu khác nhau để dễ phân biệt
        DrawBar(0, "KICK", displayBands.Kick, Color.red);
        DrawBar(1, "BASS", displayBands.Bass, new Color(1f, 0.5f, 0f)); // Cam
        DrawBar(2, "L-MID", displayBands.LowMid, Color.yellow);
        DrawBar(3, "H-MID", displayBands.HighMid, Color.green);
        DrawBar(4, "TREBLE", displayBands.Treble, Color.cyan);
    }



}
