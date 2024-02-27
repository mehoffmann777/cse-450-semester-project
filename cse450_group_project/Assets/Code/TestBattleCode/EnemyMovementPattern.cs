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
			float dist = ManhattanDistance(movTile.LocalPlace, positionOfRandomAlly);

			if (dist < distanceToAlly)
			{
				distanceToAlly = dist;
				movementChoice = movTile;
			}
		}


		return (movementChoice, null);
	}

	private static float ManhattanDistance(Vector3 v1, Vector3 v2)
	{
		return Mathf.Abs(v1.x - v2.x) + Mathf.Abs(v1.y - v2.y);
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

			if (characterStats.Team != 0) { continue; }

			locations.Add(tile.LocalPlace);
		}

		int choiceIndex = Random.Range(0, locations.Count);
		return locations[choiceIndex];
	}


}
