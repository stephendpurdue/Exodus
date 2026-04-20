using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    // This variable holds a reference to the pause menu container GameObject, which can be set in the Unity Editor.
     public GameObject container;
     void Update()
     {
         if (Input.GetKeyDown(KeyCode.Escape))
         {
             if (container.activeSelf)
             {
                 ResumeButton();
             }
             else
             {
                 container.SetActive(true);
                 Time.timeScale = 0;
             }
         }
     }

    // This method is called when the "Resume" button is clicked.
    public void ResumeButton()
    {
        container.SetActive(false);
        Time.timeScale = 1;
    }

    // This method is called when the "Main Menu" button is clicked.
    public void MainMenuButton()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
