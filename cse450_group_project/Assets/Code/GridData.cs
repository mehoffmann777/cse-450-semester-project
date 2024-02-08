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

        //    public Vector3Int localPlace { get; set; }

        //public Vector3 worldLocation { get; set; }

        //public TileBase tileBase { get; set; }

        //public Tilemap tilemapMember { get; set; }

        //public string name { get; set; }

        //public int movementCost { get; set; }

        //public bool impassable { get; set; }



        tiles = new Dictionary<Vector3, BattlefieldTile>();
        foreach (Vector3Int pos in Tilemap.cellBounds.allPositionsWithin)
        {
            var localPlace = new Vector3Int(pos.x, pos.y, pos.z);

            if (!Tilemap.HasTile(localPlace)) continue;


            Tile tile = (Tile) Tilemap.GetTile(localPlace);
            string spriteName = tile.sprite.name;

            bool isWater = !spriteName.Equals("tilemap_packed_0") && !spriteName.Equals("tilemap_packed_1");

            var bTile = new BattlefieldTile
            {
                localPlace = localPlace,
                worldLocation = Tilemap.CellToWorld(localPlace),
                tileBase = Tilemap.GetTile(localPlace),
                tilemapMember = Tilemap,
                name = localPlace.x + "," + localPlace.y,
                movementCost = 1,
                impassable = isWater

            };

            tiles.Add(bTile.worldLocation, bTile);
        }
    }

}
