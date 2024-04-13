using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSetup03 : BaseGridManager
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
        return "Island2";
    }

    public override List<CharacterSetupInfo> GetEnemySetupPattern()
    {
        return new List<CharacterSetupInfo>
        {
            // Trees on Right
            new CharacterSetupInfo(
                enemyBoss,
                "Mitch",
                12, 17,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyInfantry,
                "Mary",
                12, 13,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyRogue,
                "Devon",
                15, 15,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyRanged,
                "Jamie",
                14, 14,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            // Bottom Right
            new CharacterSetupInfo(
                enemyInfantry,
                "Liam",
                13, 4,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyInfantry,
                "Leone",
                14, -2,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyRanged,
                "Lisandre",
                16, 2,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyRogue,
                "Naomi",
                13, -3,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            // Forrest Left
            new CharacterSetupInfo(
                enemyDefense,
                "Mike",
                -13, 2,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyRogue,
                "Michelle",
                -16, 6,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyRogue,
                "Angel",
                -13, 7,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyRogue,
                "Datun",
                -15, 8,
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
                2, 11,
                EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack
            ),
            new CharacterSetupInfo(
                allyRogue,
                3, 9,
                EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack
            ),
            new CharacterSetupInfo(
                allyAwilda,
                1, 10,
                EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack
            ),
            new CharacterSetupInfo(
                allyDefenseBig,
                -1, 9,
                EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack
            ),
            new CharacterSetupInfo(
                allyDefense,
                1, 9,
                EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack
            ),
            new CharacterSetupInfo(
                allySharpShooter,
                6, 10,
                EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack
            ),
        };
    }
}
