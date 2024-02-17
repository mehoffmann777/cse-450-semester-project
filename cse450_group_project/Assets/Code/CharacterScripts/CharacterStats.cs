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
    public int defense;
    public int luck;
    public int movement;

    public bool CanMove = true;
    public int Team;
    public bool combatDone = false;

    


    public TextMeshProUGUI attackText;

    
    void Start()
    {
        attackText = GameObject.Find("Attack Text").GetComponent<TextMeshProUGUI>();
    }

   
    void Update()
    {
       
    }

   
}
