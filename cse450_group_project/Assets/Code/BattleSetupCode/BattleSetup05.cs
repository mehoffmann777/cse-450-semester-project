using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSetup05 : BaseGridManager
{

    public GameObject enemyInfantry;
    public GameObject enemyRanged;
    public GameObject enemyBoss;
    public GameObject enemyRogue;
    public GameObject enemyDefense;

    public GameObject allyInfantry;
    public GameObject allyRogue;
    public GameObject allyDefenseBig;
    public GameObject allyDefense;
    public GameObject allyAwilda;
    public GameObject allySharpShooter;


    public override string GetLevelPlayerPrefKey()
    {
        return "Island4";
    }

    public override List<CharacterSetupInfo> GetEnemySetupPattern()
    {
        return new List<CharacterSetupInfo>
        {
            // Left 
            new CharacterSetupInfo(
                enemyDefense,
                "Mitch",
                -8, 11,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyInfantry,
                "Mary",
                -8, 12,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyRogue,
                "Devon",
                -9, 11,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            // Top
            new CharacterSetupInfo(
                enemyBoss,
                "Liam",
                3, 23,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyRanged,
                "Leone",
                2, 23,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyRanged,
                "Lisandre",
                4, 23,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyInfantry,
                "Naomi",
                3, 24,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            // Right
            new CharacterSetupInfo(
                enemyInfantry,
                "Mike",
                13, 9,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyRanged,
                "Michelle",
                13, 8,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyRogue,
                "Angel",
                13, 7,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyRogue,
                "Datun",
                14, 8,
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
                0, 5,
                EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack
            ),
            new CharacterSetupInfo(
                allyRogue,
                0, 6,
                EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack
            ),
            new CharacterSetupInfo(
                allyAwilda,
                2, 5,
                EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack
            ),
            new CharacterSetupInfo(
                allyDefenseBig,
                2, 6,
                EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack
            ),
            new CharacterSetupInfo(
                allyDefense,
                1, 5,
                EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack
            ),
            new CharacterSetupInfo(
                allySharpShooter,
                1, 6,
                EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack
            ),
        };
    }
}
