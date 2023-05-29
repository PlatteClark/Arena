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
    

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        Perceive();

        if (target != null)
        {
            agent.SetDestination(target.position);
            if(agent.remainingDistance <= agent.stoppingDistance)
            {
                Quaternion look = Quaternion.LookRotation((target.position - transform.position).normalized);
                look.x = 0;
                look.z = 0;
                transform.rotation = look;
            }
        }
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
