using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.UI;
using UnityEngine;

public class FlailHead : MonoBehaviour
{
    [SerializeField] GameObject handle;
    [SerializeField] float forceStrength;
    Rigidbody rb;
    Rigidbody handlerb;

    private AudioSource audioSource;
    [SerializeField] AudioClip[] impactSounds;

    public bool swing = false;
    float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        handlerb = handle.GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(swing)
        {
            rb.AddForce((transform.position - handle.transform.position).normalized * forceStrength);
            timer = 1;
        }
        else
        {
            timer -= Time.deltaTime;
            if(timer > 0)
            {
                rb.AddForce(handle.transform.up * handlerb.velocity.sqrMagnitude);
                rb.AddForce(Vector3.forward * handlerb.velocity.sqrMagnitude);
            }
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody otherRb = other.GetComponent<Rigidbody>();

        Vector3 collisionPoint = other.ClosestPoint(transform.position);

        Vector3 forceDirection = (collisionPoint - transform.position).normalized;

        Vector3 force = forceDirection * rb.velocity.magnitude;

        if (otherRb != null)
        {
            otherRb.AddForce(force, ForceMode.Impulse);       
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            PlaySounds();

            Enemy enemy = other.gameObject.GetComponent<Enemy>();

            forceDirection = (other.transform.position - transform.position).normalized;

            forceDirection += new Vector3( 1, 2.5f, 1);

            force = forceDirection * rb.velocity.magnitude;

            if (enemy != null)
            {
                enemy.TakeDamage(rb.velocity.magnitude, force);
            }
            else
            {
                Minotaur minotaur = other.gameObject.GetComponent<Minotaur>();

                if (minotaur != null)
                {
                    minotaur.TakeDamage(rb.velocity.magnitude, force);
                }
            }
        }
    }
    public void PlaySounds()
    {
        AudioClip clip = impactSounds[UnityEngine.Random.Range(0, impactSounds.Length)];
        audioSource.PlayOneShot(clip);
    }
}
