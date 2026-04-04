using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class generates a dungeon by creating corridors first and then rooms at the end of the corridors.
public class CorridorFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [SerializeField]
    private int corridorLength = 14, corridorCount = 5;

    [SerializeField]
    [Range(0.1f, 1f)]
    private float roomPercent = 0.8f;

    [SerializeField]
    public SimpleRandomWalkSO roomGenerationParameters;
    protected override void RunProceduralGeneration()
    {
        CorridorFirstGeneration();
    }

    // This method generates corridors first and then creates rooms at the end of the corridors.
    private void CorridorFirstGeneration()
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();

        CreateCorridors(floorPositions);

        tilemapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
    }

    // This method creates corridors and adds their positions to the floorPositions HashSet.
    private void CreateCorridors(HashSet<Vector2Int> floorPositions)
    {
        var currentPosition = startPosition;

        for (int i = 0; i < corridorCount; i++)
        {
            var corridor = ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition, corridorLength);
            currentPosition = corridor[corridor.Count - 1];
            floorPositions.UnionWith(corridor);

        }
    }
}
