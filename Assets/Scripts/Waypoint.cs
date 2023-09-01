using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public static Transform[] points;

    void Awake()
    {
        points = new Transform[transform.childCount];
        for (int i = 0; i < points.Length; i++)
        {
            Debug.Log("transform is " + transform.GetChild(i).name);
            points[i] = transform.GetChild(i);
        }
    }
}

