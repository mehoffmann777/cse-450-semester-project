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

    public CharacterDead characterDead;
    public CombatOver combatOver;
    public SpriteRenderer enemySprite;
    public SpriteRenderer allySprite;
    public Sprite allyGridSprite;
    public Sprite enemyGridSprite;
    public Sprite enemyCombatSprite;
    public Sprite allyCombatSprite;

    public int enemyDamage;
    public int enemyHitChance;
    public int allyDamage;
    public int allyHitChance;

    // so enemy AI can make smart decisions about who to attack
    public Dictionary<string, int> PredictCombat(GameObject ally, GameObject enemy)
    {
        CharacterStats allyStats = ally.GetComponent<CharacterStats>();
        CharacterStats enemyStats = enemy.GetComponent<CharacterStats>();

        // does not account for crits bc that's rolled on attack
        allyDamage = allyStats.strength - enemyStats.defense;
        enemyDamage = enemyStats.strength - allyStats.defense;

        // will rework stats to factor in both accuracy + opponent's something to determine dodge
        // dodge is just rolled as a set 0-10 on attack rn, but this could be changed 
        enemyHitChance = enemyStats.accuracy;
        allyHitChance = allyStats.accuracy;

        int allyHealthAfterCombat = allyStats.health - enemyDamage;
        int enemyHealthAfterCombat = enemyStats.health - allyDamage;

        int wouldKillAlly = allyHealthAfterCombat <= 0 ? 1 : 0;
        int wouldKillEnemy = enemyHealthAfterCombat <= 0 ? 1 : 0;

        Dictionary<string, int> combatPredictions = new Dictionary<string, int>{
            { "AllyHitChance", allyHitChance },
            { "EnemyHitChance", enemyHitChance },
            { "AllyHealth", allyHealthAfterCombat  },
            { "EnemyHealth", enemyHealthAfterCombat },
            { "WouldKillAlly", wouldKillAlly },
            { "WouldKillEnemy", wouldKillEnemy }

        };

        return combatPredictions;

    }

    public void StartCombat(GameObject ally, GameObject enemy)
    {
        mainCamera.enabled = false;
        combatCamera.enabled = true;
        turnUI.enabled = false;
        mouse.GetComponent<TesterGrid>().enabled = false;

        CharacterStats allyStats = ally.GetComponent<CharacterStats>();
        CharacterStats enemyStats = enemy.GetComponent<CharacterStats>();

        allyStats.inCombat = true;
        enemyStats.inCombat = true;

        enemySprite = enemy.GetComponent<SpriteRenderer>();
        allySprite = ally.GetComponent<SpriteRenderer>();

        allySprite.sprite = allyCombatSprite;
        enemySprite.sprite = enemyCombatSprite;
        enemySprite.flipX = true;
    
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
        attackText.color = Color.white;
        if (allyStats.accuracy > enemyDodge)
        {
            bool crit = allyStats.luck > criticalHit;
            int damage = (allyStats.strength * (crit ? 2 : 1)) - enemyStats.defense;
            enemyStats.health -= damage;
            attackText.text = "Player " + (crit ? "Critical " : "") + "Hit " + damage;
        }
        else
        {
            attackText.text = "Player Miss";
        }

        yield return new WaitForSecondsRealtime(3f);

        if (enemyStats.health > 0)
        {
            int playerDodge = Random.Range(0, 10);

            attackText.color = Color.red;
            if (enemyStats.accuracy > playerDodge)
            {
                criticalHit = Random.Range(0, 100);
                bool encrit = enemyStats.luck > criticalHit;
                int endamage = (enemyStats.strength * (encrit ? 2 : 1)) - allyStats.defense;
                allyStats.health -= endamage;
                attackText.text = "Enemy " + (encrit ? "Critical " : "") + "Hit " + endamage;
            } else
            {
                attackText.text = "Enemy Miss";
            }
            yield return new WaitForSecondsRealtime(3f);
        }
        //allyStats.CanMove = false;
        //ally.GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.4f, 0.6f, 1);

        combatCamera.enabled = false;
        mainCamera.enabled = true;

        if (allyStats.health <= 0)
        {
            characterDead(ally);
        }

        if (enemyStats.health <= 0)
        {
            characterDead(enemy);
        }


        allySprite.sprite = allyGridSprite;
        enemySprite.sprite = enemyGridSprite;
        enemySprite.flipX = false;
        ally.transform.position = originalAllyPos;
        enemy.transform.position = originalEnemyPos;
        
        mouse.GetComponent<TesterGrid>().enabled = true;
        allyStats.inCombat = false;
        enemyStats.inCombat = false;

        turnUI.enabled = true;

        combatOver();
    }

}
