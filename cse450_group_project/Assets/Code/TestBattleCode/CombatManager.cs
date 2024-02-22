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
    public TMP_Text turnUI;
    public bool combatDone = false;

    public void StartCombat(GameObject ally, GameObject enemy)
    {
        mainCamera.enabled = false;
        combatCamera.enabled = true;
        turnUI.enabled = false;
        mouse.GetComponent<TesterGrid>().enabled = false;
        Vector3 originalAllyPos = ally.transform.position;
        Vector3 originalEnemyPos = enemy.transform.position;
        ally.transform.position = new Vector2(25, 0);
        enemy.transform.position = new Vector2(30, 0);

        // coroutine is so I can use wait in attack, otherwise it cuts back
        // to the grid before you can watch the combat
        StartCoroutine(Attack(ally, enemy, originalAllyPos, originalEnemyPos));
        
    }

    // this function is HUGE and I'll prob break it up
    public IEnumerator Attack(GameObject ally, GameObject enemy, Vector3 originalAllyPos, Vector3 originalEnemyPos)
    {
        
        CharacterStats allyStats = ally.GetComponent<CharacterStats>();
        CharacterStats enemyStats = enemy.GetComponent<CharacterStats>();

        int enemyDodge = Random.Range(0, 10);
        int criticalHit = Random.Range(0, 100);
        if (allyStats.accuracy > enemyDodge)
        {
            bool crit = allyStats.luck > criticalHit;
            int damage = (allyStats.strength * (crit ? 2 : 1)) - enemyStats.defense;
            enemyStats.health -= damage;
            attackText.text = "Player " + (crit ? "Critical " : "") + "Hit " + damage;
        }
        else
        {
            attackText.text = "Player " + "Miss";
        }

        yield return new WaitForSecondsRealtime(3f);

        if (enemyStats.health > 0)
        {
            bool encrit = enemyStats.luck > criticalHit;
            int endamage = (enemyStats.strength * (encrit ? 2 : 1)) - allyStats.defense;
            allyStats.health -= endamage;
            attackText.text = "Enemy " + (encrit ? "Critical " : "") + "Hit " + endamage;

            yield return new WaitForSecondsRealtime(3f);
        }
        allyStats.CanMove = false;
        ally.GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.4f, 0.6f, 1);
        checkHealth(ally);
        checkHealth(enemy);
        combatCamera.enabled = false;
        mainCamera.enabled = true;

        if(ally)
        {
            ally.transform.position = originalAllyPos;
        }

        if (enemy)
        {
            enemy.transform.position = originalEnemyPos;
        }
        mouse.GetComponent<TesterGrid>().enabled = true;
        turnUI.enabled = true;

    }

    public void checkHealth(GameObject unit)
    {
        if(unit.GetComponent<CharacterStats>().health <= 0)
        {
            unit.SetActive(false);
        }
    }
}
