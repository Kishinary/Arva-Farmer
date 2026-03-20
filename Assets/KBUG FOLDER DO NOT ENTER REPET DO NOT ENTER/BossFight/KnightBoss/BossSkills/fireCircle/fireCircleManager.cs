using UnityEngine;
using System;
using System.Collections;

public class fireCircleManager : MonoBehaviour
{
    [Header("Timings")]
    [SerializeField] private float warningDuration = 1.0f;

    [Header("Visuals")]
    [SerializeField] private GameObject warningVisual;
    [SerializeField] private GameObject strikeVisual;

    private WaitForSeconds _waitWarning;

    [SerializeField] private Transform bossPosition;


    public static event Action OnNewFireCircleSpawned;
    private void Awake()
    {
        _waitWarning = new WaitForSeconds(warningDuration);

    }

    private void Update()
    {
        transform.position = bossPosition.position;
    }
    private void OnEnable()
    {
        OnNewFireCircleSpawned?.Invoke();
        OnNewFireCircleSpawned += SelfDestruct;
        ExecuteStrike();
    }
    private void OnDisable()
    {
        OnNewFireCircleSpawned -= SelfDestruct;
    }

    public void ExecuteStrike()
    {
        StartCoroutine(StrikeRoutine());
    }
    private IEnumerator StrikeRoutine()
    {
        warningVisual.SetActive(true);
        strikeVisual.SetActive(false);

        yield return _waitWarning;

        warningVisual.SetActive(false);
        strikeVisual.SetActive(true);
    }
    private void SelfDestruct()
    {
        Destroy(gameObject);
    }
}
