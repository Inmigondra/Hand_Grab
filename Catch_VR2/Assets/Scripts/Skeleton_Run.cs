using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Skeleton_Run : MonoBehaviour
{
    [Header("Component information")]
    public Transform target;
    public NavMeshAgent nA;
    public Rigidbody rb;


    public float distanceToPlayer;
    public float strenghtEject;

    Vector3 pos;
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("OVRCameraRig").transform;
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector3.Distance(target.position, transform.position);
        if (nA.enabled == true)
        {
            nA.SetDestination(target.position);

        }


        if (distanceToPlayer <= 2f)
        {
            print("lama");
            nA.SetDestination(transform.position);
        }
    }


    /*private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Sword")
        {
            nA.enabled = false;
            
            Vector3 pos = col.gameObject.transform.position;
            rb.AddExplosionForce(strenghtEject, pos, 0.5f);
            StartCoroutine("Death");
        }
    }*/
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Sword")
        {
            
           
            ContactPoint contact = col.contacts[0];
            pos = contact.point;
            
           // StartCoroutine("Death");
        }
    }



    public IEnumerator Death()
    {
        nA.enabled = false;
        gameObject.layer = 13;
        yield return new WaitForSeconds(0.1f);
        rb.isKinematic = false;
        rb.AddExplosionForce(strenghtEject, pos, 0.5f);
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
        yield return null;
    }
}
