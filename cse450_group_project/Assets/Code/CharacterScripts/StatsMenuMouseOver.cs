using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsMenuMouseOver : MonoBehaviour
{
    public MouseOverStats mouseOverStats;
    public MouseLeaveStatCharacter mouseLeave;

    private CharacterStats stats;

    public void Start()
    {
        stats = GetComponent<CharacterStats>();
      
    }

   
    public void OnMouseOver()
    {
        if (stats != null && stats.Team == CharacterTeam.Ally && !stats.inCombat)
        {
            GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.4f, 0.6f, 1);
        }
        mouseOverStats(stats);
    }

    public void OnMouseExit()
    {
        if (stats != null && stats.CanMove)
        {
            GetComponent<SpriteRenderer>().color = new Color(1,1,1,1);
        }
        mouseLeave();
    }
}
