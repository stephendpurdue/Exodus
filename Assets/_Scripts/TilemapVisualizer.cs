using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField] 
    private Tilemap floorTilemap;

    // The tile that will be used to visualize the floor tiles on the tilemap.
    [SerializeField]
    private TileBase floorTile;

    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {

        PaintTiles(floorPositions, floorTilemap, floorTile);
    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
    {
        foreach (var position in positions)
        {
            PaintSingleTile(tilemap, tile, position);
        }   
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    // Clears all the tiles from the floor tilemap.
    public void Clear()
    {
        floorTilemap.ClearAllTiles();
    }
}
