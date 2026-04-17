using UnityEngine;

public class PauseMenu : MonoBehaviour
{

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

    public void ResumeButton()
    {
        container.SetActive(false);
        Time.timeScale = 1;
    }

    public void MainMenuButton()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
