using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Central coordinator that wires the dungeon generator to the player spawner
/// and decorator via the OnGenerationComplete event.
/// 
/// Attach this to the same GameObject as your AbstractDungeonGenerator.
/// Assign PlayerSpawner and DungeonDecorator in the Inspector.
/// Calls GenerateDungeon() automatically in Start().
/// </summary>
public class DungeonManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AbstractDungeonGenerator dungeonGenerator;
    [SerializeField] private PlayerSpawner playerSpawner;
    [SerializeField] private DungeonDecorator dungeonDecorator;

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

    /// <summary>
    /// Public entry point — call this from UI buttons (e.g. Regenerate).
    /// </summary>
    public void GenerateDungeon()
    {
        // Clean up decorations before regenerating the tilemap
        if (dungeonDecorator != null)
            dungeonDecorator.ClearDecorations();

        dungeonGenerator.GenerateDungeon();
    }

    private void HandleGenerationComplete(HashSet<Vector2Int> floorPositions)
    {
        if (playerSpawner != null)
            playerSpawner.SpawnPlayer(floorPositions);

        if (dungeonDecorator != null)
            dungeonDecorator.Decorate(floorPositions);
    }
}
