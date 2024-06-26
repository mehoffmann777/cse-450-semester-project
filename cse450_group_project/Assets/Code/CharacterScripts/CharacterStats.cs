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
    public Sprite characterImage;
    public string characterName = "Grunt";
    public int startingHealth;
    public int health;
    public int strength;
    public int defense;
    public int dex;
    public int luck;
    public int movement;

    public int minRangeInclusive = 1;
    public int maxRangeInclusive = 1;

    public bool CanMove = true;
    public bool isBoss;
    public CharacterTeam Team;

    public GetMovementDecision getMovementDecision;

    public bool inCombat = false;
    public bool clicked = false;

    public bool buffApplied = false;

    void applyBuff(string stat, int buff)
    {
        if (!buffApplied)
        {
            CharacterStats stats = GetComponent<CharacterStats>();

            stats.strength += 2;
            stats.defense += 1;
            buffApplied = true;
        }
    }

}
