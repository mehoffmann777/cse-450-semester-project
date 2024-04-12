using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSetup01 : BaseGridManager
{

	public GameObject enemyInfantry;
    public GameObject enemyRanged;
    public GameObject enemyRogue;

    public GameObject allyInfantry;
    public GameObject allyAwilda;
    public GameObject allyRogue;

    public override string GetLevelPlayerPrefKey()
    {
        return "Island0";
    }


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
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyInfantry,
                "James Jamison",
                16, -7,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
            ),
            new CharacterSetupInfo(
                enemyRogue,
                "Tom Thomas",
                15, -5,
                EnemyMovementPattern.MaximizeTotalDamageImmediate
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
                allyRogue,
                -16, -5,
                EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack
            ),
            new CharacterSetupInfo(
                allyAwilda,
                -16, -6,
                EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack
            ),
        };
    }

}
