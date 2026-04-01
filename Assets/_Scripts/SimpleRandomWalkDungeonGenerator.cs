using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


public class SimpleRandomWalkMapGenerator : AbstractDungeonGenerator
{

    [SerializeField]
    private int iterations = 10;

    [SerializeField]
    public int walkLength = 10;

    [SerializeField]
    public bool startRandomlyEachIteration = true;

    protected override void RunProceduralGeneration()
    {
        HashSet<Vector2Int> floorPositions = RunRandomWalk();
        tilemapVisualizer.Clear();
        tilemapVisualizer.PaintFloorTiles(floorPositions);
    }

    protected HashSet<Vector2Int> RunRandomWalk()
    {
        var currentPosition = startPosition;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        for (int i = 0; i < iterations; i++)
        {
            var path = ProceduralGenerationAlgorithms.SimpleRandomWalk(currentPosition, walkLength);
            floorPositions.UnionWith(path); // Uses Union to add the positions to floor positions HashSet.
            if (startRandomlyEachIteration)
                currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count)); // Gets a random element from the floor positions HashSet.
        }
        return floorPositions;
    }
}
