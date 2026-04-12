using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// Spawns the player on a valid floor tile after dungeon generation.
/// Converts tile grid positions to world positions using the floor Tilemap.
public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private bool randomSpawnPoint = false;

    private GameObject playerInstance;

    /// Called by DungeonManager after generation completes.
    public void SpawnPlayer(HashSet<Vector2Int> floorPositions)
    {
        if (!ValidateSpawnConditions(floorPositions))
            return;

        Vector2Int spawnTile = PickSpawnTile(floorPositions);
        Vector3 worldPos = GetWorldPosition(spawnTile);

        if (playerInstance == null)
        {
            playerInstance = Instantiate(playerPrefab, worldPos, Quaternion.identity);
        }
        else if (playerInstance.activeInHierarchy)
        {
            playerInstance.transform.position = worldPos;
        }
        else
        {
            Destroy(playerInstance);
            playerInstance = Instantiate(playerPrefab, worldPos, Quaternion.identity);
        }
    }

    private bool ValidateSpawnConditions(HashSet<Vector2Int> floorPositions)
    {
        if (floorPositions == null || floorPositions.Count == 0)
        {
            Debug.LogWarning("PlayerSpawner: No floor positions available.");
            return false;
        }

        if (playerPrefab == null)
        {
            Debug.LogError("PlayerSpawner: Player prefab not assigned.");
            return false;
        }

        if (floorTilemap == null)
        {
            Debug.LogWarning("PlayerSpawner: Floor tilemap not assigned. Using fallback positioning.");
        }

        return true;
    }

    private Vector2Int PickSpawnTile(HashSet<Vector2Int> floorPositions)
    {
        if (randomSpawnPoint)
        {
            int randomIndex = Random.Range(0, floorPositions.Count);
            int counter = 0;
            foreach (var pos in floorPositions)
            {
                if (counter == randomIndex)
                    return pos;
                counter++;
            }
        }

        // Find tile closest to centre of mass (typically room interior, not corridor edge)
        return FindClosestToCenter(floorPositions);
    }

    private Vector2Int FindClosestToCenter(HashSet<Vector2Int> floorPositions)
    {
        Vector2 centerOfMass = Vector2.zero;
        foreach (var pos in floorPositions)
            centerOfMass += pos;
        centerOfMass /= floorPositions.Count;

        Vector2Int closest = default;
        float closestDistance = float.MaxValue;

        foreach (var pos in floorPositions)
        {
            float distance = Vector2.SqrMagnitude(centerOfMass - (Vector2)pos);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = pos;
            }
        }

        return closest;
    }

    private Vector3 GetWorldPosition(Vector2Int gridPos)
    {
        if (floorTilemap == null)
            return GetFallbackWorldPosition(gridPos);

        Vector3Int cellPos = new Vector3Int(gridPos.x, gridPos.y, 0);
        Vector3 worldPos = floorTilemap.CellToWorld(cellPos) + floorTilemap.cellSize * 0.5f;
        worldPos.z = 0f;
        return worldPos;
    }

    private Vector3 GetFallbackWorldPosition(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x + 0.5f, gridPos.y + 0.5f, 0f);
    }
}