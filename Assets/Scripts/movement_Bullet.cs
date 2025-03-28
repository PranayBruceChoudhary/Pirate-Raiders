using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement_Bullet : MonoBehaviour
{
    [SerializeField]
    private Vector3 initialVelocity;
    [SerializeField]
    private float minVelocity = 15f;
    private Rigidbody rb;
    private Vector3 lastFrameVelocity;
    public GameObject ImpactEffect;
    private GameObject impactInstance;
    private bool isDestroyed;
    private int count = 0;
    private void OnEnable()
    {
        isDestroyed = false;
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * minVelocity;
        Debug.Log("bullet velocity is " + rb.velocity);
    }

    private void Update()
    {
        lastFrameVelocity = rb.velocity;
        //Debug.Log("bullet velocity is " + rb.velocity);
        /*
        if (transform.position.y <= 15)
        {
            Destroy(this);
            Debug.Log("bullet is destroy")
        }
        */
        /*
        Collider[] colliders = Physics.OverlapSphere(transform.position, 8);
        Debug.Log("colliders size is " + colliders.Length);
        foreach (Collider collider in colliders)
        {
            Debug.Log("collider name is " + collider.gameObject.name);
            if (collider.tag == "enemy")
            {
                impactInstance = (GameObject)Instantiate(ImpactEffect, transform.position, transform.rotation);
                Destroy(impactInstance, 2f);
                Destroy(this);
                collider.gameObject.SetActive(false);
                if (collider.gameObject.name.Equals("Bird Variant"))
                {
                    Flock_Movement flock_behaviour = collider.gameObject.GetComponentInParent(typeof(Flock_Movement)) as Flock_Movement;
                    Bird_Flock.allBirds.Remove(flock_behaviour);
                    Debug.Log("Collsion with Bird body ");
                }
            }
        }
        */
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("collided object name is " + collision.gameObject.name);
        if (collision.gameObject.name.Contains("Bird"))
        {
            if (!isDestroyed)
            {
                impactInstance = (GameObject)Instantiate(ImpactEffect, collision.gameObject.transform.position, collision.gameObject.transform.rotation);
                Destroy(impactInstance, 2f);
                Destroy(this);
                if (collision.gameObject.name.Equals("Bird Variant(Clone)"))
                {
                    Flock_Movement flock_behaviour = collision.gameObject.GetComponentInParent(typeof(Flock_Movement)) as Flock_Movement;
                    Bird_Flock.allBirds.Remove(flock_behaviour);
                    Debug.Log("Collsion with Bird body ");
                }
                collision.gameObject.SetActive(false);
                isDestroyed = true;
            }
        }
        if (collision.gameObject.name.Contains("Rocket"))
        {
            if(collision.gameObject.transform.localScale != Vector3.zero)
            {
                count++;
                Instantiate(ImpactEffect, collision.gameObject.transform.position, collision.gameObject.transform.rotation);
                if(count >= 10)
                {
                    collision.gameObject.SetActive(false);
                }
            }
        }
        else if (collision.gameObject.name.Contains("Player"))
        {
            Debug.Log("Bullet hit Player");
        }
        else
        {
            Bounce(collision.gameObject.transform.position);
        }
    }
private void Bounce(Vector3 collisionNormal)
    {
        var speed = lastFrameVelocity.magnitude;
        var direction = Vector3.Reflect(lastFrameVelocity.normalized, collisionNormal);
        rb.velocity = direction * Mathf.Max(speed, minVelocity);
        rb.angularVelocity = new Vector3(0, 0, 0);
    }
}
