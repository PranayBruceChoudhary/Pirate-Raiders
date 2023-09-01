using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    public float MovementSpeed;
    public float RotationSpeed;
    public float PitchSpeed;
    private float pitch;
    private float yaw;
    private bool fullTurn;
    private float roll;
    private void Start()
    {
        pitch = transform.rotation.z;
    }
    void Update()
    {
        pitch = PitchSpeed * Time.deltaTime;
        yaw = Time.deltaTime * RotationSpeed;
        roll = yaw;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Rotate(pitch, 0, 0);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Rotate(-pitch, 0, 0);
        }

        //float rotate = Input.GetAxis("Horizontal");
        //float rotation = rotate * RotationSpeed * Time.deltaTime;
        if (Input.GetKey("w"))
        {
            float movement = MovementSpeed * Time.deltaTime;
            transform.Translate(0, 0, movement);
        }
        if (Input.GetKey("s"))
        {
            float movement = MovementSpeed * Time.deltaTime;
            transform.Translate(0, 0, -movement);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            float temp = transform.rotation.eulerAngles.z;
            transform.Rotate(0, -yaw, 0, Space.World);
            transform.Rotate(0, 0, roll, Space.Self);
            if (Mathf.Abs(transform.rotation.eulerAngles.z - temp) >= 90)
            {
                fullTurn = true;
            }
            //transform.rotation = Quaternion.Euler(temp, transform.rotation.y, transform.rotation.z);
        }
        else if (Input.GetKey(KeyCode.RightArrow)) {
            float temp = transform.rotation.eulerAngles.z;
            transform.Rotate(0, yaw, 0, Space.World);
            transform.Rotate(0, 0, -roll, Space.Self);
            if (Mathf.Abs(transform.rotation.eulerAngles.z - temp) >= 90)
            {
                fullTurn = true;
            }
        }
        else if(Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
        {
            /*
            if (fullTurn)
            {
                int frameCount = 0;
                while(frameCount < 1000)
                {
                    transform.Rotate(0, 0, 0.09f);
                    frameCount++;
                }
                fullTurn = false;
                Debug.Log("made it");
            }
            //transform.Rotate(0, 0, 90);
            */
        }
        //transform.Rotate(rotate * Time.deltaTime * RotationSpeed, 0, 0);
        //transform.Rotate(0, -rotate * Time.deltaTime * RotationSpeed, 0);
        //float movement = Input.GetAxis("Vertical") * MovementSpeed * Time.deltaTime;
        //transform.Translate(0, movement, 0);
    }

    void Pitch(Transform transform, float delta)
    {
        float pitchSpeed = 2;
        transform.Rotate(0, 0, pitchSpeed * delta);
    }
}
