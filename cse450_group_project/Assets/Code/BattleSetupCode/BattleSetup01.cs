using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSetup01 : BaseGridManager
{

	public GameObject enemyInfantry;
    public GameObject enemyRanged;

    public GameObject allyInfantry;
    public GameObject allyRanged;

    public override List<CharacterSetupInfo> GetEnemySetupPattern()
    {
        return new List<CharacterSetupInfo>
        {
            new CharacterSetupInfo(
                enemyInfantry,
                "Vera Waters",
                16, 6,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyRanged,
                "Esme Waters",
                15, 6,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyInfantry,
                "Pete Peter",
                16, -6,
                EnemyMovementPattern.AttackMinHealth
            ),
            new CharacterSetupInfo(
                enemyInfantry,
                "James Jamison",
                16, -7,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyInfantry,
                "Tom Thomas",
                15, -5,
                EnemyMovementPattern.AttackMinHealth
            ),
            new CharacterSetupInfo(
                enemyRanged,
                "Dob Donalds",
                15, -4,
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
                -17, -4,
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
        };
    }

}
