using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    public float speedIncrease = 100f; // Extra speed
    public float boostDuration = 1f; // Duration in seconds

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Collision detected with: {other.gameObject.name}"); // Log the colliding object's name

        CarController car = other.gameObject.GetComponent<CarController>();

        if (car != null)
        {
            Debug.Log("CarController component found."); // Log if CarController is found
        }
        else
        {
            Debug.Log("CarController component not found."); // Log if CarController is missing
        }

        if (other.CompareTag("Player"))
        {
            Debug.Log("Colliding object is tagged as Player."); // Log if the tag matches
        }
        else
        {
            Debug.Log("Colliding object is not tagged as Player."); // Log if the tag doesn't match
        }

        if (car != null && other.CompareTag("Player"))
        {
            Debug.Log("Activating speed boost on car."); // Debugging log
            AudioManager.instance.PlaySfx("SpeedBoost"); 
            car.ActivateSpeedBoost(speedIncrease, boostDuration);
            Destroy(gameObject);
        }
    }
}
