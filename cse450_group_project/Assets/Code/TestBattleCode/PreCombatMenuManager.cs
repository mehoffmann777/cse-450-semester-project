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


    public void UpdateForCharacters(CharacterStats ally, CharacterStats enemy, int distanceApart)
    {
        CombatManager.PredictCombatResults results = CombatManager.PredictCombat(ally, enemy);

        hpDisplayAlly.text = ally.health + " / " + ally.startingHealth;
        attackDisplayAlly.text = results.AttackerDamageIfHit.ToString();
        hitDisplayAlly.text = string.Format("{0:0%}", results.AttackerHitChance);
        critDisplayAlly.text = string.Format("{0:0%}", results.AttackerCritChance);

        if (CombatManager.DefenderCanCoutner(enemy, distanceApart))
        {
            hpDisplayEnemy.text = enemy.health + " / " + enemy.startingHealth;
            attackDisplayEnemy.text = results.DefenderDamageIfHit.ToString();
            hitDisplayEnemy.text = string.Format("{0:0%}", results.DefenderHitChance);
            critDisplayEnemy.text = string.Format("{0:0%}", results.DefenderCritChance);
        }
        else
        {
            hpDisplayEnemy.text = enemy.health + " / " + enemy.startingHealth;
            attackDisplayEnemy.text = "-";
            hitDisplayEnemy.text = "-";
            critDisplayEnemy.text = "-";
        }
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
