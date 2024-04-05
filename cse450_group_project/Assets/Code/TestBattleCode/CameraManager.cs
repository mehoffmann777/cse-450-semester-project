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

    float halfCameraWidth;
    float halfCameraHeight;

    // Start is called before the first frame update
    void Start()
    {
        Camera camera = GetComponent<Camera>();
        halfCameraHeight = camera.orthographicSize;
        halfCameraWidth = halfCameraHeight * (camera.pixelWidth / (float) camera.pixelHeight);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            float moveDist = -horizSpeed * Time.deltaTime;
            if (transform.position.x + moveDist < horizMin)
            {
                Vector3 pos = transform.position;
                pos.x = horizMin;
                transform.position = pos;
            }
            else {
                transform.position += new Vector3(moveDist, 0, 0);
            }


        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            float moveDist = horizSpeed * Time.deltaTime;
            if (transform.position.x + moveDist > horizMax)
            {
                Vector3 pos = transform.position;
                pos.x = horizMax;
                transform.position = pos;
            }
            else {
                transform.position += new Vector3(moveDist, 0, 0);
            }

        }

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            float moveDist = vertSpeed * Time.deltaTime;
            if (transform.position.y + moveDist > vertMax)
            {
                Vector3 pos = transform.position;
                pos.y = vertMax;
                transform.position = pos;
            }
            else
            {
                transform.position += new Vector3(0, moveDist, 0);
            }
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            float moveDist = -vertSpeed * Time.deltaTime;
            if (transform.position.y + moveDist < vertMin)
            {
                Vector3 pos = transform.position;
                pos.y = vertMin;
                transform.position = pos; 
            }
            else
            {
                transform.position += new Vector3(0, moveDist, 0);
            }
        }
    }

    public void MoveToPosition(Vector3 position)
    {
        transform.position = position;
    }

    public Vector3 CurrentCameraPosition()
    {
        return transform.position;
    }

    public void MoveToShow(Vector3 position)
    {
        if (IsPointInCameraWithPadding(position, 0.5f)) { return; }

        Vector3 cameraPosition = transform.position;

        float newXPos = Mathf.Clamp(position.x, horizMin, horizMax);
        float newYPos = Mathf.Clamp(position.y, vertMin, vertMax);

        transform.position = new Vector3(newXPos, newYPos, cameraPosition.z);
    }


    // usableFOV = 1 is whole camera -- lower requires point is nearly centered 
    bool IsPointInCameraWithPadding(Vector2 position, float usableFOV) {
        usableFOV = Mathf.Clamp(usableFOV, 0, 1);

        Vector3 cameraPosition = transform.position;

        if (Mathf.Abs(position.x - cameraPosition.x) > halfCameraWidth * usableFOV)
        {
            return false;
        }


        if (Mathf.Abs(position.y - cameraPosition.y) > halfCameraHeight * usableFOV)
        {
            return false;
        }

        return true;
    }


}
