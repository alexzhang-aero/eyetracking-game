using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{

    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Optional: Add collision logic with the player here if needed
        if (collision.gameObject.tag == "Boundary")
        {
            Destroy(gameObject);  // Destroy the asteroid upon collision with the player
        }
    }
}