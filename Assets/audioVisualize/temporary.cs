using UnityEngine;

public class AudioVisualize : MonoBehaviour
{
    [Header("References")]
    private AudioSource audioSource;
    public GameObject cubePrefab;

    // ÉP KIỂU CHẶT CHẼ: Trỏ thẳng script EnemySpawner thay vì GameObject chung chung
    [SerializeField] private EnemySpawner enemySpawner;

    [Header("Visualizer Settings")]
    public float scaleMultiplier = 0.5f;
    public float smoothDampTime = 0.1f;
    public float minHeight = 0.5f;

    private Transform[] _cubes = new Transform[64];
    private float[] _spectrumData = new float[512];
    private float[] _freqBands = new float[64];
    private float[] _bandVelocities = new float[64];

    [Header("Beat Detection Settings")]
    [Tooltip("Dải tần muốn theo dõi để sinh quái (0-3 thường là Bass)")]
    public int targetBand = 0;
    [Tooltip("Độ chênh lệch. 1.3 = Năng lượng phải cao hơn 30% so với trung bình")]
    public float varianceThreshold = 1.3f;
    public float spawnCooldown = 0.2f;
    public float sampleRate = 60f; // Tốc độ phân tích AI (60 lần/giây)

    // Ring Buffer lưu lịch sử 43 mẫu
    private float[] _energyHistory = new float[43];
    private int _historyIndex = 0;

    // Biến quản lý thời gian (FPS Independent)
    private float _timePerSample;
    private float _timeAccumulator = 0f;
    private float _cooldownTimer = 0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Tính toán khoảng thời gian giữa mỗi lần lấy mẫu AI
        _timePerSample = 1f / sampleRate;

        for (int i = 0; i < 64; i++)
        {
            GameObject instance = Instantiate(cubePrefab, transform);
            // Xếp hàng ngang, khối đầu tiên ở gốc tọa độ, các khối sau cách nhau 0.4f
            instance.transform.localPosition = new Vector3(i * 0.4f, 0, 0);
            _cubes[i] = instance.transform;
        }
    }

    void Update()
    {
        // ---------------------------------------------------------
        // LUỒNG 1: ĐỒ HỌA (Chạy mỗi frame để hình ảnh mượt mà)
        // ---------------------------------------------------------
        GetSpectrumAudioSource();
        MakeFrequencyBands();
        UpdateCubesVisuals();

        // ---------------------------------------------------------
        // LUỒNG 2: LOGIC SINH QUÁI (Chạy độc lập với FPS)
        // ---------------------------------------------------------
        if (_cooldownTimer > 0) _cooldownTimer -= Time.deltaTime;

        _timeAccumulator += Time.deltaTime;

        // Vòng lặp này đảm bảo logic AI luôn chạy đúng 'sampleRate' lần mỗi giây
        // Bất kể game đang chạy ở 30 FPS hay 400 FPS
        while (_timeAccumulator >= _timePerSample)
        {
            ProcessBeatDetection();
            _timeAccumulator -= _timePerSample;
        }
    }

    private void GetSpectrumAudioSource()
    {
        // Sử dụng BlackmanHarris window để giảm nhiễu (leakage) giữa các dải tần
        audioSource.GetSpectrumData(_spectrumData, 0, FFTWindow.BlackmanHarris);
    }

    private void MakeFrequencyBands()
    {
        int currentSampleIndex = 0;

        for (int i = 0; i < 64; i++)
        {
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i * 0.125f);

            // Đảm bảo tối thiểu mỗi dải có 1 mẫu
            if (sampleCount == 0) sampleCount = 1;

            int samplesAddedThisBand = 0; // Biến phụ để đếm số mẫu thực tế trong dải này

            for (int j = 0; j < sampleCount; j++)
            {
                if (currentSampleIndex < 512)
                {
                    average += _spectrumData[currentSampleIndex] * (currentSampleIndex + 1);
                    currentSampleIndex++;
                    samplesAddedThisBand++;
                }
            }

            // Chỉ chia cho số mẫu của DẢI NÀY, không chia cho tổng currentSampleIndex
            if (samplesAddedThisBand > 0)
            {
                average /= samplesAddedThisBand;
            }

            _freqBands[i] = average * scaleMultiplier * (i + 1);
        }
    }

    private void UpdateCubesVisuals()
    {
        for (int i = 0; i < 64; i++)
        {
            float currentY = _cubes[i].localScale.y;
            float targetY = _freqBands[i] + minHeight;

            // Bảo vệ toán học: Đừng để Cube bay lên tận trời nếu nhạc quá to
            targetY = Mathf.Clamp(targetY, minHeight, 100f);

            float newY = Mathf.SmoothDamp(currentY, targetY, ref _bandVelocities[i], smoothDampTime);

            _cubes[i].localScale = new Vector3(1, newY, 1);
        }
    }

    /// <summary>
    /// Thuật toán nhận diện nhịp đập độc lập với Frame-rate
    /// </summary>
    private void ProcessBeatDetection()
    {
        // 1. Lấy năng lượng hiện tại từ dải tần số mục tiêu (đã được tính toán ở MakeFrequencyBands)
        float currentEnergy = _freqBands[targetBand];

        // 2. Tính trung bình năng lượng lịch sử
        float sumEnergy = 0;
        for (int i = 0; i < _energyHistory.Length; i++)
        {
            sumEnergy += _energyHistory[i];
        }
        float averageEnergy = sumEnergy / _energyHistory.Length;

        // 3. Kiểm tra điều kiện sinh quái bằng toán học Delta
        // Bỏ qua nếu trung bình quá nhỏ (tránh chia cho 0 hoặc nhận diện tiếng ồn nền)
        if (averageEnergy > 0.05f && currentEnergy > averageEnergy * varianceThreshold && _cooldownTimer <= 0f)
        {
            // Lấy tọa độ đỉnh của khối Cube tương ứng để sinh quái vật
            Vector3 spawnPosition = _cubes[targetBand].position + new Vector3(0, _cubes[targetBand].localScale.y, 0);

            if (enemySpawner != null)
            {
                enemySpawner.SpawnEnemyAtPosition(spawnPosition);
            }
            else
            {
                Debug.LogWarning("EnemySpawner reference is missing! Please assign it in the inspector.");
            }

                _cooldownTimer = spawnCooldown;
        }

        // 4. Ghi đè dữ liệu mới vào Ring Buffer
        _energyHistory[_historyIndex] = currentEnergy;
        _historyIndex = (_historyIndex + 1) % _energyHistory.Length;
    }
}