﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordScript : MonoBehaviour
{

    public float maxSpeed;
    
    public bool isForced;
    public bool isGrabbed;


    Rigidbody rb;


    public Transform target;
    public float force;

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
        /*Vector3 Truc = new Vector3(0, -0.5f, 0);
        if (isForced)
        {
            transform.LookAt(target.position-Truc);
        }*/
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (isForced == true && isGrabbed == false)
        {
            Vector3 targetDelta = target.position - transform.position;
            float angleDiff = Vector3.Angle(transform.forward, targetDelta);

            Vector3 cross = Vector3.Cross(transform.forward, targetDelta);

            rb.AddTorque(cross * angleDiff * force);

        }

        if(isGrabbed)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            rb.useGravity = false;
        }
        
        if (isForced == false)
        {
            if (isGrabbed == false)
            {
                rb.useGravity = true;
                rb.constraints = RigidbodyConstraints.None;
            }
        }

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
