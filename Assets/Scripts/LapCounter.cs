using UnityEngine;
using TMPro;

public class LapCounter : MonoBehaviour
{
    public int totalLaps = 3; // Total number of laps
    private int currentLap = 0;
    private bool raceFinished = false;
    private bool canTrigger = true; // Prevent multiple triggers
    private float triggerCooldown = 1f; // Cooldown time (1 second)

    public TMP_Text lapText; // TextMeshPro UI for lap count
    public GameObject finishPanel; // UI Panel to display when race finishes

    void Start()
    {
        UpdateLapUI(); // Show initial lap (0/3)
        finishPanel.SetActive(false); // Hide finish panel at start
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FinishLine") && !raceFinished && canTrigger)
        {
            canTrigger = false; // Disable trigger temporarily
            currentLap++;

            // Update the lap UI immediately
            UpdateLapUI();

            // Check if the race is complete
            if (currentLap >= totalLaps)
            {
                FinishRace(); // Show finish panel if the race is done
            }
            else
            {
                Invoke("ResetTrigger", triggerCooldown); // Reset after cooldown
            }
        }
    }

    void ResetTrigger()
    {
        canTrigger = true; // Allow next lap trigger
    }

    void UpdateLapUI()
    {
        if (!raceFinished)
        {
            lapText.text = "Lap: " + currentLap + "/" + totalLaps;
        }
    }

    void FinishRace()
    {
        raceFinished = true;
        finishPanel.SetActive(true); // Show the finish panel
    }
}