using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public delegate (BattlefieldTile, BattlefieldTile) GetMovementDecision(BattlefieldTile currentTile);

public enum CharacterTeam
{
    Ally,
    Enemy,
    Other
}


public class CharacterStats : MonoBehaviour
{
    public string characterName = "Grunt";
    public int startingHealth;
    public int health;
    public int stamina;
    public int strength;
    public int accuracy;
    public int defense;
    public int luck;
    public int movement;

    public int minRangeInclusive = 1;
    public int maxRangeInclusive = 1;

    public bool CanMove = true;
    public CharacterTeam Team;

    public GetMovementDecision getMovementDecision;

    public bool inCombat = false;
    public bool clicked = false;

}
