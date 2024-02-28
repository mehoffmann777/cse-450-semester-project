using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BattlefieldMovementTileTag {
	None,
	Reachable,
	BlockedByAlly,
	Attackable
}


public class MovementUtils
{

	private static void AddTileDecreasing(LinkedList<BattlefieldTile> list, BattlefieldTile tile)
	{
		LinkedListNode<BattlefieldTile> n = list.First;

		if (n == null) { list.AddFirst(tile); return; }


		while (tile.ReachableInDistance < n.Value.ReachableInDistance)
		{
			if (n.Next == null)
			{
				list.AddAfter(n, tile);
				return;
			}

			n = n.Next;
		}

		list.AddBefore(n, tile);
	}

	public static List<BattlefieldTile> MovementDictionaryToValidList(Dictionary<BattlefieldTile, BattlefieldMovementTileTag> dictionary)
    {
		List<BattlefieldTile> tileList = new();

		foreach(KeyValuePair<BattlefieldTile, BattlefieldMovementTileTag> pair in dictionary)
        {
			if (pair.Value == BattlefieldMovementTileTag.Reachable)
            {
				tileList.Add(pair.Key);
            }
        }

		return tileList;
    }

	public static Dictionary<BattlefieldTile, BattlefieldMovementTileTag> AttackReachableTiles(BattlefieldTile tile, CharacterStats character)
    {
		LinkedList<BattlefieldTile> queue = new();
		List<BattlefieldTile> visited = new();

		var tiles = GridData.instance.tiles;

		Vector3Int loc;

		int maxRangeInclusive = 1;
		int minRangeInclusive = 1; // cannot attack at 0

		tile.ReachableInDistance = 0;
		visited.Add(tile);

		queue.AddFirst(tile);

		while (queue.Count > 0)
		{
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

			foreach (Vector3Int possibleLoc in locToCheck)
			{

				if (tiles.TryGetValue(possibleLoc, out tileToCheck))
				{

					if (visited.Contains(tileToCheck)) { continue; }

					visited.Add(tileToCheck);

					tileToCheck.ReachableInDistance = thisTile.Value.ReachableInDistance + 1;

					if (tileToCheck.ReachableInDistance > maxRangeInclusive) { continue; }

					AddTileDecreasing(queue, tileToCheck);
				}
			}
		}

		Dictionary<BattlefieldTile, BattlefieldMovementTileTag> attackReachable = new();

		foreach (BattlefieldTile visitedTile in visited)
		{
			if (visitedTile.ReachableInDistance < minRangeInclusive) { continue; }
			if (visitedTile.ReachableInDistance > maxRangeInclusive) { continue; }

			attackReachable[visitedTile] = BattlefieldMovementTileTag.Attackable;
		}

		return attackReachable;
	}

	public static Dictionary<BattlefieldTile, BattlefieldMovementTileTag> MovementReachableTiles(BattlefieldTile tile, CharacterStats character)
	{
		Dictionary<BattlefieldTile, BattlefieldMovementTileTag> movementLocations = new();
		LinkedList<BattlefieldTile> queue = new();

		var tiles = GridData.instance.tiles;

		Vector3Int loc;

		int mov = character.movement;

		tile.ReachableInDistance = 0;
		movementLocations[tile] = BattlefieldMovementTileTag.Reachable;

		queue.AddFirst(tile);

		while (queue.Count > 0)
		{
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

			foreach (Vector3Int possibleLoc in locToCheck)
			{

				if (tiles.TryGetValue(possibleLoc, out tileToCheck))
				{

					if (movementLocations.ContainsKey(tileToCheck) && movementLocations[tileToCheck] != BattlefieldMovementTileTag.None) { continue; }

					if (tileToCheck.Impassable) { continue; }

					//Cannot move through other team
					if (tileToCheck.Character && tileToCheck.Character.GetComponent<CharacterStats>().Team != character.Team) { continue; }

					tileToCheck.ReachableInDistance = thisTile.Value.ReachableInDistance + tileToCheck.MovementCost;

					if (tileToCheck.ReachableInDistance > mov) { continue; }


					if (tileToCheck.Character != null)
					{
						movementLocations[tileToCheck] = BattlefieldMovementTileTag.BlockedByAlly;
					}
					else
					{
						movementLocations[tileToCheck] = BattlefieldMovementTileTag.Reachable;
					}

					AddTileDecreasing(queue, tileToCheck);
				}
			}
		}

		return movementLocations;
	}

}