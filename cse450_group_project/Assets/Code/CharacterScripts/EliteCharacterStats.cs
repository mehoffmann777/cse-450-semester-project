using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteCharacterStatus : MonoBehaviour
{
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
