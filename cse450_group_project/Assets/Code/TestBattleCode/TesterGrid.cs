using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using TMPro;

/* Credit to: https://medium.com/@allencoded/unity-tilemaps-and-storing-individual-tile-data-8b95d87e9f32
*	for GridData where we transform the gridmap to a dictionary where mouse clicks can look up tiles 
*	and I can annotate BattlefiledTiles to store data.
*/

public interface GameManager
{
	void MouseOverStats(CharacterStats stats);
	void MouseLeaveStatCharacter();
	void CharacterDead(GameObject character);
	void CombatOver();
}

public class TesterGrid : MonoBehaviour, GameManager
{

	public TMP_Text turnUI;
	public GameObject statMenu;
	private TextMeshProUGUI statText;
	public GameObject resultMenu;
	public GameObject movementMenu;
	private Button attackButton;

	public GameObject enemyInfantry;
	public GameObject allyInfantry;
	//private BattlefieldTile _tilePrev;
	//private BattlefieldTile _tileCur;

	// private GameObject _seletedCharacter;
	// private GameObject _enemyCharacter;

	private List<Vector3> validMovementLocations = new();

	private List<GameObject> enemyCharacters = new();
	private List<GameObject> allyCharacters = new();

	private int turnCount;

	private enum BattleState {
		PlayerTurn,
		EnemyTurn,
		Won,
		Lost
	}

	private BattleState battleState;

	private enum CharacterMovementState {
		NoCharacterSelected,
		CharacterSelected,
		MoveLocationSet,
		Attacking
	}

	private CharacterMovementState characterMovementState;
	
	private struct CharacterMovementData
    {
		public GameObject selectedCharacter;
		public CharacterStats selectedCharacterStats;
		public BattlefieldTile currentTile;
		public BattlefieldTile potentialTile;

		public void Reset()
        {
			selectedCharacter = null;
			selectedCharacterStats = null;
			currentTile = null;
			potentialTile = null;
        }
	}

	private CharacterMovementData movementData;

	public Camera mainCamera;
	public Camera combatCamera;

	private CombatManager combatManager;

	private void Start()
    {
		resultMenu.SetActive(false);
		combatCamera.enabled = false;
		combatManager = combatCamera.GetComponent<CombatManager>();
		combatManager.myGameManager = this;
		battleState = BattleState.PlayerTurn;
		characterMovementState = CharacterMovementState.NoCharacterSelected;
		movementData = new CharacterMovementData();

		statText = statMenu.GetComponent<TextMeshProUGUI>();
		var rectTransform = statMenu.GetComponent<RectTransform>();

		var screenPoint = Camera.main.WorldToScreenPoint(new Vector3(-3, 3, 0));
		var screenRect = Camera.main.pixelRect;


		var rectTransPoint = new Vector3(
			screenPoint.x - screenRect.width / 2.0f,
			screenPoint.y - screenRect.height / 2.0f,
			screenPoint.z);
		rectTransform.SetLocalPositionAndRotation(rectTransPoint, Quaternion.identity);


		enemyCharacters.Add(PlaceCharacterAt(enemyInfantry, 1, 1));
        enemyCharacters.Add(PlaceCharacterAt(enemyInfantry, 2, 2));
        enemyCharacters.Add(PlaceCharacterAt(enemyInfantry, 1, 2));

        allyCharacters.Add(PlaceCharacterAt(allyInfantry, -5, -4));
        allyCharacters.Add(PlaceCharacterAt(allyInfantry, -4, -4));
        allyCharacters.Add(PlaceCharacterAt(allyInfantry, -3, -4));

		movementMenu.SetActive(false);
		Button[] buttons = movementMenu.GetComponentsInChildren<Button>();

		foreach (Button button in buttons)
        {
			if (button.name.Equals("CancelButton"))
			{
				button.onClick.AddListener(CancelMovement);
			}
			else if (button.name.Equals("WaitButton"))
			{
				button.onClick.AddListener(WaitMovement);
			}
			else if (button.name.Equals("AttackButton"))
            {
				attackButton = button;
				button.onClick.AddListener(() => { AttackMovement(button); });
            }
        }


		turnCount = 1;
		turnUI.text = "Turn " + turnCount.ToString();

	}

	private void Update()
	{
		if (battleState == BattleState.Won && !resultMenu.activeSelf)
        {
			resultMenu.GetComponentInChildren<TextMeshProUGUI>().text = "You Won!";
			resultMenu.SetActive(true);
        }

		if (battleState == BattleState.Lost && !resultMenu.activeSelf)
		{
			resultMenu.GetComponentInChildren<TextMeshProUGUI>().text = "You Lost!";
			resultMenu.SetActive(true);
		}


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

			TowardsPlayerEnemyMovement();

			turnUI.text = "Turn " + (++turnCount).ToString();

			ResetAllyCanMove();
			battleState = BattleState.PlayerTurn;
		}

	}

	private void TowardsPlayerEnemyMovement() {

		foreach (BattlefieldTile tile in GridData.instance.tiles.Values) {
			GameObject character = tile.Character;

			if (character == null) { continue; }

			CharacterStats characterStats = character.GetComponent<CharacterStats>();

			if (characterStats == null) { continue; }

			if (characterStats.Team == 0) { continue; }

			if (!characterStats.CanMove) { continue; }

			movementData.currentTile = tile;
			movementData.selectedCharacter = character;
			movementData.selectedCharacterStats = characterStats;

			TagReachableBySelectedChar();


			Vector3 positionOfRandomAlly = GetRandomAllyPosition();

			float distanceToAlly = int.MaxValue;
			Vector3 movementChoice = validMovementLocations[0];
			foreach (Vector3 location in validMovementLocations)
            {
				float dist = ManhattanDistance(location, positionOfRandomAlly);

				if (dist < distanceToAlly)
                {
					distanceToAlly = dist;
					movementChoice = location;
                }
			}

			GridData.instance.tiles.TryGetValue(movementChoice, out movementData.potentialTile);
			MoveCharacterTo(movementChoice);
			CharacterTurnOver();

			movementData.Reset();
			HandleCharacterDeselectBFS();
		}

		ResetEnemyCanMove();
	}


	private Vector3 GetRandomAllyPosition() {

		List<Vector3> locations = new();

		foreach (BattlefieldTile tile in GridData.instance.tiles.Values)
		{
			GameObject character = tile.Character;

			if (character == null) { continue; }

			CharacterStats characterStats = character.GetComponent<CharacterStats>();

			if (characterStats == null) { continue; }

			if (characterStats.Team != 0) { continue; }

			locations.Add(tile.LocalPlace);
		}

		int choiceIndex = Random.Range(0, locations.Count);
		return locations[choiceIndex];
	}


	private void RandomEnemyMovement()
	{

		foreach (BattlefieldTile tile in GridData.instance.tiles.Values)
		{
			GameObject character = tile.Character;

			if (character == null) { continue; }

			CharacterStats characterStats = character.GetComponent<CharacterStats>();

			if (characterStats == null) { continue; }

			if (characterStats.Team == 0) { continue; }

			if (!characterStats.CanMove) { continue; }

			movementData.currentTile = tile;
			movementData.selectedCharacter = character;
			movementData.selectedCharacterStats = characterStats;

			TagReachableBySelectedChar();

			int movementChoiceIndex = Random.Range(0, validMovementLocations.Count);
			Vector3 movementChoice = validMovementLocations[movementChoiceIndex];

			GridData.instance.tiles.TryGetValue(movementChoice, out movementData.potentialTile);
			MoveCharacterTo(movementChoice);
			CharacterTurnOver();

			movementData.Reset();
			HandleCharacterDeselectBFS();
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


	public void MouseOverStats(CharacterStats stats)
    {

		statText.text = "HP: " + stats.health
						+ "\nStr: " + stats.strength
						+ "\nDef: " + stats.defense
						+ "\nMov: " + stats.movement;

		statMenu.SetActive(true);

	}

	public void MouseLeaveStatCharacter()
    {
		statMenu.SetActive(false);
    }


	private void MoveMenuTo(Vector3 point)
    {
		var rectTransform = movementMenu.GetComponent<RectTransform>();
		var screenPoint = Camera.main.WorldToScreenPoint(point);

		var screenRect = Camera.main.pixelRect;

		var rectTransPoint = new Vector3(
			screenPoint.x - screenRect.width / 2.0f,
			screenPoint.y - screenRect.height / 2.0f,
			screenPoint.z
		);


		rectTransform.SetLocalPositionAndRotation(rectTransPoint, Quaternion.identity);

		//rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100);
		//rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 200);
	}


	/*
	 * Positions menu in the direction opposite movement. There are issues when moving on the 
	 * top or bottom row. This will be tweaked going forward, but works well enough now to keep the menu 
	 * out no on top of important enemies.
	 */
	private Vector3 MenuPositionAdjust()
    {
		Vector3 tileSize = movementData.potentialTile.TilemapMember.cellSize;
		Vector3 tileChange = movementData.potentialTile.WorldLocation - movementData.currentTile.WorldLocation;
		tileChange.Normalize();

		Vector3 finalPoint = movementData.potentialTile.WorldLocation + 0.5f * tileSize;

		if (Mathf.Abs(tileChange.x) > Mathf.Abs(tileChange.y)) {
			finalPoint.x -= 2f * tileChange.x;
		}
		else
        {
			finalPoint.y -= 2.1f * tileChange.y;
		}


		return finalPoint;
    }

	private void HandleClickAt(Vector3 point) {

		if (characterMovementState == CharacterMovementState.MoveLocationSet)
		{
			return;
		}

		// check tile clicked
		var worldPoint = new Vector3Int(Mathf.FloorToInt(point.x), Mathf.FloorToInt(point.y), 0);
		var tiles = GridData.instance.tiles; // This is our Dictionary of tiles

		BattlefieldTile clickedTile;
		if (tiles.TryGetValue(worldPoint, out clickedTile))
		{
			HandleTileClick(clickedTile);
			UpdateTileShading();
		}

	}

	private void CancelMovement() {
		MoveCharacterBackTo(movementData.currentTile.WorldLocation);

		characterMovementState = CharacterMovementState.NoCharacterSelected;
		movementData.Reset();
		movementMenu.SetActive(false);

		HandleCharacterDeselectBFS();
		UpdateTileShading();
	}

	private void WaitMovement()
    {
		CharacterTurnOver();

		characterMovementState = CharacterMovementState.NoCharacterSelected;
		movementData.Reset();
		movementMenu.SetActive(false);

		HandleCharacterDeselectBFS();
		UpdateTileShading();
	}

	private void AttackMovement(Button sender)
    {
		sender.interactable = false;
		characterMovementState = CharacterMovementState.Attacking;

		HandleCharacterDeselectBFS();
		AttackTileShading();

		UpdateTileShading();
    }


	private void HandleTileClick(BattlefieldTile clickedTile) {

		CharacterStats stats;

		switch (characterMovementState)
        {
			case CharacterMovementState.NoCharacterSelected:

				if (!clickedTile.Character) { break; }

				stats = clickedTile.Character.GetComponent<CharacterStats>();

				movementData.Reset();
				HandleCharacterDeselectBFS();

				if (stats.Team == 0 && stats.CanMove)
				{
					movementData.selectedCharacter = clickedTile.Character;
					movementData.selectedCharacterStats = stats;
					movementData.currentTile = clickedTile;

					characterMovementState = CharacterMovementState.CharacterSelected;

					HandleCharacterSelectBFS();
				}
				break;
			case CharacterMovementState.CharacterSelected:
				if (CharacterCanMove(clickedTile))
				{
					movementData.potentialTile = clickedTile;
					MoveCharacterTo(clickedTile.WorldLocation);

					Vector3 menuPoint = MenuPositionAdjust();

					MoveMenuTo(menuPoint);

					// TODO  We need a movementMenu manager instead of this
					attackButton.interactable = true;
					movementMenu.SetActive(true);
					characterMovementState = CharacterMovementState.MoveLocationSet;
				}
				else
				{
					characterMovementState = CharacterMovementState.NoCharacterSelected;
					movementData.Reset();

					HandleCharacterDeselectBFS();
				}
				break;
			case CharacterMovementState.MoveLocationSet:
				break;
			case CharacterMovementState.Attacking:

				if (clickedTile.SelectedCharacterPathing != -1) { break; }

				if (!clickedTile.Character) { break; }

				stats = clickedTile.Character.GetComponent<CharacterStats>();

                if (stats.Team == 0) { break; }

				movementMenu.SetActive(false);

				combatManager.StartCombat(movementData.selectedCharacter, clickedTile.Character);

				break;
		}

		UpdateTileShading();
	}

	public void CombatOver()
    {
		WaitMovement();
	}


	private bool CharacterCanMove(BattlefieldTile clickedTile) {
		return clickedTile.SelectedCharacterPathing == 1;
	}


	private void MoveCharacterTo(Vector3 location) {
		location.x += 0.5f;
		location.y += 0.5f;
		location.z = -1;

		movementData.selectedCharacter.transform.position = location;
		
		movementData.currentTile.Character = null;
		movementData.potentialTile.Character = movementData.selectedCharacter;

	}

	private void MoveCharacterBackTo(Vector3 location)
	{
		location.x += 0.5f;
		location.y += 0.5f;
		location.z = -1;

		movementData.selectedCharacter.transform.position = location;

		movementData.potentialTile.Character = null;
		movementData.currentTile.Character = movementData.selectedCharacter;
	}

	private void CharacterTurnOver()
    {
		movementData.selectedCharacter.GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.4f, 0.6f, 1);
		movementData.selectedCharacterStats.CanMove = false;
	}

	public void CharacterDead(GameObject deadCharacter)
    {
		foreach (BattlefieldTile tile in GridData.instance.tiles.Values)
		{
			GameObject character = tile.Character;

			if (character != deadCharacter) { continue; }

			tile.Character = null;

			break;
		}

		allyCharacters.Remove(deadCharacter);
		enemyCharacters.Remove(deadCharacter);

		if (allyCharacters.Count == 0)
        {
			battleState = BattleState.Lost;
        }
		else if (enemyCharacters.Count == 0)
		{
			battleState = BattleState.Won;
		}

		deadCharacter.SetActive(false);
	}

	private void HandleCharacterSelectBFS() {
		TagReachableBySelectedChar();
    }

	private float ManhattanDistance(Vector3 v1, Vector3 v2)
    {
		return Mathf.Abs(v1.x - v2.x) + Mathf.Abs(v1.y - v2.y);
	}

	private void AttackTileShading()
    {
		foreach (var tile in GridData.instance.tiles.Values)
		{

			float manhatDist = ManhattanDistance(tile.LocalPlace, movementData.potentialTile.LocalPlace);

			// movementData.characterStats.attackRange in future
			if (manhatDist <= 1)
            {
				tile.SelectedCharacterPathing = -1;
            }

		}
	}

	private void UpdateTileShading() {
		foreach (var tile in GridData.instance.tiles.Values)
		{

			Color changeColor = new Color(1, 1, 1, 1);

			if (tile.SelectedCharacterPathing == 1) {
				changeColor = new Color(0.4f, 0.5f, 1, 1);
			}
			else if (tile.SelectedCharacterPathing == -1)
            {
				changeColor = new Color(1, 0.4f, 0.4f, 1);
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

		int mov = movementData.selectedCharacterStats.movement;

		movementData.currentTile.ReachableInDistance = 0;
		movementData.currentTile.SelectedCharacterPathing = 1;
		validMovementLocations.Add(movementData.currentTile.LocalPlace);

		queue.AddFirst(movementData.currentTile);

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

					//if(tileToCheck.Character.activeSelf == false) { continue; }

					//Cannot move through other team
					if (tileToCheck.Character && tileToCheck.Character.GetComponent<CharacterStats>().Team != movementData.selectedCharacterStats.Team) { continue; }	

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


	private void HandleCharacterDeselectBFS()
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

		spawn.GetComponent<StatsMenuMouseOver>().myGameManager = this;

		BattlefieldTile tile;

		if (!tiles.TryGetValue(tileLocation, out tile))
		{
			return null;
		}

		tile.Character = spawn;

		return spawn;
	}

}
