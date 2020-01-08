using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordScript : MonoBehaviour
{

    public float maxSpeed;
    
    public bool isForced;
    public bool isGrabbed;


    Rigidbody rb;


    public Transform target;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }


    void Update()
    {
        /*if (isForced)
        {
            transform.LookAt(target);
        }*/
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
        }
    }


    private void OnCollisionEnter(Collision col)
    {
        if (isForced == false && isGrabbed == false)
        {
            rb.velocity = Vector3.zero;
        }
    }
}
