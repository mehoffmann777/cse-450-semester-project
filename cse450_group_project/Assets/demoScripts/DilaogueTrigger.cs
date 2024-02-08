using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DilaogueTrigger : MonoBehaviour
{
    private bool playerDetected;
    [SerializeField] private GameObject visualCue;

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
