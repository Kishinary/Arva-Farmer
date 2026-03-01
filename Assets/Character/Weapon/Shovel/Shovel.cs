using System.Collections;
using UnityEditor.Analytics;
using UnityEngine;

public class Shovel : MonoBehaviour
{

    private Animator animator;

    [Header("Attack Details")]
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    public float NormalAttackCooldown = 0.5f;
    private float nextAttackTime = 0f;
    private float nextNormalAttackTime = 0f;

    public float NormalAttackDamage;
    public float SpecialAttackDamage;
    [Header("Particle")]
    public ParticleSystem dirtParticle;
    private GameObject DamageCollider;
    public ParticleSystem SmashParticle;

    [Header("Parents")]
    private WeaponParent weaponParent;
    private PlayerMovement PlayerMove;

    [Header("Checker")]
    public bool isNormaling;
    void Start()
    {
        animator = GetComponent<Animator>();
        DamageCollider = FindFirstObjectByType<DirtDamage>().gameObject;
        weaponParent = GetComponentInParent<WeaponParent>();
        PlayerMove = GetComponentInParent<PlayerMovement>();
        DamageCollider.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Attack()
    {
        if (Time.time >= nextAttackTime) {
            animator.SetTrigger("Dig");
            PlayerMove.movespeed = 5f;
            PlayerMove.gameObject.GetComponent<Animator>().speed = 0.5f;
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    public void ApplyParticle()
    {
        if (weaponParent.transform.localScale.y == -1)
        {
            Instantiate(dirtParticle, transform.position, weaponParent.transform.rotation * Quaternion.Euler(dirtParticle.transform.eulerAngles.x * -1 -10f, dirtParticle.transform.eulerAngles.y, dirtParticle.transform.eulerAngles.z));
        }
        else
        {
            Instantiate(dirtParticle, transform.position, weaponParent.transform.rotation * Quaternion.Euler(dirtParticle.transform.eulerAngles.x, dirtParticle.transform.eulerAngles.y, dirtParticle.transform.eulerAngles.z));
        }
        StartCoroutine(CheckDamage());
    }
    public void ApplyParticleForNormal()
    {
        if (weaponParent.transform.localScale.y == -1)
        {
            Instantiate(SmashParticle, transform.position, weaponParent.transform.rotation * Quaternion.Euler(SmashParticle.transform.eulerAngles.x * -1, SmashParticle.transform.eulerAngles.y, SmashParticle.transform.eulerAngles.z));
        }
        else
        {
            Instantiate(SmashParticle, transform.position, weaponParent.transform.rotation * SmashParticle.transform.localRotation);
        }
        StartCoroutine(CheckDamage());
    }
    IEnumerator CheckDamage()
    {
        yield return new WaitForSeconds(0.15f);
        DamageCollider.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        PlayerMove.EnablePlayerInput();
        PlayerMove.gameObject.GetComponent<Animator>().speed = 1f;
        yield return new WaitForSeconds(0.2f);
        DamageCollider.SetActive(false);
    }



    public void NormalAttack()
    {
        if (Time.time >= nextNormalAttackTime)
        {
            Debug.Log("sdf");
            animator.SetTrigger("Smash");
            PlayerMove.movespeed = 7f;
            nextNormalAttackTime = Time.time + NormalAttackCooldown;
        }
    }

    public void AttackFalse()
    {
        StartCoroutine(AttackYes());    }

    IEnumerator AttackYes()
    {
        isNormaling = true;
        yield return new WaitForSeconds(0.15f);
        isNormaling = false;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isNormaling)
        {
            if (collision.CompareTag("Enemy"))
            {
                collision.GetComponent<EnemyStats>().TakeDamage(PlayerMove.transform.position, NormalAttackCooldown);
            }
        }
    }
}
