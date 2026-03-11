using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugRacket : MonoBehaviour, IWeapon
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
    public int maxBugs = 5;
    public Transform shootPoint;
    public GameObject butterflyProjectile;
    public GameObject fireflyProjectile;
    public GameObject beetleProjectile;

    List<BugType> storedBugs = new List<BugType>();
    List<GameObject> storedBugObject = new List<GameObject>();


    [Header("Cooldowns")]
    public float catchCooldown = 0.5f;
    public float releaseCooldown = 1f;

    private float nextCatchTime;
    private float nextReleaseTime;

    [Header("Damage")]
    public int butterflyDamage = 12;
    public int fireflyDamage = 18;
    public int beetleDamage = 25;

    [Header("Particles")]
    public ParticleSystem catchParticle;
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
            particleRotation *= Quaternion.Euler(catchParticle.transform.eulerAngles.x * -1, 0, 0);
        }

        Instantiate(catchParticle, transform.position, particleRotation);

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

        ReleaseBugs();
    }

    private void PerformCatchCheck()
    {
        if (storedBugs.Count >= maxBugs) return;

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

    public void ReleaseBugs()
    {
        if (storedBugs.Count == 0) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePos - shootPoint.position).normalized;

        StartCoroutine(ShootRoutine(dir));
    }

    IEnumerator ShootRoutine(Vector2 dir)
    {
        while (storedBugs.Count > 0)
        {
            BugType bug = storedBugs[0];
            storedBugs.RemoveAt(0);
            foreach (GameObject obj in storedBugObject)
            {
                // The Destroy function removes the object from the scene
                // This does not happen immediately, but at the end of the current frame
                if (obj != null) // Check if the object is still valid before destroying (optional, but good practice)
                {
                    Destroy(obj);
                }
            }

            // After the loop, clear the list itself. 
            // The list only holds references, so clearing it is separate from destroying the actual GameObjects.
            storedBugObject.Clear(); 
            ShootBug(bug, dir);

            yield return new WaitForSeconds(0.15f);
        }
    }
    void ShootBug(BugType bug, Vector2 dir)
    {
        GameObject proj = null;

        switch (bug)
        {
            case BugType.Butterfly:
                proj = Instantiate(butterflyProjectile, shootPoint.position, weaponParent.transform.rotation);
                break;

            case BugType.Firefly:
                proj = Instantiate(fireflyProjectile, shootPoint.position, weaponParent.transform.rotation);
                break;

            case BugType.Beetle:
                proj = Instantiate(beetleProjectile, shootPoint.position, weaponParent.transform.rotation);
                break;
        }

        if (proj != null)
        {
            return;
        }
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