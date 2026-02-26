using UnityEngine;

public class audioVisualize : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    
    [Header("References")]
    private AudioSource audioSource;
    public GameObject cubePrefab;


    [Header("Settings")]
    public float scaleMultiplier = 0.5f;
    public float smoothDampTime = 0.1f;
    public float minHeight = 0.5f;

    private Transform[] _cubes = new Transform[64];
    private float[] _spectrumData = new float[512];
    private float[] _freqBands = new float[64];
    private float[] _bandVelocities = new float[64];


    public Vector3 currentScale;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();



        for (int i = 0; i < 64; i++)
        {
            GameObject instance = Instantiate(cubePrefab, transform);
            // Xếp hàng ngang, khối đầu tiên ở gốc tọa độ, các khối sau cách nhau 0.2f
            instance.transform.localPosition = new Vector3(i * 0.4f, 0, 0);
            _cubes[i] = instance.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        GetSpectrumAudioSource();
        MakeFrequencyBands();
        UpdateCubesVisuals();
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

            // FIX LỖI 2: Chỉ chia cho số mẫu của DẢI NÀY, không chia cho tổng currentSampleIndex
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
}
