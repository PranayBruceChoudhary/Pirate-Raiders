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
    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = initialVelocity;
    }

    private void Update()
    {
        lastFrameVelocity = rb.velocity;
        if(transform.position.y >= 15)
        {
            Destroy(this);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Contains("Insect"))
        {
            impactInstance = (GameObject)Instantiate(ImpactEffect, transform.position, transform.rotation);
            Destroy(impactInstance, 2f);
            collision.gameObject.SetActive(false);
            FlockUnit.score += UnityEngine.Random.Range(1,10);
            Debug.Log("Current Score is" + FlockUnit.score);
        }
        Bounce(collision.contacts[0].normal);
    }

    private void Bounce(Vector3 collisionNormal)
    {
        var speed = lastFrameVelocity.magnitude;
        var direction = Vector3.Reflect(lastFrameVelocity.normalized, collisionNormal);
        direction.z = 0;
        rb.velocity = direction * Mathf.Max(speed, minVelocity);
        rb.angularVelocity = new Vector3(0, 0, 0);
    }
}
