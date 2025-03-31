using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpeedBoost : MonoBehaviour
{
    public float speedIncrease = 100f; // Extra speed
    public float boostDuration = 1f; // Duration in seconds

    public static List<GameObject> collectedBoosts = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Collision detected with: {other.gameObject.name}"); // Log the colliding object's name

        CarController car = other.gameObject.GetComponent<CarController>();

        if (car != null && other.CompareTag("Player"))
        {
            Debug.Log("Activating speed boost on car."); // Debugging log
            AudioManager.instance.PlaySfx("SpeedBoost"); 
            car.ActivateSpeedBoost(speedIncrease, boostDuration);
            if (!collectedBoosts.Contains(gameObject)) {
                collectedBoosts.Add(gameObject);
            }
            gameObject.SetActive(false); 
        }
    }
}
