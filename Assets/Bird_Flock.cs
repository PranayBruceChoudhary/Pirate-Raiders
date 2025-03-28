using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird_Flock : MonoBehaviour
{
    [SerializeField] private Flock_Movement flockUnitPrefab;
    [SerializeField] private int flockSize;
    [SerializeField] private Vector3 spawnBounds;
    [Header("Detection Distances")]
    [Range(0, 10)]
    [SerializeField] private float _cohesionDistance;
    public float cohesionDistance { get { return _cohesionDistance; } }

    [Range(0, 10)]
    [SerializeField] private float _alignmentDistance;
    public float alignmentDistance { get { return _alignmentDistance; } }

    [Range(0, 10)]
    [SerializeField] private float _avoidanceDistance;
    public float avoidanceDistance { get { return _avoidanceDistance; } }

    [Range(0, 30)]
    [SerializeField] private float _obsticaleDistance;
    public float obsticaleDistance { get { return _obsticaleDistance; } }


    [Header("Speed Setup")]
    [Range(0, 10)]
    [SerializeField] private float _minSpeed;
    public float minSpeed { get { return _minSpeed; } }
    [Range(0, 10)]
    [SerializeField] private float _maxSpeed;
    public float maxSpeed { get { return _maxSpeed; } }
    public FlockUnit[] allUnits { get; set; }

    [Header("Behavior Weights")]
    [Range(0, 10)]
    [SerializeField] private float _cohesionWeight;
    public float cohesionWeight { get { return _cohesionWeight; } }

    [Range(0, 10)]
    [SerializeField] private float _alignmentWeight;
    public float alignmentWeight { get { return _alignmentWeight; } }

    [Range(0, 10)]
    [SerializeField] private float _avoidanceWeight;
    public float avoidanceWeight { get { return _avoidanceWeight; } }

    [Range(0, 10)]
    [SerializeField] private float _obsticaleWeight;
    public float obsticaleWeight { get { return _obsticaleWeight; } }
    public static List<Flock_Movement> allBirds;
    private Transform nextWaypoint;
    public float MovementSpeed;
    public float RotationSpeed;
    public float PitchSpeed;
    private float pitch;
    private float yaw;
    private float roll;
    // Start is called before the first frame update
    private void Start()
    {
        nextWaypoint = Waypoint.points[0];
        allBirds = new List<Flock_Movement>();
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
            allBirds.Add(Instantiate(flockUnitPrefab, spawnPosition, rotation));
            allBirds[i].assignFlock(this);
        }
    }

    void Update()
    {
        //Debug.Log("Bird flock size is " + allBirds.Count);
        pitch = PitchSpeed * Time.deltaTime;
        yaw = Time.deltaTime * RotationSpeed;
        roll = yaw;
        foreach(Flock_Movement g in allBirds)
        {
            g.MoveUnit();
        }
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
    }
}
