using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_Player : MonoBehaviour
{
    public GameObject bullet;
    public Transform laser;
    void Update()
    {

        if ((Input.GetKey("d") && transform.position.x <= 15))
        {
            transform.Translate(Vector3.right * Time.deltaTime * 15, Space.World);
        }
        if ((Input.GetKey("a") && transform.position.x >= -15))
        {
            transform.Translate(Vector3.left * Time.deltaTime * 15, Space.World);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(Vector3.up * Time.deltaTime * 15, Space.World);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.down * Time.deltaTime * 15, Space.World);
        }
        if (Input.GetKeyDown("space"))
        {
            Instantiate(bullet, laser.position + new Vector3(0,2,0), laser.rotation);
        }

    }
}
