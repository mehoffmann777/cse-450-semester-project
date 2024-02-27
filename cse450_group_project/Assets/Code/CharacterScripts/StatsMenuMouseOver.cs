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
        mouseOverStats(stats);
    }

    public void OnMouseExit()
    {
        mouseLeave();
    }
}
