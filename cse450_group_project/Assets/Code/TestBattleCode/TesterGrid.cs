using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Credit to: https://medium.com/@allencoded/unity-tilemaps-and-storing-individual-tile-data-8b95d87e9f32

public class TesterGrid : MonoBehaviour
{

	public GameObject enemyInfantry;

	private BattlefieldTile _tilePrev;
	private BattlefieldTile _tileCur;

	private GameObject _seletedCharacter;

	private void Start()
    {

		PlaceCharacterAt(enemyInfantry, 1, 1);
		PlaceCharacterAt(enemyInfantry, 2, 2);
		PlaceCharacterAt(enemyInfantry, 1, 2);

	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			HandleClickAt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
		}
	}


	private void HandleClickAt(Vector3 point) {
		// check tile clicked
		var worldPoint = new Vector3Int(Mathf.FloorToInt(point.x), Mathf.FloorToInt(point.y), 0);
		var tiles = GridData.instance.tiles; // This is our Dictionary of tiles


		// I can see this causing issues later :)
		_tilePrev = _tileCur;
		if (tiles.TryGetValue(worldPoint, out _tileCur))
		{
			HandleTileClick();
			UpdateTileShading();
		}

	}


	private void HandleTileClick() {

		if(_tileCur.Character) {
			HandleCharacterDeselect();
			_seletedCharacter = _tileCur.Character;
			HandleCharacterSelect();
		}
		else if (_seletedCharacter) {

			if (CharacterCanMove())
            {
				MoveCharacterTo(_tileCur.WorldLocation);
            }

			HandleCharacterDeselect();
		}

		UpdateTileShading();
	}

	private bool CharacterCanMove() {
		return _tileCur.SelectedCharacterPathing == 1;
	}


	private void MoveCharacterTo(Vector3 location) {
		location.x += 0.5f;
		location.y += 0.5f;
		location.z = -1;
		_seletedCharacter.transform.position = location;

		_tilePrev.Character = null;
		_tileCur.Character = _seletedCharacter;
		_seletedCharacter = null;
	}

	private void HandleCharacterSelect() {
		TagReachableBySelectedChar();
    }

	private void UpdateTileShading() {
		foreach (var tile in GridData.instance.tiles.Values)
		{

			Color changeColor = new Color(1, 1, 1, 1);

			if (tile.SelectedCharacterPathing == 1) {
				changeColor = new Color(0.3f, 0.4f, 1, 1);
			}

			tile.TilemapMember.SetTileFlags(tile.LocalPlace, TileFlags.None);
			tile.TilemapMember.SetColor(tile.LocalPlace, changeColor);

		}
	}

	private void AddTileDecreasing(LinkedList<BattlefieldTile> list, BattlefieldTile tile) {
		LinkedListNode<BattlefieldTile> n = list.First;

		if (n == null) { list.AddFirst(tile); return; }


		while (tile.ReachableInDistance < n.Value.ReachableInDistance) {
			if (n.Next == null) {
				list.AddAfter(n, tile);
				return;
			}

			n = n.Next;
		}

		list.AddBefore(n, tile);
	}


	private void TagReachableBySelectedChar() {
		LinkedList<BattlefieldTile> queue = new();

		var tiles = GridData.instance.tiles;

		Vector3Int loc;

		int mov = 3;

		_tileCur.ReachableInDistance = 0;
		_tileCur.SelectedCharacterPathing = 1;

		queue.AddFirst(_tileCur);

		while (queue.Count > 0) {
			LinkedListNode<BattlefieldTile> thisTile = queue.Last;
			queue.RemoveLast();

			loc = thisTile.Value.LocalPlace;

			Vector3Int upLoc = loc;
			upLoc.y++;

			Vector3Int downLoc = loc;
			downLoc.y--;

			Vector3Int rightLoc = loc;
			rightLoc.x++;

			Vector3Int leftLoc = loc;
			leftLoc.x--;

			Vector3Int[] locToCheck = { upLoc, downLoc, rightLoc, leftLoc };

			BattlefieldTile tileToCheck;

			foreach (Vector3Int possibleLoc in locToCheck) {

				if (tiles.TryGetValue(possibleLoc, out tileToCheck))
				{
					if (tileToCheck.SelectedCharacterPathing != 0) { continue; }

					tileToCheck.ReachableInDistance = thisTile.Value.ReachableInDistance + tileToCheck.MovementCost;
					
					if (tileToCheck.ReachableInDistance > mov) { continue; }

					if (tileToCheck.Impassable) { continue; }

					tileToCheck.SelectedCharacterPathing = 1;

					if (tileToCheck.Character != null) { tileToCheck.SelectedCharacterPathing++; }
					

					AddTileDecreasing(queue, tileToCheck);
				}
			}
		}
	}



	private void HandleCharacterDeselect()
	{
		// Reset BFS
		foreach (var tile in GridData.instance.tiles.Values)
		{
			tile.ReachableInDistance = int.MaxValue;
			tile.SelectedCharacterPathing = 0;
		}

	}

	private GameObject PlaceCharacterAt(GameObject gameObject, int x, int y)
	{

		Vector3 spawnLocation = new Vector3(x + 0.5f, y + 0.5f, -1);
		Vector3Int tileLocation = new Vector3Int(x, y, 0);

		var tiles = GridData.instance.tiles; // This is our Dictionary of tiles

		GameObject spawn = Instantiate(gameObject, spawnLocation, Quaternion.identity);

		if (!tiles.TryGetValue(tileLocation, out _tileCur))
		{
			return null;
		}

		_tileCur.Character = spawn;

		return spawn;
	}

}
