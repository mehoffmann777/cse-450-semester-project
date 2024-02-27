using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public delegate (BattlefieldTile, BattlefieldTile) GetMovementDecision(BattlefieldTile currentTile);

public class CharacterStats : MonoBehaviour
{

    public int health;
    public int stamina;
    public int strength;
    public int accuracy;
    public int defense;
    public int luck;
    public int movement;

    public bool CanMove = true;
    public int Team;

    public GetMovementDecision getMovementDecision;
}
