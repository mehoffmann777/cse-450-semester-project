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

    private Graphic instructionsPanelGraphic;


    //state tracking
    private int idleTimer;
    public int idleTimerMax;



        // Start is called before the first frame update
    void Start()
    {
        //set the progress metric on the bar
        //numbers are hard coded for a 5 island game -- should be relatively simple to adjust or to make dynamic
        // would just have to change the * 20 in the texting setting thing 
        int progressPercent = PlayerPrefs.GetInt("Levels Complete", 0);
        progressText.text = "Progress: " + (PlayerPrefs.GetInt("Levels Complete") * 20).ToString() + "%";

        if (progressPercent == 100)
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
       // instructionsPanelGraphic.CrossFadeAlpha(0, 0, false);

    }

    // Update is called once per frame
    void Update()
    {


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
    public void CloseWinPanel()
    {
        winPanel.SetActive(false);
    }
    public void ResetGameAfterWin()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("MapTesting");
    }


}
