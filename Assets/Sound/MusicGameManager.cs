using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class MusicGameManager : MonoBehaviour
{
    [Header("References")]
    public AudioSource audioSource;
    public GameObject enemyPrefab;
    public Transform spawnPoint;

    [Header("Analysis Settings")]
    public int frameSize = 1024;
    public int hopSize = 512;
    public float onsetThresholdMultiplier = 1.5f;
    public int quantizeSubdivision = 4; // 4 = 16th notes

    private List<double> beatTimes = new List<double>();
    private double songStartDSP;
    private int nextBeatIndex = 0;

    void Update()
    {
        HandleDragAndDrop();
        HandleSpawning();
    }

    // =========================
    // DRAG & DROP (PC BUILD)
    // =========================
    void HandleDragAndDrop()
    {
        if (!Input.GetKeyDown(KeyCode.Space)) return;

        Debug.Log("Press SPACE and type full MP3 path in console.");
        // For prototype simplicity:
        // Replace this with a file browser if desired.
    }

    public void LoadSong(string path)
    {
        StartCoroutine(LoadAudio(path));
    }

    IEnumerator LoadAudio(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("File not found.");
            yield break;
        }

        string url = "file:///" + path;

        using (var www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
                yield break;
            }

            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            audioSource.clip = clip;

            Debug.Log("Loaded clip. Analyzing...");
            AnalyzeAudio(clip);
        }
    }

    // =========================
    // AUDIO ANALYSIS
    // =========================
    void AnalyzeAudio(AudioClip clip)
    {
        float[] monoSamples = GetMonoSamples(clip);
        List<double> onsets = DetectOnsetsRMS(monoSamples, clip.frequency);
        float bpm = EstimateBPM(onsets);

        Debug.Log("Estimated BPM: " + bpm);

        beatTimes = QuantizeOnsets(onsets, bpm);
        Debug.Log("Generated beats: " + beatTimes.Count);

        StartSong();
    }

    float[] GetMonoSamples(AudioClip clip)
    {
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        if (clip.channels == 1)
            return samples;

        float[] mono = new float[clip.samples];
        for (int i = 0; i < clip.samples; i++)
        {
            mono[i] = 0f;
            for (int c = 0; c < clip.channels; c++)
                mono[i] += samples[i * clip.channels + c];

            mono[i] /= clip.channels;
        }
        return mono;
    }

    List<double> DetectOnsetsRMS(float[] samples, int sampleRate)
    {
        int frameCount = (samples.Length - frameSize) / hopSize;
        float[] rms = new float[frameCount];

        for (int f = 0; f < frameCount; f++)
        {
            int start = f * hopSize;
            float sum = 0f;

            for (int i = 0; i < frameSize; i++)
            {
                float s = samples[start + i];
                sum += s * s;
            }

            rms[f] = Mathf.Sqrt(sum / frameSize);
        }

        List<double> onsetTimes = new List<double>();
        float avg = 0f;
        foreach (float r in rms) avg += r;
        avg /= rms.Length;

        float threshold = avg * onsetThresholdMultiplier;

        for (int i = 1; i < rms.Length - 1; i++)
        {
            if (rms[i] > threshold &&
                rms[i] > rms[i - 1] &&
                rms[i] > rms[i + 1])
            {
                double time = (double)(i * hopSize) / sampleRate;
                onsetTimes.Add(time);
            }
        }

        return onsetTimes;
    }

    float EstimateBPM(List<double> onsets)
    {
        if (onsets.Count < 2) return 120f;

        List<double> intervals = new List<double>();
        for (int i = 1; i < onsets.Count; i++)
            intervals.Add(onsets[i] - onsets[i - 1]);

        intervals.Sort();
        double median = intervals[intervals.Count / 2];
        double bpm = 60.0 / median;

        while (bpm < 60) bpm *= 2;
        while (bpm > 180) bpm /= 2;

        return (float)bpm;
    }

    List<double> QuantizeOnsets(List<double> onsets, float bpm)
    {
        List<double> quantized = new List<double>();
        float beatDuration = 60f / bpm;
        double grid = beatDuration / quantizeSubdivision;

        foreach (var t in onsets)
        {
            double q = Math.Round(t / grid) * grid;
            quantized.Add(q);
        }

        quantized.Sort();
        return quantized;
    }

    void StartSong()
    {
        songStartDSP = AudioSettings.dspTime + 0.5;
        audioSource.PlayScheduled(songStartDSP);
        nextBeatIndex = 0;
    }

    void HandleSpawning()
    {
        if (nextBeatIndex >= beatTimes.Count) return;

        double songTime = AudioSettings.dspTime - songStartDSP;

        while (nextBeatIndex < beatTimes.Count &&
               beatTimes[nextBeatIndex] <= songTime)
        {
            SpawnEnemy();
            nextBeatIndex++;
        }
    }

    void SpawnEnemy()
    {
        Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
    }
}
