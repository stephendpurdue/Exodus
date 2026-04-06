using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProceduralGenerationAlgorithms
{
    public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPosition, int walkLength)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();

        path.Add(startPosition);
        var previousPosition = startPosition;

        for (int i = 0; i < walkLength; i++)
        {
            var newPosition = previousPosition + Direction2D.GetRandomCardinalDirection();
            path.Add(newPosition);
            previousPosition = newPosition;
        }
        return path;
    }

    // This method is used to create a corridor by using the random walk algorithm. It takes a start position and a corridor length as parameters and returns a list of Vector2Int positions that represent the corridor.
    public static List<Vector2Int> RandomWalkCorridor(Vector2Int startPosition, int corridorLength)
    {
        List<Vector2Int> corridor = new List<Vector2Int>();
        var direction = Direction2D.GetRandomCardinalDirection();
        var currentPosition = startPosition;
        corridor.Add(currentPosition);

        for (int i = 0; i < corridorLength; i++)
        {
            currentPosition += direction;
            corridor.Add(currentPosition);
        }
        return corridor;
    }

    // This method is used to split a space into smaller rooms using the binary space partitioning algorithm. It takes a BoundsInt object that represents the space to split, and minimum width and height for the rooms as parameters. It returns a list of BoundsInt objects that represent the rooms created from the split.
    public static List<BoundsInt> BinarySpacePartitioning(BoundsInt spaceToSplit, int minWidth, int minHeight)
    {
        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();
        List<BoundsInt> roomsList = new List<BoundsInt>();
        roomsQueue.Enqueue(spaceToSplit);
        while (roomsQueue.Count > 0)
        {
            var room = roomsQueue.Dequeue();
            if (room.size.y >= minHeight && room.size.x >= minWidth)
            {
                if (Random.value < 0.5f)
                {
                    if (room.size.y >= minHeight * 2)
                    {
                        SplitHorizontally(minHeight, roomsQueue, room);
                    }
                    else if (room.size.x >= minWidth * 2)
                    {
                        SplitVertically(minWidth, roomsQueue, room);
                    }
                    else if (room.size.x >= minWidth && room.size.y >= minHeight)
                    {
                        roomsList.Add(room);
                    }
                }
                else
                {
                    if (room.size.x >= minWidth * 2)
                    {
                        SplitVertically(minWidth, roomsQueue, room);
                    }
                    else if (room.size.y >= minHeight * 2)
                    {
                        SplitHorizontally(minHeight, roomsQueue, room);
                    }
                    else if (room.size.x >= minWidth && room.size.y >= minHeight)
                    {
                        roomsList.Add(room);
                    }
                }
            }
        }
        return roomsList;
    }
}


// This class is used to get the random direction for the random walk algorithm. It contains a list of cardinal directions (up, right, down, left) and a method to get a random cardinal direction from the list.
public static class Direction2D // This class gets the random direction.
{
    public static List<Vector2Int> cardinalDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(0,1), // UP
        new Vector2Int(1,0), // RIGHT
        new Vector2Int(0,-1), // DOWN
        new Vector2Int(-1, 0) // LEFT
    };

    public static Vector2Int GetRandomCardinalDirection()
    {
        return cardinalDirectionsList[Random.Range(0, cardinalDirectionsList.Count)];
    }
}
