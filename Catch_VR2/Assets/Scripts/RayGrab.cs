﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum StatePower
{
    Sleep,
    Attract,
    Equiped
}
public class RayGrab : MonoBehaviour {


    [Header("Oculus Anchor")]
    public GameObject anchorRight;
    public GameObject anchorLeft;
    public GameObject anchorCenter;

    [Header("Raycast Parameter")]
    public float distance;
    public float sphereRadius;

    [Header("State Of Power")]
    public StatePower sPRight;
    public StatePower sPLeft;
    public GameObject swordLeft;
    public GameObject swordRight;

    [Header("Force to Apply")]
    public Rigidbody rBSwordRight;
    public Rigidbody rBSwordLeft;
    public float forceMultiplier;

    float currentHitDistanceLeft;
    float currentHitDistanceRight;


    private void Awake()
    {
        if (anchorCenter == null)
        {
            GameObject center = GameObject.Find("CenterEyeAnchor");
            if (center != null)
            {
                anchorCenter = center;
            }
        }
        if (anchorLeft == null)
        {
            GameObject left = GameObject.Find("LeftHandAnchor");
            if (left != null)
            {
                anchorLeft = left;
            }
        }
        if (anchorRight == null)
        {
            GameObject right = GameObject.Find("RightHandAnchor");
            if (right != null)
            {
                anchorRight =right;
            }
        }               
    }

    // Use this for initialization
    void Start () {
       

    }
	
	// Update is called once per frame
	void Update () {
        RaycastHit hitLeft;
        RaycastHit hitRight;

        //Part for Right controller
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0)
        {
            Debug.LogWarning("Right hand activate");
            if (sPRight == StatePower.Sleep)
            {
                if (Physics.SphereCast(anchorRight.transform.position, sphereRadius, anchorRight.transform.forward, out hitRight, distance))
                {
                    currentHitDistanceRight = hitRight.distance;
                    if (hitRight.collider.tag == "Sword")
                    {
                        GameObject registeredCol;
                        registeredCol = hitRight.collider.gameObject;
                        CheckParent(registeredCol, true);
                        sPRight = StatePower.Attract;
                    }
                }
            }else if (sPRight == StatePower.Attract)
            {
                rBSwordRight.useGravity = false;
                Vector3 directionRight = (swordRight.transform.position - anchorRight.transform.position) * -1f;
                rBSwordRight.AddForce(directionRight * forceMultiplier);
            }
        }
        else
        {
            if (swordRight != null)
            {
                if (rBSwordRight != null)
                {
                    rBSwordRight.useGravity = true;

                    rBSwordRight = null;
                    swordRight = null;
                    sPRight = StatePower.Sleep;

                }
            }
            else
            {
                sPRight = StatePower.Sleep;
            }
        }
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > 0)
        {
            Debug.LogWarning("Left hand activate");

            if (sPLeft == StatePower.Sleep)
            {
                if (Physics.SphereCast(anchorLeft.transform.position, sphereRadius, anchorLeft.transform.forward, out hitLeft, distance))
                {
                    currentHitDistanceLeft = hitLeft.distance;
                    if (hitLeft.collider.tag == "Sword")
                    {
                        GameObject registeredCol;
                        registeredCol = hitLeft.collider.gameObject;
                        CheckParent(registeredCol, false);
                        sPLeft = StatePower.Attract;
                    }
                }
            }
            else if (sPLeft == StatePower.Attract)
            {
                rBSwordLeft.useGravity = false;
                Vector3 directionLeft = (swordLeft.transform.position - anchorLeft.transform.position );
                rBSwordLeft.AddForce(directionLeft * forceMultiplier);
            }
        }
        else
        {
            if (swordLeft != null)
            {
                if (rBSwordLeft != null)
                {
                    rBSwordLeft.useGravity = true;

                    rBSwordLeft = null;
                    swordLeft = null;
                    sPLeft = StatePower.Sleep;
                }
            }
            else
            {
                sPLeft = StatePower.Sleep;
            }
        }
    }


    void CheckParent(GameObject hitObject, bool isRight)
    {
        if (hitObject.transform.parent == null)
        {
            if (isRight)
            {
                swordRight = hitObject;
                rBSwordRight = swordRight.GetComponent<Rigidbody>();
            }
            else
            {
                swordLeft = hitObject;
                rBSwordLeft = swordLeft.GetComponent<Rigidbody>();
            }
        }
        else
        {
            if (isRight)
            {
                swordRight = hitObject.transform.parent.gameObject;
                rBSwordRight = swordRight.GetComponent<Rigidbody>();

            }
            else
            {
                swordLeft = hitObject.transform.parent.gameObject;
                rBSwordLeft = swordLeft.GetComponent<Rigidbody>();
            }
        }
    }
    private void OnDrawGizmos()
    {
        if (anchorLeft != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(anchorLeft.transform.position, anchorLeft.transform.forward * distance);
            Gizmos.DrawWireSphere(anchorLeft.transform.position + anchorLeft.transform.forward * currentHitDistanceLeft, sphereRadius);
        }

        if (anchorRight != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(anchorRight.transform.position, anchorRight.transform.forward * distance);
            Gizmos.DrawWireSphere(anchorRight.transform.position + anchorRight.transform.forward * currentHitDistanceRight, sphereRadius);
        }
    }
}
