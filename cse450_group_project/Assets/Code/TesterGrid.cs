using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Credit to: https://medium.com/@allencoded/unity-tilemaps-and-storing-individual-tile-data-8b95d87e9f32

public class TesterGrid : MonoBehaviour
{
	private BattlefieldTile _tile;

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			print("click");

			Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			var worldPoint = new Vector3Int(Mathf.FloorToInt(point.x), Mathf.FloorToInt(point.y), 0);

			var tiles = GridData.instance.tiles; // This is our Dictionary of tiles

			if (tiles.TryGetValue(worldPoint, out _tile))
			{
				print("Tile " + _tile.name + " costs: " + _tile.movementCost);
				_tile.tilemapMember.SetTileFlags(_tile.localPlace, TileFlags.None);

				Color changeColor = _tile.impassable ? Color.red : Color.green;
				_tile.tilemapMember.SetColor(_tile.localPlace, changeColor);
			}
		}
	}
}
