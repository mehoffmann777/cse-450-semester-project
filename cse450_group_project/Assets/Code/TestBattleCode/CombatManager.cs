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
    private Sprite allyGridSprite;
    private Sprite enemyGridSprite;
    private Sprite enemyCombatSprite;
    private Sprite allyCombatSprite;

    public int enemyDamage;
    public int enemyHitChance;
    public int allyDamage;
    public int allyHitChance;

    public struct PredictCombatResults {
        public float AttackerHitChance; 
        public float DefenderHitChance;
        public float AttackerCritChance;
        public float DefenderCritChance;
        public int AttackerHealth;
        public int DefenderHealth;
        public int AttackerDamageIfHit;
        public int DefenderDamageIfHit;
        public bool WouldKillAttacker;
        public bool WouldKillDefender;
    }

    // so enemy AI can make smart decisions about who to attack
    public static PredictCombatResults PredictCombat(CharacterStats attackerStats, CharacterStats defenderStats)
    {
        PredictCombatResults results = new();

        // does not account for crits bc that's rolled on attack
        results.AttackerDamageIfHit = attackerStats.strength - defenderStats.defense;
        results.DefenderDamageIfHit = defenderStats.strength - attackerStats.defense;

        // will rework stats to factor in both accuracy + opponent's something to determine dodge
        // dodge is just rolled as a set 0-10 on attack rn, but this could be changed 
        results.AttackerHitChance = attackerStats.accuracy / 10.0f;
        results.DefenderHitChance = defenderStats.accuracy / 10.0f;

        results.AttackerCritChance = attackerStats.luck / 100.0f;
        results.DefenderCritChance = defenderStats.luck/ 100.0f;

        results.AttackerHealth = attackerStats.health - results.DefenderDamageIfHit;
        results.DefenderHealth = defenderStats.health - results.AttackerDamageIfHit;

        results.WouldKillAttacker = results.AttackerHealth <= 0;
        results.WouldKillDefender = results.DefenderHealth <= 0;

        return results;
    }

    public void StartCombat(GameObject ally, GameObject enemy, int manhattanDistanceApart)
    {

        ally.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        enemy.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);

        mainCamera.enabled = false;
        combatCamera.enabled = true;
        turnUI.enabled = false;
        mouse.GetComponent<BaseGridManager>().enabled = false;

        CharacterStats allyStats = ally.GetComponent<CharacterStats>();
        CharacterStats enemyStats = enemy.GetComponent<CharacterStats>();

        allyStats.inCombat = true;
        enemyStats.inCombat = true;

        enemySprite = enemy.GetComponent<SpriteRenderer>();
        allySprite = ally.GetComponent<SpriteRenderer>();

        //allySprite.sprite = allyCombatSprite;
        //enemySprite.sprite = enemyCombatSprite;
        enemySprite.flipX = true;
    
        Vector3 originalAllyPos = ally.transform.position;
        Vector3 originalEnemyPos = enemy.transform.position;

        ally.transform.position = new Vector2(25, 0);
        enemy.transform.position = new Vector2(30, 0);

        // coroutine is so I can use wait in attack, otherwise it cuts back
        // to the grid before you can watch the combat
        StartCoroutine(Attack(ally, enemy, originalAllyPos, originalEnemyPos, manhattanDistanceApart));
        
    }

    // this function is HUGE and I'll prob break it up
    public IEnumerator Attack(GameObject ally, GameObject enemy, Vector3 originalAllyPos, Vector3 originalEnemyPos, int manhattanDistanceApart)
    {

        CharacterStats allyStats = ally.GetComponent<CharacterStats>();
        CharacterStats enemyStats = enemy.GetComponent<CharacterStats>();

        Color atttackerTextColor = allyStats.Team == CharacterTeam.Ally ? Color.white : Color.red;
        Color defenderTextColor = enemyStats.Team == CharacterTeam.Ally ? Color.white : Color.red;

        string attackerName = allyStats.Team == CharacterTeam.Ally ? "Player " : "Enemy ";
        string defenderName = enemyStats.Team == CharacterTeam.Ally ? "Player " : "Enemy ";


        int enemyDodge = Random.Range(0, 10);
        int criticalHit = Random.Range(0, 100);
        attackText.color = atttackerTextColor;
        if (allyStats.accuracy > enemyDodge)
        {
            bool crit = allyStats.luck > criticalHit;
            int damage = (allyStats.strength * (crit ? 2 : 1)) - enemyStats.defense;
            enemyStats.health -= damage;
            attackText.text = attackerName + (crit ? "Critical " : "") + "Hit " + damage;
        }
        else
        {
            attackText.text = attackerName + "Miss";
        }

        yield return new WaitForSecondsRealtime(3f);

        bool defenderCanCounterAttack = DefenderCanCoutner(enemyStats, manhattanDistanceApart);

        if (enemyStats.health > 0 && defenderCanCounterAttack)
        {
            int playerDodge = Random.Range(0, 10);

            attackText.color = defenderTextColor;
            if (enemyStats.accuracy > playerDodge)
            {
                criticalHit = Random.Range(0, 100);
                bool encrit = enemyStats.luck > criticalHit;
                int endamage = (enemyStats.strength * (encrit ? 2 : 1)) - allyStats.defense;
                allyStats.health -= endamage;
                attackText.text = defenderName + (encrit ? "Critical " : "") + "Hit " + endamage;
            } else
            {
                attackText.text = defenderName + "Miss";
            }
            yield return new WaitForSecondsRealtime(3f);
        }
        //allyStats.CanMove = false;
        //ally.GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.4f, 0.6f, 1);

        ally.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        enemy.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
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


        //allySprite.sprite = allyGridSprite;
        //enemySprite.sprite = enemyGridSprite;
        enemySprite.flipX = false;
        ally.transform.position = originalAllyPos;
        enemy.transform.position = originalEnemyPos;
        
        mouse.GetComponent<BaseGridManager>().enabled = true;
        allyStats.inCombat = false;
        enemyStats.inCombat = false;

        turnUI.enabled = true;

        combatOver();
    }

    public static bool DefenderCanCoutner(CharacterStats defender, int distanceApart)
    {
        return defender.minRangeInclusive <= distanceApart && defender.maxRangeInclusive >= distanceApart;
    }

}
