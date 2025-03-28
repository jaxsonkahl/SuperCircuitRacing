using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    public float speedIncrease = 100f; // Extra speed
    public float boostDuration = 1f; // Duration in seconds

    private void OnTriggerEnter(Collider other)
    {
        // Try to get the CarController component from the colliding object
        CarController car = other.gameObject.GetComponent<CarController>();

        if (car != null && other.CompareTag("Player"))
        {
            Debug.Log("Activating speed boost on car."); // Debugging log
            AudioManager.instance.PlaySfx("SpeedBoost"); 
            car.ActivateSpeedBoost(speedIncrease, boostDuration);
            Destroy(gameObject);
        }
    }
}
