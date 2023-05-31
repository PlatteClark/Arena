using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    NavMeshAgent agent;
    Transform target;

    [Header("Perception")]
    [SerializeField] int raycasts;
    [SerializeField] float distance;
    [SerializeField] float radius;
    [SerializeField] float angle;
    [SerializeField] Transform raycastTransform;

    [Header("AI")]
    [SerializeField] float attackTime;
    [SerializeField] float idleTime;
    [SerializeField] float wanderDistance;
    [SerializeField] Animator animator;
    [SerializeField] float wanderSpeed;
    [SerializeField] float chaseSpeed;


    bool inRange = false;
    float attackTimer;
    float idleTimer;

    enum State
    {
        WANDER,
        IDLE,
        ATTACK,
        CHASE
    }

    State state = State.WANDER;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        attackTimer = attackTime;
        idleTimer = idleTime;
    }

    // Update is called once per frame
    void Update()
    {
        Perceive();

        Debug.Log(state.ToString());

        switch(state)
        {
            case State.WANDER:
                if(target != null)
                {
                    state = State.CHASE;
                }
                else
                {
                    agent.speed = wanderSpeed;

                    idleTimer -= Time.deltaTime;

                    if (agent.destination == null || agent.remainingDistance <= agent.stoppingDistance)
                    {
                        Utilities.RandomPoint(transform.position, wanderDistance, out Vector3 dest);

                        agent.SetDestination(dest);
                    }

                    if (idleTimer <= 0)
                    {
                        state = State.IDLE;

                        agent.isStopped = true;

                        animator.SetTrigger("idleInterrupt");
                    }
                    
                }
                break;
            case State.IDLE:
                if(target != null)
                {
                    state = State.CHASE;
                    break;
                }
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
                {
                    agent.isStopped = false;
                    state = State.WANDER;
                    idleTimer = idleTime;
                }
                break;
            case State.CHASE:
                if(target != null)
                {
                    agent.SetDestination(target.position);
                    agent.speed = chaseSpeed;

                    if(agent.remainingDistance <= agent.stoppingDistance)
                    {
                        state = State.ATTACK;
                        
                    }
                }
                else
                {
                    state = State.WANDER;
                    idleTimer = idleTime;
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

                animator.SetBool("inRange", inRange);

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
                        state = State.WANDER;
                        idleTimer = idleTime;
                        agent.isStopped = false;
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
            //Debug.DrawRay(ray.origin, ray.direction * distance, Color.green);

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
}
