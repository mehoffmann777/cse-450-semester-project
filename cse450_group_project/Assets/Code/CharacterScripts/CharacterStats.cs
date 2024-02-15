using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterStats : MonoBehaviour
{

    public int health;
    public int stamina;
    public int strength;
    public int accuracy;
    public int luck;
    public int movement;

    public bool CanMove = true;

    public TextMeshProUGUI attackText;

    
    void Start()
    {
        

    }

   
    void Update()
    {
       
    }

    public void Attack()
    {
        int enemyDodge = Random.Range(0, 100);
        int criticalHit = Random.Range(0, 100);
        if (accuracy > enemyDodge)
        {
            bool crit = luck > criticalHit;
            int damage = strength * (crit ? 2 : 1);
            attackText.text = (crit ? "Critical " : "") + "Hit " + damage;
        }
        else
        {
            attackText.text = "Miss";
        }
    }
}
