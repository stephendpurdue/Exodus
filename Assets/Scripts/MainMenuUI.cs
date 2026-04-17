using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private string dungeonSceneName = "DungeonScene";

    [Header("UI References")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        if (playButton != null)
            playButton.onClick.AddListener(OnPlay);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuit);
    }

    private void OnPlay()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(dungeonSceneName);
    }

    private void OnQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
