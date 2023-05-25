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

    public bool swing = false;
    float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        handlerb = handle.GetComponent<Rigidbody>();
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
                rb.AddForce(handle.transform.up * handlerb.velocity.magnitude * 2);
                rb.AddForce(Vector3.forward * handlerb.velocity.magnitude);
            }
        }
    }
}
