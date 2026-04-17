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

    private void Start()
    {
        if (playButton != null)
            playButton.onClick.AddListener(OnPlay);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuit);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettings);
    }

    private void OnPlay()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(dungeonSceneName);
    }

    private void OnQuit()
    {
        Application.Quit();
    }

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
