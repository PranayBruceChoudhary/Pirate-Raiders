using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockUnit : MonoBehaviour
{
    public static int score = 0;
    private float x;
    private float y;
    private float z;
    private float x1;
    private float y1;
    private float z1;
    [SerializeField] private float FOVAngle;
    [SerializeField] private float smoothDamp;
    [SerializeField] private LayerMask obsticaleMask;
    [SerializeField] private Vector3[] directionsToCheckWhenAvoidingObsticales;
    private Transform nextWaypoint;
    private List<FlockUnit> cohesionNeighbors = new List<FlockUnit>();
    private List<FlockUnit> avoidanceNeighbors = new List<FlockUnit>();
    private List<FlockUnit> alignmentNeighbors = new List<FlockUnit>();
    private flock assignedFlock;
    private int wavepointIndex = 0;
    private bool reverse;
    private Vector3 currentVelocity;
    private Vector3 currentObsticaleAvoidanceVector;
    private float speed;
    private GameObject depot1;
    private GameObject depot2;

    public Transform myTransform { get; set; }

    private void Awake()
    {
        myTransform = transform;
        Debug.Log("First waypoint is " + Waypoint.points[0]);
        nextWaypoint = Waypoint.points[0];
        depot1 = GameObject.Find("Depot 1");
        depot2 = GameObject.Find("Depot 2");
}
    public void assignFlock(flock flock)
    {
        assignedFlock = flock;
    }

    public void InitializeSpeed(float speed)
    {
        this.speed = speed;
        Debug.Log("inputted speed is" + speed);
    }
    public void MoveUnit() { 
        if(this.gameObject != null) { 
        FindNeighbors();
        CalculateSpeed();
        var cohesionVector = CalculateCohesionVector() * assignedFlock.cohesionWeight;
        var avoidanceVector = CalculateAvoidanceVector() * assignedFlock.avoidanceWeight;
        var alignmentVector = CalculateAlignmentVector() * assignedFlock.alignmentWeight;
        var obsticaleVector = CalculateObsticaleVector() * assignedFlock.obsticaleWeight;
        var moveVector = (cohesionVector * 2) + (alignmentVector * 4) + (avoidanceVector * 4) + (followWaypoint() * 10) + (obsticaleVector * 50);
        moveVector = Vector3.SmoothDamp(myTransform.forward, moveVector, ref currentVelocity, smoothDamp);
        moveVector = moveVector.normalized * speed;
        moveVector.z = 0;
        if (moveVector == Vector3.zero)
        {
            moveVector = transform.forward;
        }
        if (Vector3.Distance(transform.position, nextWaypoint.position) <= 1)
        {
            GetNextWaypoint();
        }
        myTransform.forward = (moveVector / 100);
        myTransform.position += moveVector * Time.deltaTime;
        }
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
            speed = cohesionNeighbors[i].speed;
        }
        speed /= cohesionNeighbors.Count;
        if (UnityEngine.Random.Range(0, 100) >= 5) { 
            speed = Mathf.Clamp(speed, assignedFlock.minSpeed, assignedFlock.maxSpeed);
        }
        else
        {
            speed = UnityEngine.Random.Range(speed, 50);
        }
    }
    private void FindNeighbors()
    {
        cohesionNeighbors.Clear();
        avoidanceNeighbors.Clear();
        alignmentNeighbors.Clear();
        var allUnits = assignedFlock.allUnits;
        Console.WriteLine(allUnits.Length);
        for (int i = 0; i < allUnits.Length; i++)
        {
            var currentUnits = allUnits[i];
            if (currentUnits != this)
            {
                float currentNeighborDistanceSqr = Vector3.SqrMagnitude(currentUnits.myTransform.position - myTransform.position);
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
            if (IsInFOV(cohesionNeighbors[i].myTransform.position))
            {
                neighborsInFOV++;
                cohesionVector += cohesionNeighbors[i].myTransform.position;
            }
            cohesionVector /= neighborsInFOV;
            cohesionVector -= myTransform.position;
            cohesionVector = cohesionVector.normalized;
        }
        return cohesionVector;
    }
    public Vector3 CalculateAlignmentVector()
    {
        var alignmentVector = myTransform.forward;
        if (alignmentNeighbors.Count == 0)
        {
            return myTransform.forward;
        }
        int neighborsInFOV = 0;
        for (int i = 0; i < alignmentNeighbors.Count; i++)
        {
            if (IsInFOV(alignmentNeighbors[i].myTransform.position))
            {
                neighborsInFOV++;
                alignmentVector += alignmentNeighbors[i].myTransform.forward;
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
            if (IsInFOV(avoidanceNeighbors[i].myTransform.position))
            {
                neighborsInFOV++;
                avoidanceVector += (myTransform.position - avoidanceNeighbors[i].myTransform.position);
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
        if (Physics.Raycast(myTransform.position, myTransform.forward, out hit, assignedFlock.obsticaleDistance, obsticaleMask))
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
            if (!Physics.Raycast(myTransform.position, myTransform.forward, out hit, assignedFlock.obsticaleDistance, obsticaleMask))
            {
                return currentObsticaleAvoidanceVector;
            }
        }
        float maxDistance = int.MinValue;
        var selectedDirection = Vector3.zero;
        for (int i = 0; i < directionsToCheckWhenAvoidingObsticales.Length; i++)
        {
            RaycastHit hit;
            var currentDirection = myTransform.TransformDirection(directionsToCheckWhenAvoidingObsticales[i].normalized);
            if (Physics.Raycast(myTransform.position, currentDirection, out hit, assignedFlock.obsticaleDistance, obsticaleMask))
            {
                float currentDistance = (hit.point - myTransform.position).sqrMagnitude;
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
        return Vector3.Angle(myTransform.forward, position - myTransform.position) <= FOVAngle;
    }

    public Vector3 followWaypoint()
    {
        Vector3 dir = nextWaypoint.position - transform.position;
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
            x = depot1.transform.localScale.x;
            y = depot1.transform.localScale.y;
            z = depot1.transform.localScale.z;
            if (x > 0 && this.isActiveAndEnabled)
            {
                x = x - 0.1f;
                y = y - 0.1f;
                z = z - 0.1f;
                depot1.transform.localScale = new Vector3(x, y, z);
            }
        }
        else
        {
            wavepointIndex++;
            x1 = depot2.transform.localScale.x;
            y1 = depot2.transform.localScale.y;
            z1 = depot2.transform.localScale.z;
            if (x1 > 0 && this.isActiveAndEnabled)
            {
                x1 = x1 - 0.1f;
                y1 = y1 - 0.1f;
                z1 = z1 - 0.1f;
                depot2.transform.localScale = new Vector3(x1, y1, z1);
            }
        }
        nextWaypoint = Waypoint.points[wavepointIndex];
        if (this.isActiveAndEnabled == true)
        {
            score -= UnityEngine.Random.Range(1,10);
            Debug.Log("Current Score is" + score);
        }
    }
}
