using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird_Flock : MonoBehaviour
{
    [SerializeField] private GameObject flockUnitPrefab;
    [SerializeField] private int flockSize;
    [SerializeField] private Vector3 spawnBounds;
    public float MovementSpeed;
    public float RotationSpeed;
    public float PitchSpeed;
    private float pitch;
    private float yaw;
    private float roll;
    // Start is called before the first frame update
    private void Start()
    {
        GenerateUnits();
    }

    // Update is called once per frame
    private void GenerateUnits()
    {
        for (int i = 0; i < flockSize; i++)
        {
            var randomVector = UnityEngine.Random.insideUnitSphere;
            randomVector = new Vector3(randomVector.x * spawnBounds.x, randomVector.y * spawnBounds.y, randomVector.z * spawnBounds.z);
            var spawnPosition = transform.position + randomVector;
            var rotation = Quaternion.Euler(0f, 0f, 0f);
            Debug.Log("Rotation is " + rotation);
            Instantiate(flockUnitPrefab, spawnPosition, rotation);
            //allUnits[i].assignFlock(this);
            //allUnits[i].InitializeSpeed((minSpeed + maxSpeed) / 2);

        }
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
            //transform.rotation = Quaternion.Euler(temp, transform.rotation.y, transform.rotation.z);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            float temp = transform.rotation.eulerAngles.z;
            transform.Rotate(0, yaw, 0, Space.World);
            transform.Rotate(0, 0, -roll, Space.Self);
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
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
}
