using UnityEngine;
using  UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
  public void playGame()
{
    if (AudioManager.instance != null)
    {
        AudioManager.instance.StopMusic(); // Stop main menu music
    }
    
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
}
  public void quitGame()
{
    Debug.Log("Quit button pressed."); // Debug log to check if it's being called

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in Unity Editor
    #else
        Application.Quit(); // Works in a built game
    #endif
}

public void Controls(){
    SceneManager.LoadScene("Controls"); // Load the controls scene
}
public void Menu(){
    SceneManager.LoadScene("Menu");
}
}
