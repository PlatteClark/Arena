using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Minotaur : MonoBehaviour
{
    NavMeshAgent agent;
    Transform target;

    [SerializeField] float health;
    [SerializeField] GameObject ragdoll;

    [Header("Key")]
    [SerializeField] GameObject KeyDrop;
    [SerializeField] GameObject interactTxt;
    [SerializeField] TMP_Text message_text;

    [Header("Perception")]
    [SerializeField] int raycasts;
    [SerializeField] float distance;
    [SerializeField] float radius;
    [SerializeField] float angle;
    [SerializeField] Transform raycastTransform;

    [Header("AI")]
    [SerializeField] float attackTime;
    [SerializeField] float chargeTime;
    [SerializeField] Animator animator;
    [SerializeField] float chaseSpeed;
    [SerializeField] float chargeSpeed;

    private AudioSource audioSource;
    [SerializeField] AudioClip[] stepSounds;
    bool inRange = false;
    float attackTimer;
    float chargeTimer;

    enum State
    {
        CHARGE,
        IDLE,
        ATTACK,
        CHASE
    }

    State state = State.IDLE;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();

        attackTimer = attackTime;
        chargeTimer = chargeTime;
    }

    // Update is called once per frame
    void Update()
    {
        if(state != State.CHARGE)
        {
            Perceive();
        }

        switch(state)
        {
            case State.CHARGE:
                agent.speed = chargeSpeed;
                agent.stoppingDistance = 0;
                if(agent.remainingDistance <= agent.stoppingDistance)
                {
                    agent.speed = chaseSpeed;
                    state = State.IDLE;
                    chargeTimer = chargeTime;
                    agent.stoppingDistance = 3;
                }
                break;
            case State.IDLE:
                if(target != null)
                {
                    state = State.CHASE;
                    animator.SetBool("Idle", false);
                    agent.isStopped = false;
                }
                else
                {
                    agent.isStopped = true;
                    animator.SetBool("Idle", true);
                }
                break;
            case State.CHASE:
                if(target != null)
                {
                    agent.SetDestination(target.position);
                    agent.speed = chaseSpeed;

                    chargeTimer -= Time.deltaTime;

                    if(chargeTimer <= 0)
                    {
                        state = State.CHARGE;
                        //Vector3 dir = (transform.position - target.position).normalized * 3;
                        //Vector3 targetPos = target.position + dir;
                        //agent.SetDestination(targetPos);
                    }
                    else if(agent.remainingDistance <= agent.stoppingDistance)
                    {
                        state = State.ATTACK;
                    }
                }
                else 
                {
                    state = State.IDLE;
                }
                break;
            case State.ATTACK:

                agent.isStopped = true;

                if(target != null)
                {
                    agent.SetDestination(target.position);
                    inRange = agent.remainingDistance <= agent.stoppingDistance;
                    agent.isStopped = true;
                }
                else
                {
                    inRange = false;
                }

                if (inRange)
                {
                    attackTimer -= Time.deltaTime;

                    Vector3 look = target.position;
                    look.y = transform.position.y;

                    transform.LookAt(look);

                    if(attackTimer <= 0)
                    {
                        animator.SetTrigger("attack");
                        attackTimer = attackTime;
                    }
                }
                else
                {
                    if (target == null)
                    {
                        state = State.IDLE;
                    }
                    else
                    {
                        state = State.CHASE;
                        agent.isStopped = false;
                    }
                }
                break;
        }

        animator.SetFloat("speed", agent.velocity.magnitude);
    }

    void Perceive()
    {
        List<Vector3> directions = new List<Vector3>();

        // if odd number, set first direction as forward (0, 0, 1)
        if (raycasts % 2 == 1) directions.Add(Vector3.forward);

        // compute the angle between rays
        float angleOffset = (angle * 2) / raycasts;
        // add the +/- directions around the circle
        for (int i = 1; i <= raycasts / 2; i++)
        {
            float modifier = (i == 1 && raycasts % 2 == 0) ? 0.65f : 1;
            directions.Add(Quaternion.AngleAxis(+angleOffset * i * modifier, Vector3.up) * Vector3.forward);
            directions.Add(Quaternion.AngleAxis(-angleOffset * i * modifier, Vector3.up) * Vector3.forward);
        }

        foreach (Vector3 direction in directions)
        {
            Ray ray = new Ray(raycastTransform.position, raycastTransform.rotation * direction);
            Debug.DrawRay(ray.origin, ray.direction * distance, Color.green);

            if (Physics.SphereCast(ray, radius, out RaycastHit raycastHit, distance))
            {
                // don't perceive self 
                if (raycastHit.collider.gameObject == gameObject) continue;
                
                if (raycastHit.collider.CompareTag("Player"))
                {
                    target = raycastHit.collider.gameObject.transform;
                    return;
                }
            }
        }

        target = null;
    }

    public void TakeDamage(float damage, Vector3 force)
    {
        health -= damage;
        if(health <= 0)
        {
            GameObject go = Instantiate(ragdoll, transform.position, transform.rotation);
            GameObject key = Instantiate(KeyDrop, transform.position, transform.rotation);
            key.gameObject.GetComponent<Interactable>().interactionPrompt = interactTxt;
            KeyController keyController = key.GetComponent<KeyController>();
            keyController.messageText = message_text;
            keyController.interactText = interactTxt;
            var rbs = go.GetComponentsInChildren<Rigidbody>();

            foreach (var rb in rbs)
            {
                rb.AddForce(force, ForceMode.Impulse);
            }

            Destroy(gameObject);
        }
    }

    public void PlayStepSounds()
    {
        AudioClip clip = stepSounds[UnityEngine.Random.Range(0, stepSounds.Length)];
        audioSource.PlayOneShot(clip);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.gameObject.CompareTag("Player"))
        {
            if(state == State.CHARGE)
            {
                Rigidbody rb = collision.collider.gameObject.GetComponent<Rigidbody>();

                Vector3 collisionPoint = collision.collider.ClosestPoint(transform.position);

                Vector3 forceDirection = (collisionPoint - transform.position).normalized;

                Vector3 force = forceDirection * (chargeSpeed * 0.75f);

                rb.AddForce(force, ForceMode.Impulse);
            }
        }
    }
}
