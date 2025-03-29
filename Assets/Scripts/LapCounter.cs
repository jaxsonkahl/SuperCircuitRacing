using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class LapCounter : MonoBehaviour
{
    public int totalLaps = 3; // Total number of laps
    private int currentLap = 0; // Start from 0 internally but show 1/3 to 3/3
    private bool raceFinished = false;
    private bool canTrigger = true; // Prevent multiple triggers
    private float triggerCooldown = 1f; // Cooldown time to prevent multiple triggers

    private float lapStartTime; // Start time of the current lap
    private List<float> lapTimes = new List<float>(); // Stores all lap times
    private float currentLapTime; // Keeps track of current lap time

    public TMP_Text lapText; // TextMeshPro for lap count
    public TMP_Text currentLapTimer; // TextMeshPro to show the current lap timer
    public GameObject finishPanel; // Panel to show when race finishes
    public TMP_Text lapSummaryText; // TextMeshPro to show lap times on finish
    public GameObject lapTimePanel; // Panel to store lap time texts
    public TMP_Text lapTimeTemplate; // Template for lap times

    void Start()
    {
        UpdateLapUI(); // Show initial lap info
        finishPanel.SetActive(false); // Hide finish panel at start
        StartNewLap(); // Start the first lap timer
    }

    void Update()
    {
        if (!raceFinished)
        {
            UpdateCurrentLapTimer(); // Update the lap timer in real-time
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FinishLine") && !raceFinished && canTrigger)
        {
            canTrigger = false; // Prevent multiple triggers
            RecordLapTime(); // Save current lap time
            currentLap++;

            // Stop at 3/3 correctly and prevent going to 4/3
            if (currentLap >= totalLaps)
            {
                FinishRace(); // Show results if race is done
                currentLap = totalLaps; // Lock lap counter at 3/3
                UpdateLapUI(); // Final update to lock lap counter
                return;
            }

            // Update lap text and start a new lap
            UpdateLapUI();
            StartNewLap(); // Start the next lap timer
            Invoke("ResetTrigger", triggerCooldown);
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
            // Lap counter starts from 1 and ends at 3/3
            lapText.text = "Lap: " + (currentLap + 1) + "/" + totalLaps;
        }
    }

    void StartNewLap()
    {
        lapStartTime = Time.time; // Record lap start time
    }

    void UpdateCurrentLapTimer()
    {
        // Continuously calculate and update lap time
        currentLapTime = Time.time - lapStartTime;
        currentLapTimer.text = "Time: " + FormatTime(currentLapTime);
    }

    void RecordLapTime()
    {
        float lapTime = Time.time - lapStartTime; // Get lap duration
        lapTimes.Add(lapTime);

        // Create a new lap time display
        TMP_Text newLapTime = Instantiate(lapTimeTemplate, lapTimePanel.transform);
        newLapTime.gameObject.SetActive(true);

        // Show lap time starting from Lap 1
        newLapTime.text = "Lap " + lapTimes.Count + ": " + FormatTime(lapTime);
    }

    void FinishRace()
    {
        raceFinished = true;
        finishPanel.SetActive(true); // Show the finish panel
        ShowLapSummary(); // Display all lap times and highlight the best lap
    }

    void ShowLapSummary()
    {
        string summaryText = "Lap Times:\n";
        float bestTime = float.MaxValue;
        int bestLapIndex = 0;

        for (int i = 0; i < lapTimes.Count; i++)
        {
            string lapTimeText = "Lap " + (i + 1) + ": " + FormatTime(lapTimes[i]);

            // Check for best lap
            if (lapTimes[i] < bestTime)
            {
                bestTime = lapTimes[i];
                bestLapIndex = i;
            }

            summaryText += lapTimeText + "\n";
        }

        // Highlight best lap
        summaryText += "\nBest Lap: Lap " + (bestLapIndex + 1) + " - " + FormatTime(bestTime);
        lapSummaryText.text = summaryText;
    }

    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        float seconds = time % 60;
        return string.Format("{0:00}:{1:00.00}", minutes, seconds);
    }
}