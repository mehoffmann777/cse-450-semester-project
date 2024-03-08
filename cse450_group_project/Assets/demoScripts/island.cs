using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class island : MonoBehaviour
{
    private bool playerDetected;
    //outlets
    [Header("Visual Cue")]
    [SerializeField] private GameObject visualCue;
    [Header("Scene To Go To")]
    [SerializeField] string nextScene;

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
        if (playerDetected)
        {
            visualCue.SetActive(true);
            //CHANGE LATER: use an input manager to standardize input throughout project
            if (Input.GetKeyDown(KeyCode.A))
            {
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
        Debug.Log("entered!");
        Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("detected!");
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
