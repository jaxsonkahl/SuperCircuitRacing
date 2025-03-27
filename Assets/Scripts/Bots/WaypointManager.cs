using UnityEngine;
using System.Linq;

public class WaypointManager : MonoBehaviour
{
    public Transform[] waypoints;

    void Awake()
    {
        waypoints = GetComponentsInChildren<Transform>()
                   .Where(t => t != transform)
                   .ToArray();
    }
}