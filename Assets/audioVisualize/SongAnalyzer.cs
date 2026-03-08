using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;


[System.Serializable]
public struct FrequencyBands
{
    public float Kick;     // SubBass
    public float Bass;     // Bass
    public float LowMid;   // Melody Trầm
    public float HighMid;  // Melody Cao
    public float Treble;   // Hi-hat / Cymbals
}

[System.Serializable]
public struct SpectralEvent
{
    public float Time;
    public FrequencyBands Bands;
}


public class SongAnalyzer : MonoBehaviour
{
    [Header("Settings")]
    public float threshold = 0.05f;
    [Header("Results")]
    public List<SpectralEvent> spectralTimeline = new List<SpectralEvent>();
    public async Task AnalyzeAudioAsync(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.Log("No Clip Detected");
            return;
        }

        // 1. Lấy dữ liệu ở Main Thread
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);
        int sampleRate = clip.frequency;

        // 2. Xử lý FFT nặng ở Background Thread
        spectralTimeline = await Task.Run(() => ProcessSpectrum(samples, sampleRate));
    }

    private List<SpectralEvent> ProcessSpectrum(float[] samples, int sampleRate)
    {
        List<SpectralEvent> timeline = new List<SpectralEvent>();
        // FFT yêu cầu kích thước cửa sổ là lũy thừa của 2
        int N = 1024;
        float binResolution = (float)sampleRate / N; // ~43Hz mỗi bin



        // Tránh cấp phát bộ nhớ liên tục trong vòng lặp
        float[] windowSamples = new float[N];

        for (int i = 0; i < samples.Length - N; i += N)
        {
            for (int j = 0; j < N; j++)
            {
                float multiplier = 0.5f * (1f - MathF.Cos(2f * MathF.PI * j / (N - 1)));
                windowSamples[j] = samples[i + j] * multiplier;
            }

            float[] spectrum = ComputeFFTMagnitude(windowSamples);
            FrequencyBands bands = new FrequencyBands();

            // Gom nhóm các Bin vào 5 dải tần số
            for (int bin = 0; bin < N / 2; bin++)
            {
                float freq = bin * binResolution;
                float mag = spectrum[bin];

                if (freq <= 60f) bands.Kick += mag;
                else if (freq <= 250f) bands.Bass += mag;
                else if (freq <= 500f) bands.LowMid += mag;
                else if (freq <= 2000f) bands.HighMid += mag;
                else bands.Treble += mag;
            }


            // Lưu lại điểm thời gian này nếu có bất kỳ âm tần nào vượt ngưỡng
            if (bands.Kick > threshold || bands.Bass > threshold ||
        bands.HighMid > threshold || bands.Treble > threshold)
            {
                float currentTime = (float)i / sampleRate;
                timeline.Add(new SpectralEvent { Time = currentTime, Bands = bands });
            }
        }

        return timeline;
    }
    private float[] ComputeFFTMagnitude(float[] data)
    {
        int n = data.Length;
        float[] real = new float[n];
        float[] imag = new float[n];
        Array.Copy(data, real, n);


        // Bit-reversal
        int j = 0;
        for (int i = 0; i < n - 1; i++)
        {
            if (i < j)
            {
                float temp = real[i];
                real[i] = real[j];
                real[j] = temp;
            }
            int m = n / 2;
            while (m >= 1 && j >= m) { j -= m; m /= 2; }
            j += m;
        }
        // Cooley-Tukey cai lon gi day
        for (int k = 1; k < n; k *= 2)
        {
            float wReal = MathF.Cos(-MathF.PI / k);
            float wImag = MathF.Sin(-MathF.PI / k);

            for (int i = 0; i < n; i += k * 2)
            {
                float uReal = 1f, uImag = 0f;
                for (j = 0; j < k; j++)
                {
                    float tReal = uReal * real[i + j + k] - uImag * imag[i + j + k];
                    float tImag = uReal * imag[i + j + k] + uImag * real[i + j + k];

                    real[i + j + k] = real[i + j] - tReal;
                    imag[i + j + k] = imag[i + j] - tImag;
                    real[i + j] += tReal;
                    imag[i + j] += tImag;

                    float nextUReal = uReal * wReal - uImag * wImag;
                    uImag = uReal * wImag + uImag * wReal;
                    uReal = nextUReal;
                }
            }
        }

        // Tính Magnitude = Sqrt(Real^2 + Imag^2)
        float[] magnitude = new float[n / 2];
        for (int i = 0; i < n / 2; i++)
        {
            magnitude[i] = MathF.Sqrt(real[i] * real[i] + imag[i] * imag[i]) / n;
        }
        return magnitude;


    }

}
