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
    private List<GameObject> activeBullets;
    public GameObject bullet;
    public Transform laser;
    private bool isStopped;
    Vector2 move;
    private void Awake()
    {
        pitch = transform.rotation.z;
        activeBullets = new List<GameObject>();
    }
    void Update()
    {
        transform.Translate(new Vector3(0,0, MovementSpeed) * Time.deltaTime);
        pitch = PitchSpeed * Time.deltaTime;
        yaw = Time.deltaTime * RotationSpeed;
        roll = yaw;
        float movement = Input.GetAxis("JoystickVertical");
        if(movement > 0.5f)
        {
          PitchUp();
        }
        else if(movement < -0.3f)
        {
           PitchDown();
        }
        float rotation = Input.GetAxis("RightStickMovement");
        if (rotation > 0.01f)
        {
            YawRight();
        }
        if(rotation < -0.01f)
        {
            YawLeft();
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            PitchUp();
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            PitchDown();
        }
        if (Input.GetButtonDown("Stop"))
        {
            if (!isStopped)
            {
                MovementSpeed = 0;
                isStopped = true;
            }
            else
            {
                MovementSpeed = 12;
                isStopped = false;  
            }
        }
         if (Input.GetKey(KeyCode.LeftArrow))
         {
            YawRight();
         }
         if (Input.GetKey(KeyCode.Return) || Input.GetButtonDown("SpeedUp"))
        {
            MovementSpeed = 35;
        }
        if (Input.GetKeyUp(KeyCode.Return)|| Input.GetButtonUp("SpeedUp"))
        {
            MovementSpeed = 12;
        }
        if (Input.GetKey(KeyCode.RightArrow)) {
            YawLeft();
        }
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire4"))
        {
            Shoot();
        }
        if(Input.GetKeyUp(KeyCode.RightShift))
        {
            if (!isStopped)
            {
                MovementSpeed = 0;
                isStopped = true;
            }
            else
            {
                MovementSpeed = 12;
                isStopped = false;
            }
        }
    }
    public void YawLeft()
    {
        transform.Rotate(0, yaw, 0, Space.World);
        transform.Rotate(0, 0, roll, Space.Self);
    }
    public void YawRight()
    {
        transform.Rotate(0, -yaw, 0, Space.World);
        transform.Rotate(0, 0, -roll, Space.Self);
    }
    public void PitchUp()
    {
        transform.Rotate(pitch, 0, 0);
    }
    public void PitchDown()
    {
        transform.Rotate(-pitch, 0, 0);
    }

    public void Shoot()
    {
        activeBullets.Add(Instantiate(bullet, laser.position, laser.rotation));
    }
   

    public void moveForward()
    {
            float movement = MovementSpeed * Time.deltaTime;
            transform.Translate(0, 0, movement);
    }
    void Pitch(Transform transform, float delta)
    {
        float pitchSpeed = 2;
        transform.Rotate(0, 0, pitchSpeed * delta);
    }
}
