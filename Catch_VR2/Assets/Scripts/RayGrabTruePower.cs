using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum StatePower
{
    Sleep,//no action is enregistered, moment to send a raycast
    Attract,//a weapon has been hit by the raycast, in this state, attract the weapon toward the player
    Equiped,//during attraction, if sword is near enough, you can grab it
    Throw//the moment you release a trigger. until you release both, you can't attract
}
public enum StrenghtVibrate
{
    Weak,
    Medium,
    Strong
}
public class RayGrabTruePower : MonoBehaviour
{
    [Header("Occulus Anchor")]
    public OVRInput.Controller controllerLeft;
    public OVRInput.Controller controllerRight;
    //will be used to store the prefab of each anchor of the ovr camera
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

    //informations on distance on raycast point hit
    float currentHitDistanceLeft;
    float currentHitDistanceRight;

    // informations on distance between hands and swords
    float distanceLeft;
    float distanceRight;


    public StrenghtVibrate sVR;
    public StrenghtVibrate sVL;
    public bool vibrateRight = false;
    public bool vibrateLeft = false;


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
                anchorRight = right;
            }
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Initialization for spherecast
        RaycastHit hitLeft;
        RaycastHit hitRight;

        //Switch state of right trigger 
        switch (sPRight)
        {
            case StatePower.Sleep:
                if (Physics.SphereCast(anchorRight.transform.position, sphereRadius, anchorRight.transform.forward, out hitRight, distance, 1 << LayerMask.NameToLayer("Sword") ))
                {
                    if (hitRight.collider.tag == "Sword")
                    {
                        if (vibrateRight == false)
                        {
                            StartCoroutine("VibrationRight");
                        }
                    }
                   
                }

                //send raycast while index trigger is pressed
                if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0)
                {
                    if (Physics.SphereCast(anchorRight.transform.position, sphereRadius, anchorRight.transform.forward, out hitRight, distance, 1 << LayerMask.NameToLayer("Sword")))
                    {
                        currentHitDistanceRight = hitRight.distance;
                        if (hitRight.collider.tag == "Sword")
                        {
                            GameObject registerdCol;
                            registerdCol = hitRight.collider.gameObject;
                            CheckParent(registerdCol, true);
                            sVR = StrenghtVibrate.Medium;
                            StartCoroutine("VibrationRight");
                            sPRight = StatePower.Attract;                            
                        }
                    }
                }
                else
                {
                    if (sSRight != null)
                    {
                        sSRight.isForced = false;
                        sSRight = null;
                    }
                    if (swordRight != null)
                    {
                        rBSwordRight.useGravity = true;
                        rBSwordRight = null;
                        swordRight = null;
                    }
                }
                break;
            case StatePower.Attract:
                //if the trigger is pressed, continue to attract the object
                if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0)
                {
                    rBSwordRight.useGravity = false;
                    sSRight.isForced = true;
                    Vector3 directionRight = (swordRight.transform.position - anchorRight.transform.position) * -1f;
                    rBSwordRight.AddForce(directionRight * forceMultiplier);

                    //when the other trigger is pulled
                    if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) > 0)
                    {
                        if (distanceRight < 0.35f)
                        {
                            swordRight.transform.SetParent(anchorRight.transform);
                            swordRight.transform.position = anchorRight.transform.position;

                            GrabObject(swordRight);
                            sVR = StrenghtVibrate.Strong;
                            StartCoroutine("VibrationRight");
                            sPRight = StatePower.Equiped;
                        }
                    }
                }
                //when a sword is attracted and the index trigger is up, we go to sleep
                else
                {
                    sVR = StrenghtVibrate.Weak;
                    sPRight = StatePower.Sleep;
                }
                break;
            case StatePower.Equiped:
                if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) == 0)
                {
                    DropObject(swordRight, true);
                    sPRight = StatePower.Throw;
                }
                if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) == 0)
                {
                    DropObject(swordRight, true);
                    sPRight = StatePower.Throw;
                }
                break;
            case StatePower.Throw:
                if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) == 0 && OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) == 0)
                {
                    sVR = StrenghtVibrate.Weak;
                    sPRight = StatePower.Sleep;
                }
                break;
        }

        //Switch state of left trigger 
        switch (sPLeft)
        {
            case StatePower.Sleep:
                if (Physics.SphereCast(anchorLeft.transform.position, sphereRadius, anchorLeft.transform.forward, out hitLeft, distance, 1 << LayerMask.NameToLayer("Sword")))
                {
                    if (hitLeft.collider.tag == "Sword")
                    {
                        if (vibrateLeft == false)
                        {
                            StartCoroutine("VibrationLeft");
                        }
                    }
                }

                if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > 0)
                {
                    if (Physics.SphereCast(anchorLeft.transform.position, sphereRadius, anchorLeft.transform.forward, out hitLeft, distance, 1 << LayerMask.NameToLayer("Sword")))
                    {
                        currentHitDistanceLeft = hitLeft.distance;
                        if (hitLeft.collider.tag == "Sword")
                        {
                            GameObject registeredCol;
                            registeredCol = hitLeft.collider.gameObject;
                            CheckParent(registeredCol, false);
                            sVL = StrenghtVibrate.Medium;
                            StartCoroutine("VibrationLeft");
                            sPLeft = StatePower.Attract;
                        }
                    }
                }
                else
                {
                    if (sSLeft != null)
                    {
                        sSLeft.isForced = false;
                        sSLeft = null;
                    }
                    if (swordLeft != null)
                    {
                        rBSwordLeft.useGravity = true;
                        rBSwordLeft = null;
                        swordLeft = null;
                    }
                }
                break;
            case StatePower.Attract:
                //while the trigger is pressed, continue attraction
                if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > 0)
                {
                    rBSwordLeft.useGravity = false;
                    sSLeft.isForced = true;
                    Vector3 directionLeft = (swordLeft.transform.position - anchorLeft.transform.position) * -1f;
                    rBSwordLeft.AddForce(directionLeft * forceMultiplier);
                    //When the other trigger is pulled
                    if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) > 0)
                    {
                        if (distanceLeft < 0.35f)
                        {
                            swordLeft.transform.SetParent(anchorLeft.transform);
                            swordLeft.transform.position = anchorLeft.transform.position;
                            GrabObject(swordLeft);
                            sVL = StrenghtVibrate.Strong;
                            StartCoroutine("VibrationLeft");
                            sPLeft = StatePower.Equiped;
                        }
                    }
                }
                else
                {
                    sVL = StrenghtVibrate.Weak;
                    sPLeft = StatePower.Sleep;
                }
                break;
            case StatePower.Equiped:
                if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) == 0)
                {
                    DropObject(swordLeft, false);
                    sPLeft = StatePower.Throw;
                }
                if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) == 0)
                {
                    DropObject(swordLeft, false);
                    sPLeft = StatePower.Throw;
                }
                break;
            case StatePower.Throw:
                if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) == 0 && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) == 0)
                {
                    sVL = StrenghtVibrate.Weak;
                    sPLeft = StatePower.Sleep;
                }
                break;
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
                sSRight.target = anchorRight.transform;

            }
            else
            {
                swordLeft = hitObject;
                rBSwordLeft = swordLeft.GetComponent<Rigidbody>();
                sSLeft = swordLeft.GetComponent<SwordScript>();
                sSLeft.target = anchorLeft.transform;
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
            if (sSRight != null)
            {
                sSRight.isForced = false;
                sSRight = null;
            }
            if (swordRight != null)
            {
                rBSwordRight.useGravity = true;
                rBSwordRight = null;
                swordRight = null;
            }

            rbDropped.velocity = OVRInput.GetLocalControllerVelocity(controllerRight) * 2 ;

            //rbDropped.velocity = anchorCenter.transform.rotation * OVRInput.GetLocalControllerVelocity(controllerRight);
            rbDropped.angularVelocity = OVRInput.GetLocalControllerAngularVelocity(controllerRight) * 2;
            /*rBSwordRight = null;
            swordRight = null;
            sSRight = null;*/
        }
        else
        {
            if (sSLeft != null)
            {
                sSLeft.isForced = false;
                sSLeft = null;
            }
            if (swordLeft != null)
            {
                rBSwordLeft.useGravity = true;
                rBSwordLeft = null;
                swordLeft = null;
            }
            rbDropped.velocity =  OVRInput.GetLocalControllerVelocity(controllerLeft) * 2;
            rbDropped.angularVelocity = OVRInput.GetLocalControllerAngularVelocity(controllerLeft) * 2;
            /*rBSwordLeft = null;
            swordLeft = null;
            sSLeft = null;*/
        }
        // 




    }

    public IEnumerator VibrationRight()
    {
        vibrateRight = true;
        switch (sVR)
        {            
            case (StrenghtVibrate.Weak):
                    OVRInput.SetControllerVibration(0.1f, 0.1f, OVRInput.Controller.RTouch);

                break;
            case (StrenghtVibrate.Medium):
                    OVRInput.SetControllerVibration(0.25f, 0.25f, OVRInput.Controller.RTouch);

                break;
            case (StrenghtVibrate.Strong):
                    OVRInput.SetControllerVibration(1, 1, OVRInput.Controller.RTouch);

                break;
        }
        yield return new WaitForSeconds(0.25f);
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        yield return new WaitForSeconds(0.5f);
        vibrateRight = false;
        yield return null;
    }

    public IEnumerator VibrationLeft()
    {
        vibrateLeft = true;
        switch (sVL)
        {
            case (StrenghtVibrate.Weak):
                OVRInput.SetControllerVibration(0.1f, 0.1f, OVRInput.Controller.LTouch);
                break;
            case (StrenghtVibrate.Medium):
                OVRInput.SetControllerVibration(0.25f, 0.25f, OVRInput.Controller.LTouch);
                break;
            case (StrenghtVibrate.Strong):
                OVRInput.SetControllerVibration(1, 1, OVRInput.Controller.LTouch);
                break;
        }
        yield return new WaitForSeconds(0.25f);
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
        yield return new WaitForSeconds(0.5f);
        vibrateLeft = false;
        yield return null;
    }
}
