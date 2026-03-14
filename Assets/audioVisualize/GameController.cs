using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;







public struct BeatPayload
{
    public bool kick;
    public bool bass;
    public bool lowMid;
    public bool highMid;
    public bool treble;
}

[RequireComponent(typeof(AudioSource))]
public class GameController : MonoBehaviour
{
    //looping
    public event Action OnSongLooped;

    private float lastAudioTime = 0f;

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

    [Header("Dynamic Threshold Tuning")]
    [Range(0.5f, 5.0f)] public float averageWindowSeconds = 1.5f;
    [Range(1.1f, 3f)] public float sensitivityMultiplier = 1.5f;

    //Dùng một mảng duy nhất lưu trữ trung bình của cả 5 dải tần
    private FrequencyBands[] precalculatedAverages;

    //Khai báo Event để các script khác (Boss, UI, Môi trường) đăng ký lắng nghe
    public event Action<BeatPayload> OnBeatDetected;

    // Thêm các Getter để Boss có thể truy cập dữ liệu an toàn
    public List<SpectralEvent> GetSpectralData() => spectralData;
    public FrequencyBands[] GetAverages() => precalculatedAverages;
    public AudioSource GetAudioSource() => audioSource;

    //Event báo hiệu dữ liệu đã sẵn sàng
    public event Action OnDataReady;

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

            // Tính toán trước Ngưỡng Động
            PrecalculateDynamicThresholds();

            //Báo cho Boss biết để quét Combo TRƯỚC khi nhạc chạy
            OnDataReady?.Invoke();

            Debug.Log("[GameController] Complete");

            // 3. Chuẩn bị phát nhạc
            audioSource.clip = songToAnalyze;
            audioSource.loop = true;//loop
            audioSource.Play();
            isPlaying = true;

            
            
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
        //Looping:
        if (currentAudioTime < lastAudioTime)
        {

            lastProcessedIndex = -1;
            OnSongLooped?.Invoke();
        }
        lastAudioTime = currentAudioTime;

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
                OnBeatHit(spectralData[i], i);
                
            }
        }

        lastProcessedIndex = currentFrameIndex;




    }

    private void PrecalculateDynamicThresholds()
    {
        int totalFrames = spectralData.Count;
        precalculatedAverages = new FrequencyBands[totalFrames];
        int sampleRate = audioSource.clip.frequency;
        int fftWindowSize = 1024;
        float audioFramesPerSecond = (float)sampleRate / fftWindowSize;

        int windowSize = Mathf.RoundToInt(averageWindowSeconds * audioFramesPerSecond);

        // Các biến lưu trữ tổng trượt (Sliding Sums)
        float sumKick = 0f, sumBass = 0f, sumLowMid = 0f, sumHighMid = 0f, sumTreble = 0f;

        for (int i = 0; i < totalFrames; i++)
        {
            // 1. CỘNG giá trị của frame MỚI vào tổng
            FrequencyBands currentBands = spectralData[i].Bands;
            sumKick += currentBands.Kick;
            sumBass += currentBands.Bass;
            sumLowMid += currentBands.LowMid;
            sumHighMid += currentBands.HighMid;
            sumTreble += currentBands.Treble;

            // 2. TRỪ giá trị của frame CŨ (đã trượt ra khỏi cửa sổ) khỏi tổng
            int outOfWindowIndex = i - windowSize - 1;
            if (outOfWindowIndex >= 0)
            {
                FrequencyBands oldBands = spectralData[outOfWindowIndex].Bands;
                sumKick -= oldBands.Kick;
                sumBass -= oldBands.Bass;
                sumLowMid -= oldBands.LowMid;
                sumHighMid -= oldBands.HighMid;
                sumTreble -= oldBands.Treble;
            }
            // 3. Tính toán số lượng phần tử thực tế đang có trong cửa sổ
            int count = Mathf.Min(i + 1, windowSize + 1);
            // 4. Lưu vào mảng kết quả
            // (Giả định FrequencyBands là một struct. Nếu là class, bạn cần dùng 'new FrequencyBands(...)')
            precalculatedAverages[i] = new FrequencyBands
            {
                Kick = sumKick / count,
                Bass = sumBass / count,
                LowMid = sumLowMid / count,
                HighMid = sumHighMid / count,
                Treble = sumTreble / count
            };
        }

    }
    private void OnBeatHit(SpectralEvent spectralInfo, int frameIndex)
    {
        FrequencyBands current = spectralInfo.Bands;
        FrequencyBands average = precalculatedAverages[frameIndex];
        float noiseGate = 0.02f;
        BeatPayload payload = new BeatPayload
        {
            kick = current.Kick > (average.Kick * sensitivityMultiplier) && current.Kick > noiseGate,
            bass = current.Bass > (average.Bass * sensitivityMultiplier) && current.Bass > noiseGate,
            lowMid = current.LowMid > (average.LowMid * sensitivityMultiplier) && current.LowMid > noiseGate,
            highMid = current.HighMid > (average.HighMid * sensitivityMultiplier) && current.HighMid > noiseGate,
            treble = current.Treble > (average.Treble * sensitivityMultiplier) && current.Treble > noiseGate
        };
        if (payload.kick || payload.bass || payload.lowMid || payload.highMid || payload.treble)
        {
            OnBeatDetected?.Invoke(payload);
        }




    }
    /*private void OnGUI()
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
    }*/



}
