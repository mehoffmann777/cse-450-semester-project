using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreCombatMenuManager : MonoBehaviour
{
    public GameObject menu;

    // Image 
    public TMPro.TextMeshProUGUI nameBadgeAlly;
    public TMPro.TextMeshProUGUI nameBadgeEnemy;


    public TMPro.TextMeshProUGUI hpDisplayAlly;
    public TMPro.TextMeshProUGUI attackDisplayAlly;
    public TMPro.TextMeshProUGUI hitDisplayAlly;
    public TMPro.TextMeshProUGUI critDisplayAlly;
    public TMPro.TextMeshProUGUI hpDisplayEnemy;
    public TMPro.TextMeshProUGUI attackDisplayEnemy;
    public TMPro.TextMeshProUGUI hitDisplayEnemy;
    public TMPro.TextMeshProUGUI critDisplayEnemy;


    public void UpdateForCharacters(CharacterStats ally, CharacterStats enemy)
    {
        
    }

    public void Hide()
    {
        menu.SetActive(false);
    }

    public void Show()
    {
        menu.SetActive(true);
    }
}
