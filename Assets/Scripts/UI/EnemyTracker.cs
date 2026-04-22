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

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ResetTracker()
    {
        totalEnemiesSpawned = 0;
        currentEnemiesAlive = 0;
        totalEnemiesKilled = 0;
        UpdateUI();
    }

    public void RegisterEnemySpawned()
    {
        totalEnemiesSpawned++;
        currentEnemiesAlive++;
        UpdateUI();
    }

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

    private void UpdateUI()
    {
        if (enemyCountText != null)
        {
            enemyCountText.text = $"Enemies Defeated: {totalEnemiesKilled} / {totalEnemiesSpawned}";
        }
    }

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