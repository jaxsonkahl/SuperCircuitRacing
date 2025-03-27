using UnityEngine;

public class AICarBrain : MonoBehaviour
{
    public Transform[] waypoints;
    private int currentWaypoint = 0;

    [Header("Settings")]
    public float waypointReachDistance = 6f;
    public float lookAheadOffset = 2f;
    public float steerSmoothing = 5f;

    private CarController carController;

    void Start()
    {
        carController = GetComponent<CarController>();
        carController.useAI = true;

        waypoints = GameObject.Find("Waypoints").GetComponent<WaypointManager>().waypoints;
    }

    void Update()
    {
        if (waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypoint];

        // Predict a point slightly ahead of the waypoint for smoother turns
        Vector3 targetPos = target.position + target.forward * lookAheadOffset;

        // Get direction to target in car's local space
        Vector3 localTarget = transform.InverseTransformPoint(targetPos);

        // Skip the waypoint if it's behind the car
        if (localTarget.z < 0f)
        {
            AdvanceToNextWaypoint();
            return;
        }

        // Calculate steering input based on angle to target
        float angleToTarget = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
        float rawSteer = Mathf.Clamp(angleToTarget / 45f, -1f, 1f);

        // Smooth steering
        carController.aiSteerInput = Mathf.Lerp(carController.aiSteerInput, rawSteer, Time.deltaTime * steerSmoothing);

        // Always move forward (can add braking logic later)
        carController.aiMoveInput = 1f;

        // Check if we're close enough to the target
        float distanceToWaypoint = Vector3.Distance(transform.position, target.position);
        if (distanceToWaypoint < waypointReachDistance)
        {
            AdvanceToNextWaypoint();
        }
    }

    void AdvanceToNextWaypoint()
    {
        currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
    }
}