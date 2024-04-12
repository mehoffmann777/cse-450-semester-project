using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


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

    public Image allyHeart;
    public Image enemyHeart;

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
        results.AttackerDamageIfHit = (int) Mathf.Max(0f, attackerStats.strength - defenderStats.defense);
        results.DefenderDamageIfHit = (int) Mathf.Max(0f, defenderStats.strength - attackerStats.defense);

        // will rework stats to factor in both accuracy + opponent's something to determine dodge
        // dodge is just rolled as a set 0-10 on attack rn, but this could be changed 

        float attackerHitPremium = attackerStats.dex - defenderStats.dex + (0.25f * attackerStats.luck);
        float defenderHitPremium = defenderStats.dex - attackerStats.dex + (0.25f * defenderStats.luck);

        results.AttackerHitChance = StatToPercentCustomSigmoid(attackerHitPremium);
        results.DefenderHitChance = StatToPercentCustomSigmoid(defenderHitPremium);

        float attackerCritPremium = attackerStats.luck - defenderStats.luck + (0.25f * attackerStats.dex);
        float defenderCritPremium = defenderStats.luck - attackerStats.luck + (0.25f * defenderStats.dex);

        results.AttackerCritChance = StatToCustomSigmoidCrit(attackerCritPremium);
        results.DefenderCritChance = StatToCustomSigmoidCrit(defenderCritPremium);

        results.AttackerHealth = attackerStats.health - results.DefenderDamageIfHit;
        results.DefenderHealth = defenderStats.health - results.AttackerDamageIfHit;

        results.WouldKillAttacker = results.AttackerHealth <= 0;
        results.WouldKillDefender = results.DefenderHealth <= 0;

        return results;
    }


    // Be careful about changing - 0 is 90, 4 is 97, -4 is 74
    private static float StatToPercentCustomSigmoid(float statVal)
    {
        const float minVal = 0.3f;
        float valTransform =  (statVal + 5.5f) / 3f;
        float increaseOverMin = ((1 - minVal) / (1.0f + Mathf.Exp(-valTransform)));
        return minVal + increaseOverMin;
    }

    private static float StatToCustomSigmoidCrit(float statVal)
    {
        const float minVal = 0.01f;
        float valTransform = (statVal - 6f) / 2.5f;
        float increaseOverMin = ((1 - minVal) / (1.0f + Mathf.Exp(-valTransform)));
        return 0.15f * (minVal + increaseOverMin);
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

        enemySprite = enemy.GetComponent<SpriteRenderer>();
        allySprite = ally.GetComponent<SpriteRenderer>();

        enemySprite.flipX = true;
    
        Vector3 originalAllyPos = ally.transform.position;
        Vector3 originalEnemyPos = enemy.transform.position;

        ally.transform.position = new Vector2(125, 0);
        enemy.transform.position = new Vector2(130, 0);

        allyHeart.fillAmount = (float)allyStats.health / (float)allyStats.startingHealth;
        enemyHeart.fillAmount = (float)enemyStats.health / (float)enemyStats.startingHealth;

        allyStats.inCombat = true;
        enemyStats.inCombat = true;

        StartCoroutine(Attack(ally, enemy, originalAllyPos, originalEnemyPos, manhattanDistanceApart));
        
    }

    public IEnumerator Attack(GameObject ally, GameObject enemy, Vector3 originalAllyPos, Vector3 originalEnemyPos, int manhattanDistanceApart)
    {

        CharacterStats allyStats = ally.GetComponent<CharacterStats>();
        CharacterStats enemyStats = enemy.GetComponent<CharacterStats>();

        allyStats.inCombat = true;
        enemyStats.inCombat = true;
        Color atttackerTextColor = allyStats.Team == CharacterTeam.Ally ? Color.white : Color.red;
        Color defenderTextColor = enemyStats.Team == CharacterTeam.Ally ? Color.white : Color.red;

        string attackerName = allyStats.Team == CharacterTeam.Ally ? "Player " : "Enemy ";
        string defenderName = enemyStats.Team == CharacterTeam.Ally ? "Player " : "Enemy ";

        PredictCombatResults combatCalculations = PredictCombat(allyStats, enemyStats);

        float attackerHitRoll = Random.Range(0f, 1f);
        float attackerCritRoll = Random.Range(0f, 1f);

        attackText.text = "";
        yield return new WaitForSecondsRealtime(0.75f);

        attackText.color = atttackerTextColor;
        if (attackerHitRoll < combatCalculations.AttackerHitChance)
        {
            bool crit = attackerCritRoll < combatCalculations.AttackerCritChance;
            int damage = combatCalculations.AttackerDamageIfHit * (crit ? 2 : 1);
            enemyStats.health -= damage;
            enemyHeart.fillAmount = ((float)enemyStats.health / (float)enemyStats.startingHealth);
            attackText.text = attackerName + (crit ? "Critical " : "") + "Hit " + damage;
            
        }
        else
        {
            attackText.text = attackerName + "Miss";
        }

        yield return new WaitForSecondsRealtime(1f);

        bool defenderCanCounterAttack = DefenderCanCoutner(enemyStats, manhattanDistanceApart);

        if (enemyStats.health > 0 && defenderCanCounterAttack)
        {
            yield return new WaitForSecondsRealtime(2f);

            float defenderHitRoll = Random.Range(0f, 1f);
            float defenderCritRoll = Random.Range(0f, 1f);

            attackText.color = defenderTextColor;
            if (defenderHitRoll < combatCalculations.DefenderHitChance)
            {
                bool crit = defenderCritRoll < combatCalculations.DefenderCritChance;
                int damage = combatCalculations.DefenderDamageIfHit * (crit ? 2 : 1);
                allyStats.health -= damage;
                allyHeart.fillAmount = ((float)allyStats.health / (float)allyStats.startingHealth);
                attackText.text = defenderName + (crit ? "Critical " : "") + "Hit " + damage;
            } else
            {
                attackText.text = defenderName + "Miss";
            }
            yield return new WaitForSecondsRealtime(3f);
        }

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
