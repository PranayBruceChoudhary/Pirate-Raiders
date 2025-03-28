using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock_Movement : MonoBehaviour
{
    public float MovementSpeed;
    public int speed = 8;
    public float RotationSpeed;
    public float PitchSpeed;
    private float pitch;
    private float yaw;
    private bool fullTurn;
    private float roll;
    [SerializeField] private float FOVAngle;
    [SerializeField] private float smoothDamp;
    [SerializeField] private LayerMask obsticaleMask;
    [SerializeField] private Vector3[] directionsToCheckWhenAvoidingObsticales;
    private Transform nextWaypoint;
    private List<Flock_Movement> cohesionNeighbors = new List<Flock_Movement>();
    private List<Flock_Movement> avoidanceNeighbors = new List<Flock_Movement>();
    private List<Flock_Movement> alignmentNeighbors = new List<Flock_Movement>();
    private Bird_Flock assignedFlock;
    private int wavepointIndex = 0;
    private bool reverse;
    private Vector3 currentVelocity;
    private Vector3 currentObsticaleAvoidanceVector;

    // Start is called before the first frame update
    private void Start()
    {
        pitch = transform.rotation.z;
        nextWaypoint = Waypoint.points[0];
    }
    public void MoveUnit()
    {
        if (this.gameObject != null)
        {
            FindNeighbors();
            CalculateSpeed();
            var cohesionVector = CalculateCohesionVector() * assignedFlock.cohesionWeight;
            var avoidanceVector = CalculateAvoidanceVector() * assignedFlock.avoidanceWeight;
            var alignmentVector = CalculateAlignmentVector() * assignedFlock.alignmentWeight;
            var obsticaleVector = CalculateObsticaleVector() * assignedFlock.obsticaleWeight;
            var moveVector = (cohesionVector * 2) + (alignmentVector * 4) + (avoidanceVector * 10) + (followWaypoint() * 50) + (obsticaleVector * 50);
            moveVector = Vector3.SmoothDamp(transform.forward, moveVector, ref currentVelocity, smoothDamp);
            moveVector = moveVector.normalized * speed;
            transform.LookAt(moveVector);
            /*
            float yawAngle = Vector3.SignedAngle(transform.forward, moveVector, transform.right);
            if(yawAngle > 0)
            {
                YawAngleRight(yawAngle);
            }
            else
            {
                YawAngleLeft(yawAngle);
            }
            */
            if (moveVector == Vector3.zero)
            {
                moveVector = transform.forward;
            }
            if (Vector3.Distance(transform.position, nextWaypoint.position) <= 4)
            {
                GetNextWaypoint();
            }
            transform.forward = moveVector;
            transform.position += moveVector * Time.deltaTime;
        }
    }
    public void assignFlock(Bird_Flock flock)
    {
        assignedFlock = flock;
    }
    private void CalculateSpeed()
    {
        if (cohesionNeighbors.Count == 0)
        {
            return;
        }
        speed = 0;
        for (int i = 0; i < cohesionNeighbors.Count; i++)
        {
            speed += 8;
        }
        speed /= cohesionNeighbors.Count;
        if (UnityEngine.Random.Range(0, 100) >= 5)
        {
            speed = Mathf.Clamp(speed, 8, 10);
        }
        else
        {
            speed = UnityEngine.Random.Range(speed, 40);
        }
    }
    private void FindNeighbors()
    {
        cohesionNeighbors.Clear();
        avoidanceNeighbors.Clear();
        alignmentNeighbors.Clear();
        var allUnits = Bird_Flock.allBirds;
        for (int i = 0; i < allUnits.Count; i++)
        {
            var currentUnits = allUnits[i];
            if (currentUnits != this)
            {
                float currentNeighborDistanceSqr = Vector3.SqrMagnitude(currentUnits.transform.position - transform.position);
                if (currentNeighborDistanceSqr <= assignedFlock.cohesionDistance * assignedFlock.cohesionDistance)
                {
                    cohesionNeighbors.Add(currentUnits);
                }
                if (currentNeighborDistanceSqr <= assignedFlock.avoidanceDistance * assignedFlock.avoidanceDistance)
                {
                    avoidanceNeighbors.Add(currentUnits);
                }
                if (currentNeighborDistanceSqr <= assignedFlock.alignmentDistance * assignedFlock.alignmentDistance)
                {
                    alignmentNeighbors.Add(currentUnits);
                }
            }
        }
    }
    //float SignedAngleBetween(Vector3 a, Vector3 b, Vector3 n)
    //{
    //    float angle = Vector3.Angle(a, b);
    //    float sign = Mathf.Sign(Vector3.Dot(n, Vector3.Cross(a, b)));
    //    float signed_angle = angle * sign;
    //    return signed_angle;
    //}

    private Vector3 CalculateCohesionVector()
    {
        var cohesionVector = Vector3.zero;
        int neighborsInFOV = 0;
        if (cohesionNeighbors.Count == 0)
        {
            return cohesionVector;
        }
        for (int i = 0; i < cohesionNeighbors.Count; i++)
        {
            if (IsInFOV(cohesionNeighbors[i].transform.position))
            {
                neighborsInFOV++;
                cohesionVector += cohesionNeighbors[i].transform.position;
            }
            cohesionVector /= neighborsInFOV;
            cohesionVector -= transform.position;
            cohesionVector = cohesionVector.normalized;
        }
        return cohesionVector;
    }
    public Vector3 CalculateAlignmentVector()
    {
        var alignmentVector = transform.forward;
        if (alignmentNeighbors.Count == 0)
        {
            return transform.forward;
        }
        int neighborsInFOV = 0;
        for (int i = 0; i < alignmentNeighbors.Count; i++)
        {
            if (IsInFOV(alignmentNeighbors[i].transform.position))
            {
                neighborsInFOV++;
                alignmentVector += alignmentNeighbors[i].transform.forward;
            }
        }
        alignmentVector /= neighborsInFOV;
        alignmentVector = alignmentVector.normalized;
        return alignmentVector;
    }

    public Vector3 CalculateAvoidanceVector()
    {
        int avoidanceFactor = 1;
        var avoidanceVector = Vector3.zero;
        if (alignmentNeighbors.Count == 0)
        {
            return Vector3.zero;
        }
        int neighborsInFOV = 0;
        for (int i = 0; i < avoidanceNeighbors.Count; i++)
        {
            if (IsInFOV(avoidanceNeighbors[i].transform.position))
            {
                neighborsInFOV++;
                avoidanceVector += (transform.position - avoidanceNeighbors[i].transform.position);
            }
        }
        avoidanceVector /= neighborsInFOV * avoidanceFactor;
        avoidanceVector = avoidanceVector.normalized;
        return avoidanceVector;
    }

    public Vector3 CalculateObsticaleVector()
    {
        var obstacleVector = Vector3.zero;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, assignedFlock.obsticaleDistance, obsticaleMask))
        {
            obstacleVector = FindBestDirectionToAvoidObsticale();
        }
        else
        {
            currentObsticaleAvoidanceVector = Vector3.zero;
        }
        return obstacleVector;
    }

    public Vector3 FindBestDirectionToAvoidObsticale()
    {
        if (currentObsticaleAvoidanceVector != Vector3.zero)
        {
            RaycastHit hit;
            if (!Physics.Raycast(transform.position, transform.forward, out hit, assignedFlock.obsticaleDistance, obsticaleMask))
            {
                return currentObsticaleAvoidanceVector;
            }
        }
        float maxDistance = int.MinValue;
        var selectedDirection = Vector3.zero;
        for (int i = 0; i < directionsToCheckWhenAvoidingObsticales.Length; i++)
        {
            RaycastHit hit;
            var currentDirection = transform.TransformDirection(directionsToCheckWhenAvoidingObsticales[i].normalized);
            if (Physics.Raycast(transform.position, currentDirection, out hit, assignedFlock.obsticaleDistance, obsticaleMask))

            {
                float currentDistance = (hit.point - transform.position).sqrMagnitude;
                if (currentDistance > maxDistance)
                {
                    maxDistance = currentDistance;
                    selectedDirection = currentDirection;
                }
            }
            else
            {
                selectedDirection = currentDirection;
                currentObsticaleAvoidanceVector = currentDirection.normalized;
                return selectedDirection.normalized;
            }
        }
        return selectedDirection.normalized;
    }

    private bool IsInFOV(Vector3 position)
    {
        return Vector3.Angle(transform.forward, position - transform.position) <= FOVAngle;
    }

    public Vector3 followWaypoint()
    {
        Vector3 dir = nextWaypoint.position - transform.position;
        transform.LookAt(nextWaypoint);
        return dir;
    }

    void GetNextWaypoint()
    {
        if (wavepointIndex >= Waypoint.points.Length - 1)
        {
            reverse = true;
        }
        if (wavepointIndex <= 0)
        {
            reverse = false;
        }
        if (reverse)
        {
            wavepointIndex--;
            
        }
        else
        {
            wavepointIndex++;
        }
        nextWaypoint = Waypoint.points[wavepointIndex];

    }
    // Update is called once per frame
    void Update()
    {

    }

    public void updateMovement()
    {
        float movement = MovementSpeed * Time.deltaTime;
        pitch = PitchSpeed * Time.deltaTime;
        yaw = Time.deltaTime * RotationSpeed;
        roll = yaw;
        transform.Translate(0, 0, movement);
        if (Input.GetKey(KeyCode.UpArrow))
        {
            PitchUp();
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            PitchDown();
        }
        if (Input.GetKey("w"))
        {
            MovementSpeed = speed;
        }
        if (Input.GetKey("s"))
        {
            MovementSpeed = 0;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            YawLeft();
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            YawRight();
        }
    }

    public void YawLeft()
    {
        transform.Rotate(0, -yaw, 0, Space.World);
        transform.Rotate(0, 0, roll, Space.Self);
    }

    public void YawRight()
    {
        transform.Rotate(0, yaw, 0, Space.World);
        transform.Rotate(0, 0, -roll, Space.Self);
    }

    public void YawAngleLeft(float angle)
    {
        float start = transform.position.y;
        transform.Rotate(0, -yaw, 0, Space.World);
        transform.Rotate(0, 0, roll, Space.Self);
    }

    public void YawAngleRight(float angle)
    {
        transform.Rotate(0, yaw, 0, Space.World);
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
}
