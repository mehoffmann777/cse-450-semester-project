using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Credit to: https://medium.com/@allencoded/unity-tilemaps-and-storing-individual-tile-data-8b95d87e9f32


public class BattlefieldTile
{
    public Vector3Int LocalPlace { get; set; }

    public Vector3 WorldLocation { get; set; }

    public TileBase TileBase { get; set; }

    public Tilemap TilemapMember { get; set; }

    public string Name { get; set; }

    public int MovementCost { get; set; }

    public bool Impassable { get; set; }

    public GameObject Character { get; set; }

    //public int SelectedCharacterPathing { get; set; }

    public int ReachableInDistance { get; set; }
}
