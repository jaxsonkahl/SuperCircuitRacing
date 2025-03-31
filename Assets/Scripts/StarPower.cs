using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StarPower : MonoBehaviour
{
    public float speedBoost = 30f; // Additional acceleration
    public float duration = 6f; // How long the effect lasts
    public static List<GameObject> collectedStars = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        // Try to get the CarController component from the colliding object
        CarController car = other.gameObject.GetComponent<CarController>();

        if (car != null && other.CompareTag("Player"))
        {
            Debug.Log("Star power-up collected! Activating effects.");
            AudioManager.instance.PlaySfx("SpeedBoost"); 
            car.ActivateStarPower(speedBoost, duration);
            if (!collectedStars.Contains(gameObject))
            {
                collectedStars.Add(gameObject);
            }
            gameObject.SetActive(false);
        }
    }
}
