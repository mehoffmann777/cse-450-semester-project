using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mapShipMovement : MonoBehaviour
{
    //Outlets
    Rigidbody2D _rb;
    public float speed;
    public float rotation_speed;
    [SerializeField] private GameObject instructionsPanel;

    private Graphic instructionsPanelGraphic;


    //state tracking
    private int idleTimer;
    public int idleTimerMax;



        // Start is called before the first frame update
    void Start()
    {
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


}
