using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Credit to: https://medium.com/@allencoded/unity-tilemaps-and-storing-individual-tile-data-8b95d87e9f32


public class GridData : MonoBehaviour
{
    public static GridData instance;
    public Tilemap Tilemap;

    public Dictionary<Vector3, BattlefieldTile> tiles;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        GetWorldTiles();
    }

    private void GetWorldTiles()
    {
        tiles = new Dictionary<Vector3, BattlefieldTile>();
        foreach (Vector3Int pos in Tilemap.cellBounds.allPositionsWithin)
        {
            var localPlace = new Vector3Int(pos.x, pos.y, 0);

            if (!Tilemap.HasTile(localPlace)) continue;


            Tile tile = (Tile) Tilemap.GetTile(localPlace);
            string spriteName = tile.sprite.name;

            Dictionary<string, int> terrainValues = new Dictionary<string, int> {
                { "tilemap_packed_108"  , 2 },
                { "tilemap_packed_112"  , 2 },
                { "tilemap_packed_0"    , 1 },
                { "tilemap_packed_1"    , 1 },
                { "tilemap_packed_2"    , 1 }
            };


            int movementCost = terrainValues.GetValueOrDefault(spriteName, -1);


            var bTile = new BattlefieldTile
            {
                LocalPlace = localPlace,
                WorldLocation = Tilemap.CellToWorld(localPlace),
                TileBase = Tilemap.GetTile(localPlace),
                TilemapMember = Tilemap,
                Name = localPlace.x + "," + localPlace.y,
                MovementCost = movementCost,
                Impassable = movementCost == -1,
                Character = null,
                ReachableInDistance = int.MaxValue
            };

            tiles.Add(bTile.WorldLocation, bTile);
        }
    }

}
