using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Credit to: https://medium.com/@allencoded/unity-tilemaps-and-storing-individual-tile-data-8b95d87e9f32


public class BattlefieldTile
{
    public Vector3Int localPlace { get; set; }

    public Vector3 worldLocation { get; set; }

    public TileBase tileBase { get; set; }

    public Tilemap tilemapMember { get; set; }

    public string name { get; set; }

    public int movementCost { get; set; }

    public bool impassable { get; set; }
}
