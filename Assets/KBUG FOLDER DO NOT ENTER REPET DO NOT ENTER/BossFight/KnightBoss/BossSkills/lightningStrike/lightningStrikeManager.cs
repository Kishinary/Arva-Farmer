using UnityEngine;
using System.Collections;

public class lightningStrikeManager : MonoBehaviour
{
    [SerializeField] private float warningDuration = 1.0f;
    [SerializeField] private float strikeDuration = 0.5f;

    [SerializeField] private GameObject warningVisual;
    [SerializeField] private GameObject strikeVisual;

    private WaitForSeconds _waitWarning;
    private WaitForSeconds _waitStrike;

    private void Awake()
    {
        _waitWarning = new WaitForSeconds(warningDuration);
        _waitStrike = new WaitForSeconds(strikeDuration);

        ExecuteStrike();
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

        yield return _waitStrike;
        Destroy(gameObject);
        ResetStrike();

    }
    private void ResetStrike()
    {
        warningVisual.SetActive(false);
        strikeVisual.SetActive(false);
    }

}
