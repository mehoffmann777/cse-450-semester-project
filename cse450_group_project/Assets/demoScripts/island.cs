using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class island : MonoBehaviour
{
    private bool playerDetected;
    //outlets
    [Header("Island Identity")]
    [SerializeField] private int islandId; 
    [Header("Visual Cue")]
    [SerializeField] private GameObject incompleteVisual;
    [SerializeField] private GameObject completeVisual;

    [Header("Scene To Go To")]
    [SerializeField] string nextScene;


    //state tracking
    private GameObject visualCue;

    private void Awake()
    {
        // if the level is complete, it sets the visual cue to be a check mark, otherwise its an A
        // this statement gets the player pre value and otherwise returns 0
        if (PlayerPrefs.GetInt("Island" + islandId, 0) == 0)
        {
            Debug.Log("Island " + islandId + " is using the incomplete visual");
            visualCue = incompleteVisual;
            completeVisual.SetActive(false);
        }
        else
        {
            Debug.Log("Island " + islandId + " is using the complete visual");
            visualCue = completeVisual;
            incompleteVisual.SetActive(false);
        }
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
        if (playerDetected)
        {
            visualCue.SetActive(true);
            //CHANGE LATER: use an input manager to standardize input throughout project
            if (Input.GetKeyDown(KeyCode.A))
            {
                Debug.Log("Should go to scene: " + nextScene);
                SceneManager.LoadScene(nextScene);
            }
        }
        else
        {
            visualCue.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.tag);
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
