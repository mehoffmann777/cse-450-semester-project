using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    public float horizMax = 0;
    public float horizMin = 0;

    public float vertMax = 0;
    public float vertMin = 0;


    public float horizSpeed = 5;
    public float vertSpeed = 5;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            float moveDist = -horizSpeed * Time.deltaTime;
            if (transform.position.x + moveDist < horizMin)
            {
                moveDist = transform.position.x - horizMin;
            }

            transform.position += new Vector3(moveDist, 0, 0);
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            float moveDist = horizSpeed * Time.deltaTime;
            if (transform.position.x + moveDist > horizMax)
            {
                moveDist = horizMax - transform.position.x;
            }

            transform.position += new Vector3(moveDist, 0, 0);
        }

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            float moveDist = vertSpeed * Time.deltaTime;
            if (transform.position.y + moveDist > vertMax)
            {
                moveDist = vertMax - transform.position.y;
            }

            transform.position += new Vector3(0, moveDist, 0);
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            float moveDist = -vertSpeed * Time.deltaTime;
            if (transform.position.y + moveDist < vertMin)
            {
                moveDist = transform.position.y - vertMin;
            }

            transform.position += new Vector3(0, moveDist, 0);
        }

    }
}
