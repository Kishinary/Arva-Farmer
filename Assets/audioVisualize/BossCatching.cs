using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public enum BandType { Kick = 0, Bass = 1, LowMid = 2, HighMid = 3, Treble = 4 }
public struct BossCombo
{
    public BandType bandType;    // Bổ sung: Cho biết Combo này thuộc dải tần nào
    public int beatCount;        // Tổng số nhịp (3 hoặc 5)
    public float[] beatTimes;    // Mảng thời gian của từng nhịp
    public int comboID;
}
public class BossCatching : MonoBehaviour
{

    public GameObject particlePrefab;

    [Header("References")]
    public GameController gameController;
    public GameObject particle;

    [Header("Combo Rules")]
    public float maxComboDelay = 0.4f;
    public float beatCooldown = 0.1f;

    // 5 Hàng đợi độc lập cho 5 dải tần
    private Queue<BossCombo>[] comboQueues = new Queue<BossCombo>[5];


    // 5 Biến theo dõi tiến độ đánh của từng dải tần trong lúc chạy nhạc
    private int[] currentBeatIndices = new int[5];


    public event Action<BossCombo, int> OnBossAttackTriggered;

    public float warningTime = 1.0f;

    private void Awake()
    {
        // Khởi tạo 5 Hàng đợi
        for (int i = 0; i < 5; i++)
        {
            comboQueues[i] = new Queue<BossCombo>();
            currentBeatIndices[i] = 0;
        }
    }

    private void OnEnable()
    {
        if (gameController != null) { 
            gameController.OnDataReady += ScanForCombos;
            gameController.OnSongLooped += ScanForCombos;

        }
    }

    private void OnDisable()
    {
        if (gameController != null) { 
            gameController.OnDataReady -= ScanForCombos;
            gameController.OnSongLooped -= ScanForCombos;
        }
    }
    private void ScanForCombos()
    {



        for (int i = 0; i < 5; i++) { 
            comboQueues[i].Clear(); 
            currentBeatIndices[i] = 0;//reset
        }

        var spectralData = gameController.GetSpectralData();
        var averages = gameController.GetAverages();

        int sampleRate = gameController.GetAudioSource().clip.frequency;
        int windowSize = 1024; // Khớp với config N của thuật toán FFT
        float noiseGate = 0.02f;
        float sens = gameController.sensitivityMultiplier;

        // Trạng thái quét (Look-ahead state) cho 5 dải
        List<float>[] tempComboTimes = new List<float>[5];
        float[] lastBeatTimes = new float[5];

        for (int i = 0; i < 5; i++)
        {
            tempComboTimes[i] = new List<float>();
            lastBeatTimes[i] = -99f;
        }

        for (int i = 0; i < spectralData.Count; i++)
        {
            float time = (float)(i * windowSize) / sampleRate;
            bool isBeat = spectralData[i].IsBeat;

            // Lấy dữ liệu hiện tại và trung bình của frame này
            FrequencyBands cur = spectralData[i].Bands;
            FrequencyBands avg = averages[i];


            // Đưa vào mảng để dùng vòng lặp cho gọn, thay vì viết if/else 5 lần
            float[] currentVals = { cur.Kick, cur.Bass, cur.LowMid, cur.HighMid, cur.Treble };
            float[] avgVals = { avg.Kick, avg.Bass, avg.LowMid, avg.HighMid, avg.Treble };

            if (!isBeat) continue;
            for (int band = 0; band < 5; band++) {
                if (currentVals[band] > (avgVals[band] * sens) && currentVals[band] > noiseGate)
                {
                    if (time - lastBeatTimes[band] > beatCooldown)
                    {
                        // Kiểm tra đứt chuỗi
                        if (tempComboTimes[band].Count > 0 && (time - lastBeatTimes[band] > maxComboDelay))
                        {
                            SaveCombo((BandType)band, tempComboTimes[band]);
                            tempComboTimes[band].Clear();
                        }
                        // Nối chuỗi
                        tempComboTimes[band].Add(time);
                        lastBeatTimes[band] = time;
                    }
                }



            }
            

        }
        // Lưu các chuỗi còn sót lại ở cuối bài
        for (int band = 0; band < 5; band++)
        {
            SaveCombo((BandType)band, tempComboTimes[band]);
        }

        PrintAllCombosToConsole();

    }
    private void SaveCombo(BandType type, List<float> times)
    {
        // Mo rong available
        if (times.Count >= 1 && times.Count <= 8)
        {
            int currentID = comboQueues[(int)type].Count + 1;

            comboQueues[(int)type].Enqueue(new BossCombo
            {
                bandType = type,
                beatCount = times.Count,
                beatTimes = times.ToArray(),
                comboID = currentID,
            });
        }
    }

    

    private void Update()
    {
        if (!gameController.GetAudioSource().isPlaying) return;
        float currentTime = gameController.GetAudioSource().time;
        for (int band = 0; band < 5; band++)
        {
            if (comboQueues[band].Count == 0) continue;
            BossCombo activeCombo = comboQueues[band].Peek();

            int bIndex = currentBeatIndices[band];

            if (bIndex < activeCombo.beatCount)
            {
                float telegraphTime = activeCombo.beatTimes[bIndex] - warningTime;
                if (currentTime >= telegraphTime)
                {
                    
                    ExecuteAttack(activeCombo, bIndex);

                   
                    currentBeatIndices[band]++;
                }

            }
            if (currentBeatIndices[band] >= activeCombo.beatCount)
            {
                comboQueues[band].Dequeue(); // Vứt bỏ combo đã xong
                currentBeatIndices[band] = 0; // Reset tiến độ
            }




            
        }

    }
    private void ExecuteAttack(BossCombo combo, int currentHitIndex)
    {

        OnBossAttackTriggered?.Invoke(combo, currentHitIndex);

        switch (combo.bandType)
        {
            case BandType.Kick:
                Instantiate(particlePrefab, new Vector3(0, 0, 0), Quaternion.identity);

                break;
            case BandType.Bass:
                Instantiate(particlePrefab, new Vector3(2, 0, 0), Quaternion.identity);

                break;
            case BandType.LowMid:
                Instantiate(particlePrefab, new Vector3(4, 0, 0), Quaternion.identity);

                break;
            case BandType.HighMid:
                Instantiate(particlePrefab, new Vector3(6, 0, 0), Quaternion.identity);

                break;
            case BandType.Treble:
                Instantiate(particlePrefab, new Vector3(8, 0, 0), Quaternion.identity);

                break;
        }
    }
    private void PrintAllCombosToConsole()
    {
        // Khởi tạo StringBuilder để nối chuỗi cực nhanh
        StringBuilder sb = new StringBuilder();
        int totalCombos = 0;

        sb.AppendLine("THỐNG KÊ SỐ LƯỢNG COMBO THEO TỪNG DẢI TẦN ");

        // Duyệt qua 5 dải tần
        for (int band = 0; band < 5; band++)
        {
            BandType currentBand = (BandType)band;
            Queue<BossCombo> queue = comboQueues[band];

            if (queue.Count > 0)
            {
                sb.AppendLine($"\n[{currentBand.ToString().ToUpper()}] có tổng cộng {queue.Count} chuỗi combo:");

                // Tạo mảng để đếm số lượng combo từ 1 đến 10 nhịp (index 0 bỏ trống cho tiện)
                int[] comboCounts = new int[22];

                // Duyệt qua hàng đợi để đếm
                foreach (BossCombo combo in queue)
                {
                    if (combo.beatCount >= 1 && combo.beatCount <= 8)
                    {
                        comboCounts[combo.beatCount]++;
                        totalCombos++;
                    }
                }

                // In ra kết quả thống kê cho dải tần hiện tại
                for (int i = 1; i <= 8; i++)
                {
                    // Chỉ in ra nếu loại combo đó có xuất hiện
                    if (comboCounts[i] > 0)
                    {
                        sb.AppendLine($"  -> Combo {i} nhịp: {comboCounts[i]} chuỗi");
                    }
                }
            }
        }

        sb.AppendLine($"\n=> TỔNG CỘNG TOÀN BÀI: {totalCombos} CHUỖI COMBO (từ 1-10 nhịp) SẼ ĐƯỢC TUNG RA.");

        // In toàn bộ ra Console đúng 1 lần duy nhất
        Debug.Log(sb.ToString());

    }



}
