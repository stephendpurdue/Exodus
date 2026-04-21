using System.Collections.Generic;
using UnityEngine;

// Central coordinator that wires the dungeon generator to the player spawner.
// Calls GenerateDungeon() automatically in Start().
public class DungeonManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AbstractDungeonGenerator dungeonGenerator;
    [SerializeField] private PlayerSpawner playerSpawner;
    [SerializeField] private DungeonDecorator dungeonDecorator;
    [SerializeField] private EnemySpawner enemySpawner;

    // Automatically find the dungeon generator if not set in the Inspector, and subscribe to its completion event.
    private void Awake()
    {
        if (dungeonGenerator == null)
            dungeonGenerator = GetComponent<AbstractDungeonGenerator>();

        if (dungeonGenerator != null)
            dungeonGenerator.OnGenerationComplete += HandleGenerationComplete;
    }

    // Automatically start dungeon generation when the scene starts.
    private void Start()
    {
        GenerateDungeon();
    }

    //  Unsubscribe from events to prevent memory leaks.
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

    // Called by the dungeon generator when generation is complete, passing the set of floor positions.
    private void HandleGenerationComplete(HashSet<Vector2Int> floorPositions)
    {
        if (playerSpawner != null)
            playerSpawner.SpawnPlayer(floorPositions);

        if (dungeonDecorator != null)
            dungeonDecorator.Decorate(floorPositions);

        if (enemySpawner != null)
        {
            List<Vector2Int> roomCenters = null;
            if (dungeonGenerator is RoomFirstDungeonGenerator rfGen)
            {
                roomCenters = rfGen.GetRoomCenters();
                Debug.Log($"[DungeonManager] Got {roomCenters?.Count ?? 0} room centers from RoomFirstDungeonGenerator.");
            }
            else
            {
                Debug.LogWarning("[DungeonManager] dungeonGenerator is not RoomFirstDungeonGenerator — room centers unavailable, using random fallback.");
            }

            enemySpawner.SpawnEnemies(floorPositions, roomCenters);
        }
        else
        {
            Debug.LogError("[DungeonManager] EnemySpawner is NULL — drag it into the Inspector field on DungeonManager.");
        }
    }
}
