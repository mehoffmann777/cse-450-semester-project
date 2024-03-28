using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatMenuManager : MonoBehaviour
{

    public GameObject menu;

    // Image 
    public TMPro.TextMeshProUGUI nameBadge;

    public TMPro.TextMeshProUGUI hpValueDisplay;
    public TMPro.TextMeshProUGUI strValueDisplay;
    public TMPro.TextMeshProUGUI defValueDisplay;
    public TMPro.TextMeshProUGUI luckValueDisplay;
    public TMPro.TextMeshProUGUI dexValueDisplay;
    public TMPro.TextMeshProUGUI movValueDisplay;
    public TMPro.TextMeshProUGUI rangeValueDisplay;


    public void UpdateForCharacterStats(CharacterStats stats)
    {
        nameBadge.text = stats.characterName;
        hpValueDisplay.text = stats.health + " / " + stats.startingHealth;

        strValueDisplay.text = stats.strength.ToString();
        defValueDisplay.text = stats.defense.ToString();
        luckValueDisplay.text = stats.luck.ToString();
        dexValueDisplay.text = stats.dex.ToString();
        movValueDisplay.text = stats.movement.ToString();

        if (stats.minRangeInclusive == stats.maxRangeInclusive)
        {
            rangeValueDisplay.text = stats.minRangeInclusive.ToString();
        }
        else
        {
            rangeValueDisplay.text = stats.minRangeInclusive + "-" + stats.maxRangeInclusive;
        }
    }

    public void Hide() {
        menu.SetActive(false);
    }

    public void Show() {
        menu.SetActive(true);
    }

}
