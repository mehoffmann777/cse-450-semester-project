using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSetup01 : BaseGridManager
{

	public GameObject enemyInfantry;
	public GameObject allyInfantry;


    public override List<CharacterSetupInfo> GetEnemySetupPattern()
    {
        return new List<CharacterSetupInfo>
        {
            new CharacterSetupInfo(enemyInfantry, 16, 6, EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack),
            new CharacterSetupInfo(enemyInfantry, 15, 6, EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack),
            new CharacterSetupInfo(enemyInfantry, 16, -6, EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack),
            new CharacterSetupInfo(enemyInfantry, 16, -7, EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack),
            new CharacterSetupInfo(enemyInfantry, 15, -5, EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack),
            new CharacterSetupInfo(enemyInfantry, 15, -4, EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack),
        };

    }

    public override List<CharacterSetupInfo> GetAllySetupPattern()
    {
        return new List<CharacterSetupInfo>
        {
            new CharacterSetupInfo(allyInfantry, -17, -4, EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack),
            new CharacterSetupInfo(allyInfantry, -16, -5, EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack),
            new CharacterSetupInfo(allyInfantry, -16, -6, EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack),
        };
    }

}
