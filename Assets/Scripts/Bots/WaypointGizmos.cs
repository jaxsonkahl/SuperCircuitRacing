// Attach this to Waypoints object
using UnityEngine;

public class WaypointGizmos : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform wp = transform.GetChild(i);
            if (i + 1 < transform.childCount)
            {
                Transform nextWp = transform.GetChild(i + 1);
                Gizmos.DrawLine(wp.position, nextWp.position);
            }
        }
    }
}