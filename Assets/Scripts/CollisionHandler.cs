using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // Only play the crash sound if colliding with obstacles (walls, barriers)
        if (collision.gameObject.CompareTag("Obstacle")) 
        {
            Debug.Log("Crashed into an obstacle: " + collision.gameObject.name);
            AudioManager.instance?.PlaySfx("Crash");
        }
    }
}
