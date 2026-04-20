using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    // This method is called when the "Restart" button is clicked.
    // It sets the time scale to 1 (normal speed) and reloads the currently active scene.
    public void RestartButton()
    {
        Time.timeScale = 1f;
        // Reloads the currently active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // This method is called when the "Main Menu" button is clicked.
    public void MainMenuButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
