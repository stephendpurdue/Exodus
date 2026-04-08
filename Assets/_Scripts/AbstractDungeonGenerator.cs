using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractDungeonGenerator : MonoBehaviour
{
    [SerializeField]
    protected TilemapVisualizer tilemapVisualizer = null;

    [SerializeField]
    protected Vector2Int startPosition = Vector2Int.zero;

    // Event fired after generation completes, passing the final set of floor positions.
    // DungeonManager subscribes to this to route data to PlayerSpawner and DungeonDecorator.
    public event Action<HashSet<Vector2Int>> OnGenerationComplete;

    public void GenerateDungeon()
    {
        tilemapVisualizer.Clear();
        RunProceduralGeneration();
    }

    protected abstract void RunProceduralGeneration();

    /// <summary>
    /// Call this at the end of every concrete generator's RunProceduralGeneration
    /// to broadcast the floor data to all subscribers.
    /// </summary>
    protected void NotifyGenerationComplete(HashSet<Vector2Int> floorPositions)
    {
        OnGenerationComplete?.Invoke(floorPositions);
    }
}
