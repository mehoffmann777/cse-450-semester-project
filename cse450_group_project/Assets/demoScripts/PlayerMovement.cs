using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] public float speed;
    private Rigidbody2D rgbdy;
    // Start is called before the first frame update
    void Start()
    {
        rgbdy = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float xMov = Input.GetAxis("Horizontal");
        float yMov = Input.GetAxis("Vertical");

        rgbdy.velocity = new Vector2(xMov * speed, yMov * speed);
    }
}
