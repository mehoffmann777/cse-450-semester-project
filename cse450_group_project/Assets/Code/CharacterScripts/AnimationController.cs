using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{

    Animator animator;
    bool inCombat;
    bool clicked;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
       
    }

    // Update is called once per frame
    void Update()
    {
        inCombat = GetComponent<CharacterStats>().inCombat;
        clicked = GetComponent<CharacterStats>().clicked;
        print(inCombat);
        animator.SetBool("inCombat", inCombat);
        animator.SetBool("clicked", clicked);
    }
}
