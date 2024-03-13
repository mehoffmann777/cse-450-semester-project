using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DilaogueTrigger : MonoBehaviour
{
    private bool playerDetected;
    [Header("Visual Cue")]
    //[SerializeField] private GameObject visualCue;

    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;

    private void Awake()
    {
       
        //playerDetected = false;
        //visualCue.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!DialogueManager.GetInstance().dialogueIsPlaying)
        {
            DialogueManager.GetInstance().EnterDilaogueMode(inkJSON);
           // Debug.Log("entered if statement in trigger dialogue");
        }
    }
   
}
