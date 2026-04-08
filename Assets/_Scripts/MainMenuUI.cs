using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Main menu controller. Attach to a Canvas in your "MainMenu" scene.
/// The Play button loads the dungeon scene (which auto-generates via DungeonManager.Start()).
/// Quit exits the application (no-op in the Editor).
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Exact name of your dungeon scene as it appears in Build Settings.")]
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
