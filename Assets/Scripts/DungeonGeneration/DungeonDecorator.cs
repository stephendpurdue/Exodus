using System.Collections.Generic;
using UnityEngine;

// Post-processing pass that places decoration prefabs on floor tiles

// Categories:
//   - Open (4 cardinal floor neighbours): room centres — torches, rugs, etc.
//   - Edge (2-3 cardinal floor neighbours): along walls — barrels, shelves, etc.
//   - Corner (0-1 cardinal floor neighbours): dead-ends/tight spots — cobwebs, skulls, etc.

// Each category has its own prefab list and independent spawn probability.
// All spawned decorations are parented under a container so they can be
// bulk-destroyed on regeneration without scanning the entire scene.
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

    [Header("Keys")]
    [SerializeField] private GameObject[] keyPrefabs;
    [SerializeField, Range(0f, 1f)] private float keySpawnChance = 0.0125f;

    [Header("Wall Decorations (spawns on north-facing walls)")]
    [SerializeField] private GameObject[] wallPrefabs;
    [SerializeField, Range(0f, 1f)] private float wallSpawnChance = 0.1f;

    private Transform decorationContainer;

    // Called by DungeonManager after generation completes.
    // Clears any existing decorations first, then places new ones.
    public void Decorate(HashSet<Vector2Int> floorPositions)
    {
        ClearDecorations();

        if (floorPositions == null || floorPositions.Count == 0)
            return;

        decorationContainer = new GameObject("--- Decorations ---").transform;

        HashSet<Vector2Int> occupiedWallTiles = new HashSet<Vector2Int>();

        foreach (var pos in floorPositions)
        {
            // Check if there is a wall directly above this floor tile (north-facing wall)
            Vector2Int northWall = pos + Vector2Int.up;
            if (!floorPositions.Contains(northWall) && !occupiedWallTiles.Contains(northWall))
            {
                if (TrySpawn(wallPrefabs, wallSpawnChance, northWall))
                {
                    occupiedWallTiles.Add(northWall);
                }
            }

            // First, roll to see if a key spawns here. If it does, don't spawn another decoration on top of it.
            if (TrySpawn(keyPrefabs, keySpawnChance, pos))
                continue;

            int floorNeighbours = CountFloorNeighbours(pos, floorPositions);

            if (floorNeighbours >= 4)
                TrySpawn(openPrefabs, openSpawnChance, pos);
            else if (floorNeighbours >= 2)
                TrySpawn(edgePrefabs, edgeSpawnChance, pos);
            else
                TrySpawn(cornerPrefabs, cornerSpawnChance, pos);
        }
    }

    // Destroys all previously spawned decorations.
    public void ClearDecorations()
    {
        if (decorationContainer != null)
            Destroy(decorationContainer.gameObject);
    }

    // Helper method to count how many of the 4 cardinal neighbours of a tile are also floor tiles.
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

    // Helper method to attempt spawning a random prefab from the given list at the specified tile position.
    private bool TrySpawn(GameObject[] prefabs, float chance, Vector2Int tilePos)
    {
        if (prefabs == null || prefabs.Length == 0)
            return false;

        if (Random.value > chance)
            return false;

        var prefab = prefabs[Random.Range(0, prefabs.Length)];
        Vector3 worldPos = new Vector3(tilePos.x + 0.5f, tilePos.y + 0.5f, 0f);
        Instantiate(prefab, worldPos, Quaternion.identity, decorationContainer);

        return true;
    }
}
