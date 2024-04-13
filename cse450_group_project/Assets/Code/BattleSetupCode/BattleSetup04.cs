using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSetup04 : BaseGridManager
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
        return "Island3";
    }

    public override List<CharacterSetupInfo> GetEnemySetupPattern()
    {
        return new List<CharacterSetupInfo>
        {
            // Bottom 
            new CharacterSetupInfo(
                enemyDefense,
                "Mitch",
                6, 1,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyInfantry,
                "Mary",
                8, 1,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyRogue,
                "Devon",
                5, -1,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyRogue,
                "Jamie",
                7, 1,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            // Middle
            new CharacterSetupInfo(
                enemyBoss,
                "Liam",
                14, 11,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyRanged,
                "Leone",
                10, 8,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyRanged,
                "Lisandre",
                17, 14,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyInfantry,
                "Naomi",
                14, 15,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyInfantry,
                "Craig",
                15, 8,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            // Top
            new CharacterSetupInfo(
                enemyInfantry,
                "Mike",
                1, 22,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyRanged,
                "Michelle",
                1, 20,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyRogue,
                "Angel",
                0, 19,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyRogue,
                "Datun",
                3, 20,
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
                -12, 12,
                EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack
            ),
            new CharacterSetupInfo(
                allyRogue,
                -16, 0,
                EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack
            ),
            new CharacterSetupInfo(
                allyAwilda,
                -16, 9,
                EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack
            ),
            new CharacterSetupInfo(
                allyDefenseBig,
                -11, 0,
                EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack
            ),
            new CharacterSetupInfo(
                allyDefense,
                -17, 12,
                EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack
            ),
            new CharacterSetupInfo(
                allySharpShooter,
                -12, 1,
                EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack
            ),
        };
    }
}
