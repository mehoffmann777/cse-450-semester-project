using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class mapShipMovement : MonoBehaviour
{
    //Outlets
    Rigidbody2D _rb;
    public float speed;
    public float rotation_speed;
    [SerializeField] private GameObject instructionsPanel;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] GameObject winPanel;
    [SerializeField] GameObject pausePanel;

    private Graphic instructionsPanelGraphic;


    //state tracking
    private int idleTimer;
    public int idleTimerMax;
    private bool gamePaused;



        // Start is called before the first frame update
    void Start()
    {
    
        //set the progress metric on the bar
        //numbers are hard coded for a 5 island game -- should be relatively simple to adjust or to make dynamic
        // would just have to change the * 20 in the texting setting thing 
     //   int progressPercent = PlayerPrefs.GetInt("Levels Complete", 0);
        int levelOne = PlayerPrefs.GetInt("Island0", 0);
        int levelTwo = PlayerPrefs.GetInt("Island1", 0);
        int levelThree = PlayerPrefs.GetInt("Island2", 0);
        int levelFour = PlayerPrefs.GetInt("Island3", 0);
        int levelFive = PlayerPrefs.GetInt("Island4", 0);

        int progressNum = levelOne + levelTwo + levelThree + levelFour + levelFive;
        string progressPercent = (progressNum * 20).ToString() + "%";

        progressText.text = "Progress: " + progressPercent;

        if (progressNum*20 == 100)
        {
            winPanel.SetActive(true);
        }
        else
        {
            winPanel.SetActive(false);
        }
        _rb = GetComponent<Rigidbody2D>();
        idleTimer = 0;
        instructionsPanelGraphic = instructionsPanel.GetComponent<Graphic>();
        gamePaused = false;
       // instructionsPanelGraphic.CrossFadeAlpha(0, 0, false);

    }

    // Update is called once per frame
    void Update()
    {
        if (gamePaused)
        {
            pausePanel.SetActive(true);
        }
        else
        {
            pausePanel.SetActive(false);
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                _rb.AddTorque(rotation_speed * Time.deltaTime);
                idleTimer = 0;
            }
            //right movement
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                _rb.AddTorque(-rotation_speed * Time.deltaTime);
                idleTimer = 0;

            }
            //forward boost
            else if (Input.GetKey(KeyCode.Space))
            {
                _rb.AddRelativeForce(new Vector2(0, -1) * speed * 10 * Time.deltaTime);
                idleTimer = 0;

            }
            else if (Input.GetKey(KeyCode.Escape))
            {
                gamePaused = true;
                idleTimer = 0;
            }
            else
            {
                idleTimer++;
            }

            if (idleTimer > idleTimerMax)
            {
                // instructionsPanelGraphic.CrossFadeAlpha(1, 1, false);
                instructionsPanel.SetActive(true);

            }
            else
            {
                //instructionsPanelGraphic.CrossFadeAlpha(0, 1, false);
                instructionsPanel.SetActive(false);
            }
        }

      
    }
    public void CloseWinPanel()
    {
        winPanel.SetActive(false);
    }
    public void ClosePausePanel()
    {
        gamePaused = false;
    }
    public void ResetGameAfterWin()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("MapTesting");
    }
    public void ResetGameFromPause()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("MapTesting");
    }


}
