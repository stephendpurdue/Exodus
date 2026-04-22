using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyTracker : MonoBehaviour
{
    public static EnemyTracker Instance { get; private set; }

    [Header("UI References")]
    public UnityEngine.UI.Text enemyCountText;
    public GameObject victoryMenu;

    private int totalEnemiesSpawned = 0;
    private int currentEnemiesAlive = 0;
    private int totalEnemiesKilled = 0;

    // Ensure that this is a singleton instance
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Call this method to reset the tracker at the start of a new level or game
    public void ResetTracker()
    {
        totalEnemiesSpawned = 0;
        currentEnemiesAlive = 0;
        totalEnemiesKilled = 0;
        UpdateUI();
    }

    // This method should be called whenever an enemy is spawned
    public void RegisterEnemySpawned()
    {
        totalEnemiesSpawned++;
        currentEnemiesAlive++;
        UpdateUI();
    }

    // This method should be called whenever an enemy is killed
    public void RegisterEnemyKilled()
    {
        currentEnemiesAlive--;
        totalEnemiesKilled++;
        UpdateUI();

        if (currentEnemiesAlive <= 0 && totalEnemiesSpawned > 0)
        {
            OnAllEnemiesDefeated();
        }
    }

    // This method can be called to update the UI whenever there's a change in enemy counts
    private void UpdateUI()
    {
        if (enemyCountText != null)
        {
            enemyCountText.text = $"Enemies Defeated: {totalEnemiesKilled} / {totalEnemiesSpawned}";
        }
    }

    // This method is called when all enemies have been defeated
    private void OnAllEnemiesDefeated()
    {
        Debug.Log("All enemies defeated! Victory!");
        if (victoryMenu != null)
        {
            victoryMenu.SetActive(true);
            Time.timeScale = 0f; // Pause the game
        }
    }
}