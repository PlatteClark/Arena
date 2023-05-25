using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.UI;
using UnityEngine;

public class FlailHead : MonoBehaviour
{
    [SerializeField] Transform handle;
    [SerializeField] float forceStrength;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddForce((transform.position - handle.position).normalized * forceStrength);
    }
}
