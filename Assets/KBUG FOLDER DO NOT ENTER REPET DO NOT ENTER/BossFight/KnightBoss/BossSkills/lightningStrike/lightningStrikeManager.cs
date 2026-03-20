using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class lightningStrikeManager : MonoBehaviour
{
    [SerializeField] private float warningDuration = 1.0f;
    [SerializeField] private float strikeDuration = 0.5f;

    [Header("Movement")]
    [SerializeField] private float moveDistance = 3f;

    [SerializeField] private GameObject warningVisual;
    [SerializeField] private GameObject strikeVisual;

    private WaitForSeconds _waitWarning;
    private WaitForSeconds _waitStrike;


    private Rigidbody2D _rb;
    private float _moveSpeed;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();


        _waitWarning = new WaitForSeconds(warningDuration);
        _waitStrike = new WaitForSeconds(strikeDuration);

        _moveSpeed = moveDistance / strikeDuration;

        
    }
    private void OnEnable()
    {
        ExecuteStrike();
    }

    public void ExecuteStrike()
    {
        StartCoroutine(StrikeRoutine());
    }
    private IEnumerator StrikeRoutine()
    {
        _rb.linearVelocity = Vector2.zero;

        warningVisual.SetActive(true);
        strikeVisual.SetActive(false);

        yield return _waitWarning;

        warningVisual.SetActive(false);
        strikeVisual.SetActive(true);

        Vector2 randomDir = Random.insideUnitCircle.normalized;
        _rb.linearVelocity = randomDir * _moveSpeed;

        yield return _waitStrike;
        _rb.linearVelocity = Vector2.zero;


        ResetStrike();
        Destroy(gameObject);
        

    }
    private void ResetStrike()
    {
        warningVisual.SetActive(false);
        strikeVisual.SetActive(false);
    }

}
