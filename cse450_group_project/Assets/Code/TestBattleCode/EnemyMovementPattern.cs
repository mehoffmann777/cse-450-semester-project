using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementPattern
{

	//private void RandomEnemyMovement()
	//{

	//	foreach (BattlefieldTile tile in GridData.instance.tiles.Values)
	//	{
	//		GameObject character = tile.Character;

	//		if (character == null) { continue; }

	//		CharacterStats characterStats = character.GetComponent<CharacterStats>();

	//		if (characterStats == null) { continue; }

	//		if (characterStats.Team == 0) { continue; }

	//		if (!characterStats.CanMove) { continue; }

	//		movementData.currentTile = tile;
	//		movementData.selectedCharacter = character;
	//		movementData.selectedCharacterStats = characterStats;

	//		TagReachableBySelectedChar();

	//		int movementChoiceIndex = Random.Range(0, validMovementLocations.Count);
	//		Vector3 movementChoice = validMovementLocations[movementChoiceIndex];

	//		GridData.instance.tiles.TryGetValue(movementChoice, out movementData.potentialTile);
	//		MoveCharacterTo(movementChoice);
	//		CharacterTurnOver();

	//		movementData.Reset();
	//		HandleCharacterDeselectBFS();
	//	}

	//	ResetEnemyCanMove();
	//}

	public static (BattlefieldTile, BattlefieldTile) TowardsPlayerEnemyMovementWithAttack(BattlefieldTile startTile)
	{

		CharacterStats stats = startTile.Character.GetComponent<CharacterStats>();
		var movementLocations = MovementUtils.MovementReachableTiles(startTile, stats);
		var movLocList = MovementUtils.MovementDictionaryToValidList(movementLocations);

		Vector3 positionOfRandomAlly = GetRandomAllyPosition();

		float distanceToAlly = int.MaxValue;
		BattlefieldTile movementChoice = movLocList[0];

		foreach (BattlefieldTile movTile in movLocList)
		{
			float dist = MovementUtils.ManhattanDistance(movTile.LocalPlace, positionOfRandomAlly);

			if (dist < distanceToAlly)
			{
				distanceToAlly = dist;
				movementChoice = movTile;
			}
		}

		Dictionary<BattlefieldTile, BattlefieldMovementTileTag> attackableTileDict = MovementUtils.AttackReachableTiles(movementChoice, stats);

		BattlefieldTile tileAttack = null;

		foreach (KeyValuePair<BattlefieldTile, BattlefieldMovementTileTag> pair in attackableTileDict)
        {
			if (pair.Value != BattlefieldMovementTileTag.Attackable) { continue; }

			if (pair.Key.Character == null) { continue; }

			CharacterStats charStats = pair.Key.Character.GetComponent<CharacterStats>();

			if (charStats.Team != CharacterTeam.Ally) { continue; }

			tileAttack = pair.Key;
			break; // We don't need to search for more than one
		}


		return (movementChoice, tileAttack);
	}




	public static (BattlefieldTile, BattlefieldTile) AttackMinHealth(BattlefieldTile startTile)
	{

		CharacterStats stats = startTile.Character.GetComponent<CharacterStats>();

		// Determine who to attack
		List<BattlefieldTile> allAllies = GetAllAllyTiles();
		BattlefieldTile allyToAttack = allAllies[0];
		int minHealth = int.MaxValue;

		foreach (BattlefieldTile allyTile in allAllies)
        {
			CombatManager.PredictCombatResults combatResults = CombatManager.PredictCombat(stats, allyTile.Character.GetComponent<CharacterStats>());

			if (combatResults.DefenderHealth < minHealth)
            {
				minHealth = combatResults.DefenderHealth;
				allyToAttack = allyTile;
            }

        }

		// Setup for search
		var movementLocations = MovementUtils.MovementReachableTiles(startTile, stats);
		HashSet<BattlefieldTile> movementLocationSet = new HashSet<BattlefieldTile>(MovementUtils.MovementDictionaryToValidList(movementLocations));

		// Find tiles that can attack the ally based on enemy range stats
		var attackableGoalTiles = MovementUtils.AttackReachableTiles(allyToAttack, stats);
		HashSet<BattlefieldTile> attackableGoalSet = new HashSet<BattlefieldTile>(MovementUtils.MovementDictToListWithTag(attackableGoalTiles, BattlefieldMovementTileTag.Attackable));


		MovementUtils.TileInSetToGoalReturn movementDataReturn = MovementUtils.TileInSetClosestOnMinPathToAGoal(startTile, movementLocationSet, attackableGoalSet);
		BattlefieldTile movementChoice = movementDataReturn.tileToMoveTo;

		var attackableTiles = MovementUtils.AttackReachableTiles(movementChoice, stats);

		BattlefieldTile tileAttack = null;
		BattlefieldMovementTileTag attackTag = attackableTiles.GetValueOrDefault(allyToAttack, BattlefieldMovementTileTag.None);

		if (attackTag == BattlefieldMovementTileTag.Attackable)
        {
			tileAttack = allyToAttack;
        }


		return (movementChoice, tileAttack);
	}



	public static (BattlefieldTile, BattlefieldTile) MaximizeTotalDamageImmediate(BattlefieldTile startTile)
	{

		CharacterStats stats = startTile.Character.GetComponent<CharacterStats>();


		// Setup for search
		var movementLocations = MovementUtils.MovementReachableTiles(startTile, stats);
		HashSet<BattlefieldTile> movementLocationSet = new HashSet<BattlefieldTile>(MovementUtils.MovementDictionaryToValidList(movementLocations));



		// Determine who to attack
		List<BattlefieldTile> allAllies = GetAllAllyTiles();
		BattlefieldTile allyToAttack = allAllies[0];
		float maxDamageInTurns = 0;

		BattlefieldTile movementChoice = startTile;


		foreach (BattlefieldTile allyTile in allAllies)
		{
			CombatManager.PredictCombatResults combatResults = CombatManager.PredictCombat(stats, allyTile.Character.GetComponent<CharacterStats>());

			// Find tiles that can attack the ally based on enemy range stats
			var attackableGoalTiles = MovementUtils.AttackReachableTiles(allyTile, stats);
			HashSet<BattlefieldTile> attackableGoalSet = new HashSet<BattlefieldTile>(MovementUtils.MovementDictToListWithTag(attackableGoalTiles, BattlefieldMovementTileTag.Attackable));

			MovementUtils.TileInSetToGoalReturn movementDataReturn = MovementUtils.TileInSetClosestOnMinPathToAGoal(startTile, movementLocationSet, attackableGoalSet);

			int turnsForFullPath = (movementDataReturn.totalPathDistance / stats.movement);

			if (turnsForFullPath < 1)
            {
				turnsForFullPath = 1;
            }

			float damageInTurns = (combatResults.AttackerDamageIfHit * combatResults.AttackerHitChance) / turnsForFullPath;

			if (damageInTurns > maxDamageInTurns)
            {
				maxDamageInTurns = damageInTurns;
				allyToAttack = allyTile;
				movementChoice = movementDataReturn.tileToMoveTo;
			}

		}


		var attackableTiles = MovementUtils.AttackReachableTiles(movementChoice, stats);

		BattlefieldTile tileAttack = null;
		BattlefieldMovementTileTag attackTag = attackableTiles.GetValueOrDefault(allyToAttack, BattlefieldMovementTileTag.None);

		if (attackTag == BattlefieldMovementTileTag.Attackable)
		{
			tileAttack = allyToAttack;
		}


		return (movementChoice, tileAttack);
	}










	public static (BattlefieldTile, BattlefieldTile) TowardsPlayerEnemyMovement(BattlefieldTile startTile)
	{

		CharacterStats stats = startTile.Character.GetComponent<CharacterStats>();
		var movementLocations = MovementUtils.MovementReachableTiles(startTile, stats);
		var movLocList = MovementUtils.MovementDictionaryToValidList(movementLocations);

		Vector3 positionOfRandomAlly = GetRandomAllyPosition();

		float distanceToAlly = int.MaxValue;
		BattlefieldTile movementChoice = movLocList[0];

		foreach (BattlefieldTile movTile in movLocList)
		{
			float dist = MovementUtils.ManhattanDistance(movTile.LocalPlace, positionOfRandomAlly);

			if (dist < distanceToAlly)
			{
				distanceToAlly = dist;
				movementChoice = movTile;
			}
		}


		return (movementChoice, null);
	}

	

	private static Vector3 GetRandomAllyPosition()
	{

		List<Vector3> locations = new();

		foreach (BattlefieldTile tile in GridData.instance.tiles.Values)
		{
			GameObject character = tile.Character;

			if (character == null) { continue; }

			CharacterStats characterStats = character.GetComponent<CharacterStats>();

			if (characterStats == null) { continue; }

			if (characterStats.Team != CharacterTeam.Ally) { continue; }

			locations.Add(tile.LocalPlace);
		}

		int choiceIndex = Random.Range(0, locations.Count);
		return locations[choiceIndex];
	}


	private static List<BattlefieldTile> GetAllAllyTiles()
    {
		List<BattlefieldTile> locations = new();

		foreach (BattlefieldTile tile in GridData.instance.tiles.Values)
		{
			GameObject character = tile.Character;

			if (character == null) { continue; }

			CharacterStats characterStats = character.GetComponent<CharacterStats>();

			if (characterStats == null) { continue; }

			if (characterStats.Team != CharacterTeam.Ally) { continue; }

			locations.Add(tile);
		}

		return locations;
	}
}



