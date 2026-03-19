using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class CineCamera : MonoBehaviour
{
    public static CineCamera instance { get; private set; }

    private CinemachineCamera m_Camera;
    private CinemachineBasicMultiChannelPerlin perlin;

    private float shakeTimer;
    private float shakeTimerTotal;
    private float startingIntensity;

    [Header("Transition Settings")]
    public float startZoomSize = 2f;
    public float endZoomSize; 
    public float zoomOutDuration = 1.5f;
    public float initialDelay = 0.5f;
    private CinemachineConfiner2D confiner;

    public float globalFrequency = 15f;

    public bool following = true;

    private void Awake()
    {
        instance = this;
        m_Camera = GetComponent<CinemachineCamera>();
        confiner = GetComponent<CinemachineConfiner2D>();
        perlin = m_Camera.GetComponent<CinemachineBasicMultiChannelPerlin>();

        // Ensure the frequency is high from the start for that jittery feel
        if (perlin) perlin.FrequencyGain = globalFrequency;
    }

    private void Start()
    {
        endZoomSize = m_Camera.Lens.OrthographicSize;
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null && following) m_Camera.Follow = player.transform;
        else Debug.LogWarning("CineCamera: No Player found with tag 'Player'.");
        m_Camera.Follow = null;
        StartCoroutine(StartSequence());
    }

    public void RequestShake(float intensity, float time)
    {
        if (intensity > perlin.AmplitudeGain || shakeTimer <= 0)
        {
            perlin.AmplitudeGain = intensity;
            startingIntensity = intensity;
            shakeTimerTotal = time;
            shakeTimer = time;
        }
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;

            float progress = shakeTimer / shakeTimerTotal;
            perlin.AmplitudeGain = Mathf.Lerp(0f, startingIntensity, progress * progress);
        }
        else
        {
            perlin.AmplitudeGain = 0f;
        }
    }

    public void TriggerPreset(string actionName)
    {
        switch (actionName)
        {
            case "Scissors": RequestShake(10f, 0.04f); break;
            case "Watercan": RequestShake(20f, 0.10f); break;
            case "Slash": RequestShake(15f, 0.06f); break;
            case "ShovelNormal": RequestShake(15f, 0.08f); break;
            case "ShovelSpecial": RequestShake(12f, 0.15f); break;
            case "TakeDamage": RequestShake(12.0f, 0.20f); break;
            case "EnemyHit": RequestShake(10f, 0.05f); break;
            case "Explosion": RequestShake(40.0f, 0.40f); break;
            case "BugRacket": RequestShake(20f, 0.5f); break;
            case "KnightBossMovement": RequestShake(10f, 0.5f); break;
        }
    }

    IEnumerator StartSequence()
    {
        Transform player = GameObject.FindWithTag("Player").transform;

        // Snap camera to player at start
        m_Camera.ForceCameraPosition(player.position - new Vector3(0,0, 10f), Quaternion.identity);

        m_Camera.Lens.OrthographicSize = startZoomSize;

        if (confiner != null)
            confiner.InvalidateBoundingShapeCache();

        yield return new WaitForSeconds(initialDelay);

        float timer = 0f;

        while (timer < zoomOutDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.SmoothStep(0, 1, timer / zoomOutDuration);

            m_Camera.Lens.OrthographicSize =
                Mathf.Lerp(startZoomSize, endZoomSize, t);

            yield return null;
        }

        // Enable follow AFTER zoom
        m_Camera.Follow = player;

        if (confiner != null)
            confiner.InvalidateBoundingShapeCache();
    }
}