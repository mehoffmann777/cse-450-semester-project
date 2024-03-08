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
            var localPlace = new Vector3Int(pos.x, pos.y, 0);

            if (!Tilemap.HasTile(localPlace)) continue;


            Tile tile = (Tile) Tilemap.GetTile(localPlace);
            string spriteName = tile.sprite.name;

            bool isRoughTerain = spriteName.Equals("tilemap_packed_108") || spriteName.Equals("tilemap_packed_112");

            bool isWater = !isRoughTerain && !spriteName.Equals("tilemap_packed_0") && !spriteName.Equals("tilemap_packed_1");



            var bTile = new BattlefieldTile
            {
                LocalPlace = localPlace,
                WorldLocation = Tilemap.CellToWorld(localPlace),
                TileBase = Tilemap.GetTile(localPlace),
                TilemapMember = Tilemap,
                Name = localPlace.x + "," + localPlace.y,
                MovementCost = isRoughTerain ? 2 : 1,
                Impassable = isWater,
                Character = null,
                ReachableInDistance = int.MaxValue
            };

            tiles.Add(bTile.WorldLocation, bTile);
        }
    }

}
