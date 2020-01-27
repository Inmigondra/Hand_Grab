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
    public bool vibrate = false;
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
                if (Physics.SphereCast(anchorRight.transform.position, sphereRadius, anchorRight.transform.forward, out hitRight, distance))
                {
                   if (hitRight.collider.tag == "Sword")
                    {
                        if(vibrate == false)
                        {
                            StartCoroutine("VibrationRight");
                        }
                    }

                }

                //send raycast while index trigger is pressed
                if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0)
                {
                    if (Physics.SphereCast(anchorRight.transform.position, sphereRadius, anchorRight.transform.forward, out hitRight, distance))
                    {
                        currentHitDistanceRight = hitRight.distance;
                        if (hitRight.collider.tag == "Sword")
                        {
                            GameObject registerdCol;
                            registerdCol = hitRight.collider.gameObject;
                            CheckParent(registerdCol, true);
                            sPRight = StatePower.Attract;
                            sVR = StrenghtVibrate.Medium;
                            if (vibrate == false)
                            {
                                StartCoroutine("VibrationRight");
                            }

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
                            GrabObject(swordRight);
                            swordRight.transform.position = anchorRight.transform.position;
                            sPRight = StatePower.Equiped;
                            sVR = StrenghtVibrate.Strong;
                            if (vibrate == false)
                            {
                                StartCoroutine("VibrationRight");
                            }
                        }
                    }
                }
                //when a sword is attracted and the index trigger is up, we go to sleep
                else
                {
                    vibrate = false;
                    sPRight = StatePower.Sleep;
                }
                break;
            case StatePower.Equiped:
                if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) == 0)
                {
                    DropObject(swordRight, true);
                    sPRight = StatePower.Throw;
                    vibrate = false;
                }
                if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) == 0)
                {
                    DropObject(swordRight, true);
                    sPRight = StatePower.Throw;
                    vibrate = false;
                }
                break;
            case StatePower.Throw:
                if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) == 0 && OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) == 0)
                {
                    sPRight = StatePower.Sleep;
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
            rbDropped.velocity = anchorCenter.transform.rotation * OVRInput.GetLocalControllerVelocity(controllerLeft) * forceMultiplier;
            rbDropped.angularVelocity = OVRInput.GetLocalControllerAngularVelocity(controllerLeft) * forceMultiplier;
            /*rBSwordLeft = null;
            swordLeft = null;
            sSLeft = null;*/
        }
        // 




    }

    public IEnumerator VibrationRight()
    {
        vibrate = true;
        switch (sVR)
        {            
            case (StrenghtVibrate.Weak):
                Debug.Log("0");
                for (float i = 0; i <= 0.5f; i += Time.deltaTime)
                {
                    Debug.Log("01");

                    OVRInput.SetControllerVibration(0.15f, 0.15f, OVRInput.Controller.RTouch);
                }
                break;
            case (StrenghtVibrate.Medium):
                for (float i = 0; i <= 0.5f; i += Time.deltaTime)
                {
                    OVRInput.SetControllerVibration(0.5f, 0.5f, OVRInput.Controller.RTouch);
                }
                break;
            case (StrenghtVibrate.Strong):
                for (float i = 0; i <= 0.5f; i += Time.deltaTime)
                {
                    OVRInput.SetControllerVibration(1, 1, OVRInput.Controller.RTouch);
                }
                break;
        }
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        yield return null;
    }
}
