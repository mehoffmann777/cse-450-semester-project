using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CombatManager : MonoBehaviour
{
    public GameObject mouse;
    public Camera mainCamera;
    public Camera combatCamera;
    public TextMeshProUGUI attackText;
    public bool combatDone = false;

    public void StartCombat(GameObject ally, GameObject enemy)
    {

        mouse.GetComponent<TesterGrid>().enabled = false;
        Vector3 originalAllyPos = ally.transform.position;
        Vector3 originalEnemyPos = enemy.transform.position;
        ally.transform.position = new Vector2(25, 0);
        enemy.transform.position = new Vector2(30, 0);

  
        StartCoroutine(Attack(ally, enemy, originalAllyPos, originalEnemyPos));
        AttackDelay();

        print("here");
        
    }

    IEnumerator AttackDelay()
    {
        yield return new WaitForSecondsRealtime(7f);
    }

    public IEnumerator Attack(GameObject ally, GameObject enemy, Vector3 originalAllyPos, Vector3 originalEnemyPos)
    {
        
        print("attacking");
        CharacterStats allyStats = ally.GetComponent<CharacterStats>();
        CharacterStats enemyStats = enemy.GetComponent<CharacterStats>();
        int enemyDodge = Random.Range(0, 100);
        int criticalHit = Random.Range(0, 100);
        if (allyStats.accuracy > enemyDodge)
        {
            bool crit = allyStats.luck > criticalHit;
            int damage = (allyStats.strength * (crit ? 2 : 1)) - enemyStats.defense;
            attackText.text = "Player " + (crit ? "Critical " : "") + "Hit " + damage;
        }
        else
        {
            attackText.text = "Player " + "Miss";
        }

        yield return new WaitForSecondsRealtime(3f);

        bool encrit = enemyStats.luck > criticalHit;
        int endamage = (enemyStats.strength * (encrit ? 2 : 1)) - allyStats.defense;
        attackText.text = "Enemy " + (encrit ? "Critical " : "") + "Hit " + endamage;


        this.combatDone = true;
        yield return new WaitForSecondsRealtime(3f);
        print("shiftback");
        combatCamera.enabled = false;
        mainCamera.enabled = true;

        ally.transform.position = originalAllyPos;
        enemy.transform.position = originalEnemyPos;
        mouse.GetComponent<TesterGrid>().enabled = true;

    }
}
