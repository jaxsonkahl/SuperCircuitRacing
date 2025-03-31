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
  public void quitGame(){
    
    Application.Quit();
  }
}
