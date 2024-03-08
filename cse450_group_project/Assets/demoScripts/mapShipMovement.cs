using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapShipMovement : MonoBehaviour
{
    //Outlets
        Rigidbody2D _rb;

        public float speed;
        public float rotation_speed;

        // Start is called before the first frame update
        void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame
        void Update()
        {

        ////left movement
        //if (Input.GetKey(KeyCode.LeftArrow))
        //{
        //    Debug.Log("left!");
        //    _rb.AddForce(Vector2.left * speed * Time.deltaTime);

        //}

        ////right movement
        //if (Input.GetKey(KeyCode.RightArrow))
        //{
        //    _rb.AddForce(Vector2.right * speed * Time.deltaTime);

        //}

        //if (Input.GetKey(KeyCode.UpArrow))
        //{
        //    Debug.Log("up!");
        //    _rb.AddForce(Vector2.up * speed * Time.deltaTime);

        //}
        //if (Input.GetKey(KeyCode.DownArrow))
        //{
        //    Debug.Log("down");
        //    _rb.AddForce(Vector2.down * speed * Time.deltaTime);

        //}
        //left movement
        if (Input.GetKey(KeyCode.LeftArrow))
            {
                _rb.AddTorque(rotation_speed * Time.deltaTime);
            }

            //right movement
            if (Input.GetKey(KeyCode.RightArrow))
            {
                _rb.AddTorque(-rotation_speed * Time.deltaTime);
            }

            //forward boost
            if (Input.GetKey(KeyCode.Space))
            {
                _rb.AddRelativeForce(new Vector2(0,-1) * speed * 10 * Time.deltaTime);
            }
        }


}
