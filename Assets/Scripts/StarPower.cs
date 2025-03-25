using UnityEngine;

public class StarPower : MonoBehaviour
{
    public float speedBoost = 30f; // Additional acceleration
    public float duration = 6f; // How long the effect lasts

    private void OnTriggerEnter(Collider other)
    {
        // Try to get the CarController component from the colliding object
        CarController car = other.gameObject.GetComponent<CarController>();

        if (car != null && other.CompareTag("Player"))
        {
            Debug.Log("Star power-up collected! Activating effects.");
            car.ActivateStarPower(speedBoost, duration);
            Destroy(gameObject); // Remove the power-up after activation
        }
    }
} // ✅ This should be the last closing brace!
