using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private string dungeonSceneName = "DungeonScene";

    [Header("UI References")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private GameObject settingsMenu;

    // This method is called when the script instance is being loaded.
    private void Start()
    {
        if (playButton != null)
            playButton.onClick.AddListener(OnPlay);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuit);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettings);
    }

    // This method is called when the "Play" button is clicked.
    // It sets the time scale to 1 (normal speed) and loads the specified dungeon scene.
    private void OnPlay()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(dungeonSceneName);
    }

    // This method is called when the "Quit" button is clicked.
    private void OnQuit()
    {
        Application.Quit();
    }

    // This method is called every frame to check for user input.
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsMenu != null && settingsMenu.activeSelf)
            {
                settingsMenu.SetActive(false);

                if (settingsMenu.transform.childCount > 0)
                {
                    settingsMenu.transform.GetChild(0).gameObject.SetActive(false);
                }
            }
        }
    }

    // This method is called when the "Settings" button is clicked.
    private void OnSettings()
    {
        if (settingsMenu != null)
        {
            settingsMenu.SetActive(true);

            if (settingsMenu.transform.childCount > 0)
            {
                settingsMenu.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }
}
