using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSetup02 : BaseGridManager
{

    public GameObject enemyInfantry;
    public GameObject enemyInfantryWeakAndFast;

    public GameObject enemyRanged;
    public GameObject enemyRangedWeak;

    public GameObject allyInfantry;
    public GameObject allyRanged;

    private int winZoneMinX = 6;
    private int winZoneMaxX = 8;
    private int winZoneMinY = 23;
    private int winZoneMaxY = 30;

    private int turnWonChecked = 0;

    public override bool BattleIsWon()
    {
        BattleState battleState = GetBattleState();

        // Must end turn with all people there
        if (battleState == BattleState.PlayerTurn) { return false; }

        // Don't check if we already checked this turn
        int curTurn = getTurnCount();
        if (turnWonChecked >= curTurn) { return false; }

        turnWonChecked = curTurn;

        int allyCount = GetAllyCharacters().Count;

        var tiles = GridData.instance.tiles;

        int alliesInZone = 0;

        for (int x = winZoneMinX; x <= winZoneMaxX; x++)
        {
            for (int y = winZoneMinY; y <= winZoneMaxY; y++)
            {
                Vector3Int locToCheck = new Vector3Int(x, y, 0);

                BattlefieldTile tile;

                if (tiles.TryGetValue(locToCheck, out tile))
                {
                    if (tile.Character == null) { continue; }

                    CharacterStats stats = tile.Character.GetComponent<CharacterStats>();

                    if (stats.Team != CharacterTeam.Ally) { continue; }

                    alliesInZone++;
                }

            }
        }

        return alliesInZone == allyCount;
    }


    public override List<CharacterSetupInfo> GetEnemySetupPattern()
    {
        return new List<CharacterSetupInfo>
        {
            // Primary Units
            new CharacterSetupInfo(
                enemyInfantry,
                "Mitch",
                -9, -3,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyInfantry,
                "Mary",
                -11, -2,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            // Top Left Units
            new CharacterSetupInfo(
                enemyInfantry,
                "Devon",
                -15, 20,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyInfantry,
                "Jamie",
                -16, 21,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyInfantry,
                "Amanda",
                -11, 19,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyInfantryWeakAndFast,
                "Tori",
                -15, 17,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyInfantryWeakAndFast,
                "Chalie",
                -12, 17,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyRanged,
                "Cami",
                -15, 16,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            // Bottom Right
            new CharacterSetupInfo(
                enemyInfantry,
                "Liam",
                17, -6,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyInfantry,
                "Leone",
                13, -9,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyInfantryWeakAndFast,
                "Lisandre",
                15, -10,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyRangedWeak,
                "Naomi",
                20, -12,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyInfantry,
                "Charles",
                13, -14,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyRanged,
                "Hannah",
                20, -14,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            // Top Right
            new CharacterSetupInfo(
                enemyRangedWeak,
                "Mike",
                13, 19,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyInfantryWeakAndFast,
                "Michelle",
                19, 19,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyInfantry,
                "Angel",
                21,  17,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyRangedWeak,
                "Datun",
                18, 17,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyInfantry,
                "Xander",
                21, 14,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
        };

    }

    public override List<CharacterSetupInfo> GetAllySetupPattern()
    {
        return new List<CharacterSetupInfo>
        {
            new CharacterSetupInfo(
                allyInfantry,
                -13, -7,
                EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack
            ),
            new CharacterSetupInfo(
                allyInfantry,
                -16, -5,
                EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack
            ),
            new CharacterSetupInfo(
                allyRanged,
                -16, -6,
                EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack
            ),
             new CharacterSetupInfo(
                allyRanged,
                -14, -4,
                EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack
            ),
        };
    }
}
