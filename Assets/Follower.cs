using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public GameObject leader;
    public Vector3 followBounds;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //float x_distance = leader.transform.position.x - transform.position.x;
       // float y_distance = leader.transform.position.y - transform.position.y;
        //float z_distance = leader.transform.position.z - transform.position.z;
        transform.position = Vector3.MoveTowards(transform.position, leader.transform.position - followBounds, 8 * Time.deltaTime);
        //transform.position = new Vector3(leader.transform.position.x - UnityEngine.Random.Range(0f, followBounds.x), leader.transform.position.y - UnityEngine.Random.Range(0f, followBounds.y), leader.transform.position.z - UnityEngine.Random.Range(0, followBounds.z));
        transform.rotation = leader.transform.rotation;
    }
}
