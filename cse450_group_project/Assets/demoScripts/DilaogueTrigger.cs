using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DilaogueTrigger : MonoBehaviour
{
    private bool playerDetected;
    [Header("Visual Cue")]
    [SerializeField] private GameObject visualCue;

    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;

    private void Awake()
    {
        playerDetected = false;
        visualCue.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerDetected && !DialogueManager.GetInstance().dialogueIsPlaying)
        {
            visualCue.SetActive(true);
            //CHANGE LATER: use an input manager to standardize input throughout project
            if (Input.GetKeyDown(KeyCode.I))
            {
                //IMPORTANT NOTE: THIS IS WHERE THE DIALOGUE IS STARTING, SO ANYTHING THAT SHOULD TRIGGER DIALOGUE SHOULD HAVE A FILE LIKE THIS, THAT CALLS THE DILAOGUE MANAGER UPON THE COMPLETION OF SOME CONDITION
                DialogueManager.GetInstance().EnterDilaogueMode(inkJSON);
            }
        }
        else
        {
            visualCue.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerDetected = true;
        }
       
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerDetected = false;
        }
    }
}
