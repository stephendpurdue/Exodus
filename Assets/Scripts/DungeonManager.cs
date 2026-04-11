using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AbstractDungeonGenerator dungeonGenerator;
    [SerializeField] private PlayerSpawner playerSpawner;
    [SerializeField] private DungeonDecorator dungeonDecorator;
    [SerializeField] private EnemySpawner enemySpawner;

    private void Awake()
    {
        if (dungeonGenerator == null)
            dungeonGenerator = GetComponent<AbstractDungeonGenerator>();

        if (dungeonGenerator != null)
            dungeonGenerator.OnGenerationComplete += HandleGenerationComplete;
    }

    private void Start()
    {
        GenerateDungeon();
    }

    private void OnDestroy()
    {
        if (dungeonGenerator != null)
            dungeonGenerator.OnGenerationComplete -= HandleGenerationComplete;
    }

    // Public entry point — call this from UI buttons (e.g. Regenerate).
    public void GenerateDungeon()
    {
        // Clear decorations and enemies before regenerating
        if (dungeonDecorator != null)
            dungeonDecorator.ClearDecorations();

        if (enemySpawner != null)
            enemySpawner.ClearEnemies();

        dungeonGenerator.GenerateDungeon();
    }

    private void HandleGenerationComplete(HashSet<Vector2Int> floorPositions)
    {
        if (playerSpawner != null)
            playerSpawner.SpawnPlayer(floorPositions);

        if (dungeonDecorator != null)
            dungeonDecorator.Decorate(floorPositions);

        if (enemySpawner != null)
        {
            // Try to pass room centres from RoomFirstDungeonGenerator for better spread
            List<Vector2Int> roomCenters = null;
            if (dungeonGenerator is RoomFirstDungeonGenerator rfGen)
                roomCenters = rfGen.GetRoomCenters();

            enemySpawner.SpawnEnemies(floorPositions, roomCenters);
        }
    }
}
