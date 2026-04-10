using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

/// Spawns the player on a valid floor tile after dungeon generation.
/// Converts tile grid positions to world positions using the actual floor Tilemap.

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    [SerializeField] private Tilemap floorTilemap;

    [SerializeField] private bool randomSpawnPoint = false;

    private GameObject playerInstance;

    /// Called by DungeonManager after generation completes.
    public void SpawnPlayer(HashSet<Vector2Int> floorPositions)
    {
        if (floorPositions == null || floorPositions.Count == 0)
        {
            Debug.LogWarning("PlayerSpawner: No floor positions to spawn on.");
            return;
        }

        Vector2Int spawnTile = PickSpawnTile(floorPositions);

        // Convert grid position to world position using the tilemap, 
        // matching exactly how TilemapVisualizer places tiles.
        Vector3 worldPos;
        if (floorTilemap != null)
        {
            Vector3Int cellPos = new Vector3Int(spawnTile.x, spawnTile.y, 0);
            // CellToWorld gives the cell's bottom-left corner; add half cell size to centre
            worldPos = floorTilemap.CellToWorld(cellPos) + floorTilemap.cellSize * 0.5f;
            worldPos.z = 0f;
        }
        else
        {
            // Fallback if tilemap not assigned
            worldPos = new Vector3(spawnTile.x + 0.5f, spawnTile.y + 0.5f, 0f);
        }

        if (playerInstance == null)
        {
            playerInstance = Instantiate(playerPrefab, worldPos, Quaternion.identity);
        }
        else
        {
            playerInstance.transform.position = worldPos;
        }
    }

    private Vector2Int PickSpawnTile(HashSet<Vector2Int> floorPositions)
    {
        if (randomSpawnPoint)
        {
            return floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
        }

        // Pick tile closest to the centre of mass — usually a room interior, not a corridor edge.
        Vector2 centre = Vector2.zero;
        foreach (var pos in floorPositions)
            centre += pos;
        centre /= floorPositions.Count;

        Vector2Int best = floorPositions.First();
        float bestDist = float.MaxValue;
        foreach (var pos in floorPositions)
        {
            float dist = Vector2.Distance(centre, pos);
            if (dist < bestDist)
            {
                bestDist = dist;
                best = pos;
            }
        }
        return best;
    }
}