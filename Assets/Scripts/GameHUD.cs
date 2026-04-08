using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// In-game HUD for the dungeon scene.
/// Provides Regenerate (re-runs the algorithm in-place) and Main Menu buttons.
/// </summary>
public class GameHUD : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Exact name of your main menu scene as it appears in Build Settings.")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("UI References")]
    [SerializeField] private Button regenerateButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Dungeon Reference")]
    [SerializeField] private DungeonManager dungeonManager;

    private void Start()
    {
        if (regenerateButton != null)
            regenerateButton.onClick.AddListener(OnRegenerate);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenu);
    }

    private void OnRegenerate()
    {
        if (dungeonManager != null)
            dungeonManager.GenerateDungeon();
    }

    private void OnMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
