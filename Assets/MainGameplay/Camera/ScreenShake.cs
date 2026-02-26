using UnityEngine;
using Unity.Cinemachine;
public class ScreenShake : MonoBehaviour
{
    public CinemachineCamera Vcam;
    public float intensity;
    public float time;

    private float timer;
    private CinemachineBasicMultiChannelPerlin perlin;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vcam = GetComponent<CinemachineCamera>();
        StopShake();
    }
    public void ShakeCamera()
    {
        perlin = Vcam.GetComponent<CinemachineBasicMultiChannelPerlin>();
        perlin.AmplitudeGain = intensity;

        timer = time;

        timer -= Time.deltaTime;
        if (timer < 0)
        {
            StopShake();
        }
     }

    void StopShake()
    {
        perlin = Vcam.GetComponent<CinemachineBasicMultiChannelPerlin>();
        perlin.AmplitudeGain = 0.0f;
        timer = 0;
    }
}
