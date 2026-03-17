using System.Collections;
using UnityEngine;

public class KnightDartAttack : MonoBehaviour
{
    [Header("Dart Attack Settings")]
    public GameObject dartPrefab;
    public Transform firePoint;
    public GameObject warningPrefab;


    public float castDuration = 1f;

    public float recoveryDuration = 0.2f;
    public float spreadDistance = 1.5f;

    private knightMovement boss;

    private WaitForSeconds castWait;
    private WaitForSeconds recoveryWait;

    


    private void Awake()
    {
        boss = GetComponent<knightMovement>();
        castWait = new WaitForSeconds(castDuration);
        recoveryWait = new WaitForSeconds(recoveryDuration);
    }

    public void ExecuteAttack()
    {
        if (boss.isActionLocked) return;
        StartCoroutine(AttackRoutine());
    }
    private IEnumerator AttackRoutine()
    {
        boss.isActionLocked = true;
        boss.SetMovementDirection(Vector2.zero);

        if (boss.player == null || firePoint == null) yield break;

        Vector2 directionToPlayer = (boss.player.transform.position - firePoint.position).normalized;
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        Quaternion dartRotation = Quaternion.Euler(0, 0, angle);

        Vector3 perpendicularOffset = new Vector3(-directionToPlayer.y, directionToPlayer.x, 0f) * spreadDistance;

        Vector3 posCenter = firePoint.position;
        Vector3 posLeft = firePoint.position + perpendicularOffset;
        Vector3 posRight = firePoint.position - perpendicularOffset;

        if (warningPrefab != null)
        {
            Instantiate(warningPrefab, posCenter, dartRotation);
            Instantiate(warningPrefab, posLeft, dartRotation);
            Instantiate(warningPrefab, posRight, dartRotation);
        }
        yield return castWait;
        ShootDart(posCenter, posLeft, posRight, dartRotation);
        yield return recoveryWait;
        boss.isActionLocked = false;
    }

    private void ShootDart(Vector3 pos1, Vector3 pos2, Vector3 pos3, Quaternion rotation)
    {
        if (dartPrefab == null)
        {
            Debug.LogWarning("No prefab detected!");
            return;
        }
        Instantiate(dartPrefab, pos1, rotation);
        Instantiate(dartPrefab, pos2, rotation);
        Instantiate(dartPrefab, pos3, rotation);
    }



}
