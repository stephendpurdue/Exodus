using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using System;

// This class is responsible for visualizing the tilemap based on the floor and wall positions.
// It uses the Tilemap component to paint the tiles on the screen.
public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField] 
    private Tilemap floorTilemap, wallTilemap;

    // The tile that will be used to visualize the floor tiles on the tilemap.
    [SerializeField]
    private TileBase floorTile, wallTop, wallSideRight, wallSideLeft, wallBottom, wallFull;

    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {

        PaintTiles(floorPositions, floorTilemap, floorTile);
    }

    // Paints the given tiles on the tilemap at the specified positions. Loops t
    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
    {
        foreach (var position in positions)
        {
            PaintSingleTile(tilemap, tile, position);
        }   
    }

    // Paints a single wall tile on the tilemap based on the binary type of the wall.
    // The binary type represents the configuration of neighboring walls, which determines how the wall should be rendered.
    internal void PaintSingleBasicWall(Vector2Int position, string binaryType)
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2); // Converts Binary to Integer.
        TileBase tile = null;
        if (WallTypesHelper.wallTop.Contains(typeAsInt))
        {
            tile = wallTop;
        }
        else if (WallTypesHelper.wallSideRight.Contains(typeAsInt))
        {
            tile = wallSideRight;
        }
        else if (WallTypesHelper.wallSideLeft.Contains(typeAsInt))
        {
            tile = wallSideLeft;
        }
        else if (WallTypesHelper.wallBottom.Contains(typeAsInt))
        {
            tile = wallBottom;
        }
        else if (WallTypesHelper.wallFull.Contains(typeAsInt))
        {
            tile = wallFull;
        }
        if (tile!= null)
            PaintSingleTile(wallTilemap, tile, position);
    }

    // Paints a single tile on the tilemap at the given position.
    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    // Clears all the tiles from the floor tilemap.
    public void Clear()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
    }

    internal void PaintSingleCornerWall(Vector2Int position, string neighboursBinaryType)
    {

    }
}
