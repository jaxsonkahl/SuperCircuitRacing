using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpeedDecrease : MonoBehaviour
{
   public float speedDecrease = 50f; // Speed decrease
   public static List<GameObject> collectedDecreases = new List<GameObject>();
    public float decreaseDuration = 1f; // Duration in seconds
    private void OnTriggerEnter(Collider other)
    {
        // Try to get the CarController component from the colliding object
        CarController car = other.gameObject.GetComponent<CarController>();

        if (car != null && other.CompareTag("Player"))
        {
            Debug.Log("Activating speed decrease on car."); // Debugging log
            AudioManager.instance.PlaySfx("SpeedDecrease"); // Play sound effect
            car.ActivateSpeedDecrease(speedDecrease, decreaseDuration);
            if (!collectedDecreases.Contains(gameObject)) {
                collectedDecreases.Add(gameObject);
            }
            gameObject.SetActive(false);
        }
    }
}
