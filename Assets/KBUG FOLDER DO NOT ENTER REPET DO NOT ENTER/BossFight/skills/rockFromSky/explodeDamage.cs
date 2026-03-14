using UnityEngine;
using System.Collections;

public class explodeDamage : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private ParticleSystem ps;

    public float totalLifetime = 1f;

    public GameObject BlowEffect;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        

        StartCoroutine(DestroyAfterTime(totalLifetime));
    }

    // Update is called once per frame
    private IEnumerator DestroyAfterTime(float delay)
    {
        

        yield return new WaitForSeconds(delay);

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Instantiate(BlowEffect, transform.position, Quaternion.identity);
           
        }
    }

}
