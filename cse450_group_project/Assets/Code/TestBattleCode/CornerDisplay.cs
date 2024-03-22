using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornerDisplay : MonoBehaviour
{
    public GameObject CornerMenu;
    public PreCombatMenuManager PreCombatMenu;
    public StatMenuManager CharacterStatsMenu;

    private void Start()
    {
        CornerMenu.SetActive(false);
    }

    public void HideMenu()
    {
        CornerMenu.SetActive(false);
    }

    public void ShowCharacterStats(CharacterStats stats)
    {
        CharacterStatsMenu.UpdateForCharacterStats(stats);
        CornerMenu.SetActive(true);
        CharacterStatsMenu.Show();
        PreCombatMenu.Hide();
    }

    public void ShowPreCombatWithCharacter(CharacterStats ally, CharacterStats enemy, int distanceApart)
    {
        PreCombatMenu.UpdateForCharacters(ally, enemy, distanceApart);
        CornerMenu.SetActive(true);
        PreCombatMenu.Show();
        CharacterStatsMenu.Hide();
    }

}
