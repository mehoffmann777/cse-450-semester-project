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

public delegate void MouseOverStats(CharacterStats stats);
public delegate void MouseLeaveStatCharacter();
public delegate void CharacterDead(GameObject character);
public delegate void CombatOver();

public abstract class BaseGridManager : MonoBehaviour
{

	public TMP_Text turnUI;
	public GameObject statMenu;
	private TextMeshProUGUI statText;
	public CornerDisplay cornerDisplay;
	public ResultMenuManager resultMenuManager;
	public FullScreenMenuManager fullScreenMenuManager;
	public GameObject movementMenu;
	private Button attackButton;
	private bool isShadingEnemyMovement;
	private List<BattlefieldTile> enemyMovementShadedTiles;

	private Dictionary<BattlefieldTile, BattlefieldMovementTileTag> movementLocations = new();

	private List<GameObject> enemyCharacters = new();
	private List<GameObject> allyCharacters = new();

	private int turnCount;

	public enum BattleState {
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
	public CameraManager mainCameraManager;
	public Camera combatCamera;

	private CombatManager combatManager;
	private Vector3 playersCameraPosition;

	private void Start()
    {
		combatCamera.enabled = false;
		combatManager = combatCamera.GetComponent<CombatManager>();
		combatManager.combatOver = CombatOver;
		combatManager.characterDead = CharacterDead;

		mainCameraManager = mainCamera.GetComponent<CameraManager>();

		battleState = BattleState.PlayerTurn;
		characterMovementState = CharacterMovementState.NoCharacterSelected;
		movementData = new CharacterMovementData();

		if (!cornerDisplay)
        {
			statText = statMenu.GetComponent<TextMeshProUGUI>();
			var rectTransform = statMenu.GetComponent<RectTransform>();
        }

		var screenPoint = Camera.main.WorldToScreenPoint(new Vector3(-3, 3, 0));
		var screenRect = Camera.main.pixelRect;

		fullScreenMenuManager.SetWinLoseText(InstructionsText());

		//var rectTransPoint = new Vector3(
		//	screenPoint.x - screenRect.width / 4.0f,
		//	screenPoint.y - screenRect.height / 4.0f,
		//	screenPoint.z);
		//rectTransform.SetLocalPositionAndRotation(rectTransPoint, Quaternion.identity);

		PlaceCharacters();

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

	public virtual string InstructionsText()
	{
		return "Win by defeating all enemy characters.\nYou lose if all your characters fall in battle.";
	}


	public List<GameObject> GetAllyCharacters()
    {
		return allyCharacters;
    }

	public int getTurnCount()
    {
		return turnCount;
    }

	public BattleState GetBattleState()
	{
		return battleState;
	}

	public struct CharacterSetupInfo
    {
		public GameObject character;
		public string characterName;
		public int x;
		public int y;
		public GetMovementDecision decisionPattern;

		public CharacterSetupInfo(GameObject character, int x, int y, GetMovementDecision decisionPattern)
		{
			this.character = character;

			CharacterStats stats = character.GetComponent<CharacterStats>();

			if (stats)
            {
				this.characterName = stats.characterName;
            }
			else
            {
				this.characterName = "Grunt";
            }

			this.x = x;
			this.y = y;
			this.decisionPattern = decisionPattern;
		}

		public CharacterSetupInfo(GameObject character, string characterName, int x, int y, GetMovementDecision decisionPattern)
		{
			this.character = character;
			this.characterName = characterName;
			this.x = x;
			this.y = y;
			this.decisionPattern = decisionPattern;
		}

	}

	public abstract List<CharacterSetupInfo> GetEnemySetupPattern();
	public abstract List<CharacterSetupInfo> GetAllySetupPattern();


	private void PlaceCharacters()
    {
		foreach (CharacterSetupInfo info in GetEnemySetupPattern())
        {
			enemyCharacters.Add(PlaceCharacterAt(info));
        }

		foreach (CharacterSetupInfo info in GetAllySetupPattern())
		{
			allyCharacters.Add(PlaceCharacterAt(info));
		}
	}

	public abstract string GetLevelPlayerPrefKey();

	private void Update()
	{
		CheckWinLossConditions();

		if (Input.GetKeyDown(KeyCode.Escape))
        {
			fullScreenMenuManager.Show();
        }

		if (battleState == BattleState.Won && !resultMenuManager.gameObject.activeSelf)
        {
			resultMenuManager.PlayerWon();

			//Set PlayerPref to mark level complete
			string playerPrefKey = this.GetLevelPlayerPrefKey();
			PlayerPrefs.SetInt(playerPrefKey, 1);
		}

		if (battleState == BattleState.Lost && !resultMenuManager.gameObject.activeSelf)
		{
			resultMenuManager.PlayerLost();
		}

		// Do not take interactions if the menu is up
		if (fullScreenMenuManager.gameObject.activeSelf) { return; }

		if (battleState == BattleState.PlayerTurn)
        {
			if (Input.GetMouseButtonDown(0))
			{
				HandleClickAt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
			}

			if (AllyTurnOver()) {
				battleState = BattleState.EnemyTurn;
				playersCameraPosition = mainCameraManager.CurrentCameraPosition();
				characterMovementState = CharacterMovementState.NoCharacterSelected;
				movementData.Reset();
			}

		}

		if (battleState == BattleState.EnemyTurn) {

			if (characterMovementState == CharacterMovementState.NoCharacterSelected)
            {
				characterMovementState = CharacterMovementState.CharacterSelected;
				StartCoroutine(TakeEnemyTurn());
            }


			if (EnemyTurnOver())
            {
				turnUI.text = "Turn " + (++turnCount).ToString();
				ResetEnemyCanMove();
				ResetAllyCanMove();
				movementData.Reset();
				characterMovementState = CharacterMovementState.NoCharacterSelected;
				battleState = BattleState.PlayerTurn;
				mainCameraManager.MoveToPosition(playersCameraPosition);
            }

		}

	}

	private void CheckWinLossConditions()
    {
		if (battleState == BattleState.Won || battleState == BattleState.Lost)
        {
			return;
        }


		// Defeating last enemy while player dies in process should be a loss.
		// Eval loss first
		if (BattleIsLost())
        {
			battleState = BattleState.Lost;
        }
		else if (BattleIsWon())
		{
			battleState = BattleState.Won;
		}

	}

	public virtual bool BattleIsLost() {
		return allyCharacters.Count == 0;
	}

	public virtual bool BattleIsWon()
    {
		return enemyCharacters.Count == 0;
	}

	// Moves one enemy 
	private IEnumerator TakeEnemyTurn()
    {

		foreach (BattlefieldTile tile in GridData.instance.tiles.Values)
		{
			GameObject character = tile.Character;

			if (character == null) { continue; }

			CharacterStats characterStats = character.GetComponent<CharacterStats>();

			if (characterStats == null) { continue; }

			if (characterStats.Team != CharacterTeam.Enemy) { continue; }

			if (!characterStats.CanMove) { continue; }

			// trigger movement animation
			characterStats.clicked = true;

			movementData.currentTile = tile;
			movementData.selectedCharacter = character;
			movementData.selectedCharacterStats = characterStats;

			mainCameraManager.MoveToShow(tile.LocalPlace);

			(BattlefieldTile, BattlefieldTile) movementDecision = characterStats.getMovementDecision(tile);
			
			BattlefieldTile movementChoice = movementDecision.Item1;
			BattlefieldTile attackingTile = movementDecision.Item2;

			movementData.potentialTile = movementChoice;

			yield return new WaitForSecondsRealtime(0.4f);

			cornerDisplay.ShowCharacterStats(movementData.selectedCharacterStats);
			HandleCharacterSelectBFS();

			yield return new WaitForSecondsRealtime(0.4f);

			MoveCharacterTo(movementChoice.LocalPlace);

			yield return new WaitForSecondsRealtime(0.5f);

			if (attackingTile != null)
            {
				yield return new WaitForSecondsRealtime(0.25f);

				int distance = (int) MovementUtils.ManhattanDistance(attackingTile.LocalPlace, movementData.potentialTile.LocalPlace);

				cornerDisplay.HideMenu();

				combatManager.StartCombat(movementData.selectedCharacter, attackingTile.Character, distance);
			}
			else
            {
				WaitMovement();
            }

			// After first character moved, we are done. This function must be called multiple times
			break;
		}
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


	private bool EnemyTurnOver()
	{
		foreach (GameObject enemy in enemyCharacters)
		{
			CharacterStats enemyData = enemy.GetComponent<CharacterStats>();

			if (enemyData.CanMove)
			{
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
		if (stats == null) { return; }

		if (stats.inCombat) { return; }

		if (cornerDisplay)
        {
			if (battleState == BattleState.PlayerTurn && characterMovementState == CharacterMovementState.Attacking)
            {

				if (stats == movementData.selectedCharacterStats) { return; }
				if (stats.Team == CharacterTeam.Ally) { cornerDisplay.ShowCharacterStats(stats); return; }

				// Is the character attackable?
				List<BattlefieldTile> attackableList = MovementUtils.MovementDictToListWithTag(movementLocations, BattlefieldMovementTileTag.Attackable);

				foreach (BattlefieldTile tile in attackableList)
                {
					if (tile.Character && tile.Character.GetComponent<CharacterStats>() == stats)
                    {
						int manhattDist = (int) MovementUtils.ManhattanDistance(movementData.potentialTile.LocalPlace, tile.LocalPlace);
						cornerDisplay.ShowPreCombatWithCharacter(movementData.selectedCharacterStats, stats, manhattDist);
						return;
                    }
                }
            }

			// implicitly not attacking
			if (battleState == BattleState.PlayerTurn && stats.Team == CharacterTeam.Enemy)
            {

				Vector3 enemyLoc = stats.gameObject.transform.position;

				var worldPoint = new Vector3Int(Mathf.FloorToInt(enemyLoc.x), Mathf.FloorToInt(enemyLoc.y), 0);
				var tiles = GridData.instance.tiles; // This is our Dictionary of tiles

				BattlefieldTile enemyTile;
				if (tiles.TryGetValue(worldPoint, out enemyTile))
				{
					enemyMovementShadedTiles = MovementUtils.MovementDictionaryToValidList(MovementUtils.MovementReachableTiles(enemyTile, stats));

					ShadeTilesInListForEnemyMovement(enemyMovementShadedTiles);
					isShadingEnemyMovement = true;
				}



			}


			cornerDisplay.ShowCharacterStats(stats);
			
		}
		else
        {
			statText.text = "HP: " + stats.health
						+ "\nStr: " + stats.strength
						+ "\nDef: " + stats.defense
						+ "\nMov: " + stats.movement;

			statMenu.SetActive(true);
			
		}

	}

	public void MouseLeaveStatCharacter()
    {
		if (isShadingEnemyMovement)
        {
			isShadingEnemyMovement = false;
			UnshadeTilesInListForEnemyMovement(enemyMovementShadedTiles);
		}


		if (cornerDisplay)
		{

			if (characterMovementState == CharacterMovementState.NoCharacterSelected)
			{
				cornerDisplay.HideMenu();
				return;
			}


			// Any time a character is selected, this should exist. However, mouse over is scary
			// There is room for error, so we check
			if (movementData.selectedCharacterStats)
            {
				cornerDisplay.ShowCharacterStats(movementData.selectedCharacterStats);
            }
			else
            {
				cornerDisplay.HideMenu();
            }

		}
		else {
			statMenu.SetActive(false);
		}

    }

	private void DisplayStats()
	{
		if (movementData.selectedCharacterStats)
		{
			cornerDisplay.ShowCharacterStats(movementData.selectedCharacterStats);
		}
	}



	private void MoveMenuTo(Vector3 point)
    {
		return;
		//var rectTransform = movementMenu.GetComponent<RectTransform>();
		//var screenPoint = Camera.main.WorldToScreenPoint(point);

		//var screenRect = Camera.main.pixelRect;

		//var rectTransPoint = new Vector3(
		//	screenPoint.x - screenRect.width / 2.0f,
		//	screenPoint.y - screenRect.height / 2.0f,
		//	screenPoint.z
		//);


		//rectTransform.SetLocalPositionAndRotation(rectTransPoint, Quaternion.identity);

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

		if (battleState != BattleState.PlayerTurn) { return; }
		
		// check tile clicked
		var worldPoint = new Vector3Int(Mathf.FloorToInt(point.x), Mathf.FloorToInt(point.y), 0);
		var tiles = GridData.instance.tiles; // This is our Dictionary of tiles

		BattlefieldTile clickedTile;
		if (tiles.TryGetValue(worldPoint, out clickedTile))
		{
			HandleTileClick(clickedTile);
		}

	}

	private void CancelMovement() {
		MoveCharacterBackTo(movementData.currentTile.WorldLocation);

		characterMovementState = CharacterMovementState.NoCharacterSelected;
		movementData.selectedCharacterStats.clicked = false;
		movementData.Reset();
		movementMenu.SetActive(false);

		

		cornerDisplay.HideMenu();
		HandleCharacterDeselectBFS();
	}

	private void WaitMovement()
    {
		CharacterTurnOver();

		characterMovementState = CharacterMovementState.NoCharacterSelected;
		movementData.selectedCharacterStats.clicked = false;
		movementData.Reset();
		movementMenu.SetActive(false);

		cornerDisplay.HideMenu();
		HandleCharacterDeselectBFS();
	}

	private void AttackMovement(Button sender)
    {
		sender.interactable = false;
		characterMovementState = CharacterMovementState.Attacking;

		HandleCharacterDeselectBFS();
		AttackTileShading();
		ShadeTiles();
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

				if (stats.Team == CharacterTeam.Ally && stats.CanMove)
				{
					movementData.selectedCharacter = clickedTile.Character;
					// for animation
					stats.clicked = true;
					movementData.selectedCharacterStats = stats;
					movementData.currentTile = clickedTile;

					characterMovementState = CharacterMovementState.CharacterSelected;

					DisplayStats();
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

					attackButton.interactable = true;
					movementMenu.SetActive(true);
					characterMovementState = CharacterMovementState.MoveLocationSet;
				}
				else
				{
					movementData.selectedCharacterStats.clicked = false;
					characterMovementState = CharacterMovementState.NoCharacterSelected;
					cornerDisplay.HideMenu();
					movementData.Reset();

					HandleCharacterDeselectBFS();
				}
				break;
			case CharacterMovementState.MoveLocationSet:
				break;
			case CharacterMovementState.Attacking:

				if (!movementLocations.ContainsKey(clickedTile) || movementLocations[clickedTile] != BattlefieldMovementTileTag.Attackable) { break; }

				if (!clickedTile.Character) { break; }

				stats = clickedTile.Character.GetComponent<CharacterStats>();

                if (stats.Team == CharacterTeam.Ally) { break; }

				movementMenu.SetActive(false);

				int distance = (int) MovementUtils.ManhattanDistance(clickedTile.LocalPlace, movementData.potentialTile.LocalPlace);

				cornerDisplay.HideMenu();
				combatManager.StartCombat(movementData.selectedCharacter, clickedTile.Character, distance);

				break;
		}
	}

	public void CombatOver()
    {
		WaitMovement();
	}


	private bool CharacterCanMove(BattlefieldTile clickedTile) {
		return movementLocations.ContainsKey(clickedTile) && movementLocations[clickedTile] == BattlefieldMovementTileTag.Reachable;
	}


	private void MoveCharacterTo(Vector3 location) {
		location.x += 0.5f;
		location.y += 0f;
		location.z = -1;

		movementData.selectedCharacter.transform.position = location;
		
		movementData.currentTile.Character = null;
		movementData.potentialTile.Character = movementData.selectedCharacter;
	}

	private void MoveCharacterBackTo(Vector3 location)
	{
		location.x += 0.5f;
		location.y += 0f;
		location.z = -1;

		movementData.selectedCharacter.transform.position = location;

		movementData.potentialTile.Character = null;
		movementData.currentTile.Character = movementData.selectedCharacter;
	}

	private void CharacterTurnOver()
    {
		movementData.selectedCharacter.GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.4f, 0.6f, 1);
		movementData.selectedCharacterStats.CanMove = false;
		movementData.selectedCharacterStats.clicked = false;
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

        deadCharacter.SetActive(false);
	}

	private void HandleCharacterSelectBFS() {
		movementLocations = MovementUtils.MovementReachableTiles(movementData.currentTile, movementData.selectedCharacterStats);
		ShadeTiles();
	}

	

	private void AttackTileShading()
    {
		movementLocations = MovementUtils.AttackReachableTiles(movementData.potentialTile, movementData.selectedCharacterStats);
	}

    private void UnshadeTiles()
    {
        foreach (var tileKeyPair in movementLocations)
        {
			BattlefieldTile tile = tileKeyPair.Key;

			tile.TilemapMember.SetTileFlags(tile.LocalPlace, TileFlags.None);
            tile.TilemapMember.SetColor(tile.LocalPlace, Color.white);
        }
    }

	private void ShadeTilesInListForEnemyMovement(List<BattlefieldTile> tileList)
    {
		foreach (var tile in tileList)
		{
			Color changeColor = new Color(0.3f, 0.4f, 0.95f, 1);

			tile.TilemapMember.SetTileFlags(tile.LocalPlace, TileFlags.None);
			tile.TilemapMember.SetColor(tile.LocalPlace, changeColor);

		}
	}

	private void UnshadeTilesInListForEnemyMovement(List<BattlefieldTile> tileList)
	{
		foreach (var tile in tileList)
		{
			Color changeColor = new Color(1, 1, 1, 1);

			BattlefieldMovementTileTag value;
			if (movementLocations.TryGetValue(tile, out value))
            {
				if (value == BattlefieldMovementTileTag.Reachable)
				{
					changeColor = new Color(0.4f, 0.5f, 1, 1);
				}
				else if (value == BattlefieldMovementTileTag.Attackable)
				{
					changeColor = new Color(1, 0.4f, 0.4f, 1);
				}
			}

			tile.TilemapMember.SetTileFlags(tile.LocalPlace, TileFlags.None);
			tile.TilemapMember.SetColor(tile.LocalPlace, changeColor);

		}
	}

	private void ShadeTiles()
    {
		foreach (var tileKeyPair in movementLocations)
        {
            Color changeColor = new Color(1, 1, 1, 1);

            if (tileKeyPair.Value == BattlefieldMovementTileTag.Reachable)
            {
                changeColor = new Color(0.4f, 0.5f, 1, 1);
            }
			else if (tileKeyPair.Value == BattlefieldMovementTileTag.Attackable)
			{
				changeColor = new Color(1, 0.4f, 0.4f, 1);
            }

            tileKeyPair.Key.TilemapMember.SetTileFlags(tileKeyPair.Key.LocalPlace, TileFlags.None);
			tileKeyPair.Key.TilemapMember.SetColor(tileKeyPair.Key.LocalPlace, changeColor);

        }
    }

	private void HandleCharacterDeselectBFS()
	{
		UnshadeTiles();
		movementLocations.Clear();
	
	}

	private GameObject PlaceCharacterAt(CharacterSetupInfo info)
	{
		GameObject gameObject = info.character;
		int x = info.x;
		int y = info.y;
		GetMovementDecision movementDecision = info.decisionPattern;


		Vector3 spawnLocation = new Vector3(x + 0.5f, y, -1);
		Vector3Int tileLocation = new Vector3Int(x, y, 0);

		var tiles = GridData.instance.tiles; // This is our Dictionary of tiles

		GameObject spawn = Instantiate(gameObject, spawnLocation, Quaternion.identity);
		CharacterStats spawnStats = spawn.GetComponent<CharacterStats>();
		spawnStats.characterName = info.characterName;
		spawnStats.getMovementDecision = movementDecision;

		StatsMenuMouseOver statMenu = spawn.GetComponent<StatsMenuMouseOver>();

		statMenu.mouseOverStats = MouseOverStats;
		statMenu.mouseLeave = MouseLeaveStatCharacter;


		BattlefieldTile tile;

		if (!tiles.TryGetValue(tileLocation, out tile))
		{
			return null;
		}

		tile.Character = spawn;

		return spawn;
	}

}
