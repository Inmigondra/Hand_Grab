using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum StatePower
{
    Sleep,
    Attract,
    Equiped
}
public class RayGrab : MonoBehaviour {

    public GameObject anchorRight;
    public GameObject anchorLeft;
    public GameObject anchorCenter;

    public float distance;
    public float sphereRadius;

    StatePower sPRight;
    StatePower sPLeft;

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


        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0)
        {
            if (Physics.SphereCast(anchorLeft.transform.position, sphereRadius ,anchorLeft.transform.forward, out hitLeft, distance))
            {
                currentHitDistanceLeft = hitLeft.distance;
            }
        }
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > 0)
        {
            if (Physics.SphereCast(anchorRight.transform.position, sphereRadius, anchorRight.transform.forward, out hitRight, distance))
            {
                currentHitDistanceRight = hitRight.distance;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (anchorLeft != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(anchorLeft.transform.position, transform.forward * distance);
            Gizmos.DrawWireSphere(anchorLeft.transform.position + transform.forward * currentHitDistanceLeft, sphereRadius);
        }

        if (anchorRight != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(anchorRight.transform.position, transform.forward * distance);
            Gizmos.DrawWireSphere(anchorRight.transform.position + transform.forward * currentHitDistanceRight, sphereRadius);
        }
    }
}
