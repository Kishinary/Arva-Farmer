using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rake : MonoBehaviour, IWeapon
{
    private Animator animator;

    [Header("Catch Settings")]
    public LayerMask bugLayer;
    public Transform catchPoint;
    public float catchRadius = 1.2f;

    [Header("Release Settings")]
    public LayerMask enemyLayer;
    public float releaseRadius = 3f;

    [Header("Storage")]
    public int maxtraps = 5;
    public Transform shootPoint;
    public GameObject BearTrap;
    public GameObject RakeTrap;
    public GameObject Holetrap;

    List<BugType> storedBugs = new List<BugType>();
    List<GameObject> storedBugObject = new List<GameObject>();


    [Header("Cooldowns")]
    public float catchCooldown = 0.5f;
    public float releaseCooldown = 1f;

    private float nextCatchTime;
    private float nextReleaseTime;

    [Header("Damage")]

    [Header("Particles")]
    public ParticleSystem TrapParticle;
    public ParticleSystem releaseParticle;

    private WeaponParent weaponParent;
    private PlayerMovement playerMove;

    public string GetNormalShake() => "BugRacket";
    public string GetSpecialShake() => "BugRacket";

    void Start()
    {
        animator = GetComponent<Animator>();
        weaponParent = GetComponentInParent<WeaponParent>();
        playerMove = GetComponentInParent<PlayerMovement>();
    }

    public void NormalAttack() // Catch Bugs
    {
        if (Time.time >= nextCatchTime)
        {
            animator.SetTrigger("Swing");
            nextCatchTime = Time.time + catchCooldown;
        }
    }

    public void SpecialAttack()
    {
        if (Time.time >= nextReleaseTime && storedBugs.Count > 0)
        {
            animator.SetTrigger("Release");
            nextReleaseTime = Time.time + releaseCooldown;
        }
    }

    public void ApplyCatch()
    {
        Quaternion particleRotation = weaponParent.transform.rotation;

        if (weaponParent.transform.localScale.y == -1)
        {
            particleRotation *= Quaternion.Euler(TrapParticle.transform.eulerAngles.x * -1, 0, 0);
        }

        Instantiate(TrapParticle, transform.position, particleRotation);

        PerformCatchCheck();
    }

    public void ApplyRelease()
    {
        Quaternion particleRotation = weaponParent.transform.rotation;

        if (weaponParent.transform.localScale.y == -1)
        {
            particleRotation *= Quaternion.Euler(releaseParticle.transform.eulerAngles.x * -1, 0, 0);
        }

        Instantiate(releaseParticle, transform.position, particleRotation);

        ReleaseTrap();
    }

    private void PerformCatchCheck()
    {
        if (storedBugs.Count >= maxtraps) return;

        Collider2D[] bugs = Physics2D.OverlapCircleAll(catchPoint.position, catchRadius, bugLayer);

        foreach (Collider2D bug in bugs)
        {
            Bug bugScript = bug.GetComponent<Bug>();

            if (bugScript != null)
            {
                storedBugs.Add(bugScript.type);
                bug.gameObject.GetComponent<Orbiter>().enabled = true;
                storedBugObject.Add(bugScript.gameObject);
                break;
            }
        }
    }

    public void ReleaseTrap()
    {
        Instantiate(BearTrap);

    }


    private void OnDrawGizmosSelected()
    {
        if (catchPoint == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(catchPoint.position, catchRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, releaseRadius);
    }
}