using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RayGrab : MonoBehaviour {


    [Header("Oculus Anchor")]
    public OVRInput.Controller controllerLeft;
    public OVRInput.Controller controllerRight;

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

    [Header("Sword Components")]
    public SwordScript sSRight;
    public SwordScript sSLeft;

    float currentHitDistanceLeft;
    float currentHitDistanceRight;


    float distanceLeft;
    float distanceRight;

    private void Awake()
    {
        //get the anchor from OVR
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

        //Part for Right controller to attract
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
                sSRight.isForced = true;
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

                    if (sPRight == StatePower.Attract)
                    {
                        rBSwordRight.useGravity = true;
                        rBSwordRight = null;
                        swordRight = null;
                        sPRight = StatePower.Sleep;
                    }
                    if (sPRight == StatePower.Equiped)
                    {
                        DropObject(swordRight, true);
                        sPRight = StatePower.Throw;
                    }
                    

                }
                if (sSRight != null)
                {
                    sSRight.isForced = false;
                    sSRight = null;
                }
            }
            else
            {

                sPRight = StatePower.Sleep;
            }
        }

        //Part for Left controller to attract
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
                sSLeft.isForced = true;
                Vector3 directionLeft = (swordLeft.transform.position - anchorLeft.transform.position ) * -1f;
                rBSwordLeft.AddForce(directionLeft * forceMultiplier);
            }
        }
        else
        {
            if (swordLeft != null)
            {


                if (rBSwordLeft != null)
                {
                    if (sPLeft == StatePower.Attract)
                    {
                        rBSwordLeft.useGravity = true;
                        rBSwordLeft = null;
                        swordLeft = null;
                        sPLeft = StatePower.Sleep;
                    }
                    if (sPLeft == StatePower.Equiped)
                    {
                        DropObject(swordLeft, false);
                        sPLeft = StatePower.Throw;
                    }
                    
                }
                if (sSLeft != null)
                {
                    sSLeft.isForced = false;
                    sSLeft = null;
                }
            }
            else
            {
                sPLeft = StatePower.Sleep;
            }
        }

        if (swordLeft != null)
        {
            distanceLeft = Vector3.Distance(anchorLeft.transform.position, swordLeft.transform.position);
        }

        if (swordRight != null)
        {
            distanceRight = Vector3.Distance(anchorRight.transform.position, swordRight.transform.position);
        }

        
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) > 0)
        {
            if (swordRight != null)
            {
                if (sPRight == StatePower.Attract)
                {
                    if (distanceRight < 0.35f)
                    {
                        swordRight.transform.SetParent(anchorRight.transform);
                        GrabObject(swordRight);
                        swordRight.transform.position = anchorRight.transform.position;
                        sPRight = StatePower.Equiped;
                    }
                }
            }
        }
        else
        {
            if (sPRight == StatePower.Equiped)
            {
                DropObject(swordRight, false);
                sPRight = StatePower.Throw;
            }
        }

        if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) > 0)
        {
            if (swordLeft != null)
            {
                if (sPLeft == StatePower.Attract)
                {
                    if (distanceLeft < 0.35f)
                    {
                        swordLeft.transform.SetParent(anchorLeft.transform);
                        GrabObject(swordLeft);
                        swordLeft.transform.position = anchorLeft.transform.position;
                        sPLeft = StatePower.Equiped;
                    }
                }
            }
        }
        else
        {
            if (sPLeft == StatePower.Equiped)
            {
                DropObject(swordLeft, true);
                sPLeft = StatePower.Throw;
            }
        }

        //when right hand throw an object, made to be sure every state is reinitialised as it should
        if (sPRight == StatePower.Throw)
        {
            if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) == 0)
            {
                if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) == 0)
                {
                    sPRight = StatePower.Sleep;
                }
            }
        }
        //when left hand throw an object, made to be sure every state is reinitialised as it should
        if (sPLeft == StatePower.Throw)
        {
            if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) == 0)
            {
                if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) == 0)
                {
                    sPRight = StatePower.Sleep;
                }
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
                sSRight = swordRight.GetComponent<SwordScript>();

            }
            else
            {
                swordLeft = hitObject;
                rBSwordLeft = swordLeft.GetComponent<Rigidbody>();
                sSLeft = swordLeft.GetComponent<SwordScript>();
            }
        }
        else
        {
            if (isRight)
            {
                swordRight = hitObject.transform.parent.gameObject;
                rBSwordRight = swordRight.GetComponent<Rigidbody>();
                sSRight = swordRight.GetComponent<SwordScript>();

            }
            else
            {
                swordLeft = hitObject.transform.parent.gameObject;
                rBSwordLeft = swordLeft.GetComponent<Rigidbody>();
                sSLeft = swordLeft.GetComponent<SwordScript>();
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

    void GrabObject(GameObject grabbed)
    {
        SwordScript sSGrabbed = grabbed.GetComponent<SwordScript>();
        sSGrabbed.isGrabbed = true;
        grabbed.transform.eulerAngles = new Vector3(-240f, 0f, 0f);

    }

    void DropObject(GameObject dropped, bool isRight)
    {

        if (dropped.transform.parent != null)
        {
            dropped.transform.parent = null;
        }
        SwordScript sSDropped = dropped.GetComponent<SwordScript>();
        sSDropped.isGrabbed = false;
        Rigidbody rbDropped = dropped.GetComponent<Rigidbody>();
        rbDropped.constraints = RigidbodyConstraints.None;

        if (isRight)
        {
            rbDropped.velocity = anchorCenter.transform.rotation * OVRInput.GetLocalControllerVelocity(controllerRight);
            rbDropped.angularVelocity = OVRInput.GetLocalControllerAngularVelocity(controllerRight);
            /*rBSwordRight = null;
            swordRight = null;
            sSRight = null;*/
        }
        else
        {
            rbDropped.velocity = anchorCenter.transform.rotation * OVRInput.GetLocalControllerVelocity(controllerLeft) * forceMultiplier;
            rbDropped.angularVelocity = OVRInput.GetLocalControllerAngularVelocity(controllerLeft) * forceMultiplier;
            /*rBSwordLeft = null;
            swordLeft = null;
            sSLeft = null;*/
        }
        // 
        



    }
}
