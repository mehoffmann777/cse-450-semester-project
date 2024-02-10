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
		}

	}


	private void HandleTileClick() {

		if(_tileCur.Character) {
			_seletedCharacter = _tileCur.Character;
			print(_seletedCharacter);
			HandleCharacterSelect();
		}
		else if (_seletedCharacter) {
			MoveCharacterTo(_tileCur.WorldLocation);
			HandleCharacterDeselect();
		}
		


		//print("Tile " + _tile.Name + " costs: " + _tile.MovementCost);
		//_tile.TilemapMember.SetTileFlags(_tile.LocalPlace, TileFlags.None);

		//Color changeColor = _tile.Impassable ? Color.red : Color.green;
		//_tile.TilemapMember.SetColor(_tile.LocalPlace, changeColor);
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
		print("ToBlue");
		print(_tileCur);
		_tileCur.TilemapMember.SetTileFlags(_tileCur.LocalPlace, TileFlags.None);
		_tileCur.TilemapMember.SetColor(_tileCur.LocalPlace, Color.blue);
		_tileCur.TilemapMember.GetColor(_tileCur.LocalPlace);
    }

	private void HandleCharacterDeselect()
	{
		Color changeColor = Color.clear;
		_tilePrev.TilemapMember.SetTileFlags(_tilePrev.LocalPlace, TileFlags.None);
		_tilePrev.TilemapMember.SetColor(_tilePrev.LocalPlace, changeColor);
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
