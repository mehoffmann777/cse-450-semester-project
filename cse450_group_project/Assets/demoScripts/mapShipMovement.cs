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

    //state tracking
    private int idleTimer;
    public int idleTimerMax;



        // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        idleTimer = 0;
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
        }
        //forward boost
        else if (Input.GetKey(KeyCode.Space))
        {
            _rb.AddRelativeForce(new Vector2(0,-1) * speed * 10 * Time.deltaTime);
            
        }
        else
        {
            idleTimer++;
        }

        if (idleTimer > idleTimerMax)
        {
            //we wanna make this a coroutine where we fade in the panel based on the alpha
            //StartCoroutine("FadePanelIn");
           instructionsPanel.SetActive(true); 
        }
        else
        {
            //we wanna make this a coroutine where we fade out the panel based on the alpha
            //StartCoroutine("FadePanelOut");
            instructionsPanel.SetActive(false);
        }

    }

        


}
