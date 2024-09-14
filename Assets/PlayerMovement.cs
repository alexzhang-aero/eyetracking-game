using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movespeed = 5;
    private float hInput;
    private float vInput; 


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        hInput = Input.GetAxisRaw("Horizontal");
        vInput = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(hInput, vInput).normalized;

        transform.Translate(movement * movespeed * Time.deltaTime);    
    }
}
