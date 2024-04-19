using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{

    Animator animator;
    bool inCombat;
    bool clicked;
    
    void Start()
    {
        animator = GetComponent<Animator>();
       
    }

    void Update()
    {
        inCombat = GetComponent<CharacterStats>().inCombat;
        clicked = GetComponent<CharacterStats>().clicked;
        animator.SetBool("inCombat", inCombat);
        animator.SetBool("clicked", clicked);
    }
}
