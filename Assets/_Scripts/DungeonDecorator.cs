using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Post-processing pass that places decoration prefabs on floor tiles
/// based on their neighbour configuration (same logic pattern as WallGenerator).
/// 
/// Categories:
///   - Open (4 cardinal floor neighbours): room centres — torches, rugs, etc.
///   - Edge (2-3 cardinal floor neighbours): along walls — barrels, shelves, etc.
///   - Corner (0-1 cardinal floor neighbours): dead-ends/tight spots — cobwebs, skulls, etc.
///
/// Each category has its own prefab list and independent spawn probability.
/// All spawned decorations are parented under a container so they can be
/// bulk-destroyed on regeneration without scanning the entire scene.
/// </summary>
public class DungeonDecorator : MonoBehaviour
{
    [Header("Open Tile Decorations (4 floor neighbours)")]
    [SerializeField] private GameObject[] openPrefabs;
    [SerializeField, Range(0f, 1f)] private float openSpawnChance = 0.05f;

    [Header("Edge Tile Decorations (2-3 floor neighbours)")]
    [SerializeField] private GameObject[] edgePrefabs;
    [SerializeField, Range(0f, 1f)] private float edgeSpawnChance = 0.08f;

    [Header("Corner / Dead-End Decorations (0-1 floor neighbours)")]
    [SerializeField] private GameObject[] cornerPrefabs;
    [SerializeField, Range(0f, 1f)] private float cornerSpawnChance = 0.15f;

    private Transform decorationContainer;

    /// <summary>
    /// Called by DungeonManager after generation completes.
    /// Clears any existing decorations first, then places new ones.
    /// </summary>
    public void Decorate(HashSet<Vector2Int> floorPositions)
    {
        ClearDecorations();

        if (floorPositions == null || floorPositions.Count == 0)
            return;

        decorationContainer = new GameObject("--- Decorations ---").transform;

        foreach (var pos in floorPositions)
        {
            int floorNeighbours = CountFloorNeighbours(pos, floorPositions);

            if (floorNeighbours >= 4)
                TrySpawn(openPrefabs, openSpawnChance, pos);
            else if (floorNeighbours >= 2)
                TrySpawn(edgePrefabs, edgeSpawnChance, pos);
            else
                TrySpawn(cornerPrefabs, cornerSpawnChance, pos);
        }
    }

    /// <summary>
    /// Destroys all previously spawned decorations.
    /// </summary>
    public void ClearDecorations()
    {
        if (decorationContainer != null)
            Destroy(decorationContainer.gameObject);
    }

    private int CountFloorNeighbours(Vector2Int position, HashSet<Vector2Int> floorPositions)
    {
        int count = 0;
        foreach (var dir in Direction2D.cardinalDirectionsList)
        {
            if (floorPositions.Contains(position + dir))
                count++;
        }
        return count;
    }

    private void TrySpawn(GameObject[] prefabs, float chance, Vector2Int tilePos)
    {
        if (prefabs == null || prefabs.Length == 0)
            return;

        if (Random.value > chance)
            return;

        var prefab = prefabs[Random.Range(0, prefabs.Length)];
        Vector3 worldPos = new Vector3(tilePos.x + 0.5f, tilePos.y + 0.5f, 0f);
        Instantiate(prefab, worldPos, Quaternion.identity, decorationContainer);
    }
}
