using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Spawns enemies on valid floor tiles after dungeon generation.
/// Hooks into the same OnGenerationComplete event used by PlayerSpawner.
///
/// Spawn density: enemiesPerRoom enemies placed per BSP room.
/// Enemies are kept at least minSpawnDistFromPlayer tiles from the player spawn.
/// All instances are parented under a container so they can be bulk-destroyed on regen.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [Tooltip("Add one or more enemy prefabs. A random one is picked for each spawn.")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Spawn Settings")]
    [SerializeField] private int enemiesPerRoom = 2;
    [SerializeField] private float minSpawnDistFromPlayer = 5f;

    [Header("Tilemap Reference")]
    [Tooltip("Assign your floor Tilemap so world positions match tile centres.")]
    [SerializeField] private Tilemap floorTilemap;

    private Transform enemyContainer;
    private HashSet<Vector2Int> lastFloorPositions;

    // ── Called by DungeonManager ─────────────────────────────────────────────

    /// <summary>
    /// Clears existing enemies and spawns a fresh batch for the new dungeon.
    /// Pass roomCenters from RoomFirstDungeonGenerator so we can place 1-2 per room.
    /// If roomCenters is null/empty we fall back to random floor tile placement.
    /// </summary>
    public void SpawnEnemies(HashSet<Vector2Int> floorPositions, List<Vector2Int> roomCenters = null)
    {
        ClearEnemies();

        Debug.Log($"[EnemySpawner] SpawnEnemies called. FloorTiles={floorPositions?.Count ?? 0}, RoomCenters={roomCenters?.Count ?? 0}, Prefabs={enemyPrefabs?.Length ?? 0}");

        if (floorPositions == null || floorPositions.Count == 0)
        {
            Debug.LogError("[EnemySpawner] ABORT: floorPositions is null or empty.");
            return;
        }
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogError("[EnemySpawner] ABORT: No enemy prefabs assigned in the Inspector.");
            return;
        }

        lastFloorPositions = floorPositions;
        enemyContainer = new GameObject("--- Enemies ---").transform;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        Vector2? playerPos = playerObj != null ? (Vector2?)playerObj.transform.position : null;
        Debug.Log($"[EnemySpawner] Player found: {playerObj != null}, PlayerPos: {playerPos}");
        Debug.Log($"[EnemySpawner] FloorTilemap assigned: {floorTilemap != null}");

        if (roomCenters != null && roomCenters.Count > 0)
        {
            Debug.Log($"[EnemySpawner] Using SpawnByRoom with {roomCenters.Count} rooms.");
            SpawnByRoom(floorPositions, roomCenters, playerPos);
        }
        else
        {
            Debug.Log("[EnemySpawner] Using SpawnRandom fallback.");
            SpawnRandom(floorPositions, playerPos);
        }
    }

    public void ClearEnemies()
    {
        if (enemyContainer != null)
            Destroy(enemyContainer.gameObject);
    }

    // ── Spawn strategies ─────────────────────────────────────────────────────

    // Preferred: scatter enemies around each room centre
    private void SpawnByRoom(HashSet<Vector2Int> floorPositions, List<Vector2Int> roomCenters, Vector2? playerPos)
    {
        int totalSpawned = 0;
        foreach (var centre in roomCenters)
        {
            var candidates = floorPositions
                .Where(t => Vector2Int.Distance(t, centre) <= 6f)
                .ToList();

            Debug.Log($"[EnemySpawner] Room at {centre}: {candidates.Count} candidate tiles.");

            int spawned = 0;
            int attempts = 0;
            int skippedDist = 0;

            while (spawned < enemiesPerRoom && attempts < 30 && candidates.Count > 0)
            {
                attempts++;
                int idx = Random.Range(0, candidates.Count);
                Vector2Int tile = candidates[idx];
                candidates.RemoveAt(idx);

                Vector3 worldPos = TileToWorld(tile);

                if (playerPos.HasValue && Vector2.Distance(worldPos, playerPos.Value) < minSpawnDistFromPlayer)
                {
                    skippedDist++;
                    continue;
                }

                SpawnEnemyAt(worldPos);
                spawned++;
                totalSpawned++;
            }

            if (spawned == 0)
                Debug.LogWarning($"[EnemySpawner] Room at {centre}: 0 enemies spawned. Skipped by distance: {skippedDist}, Candidates exhausted: {candidates.Count == 0}");
        }
        Debug.Log($"[EnemySpawner] SpawnByRoom complete. Total spawned: {totalSpawned}");
    }

    // Fallback: pick random floor tiles
    private void SpawnRandom(HashSet<Vector2Int> floorPositions, Vector2? playerPos)
    {
        var allTiles = floorPositions.ToList();
        int totalEnemies = Mathf.Max(3, allTiles.Count / 10); // at least 3, roughly 1 per 10 tiles

        int spawned = 0;
        int attempts = 0;

        while (spawned < totalEnemies && attempts < totalEnemies * 10)
        {
            attempts++;
            Vector2Int tile = allTiles[Random.Range(0, allTiles.Count)];
            Vector3 worldPos = TileToWorld(tile);

            if (playerPos.HasValue && Vector2.Distance(worldPos, playerPos.Value) < minSpawnDistFromPlayer)
                continue;

            SpawnEnemyAt(worldPos);
            spawned++;
        }
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private void SpawnEnemyAt(Vector3 worldPos)
    {
        if (enemyPrefabs.Length == 0) return;
        var prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        Instantiate(prefab, worldPos, Quaternion.identity, enemyContainer);
    }

    private Vector3 TileToWorld(Vector2Int tile)
    {
        if (floorTilemap != null)
        {
            Vector3Int cellPos = new Vector3Int(tile.x, tile.y, 0);
            Vector3 world = floorTilemap.CellToWorld(cellPos) + floorTilemap.cellSize * 0.5f;
            world.z = 0f;
            return world;
        }
        // Fallback
        return new Vector3(tile.x + 0.5f, tile.y + 0.5f, 0f);
    }
}
