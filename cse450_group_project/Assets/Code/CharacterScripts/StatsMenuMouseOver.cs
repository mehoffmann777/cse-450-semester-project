using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsMenuMouseOver : MonoBehaviour
{

    public GameManager myGameManager;
    private CharacterStats stats;

    public void Start()
    {
        stats = GetComponent<CharacterStats>();
    }

    public void OnMouseOver()
    {
        myGameManager.MouseOverStats(stats);
    }

    public void OnMouseExit()
    {
        myGameManager.MouseLeaveStatCharacter();
    }
}
