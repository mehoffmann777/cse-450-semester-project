using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

// Credit to: https://medium.com/@allencoded/unity-tilemaps-and-storing-individual-tile-data-8b95d87e9f32

public class TesterGrid : MonoBehaviour
{

	public TMP_Text turnUI;

	public GameObject enemyInfantry;
	public GameObject allyInfantry;

	private BattlefieldTile _tilePrev;
	private BattlefieldTile _tileCur;

	public GameObject _seletedCharacter;
	public GameObject _enemyCharacter;

	private List<Vector3> validMovementLocations = new List<Vector3>();

	private List<GameObject> enemyCharacters = new();
	private List<GameObject> allyCharacters = new();

	private int turnCount;

	private enum BattleState {
		PlayerTurn,
		EnemyTurn
	}

	private BattleState battleState;

	public Camera mainCamera;
	public Camera combatCamera;
	

	private void Start()
    {
		combatCamera.enabled = false;
		battleState = BattleState.PlayerTurn;

		enemyCharacters.Add(PlaceCharacterAt(enemyInfantry, 1, 1));
		enemyCharacters.Add(PlaceCharacterAt(enemyInfantry, 2, 2));
		enemyCharacters.Add(PlaceCharacterAt(enemyInfantry, 1, 2));

		allyCharacters.Add(PlaceCharacterAt(allyInfantry, -5, -4));
		allyCharacters.Add(PlaceCharacterAt(allyInfantry, -4, -4));
		allyCharacters.Add(PlaceCharacterAt(allyInfantry, -3, -4));


		turnCount = 1;
		turnUI.text = "Turn " + turnCount.ToString();

	}

	private void Update()
	{
		if (battleState == BattleState.PlayerTurn)
        {
			if (Input.GetMouseButtonDown(0))
			{
				HandleClickAt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
			}

			if (AllyTurnOver()) {
				battleState = BattleState.EnemyTurn;
			}

		}

		if (battleState == BattleState.EnemyTurn) {

			RandomEnemyMovement();


			turnCount++;
			turnUI.text = "Turn " + turnCount.ToString();

			ResetAllyCanMove();
			battleState = BattleState.PlayerTurn;
		}
	}

	private void RandomEnemyMovement() {

		foreach (BattlefieldTile tile in GridData.instance.tiles.Values) {
			GameObject character = tile.Character;

			if (character == null) { continue; }

			CharacterStats characterStats = character.GetComponent<CharacterStats>();

			if (characterStats == null) { continue; }

			if (characterStats.Team == 0) { continue; }

			if (!characterStats.CanMove) { continue; }

			_tileCur = tile;
			_seletedCharacter = character;

			TagReachableBySelectedChar();

			int movementChoiceIndex = Random.Range(0, validMovementLocations.Count);
			Vector3 movementChoice = validMovementLocations[movementChoiceIndex];

			_tilePrev = _tileCur;

			GridData.instance.tiles.TryGetValue(movementChoice, out _tileCur);
			MoveCharacterTo(movementChoice);

			_seletedCharacter = null;
			HandleCharacterDeselect();
		}

		ResetEnemyCanMove();
	}


	private bool AllyTurnOver() {
		foreach (GameObject ally in allyCharacters)
        {
			CharacterStats allyData = ally.GetComponent<CharacterStats>();

			if (allyData.CanMove) {
				return false;
			}

        }

		return true;
	}

	private void ResetAllyCanMove() {
		foreach (GameObject ally in allyCharacters)
		{
			CharacterStats allyData = ally.GetComponent<CharacterStats>();

			allyData.CanMove = true;

			ally.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);

		}
	}

	private void ResetEnemyCanMove()
	{
		foreach (GameObject enemy in enemyCharacters)
		{
			CharacterStats enemyData = enemy.GetComponent<CharacterStats>();

			enemyData.CanMove = true;

			enemy.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);

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
		print(_seletedCharacter);
		print(_tileCur.Character);


		if (_seletedCharacter) {

			
			CharacterStats allyStats = _seletedCharacter.GetComponent<CharacterStats>();
		
			if (CharacterCanMove())
            {
				MoveCharacterTo(_tileCur.WorldLocation);
            } else if(_tileCur.Character && _tileCur.ReachableInDistance <= allyStats.movement)
				
            {
				CharacterStats stats = _tileCur.Character.GetComponent<CharacterStats>();
				if (stats.Team != 0)
				{
					print("Combat!");
					combatCamera.GetComponent<CombatManager>().StartCombat(_seletedCharacter, _tileCur.Character);
	
				}
			}

			_seletedCharacter = null;
			HandleCharacterDeselect();
		}
		else if (_tileCur.Character)
		{
			CharacterStats stats = _tileCur.Character.GetComponent<CharacterStats>();

			_seletedCharacter = null;
			HandleCharacterDeselect();

			if (stats.Team == 0 && stats.CanMove)
			{
				_seletedCharacter = _tileCur.Character;
				HandleCharacterSelect();
			}

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


		_seletedCharacter.GetComponent<CharacterStats>().CanMove = false;
		_seletedCharacter.GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.4f, 0.6f, 1);


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
				changeColor = new Color(0.4f, 0.5f, 1, 1);
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

		CharacterStats thisCharStats = _seletedCharacter.GetComponent<CharacterStats>();
		int mov = thisCharStats.movement;

		_tileCur.ReachableInDistance = 0;
		_tileCur.SelectedCharacterPathing = 1;
		validMovementLocations.Add(_tileCur.LocalPlace);

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

					if (tileToCheck.Impassable) { continue; }

					//Cannot move through other team
					// commented this out to allow for combat to occur
					//if (tileToCheck.Character && tileToCheck.Character.GetComponent<CharacterStats>().Team != thisCharStats.Team) { continue; }	

					tileToCheck.ReachableInDistance = thisTile.Value.ReachableInDistance + tileToCheck.MovementCost;
					
					if (tileToCheck.ReachableInDistance > mov) { continue; }


					if (tileToCheck.Character != null) {
						tileToCheck.SelectedCharacterPathing = 2;
					}
					else {
						tileToCheck.SelectedCharacterPathing = 1;
						validMovementLocations.Add(tileToCheck.LocalPlace);
					}

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

		validMovementLocations.Clear();

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
