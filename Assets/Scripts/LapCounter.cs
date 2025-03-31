using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LapCounter : MonoBehaviour
{
    public int totalLaps = 3; // Total number of laps
    private int currentLap = 0; // Start from 0 internally but show 1/3 to 3/3
    private bool raceFinished = false;
    private bool canTrigger = true; // Prevent multiple triggers
    private float triggerCooldown = 1f; // Cooldown time to prevent multiple triggers
    private int crossingCount = 0; // Track the number of crossings

    private float lapStartTime; // Start time of the current lap
    private List<float> lapTimes = new List<float>(); // Stores all lap times
    private float currentLapTime; // Keeps track of current lap time

    public TMP_Text lapText; // TextMeshPro for lap count
    public TMP_Text currentLapTimer; // TextMeshPro to show the current lap timer
    public GameObject finishPanel; // Panel to show when race finishes
    public TMP_Text lapSummaryText; // TextMeshPro to show lap times on finish
    public GameObject lapTimePanel; // Panel to store lap time texts
    public TMP_Text lapTimeTemplate; // Template for lap times

    public GameObject restartButton; // Button to restart game
    public GameObject exitButton; // Button to exit game

    private bool raceStarted = false; // Ensure the race starts after countdown

    void Start()
    {
        UpdateLapUI(); // Show initial lap info
        finishPanel.SetActive(false); // Hide finish panel at start

        // Delay the start of the race after 3.48 seconds
        Invoke("StartRace", 3.48f);
    }

    void Update()
    {
        if (raceStarted && !raceFinished)
        {
            UpdateCurrentLapTimer(); // Update the lap timer in real-time
        }
    }

    // when player collects coin take off one second
    public void SubtractTimeFromLap(float seconds) {
        lapStartTime += seconds; 
        Debug.Log($"Time reduced by {seconds} seconds!");
    }

    // Starts the race and timer after the 3.48-second delay
    void StartRace()
    {
        raceStarted = true;
        StartNewLap(); // Start lap timer after 3.48s delay
        Debug.Log("Race Started! Timer Active.");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FinishLine") && raceStarted && !raceFinished && canTrigger)
        {
            canTrigger = false; // Prevent multiple triggers
            crossingCount++; // Count the crossings

            // Skip the first crossing to ignore the initial pass by the finish line
            if (crossingCount == 1)
            {
                Debug.Log("Initial crossing ignored.");
                Invoke("ResetTrigger", triggerCooldown);
                return;
            }

            // Count the first valid lap after the second crossing
            if (crossingCount > 1 && crossingCount <= totalLaps + 1)
            {
                RecordLapTime(); // Save current lap time
                currentLap++;

                // Stop at 3/3 correctly and prevent going beyond
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
            }

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
            int displayLap = Mathf.Clamp(currentLap + 1, 1, totalLaps);
            lapText.text = "Lap: " + displayLap + "/" + totalLaps;
        }
    }

    void StartNewLap() {
        lapStartTime = Time.time; // Record lap start time

        foreach (var coin in CoinCollection.collectedCoins) {
        if (coin != null)
            coin.SetActive(true);
        }
        CoinCollection.collectedCoins.Clear();

        foreach (var boost in SpeedBoost.collectedBoosts) {
        if (boost != null)
            boost.SetActive(true);
        }
        SpeedBoost.collectedBoosts.Clear();

        foreach (var decrease in SpeedDecrease.collectedDecreases) {
        if (decrease != null)
            decrease.SetActive(true);
        }
        SpeedDecrease.collectedDecreases.Clear();

        foreach (var star in StarPower.collectedStars){
        if (star != null)
            star.SetActive(true);
        }
        StarPower.collectedStars.Clear();
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

        // Show lap time starting from Lap 1 to Lap 3
         AudioManager.instance.PlaySfx("Lap");
        newLapTime.text = "Lap " + lapTimes.Count + ": " + FormatTime(lapTime);
    }

    void FinishRace()
    {
        raceFinished = true;
        finishPanel.SetActive(true); // Show the finish panel
        ShowLapSummary(); // Display all lap times and highlight the best lap
        EnableButtons(); // Show restart and exit buttons
        Time.timeScale = 0; // Pause the game after race ends
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

            // Highlight best lap with bold and color (optional)
            if (i == bestLapIndex)
            {
                lapTimeText = $"<b><color=#FFD700>{lapTimeText}</color></b>"; // Highlight best lap
            }

            summaryText += lapTimeText + "\n";
        }

        // Add Best Lap separately for extra emphasis (optional)
        summaryText += "\nBest Lap: <b><color=#FFD700>Lap " + (bestLapIndex + 1) + " - " + FormatTime(bestTime) + "</color></b>";
        lapSummaryText.text = summaryText;
    }

    void EnableButtons()
    {
        if (restartButton != null)
        {
            restartButton.SetActive(true);
        }

        if (exitButton != null)
        {
            exitButton.SetActive(true);
        }
    }

    // Restart Game - Reload Current Scene
    public void RestartGame()
    {
        Time.timeScale = 1; // Resume game time before reloading
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Exit Game - Quit Application
    public void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in Editor
        #else
        Application.Quit(); // Quit the game in a built version
        #endif
    }

    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        float seconds = time % 60;
        return string.Format("{0:00}:{1:00.00}", minutes, seconds);
    }

}