using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;
    public GameObject pauseMenuUI;

    // ✅ Prevent pause if the race is finished
    public static bool raceFinished = false; // New static variable to prevent pausing

    void Update()
    {
        // ✅ Prevent pause if race is finished
        if (raceFinished)
        {
            return; // Ignore pause if the race is complete
        }

        // Press Escape to toggle pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume(); // Resume if already paused
            }
            else
            {
                Pause(); // Pause otherwise
            }
        }
    }

    public void Resume()
{
    Debug.Log("Resume() called"); // Add this
    if (pauseMenuUI == null)
    {
<<<<<<< Updated upstream
        Debug.LogError("pauseMenuUI is not assigned!");
    }

    pauseMenuUI.SetActive(false);
    Time.timeScale = 1f;
    isPaused = false;
}

=======
        Debug.Log("Resume button clicked!");
        pauseMenuUI.SetActive(false); // Hide pause menu
        Time.timeScale = 1f; // Resume game time
        isPaused = false; // Update pause state
    }
>>>>>>> Stashed changes

    public void Pause()
    {
        Debug.Log("Pause button clicked!");
        pauseMenuUI.SetActive(true); // Show pause menu
        Time.timeScale = 0f; // Pause game time
        isPaused = true; // Update pause state
    }

    public void RestartGame()
    {
        Debug.Log("Restart Game button clicked!");
        Time.timeScale = 1f; // Resume game time before reloading
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame()
    {
        Debug.Log("Exit button clicked!");
        Time.timeScale = 1f; // Resume game time before exiting
        SceneManager.LoadScene("Menu");
    }

    // ✅ Disable Pause Menu after race ends
    public void DisablePause()
    {
        Debug.Log("Pause menu disabled after race.");
        raceFinished = true; // Prevent opening pause menu after race
        isPaused = false; // Ensure no lingering pause state
        pauseMenuUI.SetActive(false); // Hide pause menu just in case
        Time.timeScale = 1f; // Ensure time resumes normally
    }
}