using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstMapManager : BaseGridManager
{

	public GameObject enemyInfantry;
	public GameObject allyInfantry;


    public override List<CharacterSetupInfo> GetEnemySetupPattern()
    {
        return new List<CharacterSetupInfo>
        {
            new CharacterSetupInfo(enemyInfantry, 1, 1, EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack),
            new CharacterSetupInfo(enemyInfantry, 2, 2, EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack),
            new CharacterSetupInfo(enemyInfantry, 1, 2, EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack),
        };

    }



    //public struct CharacterSetupInfo
    //{
    //    public GameObject character;
    //    public int x;
    //    public int y;
    //    public GetMovementDecision decisionPattern;

    //    public CharacterSetupInfo(GameObject character, int x, int y, GetMovementDecision decisionPattern)
    //    {
    //        this.character = character;
    //        this.x = x;
    //        this.y = y;
    //        this.decisionPattern = decisionPattern;
    //    } 

    //}


    public override List<CharacterSetupInfo> GetAllySetupPattern()
    {
        return new List<CharacterSetupInfo>
        {
            new CharacterSetupInfo(allyInfantry, -5, -4, EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack),
            new CharacterSetupInfo(allyInfantry, -4, -4, EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack),
            new CharacterSetupInfo(allyInfantry, -3, -4, EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack),
        };
    }

  //  enemyCharacters.Add(PlaceCharacterAt(enemyInfantry, 1, 1, EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack));
		//enemyCharacters.Add(PlaceCharacterAt(enemyInfantry, 2, 2, EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack));
		//enemyCharacters.Add(PlaceCharacterAt(enemyInfantry, 1, 2, EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack));
																															
		//allyCharacters.Add(PlaceCharacterAt(allyInfantry, -5, -4, EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack));
		//allyCharacters.Add(PlaceCharacterAt(allyInfantry, -4, -4, EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack));
		//allyCharacters.Add(PlaceCharacterAt(allyInfantry, -3, -4, EnemyMovementPattern.TowardsPlayerEnemyMovementWithAttack));

}
