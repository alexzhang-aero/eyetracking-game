using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerMovement : MonoBehaviour
{
    public float movespeed = 5;
    private float hInput;
    private float vInput; 
    public GameObject explosionPrefab;
    public float restartDelay = 2.0f;  // Delay before restarting the game

    void Start()
    {

    }
    void Update()
    {
        hInput = Input.GetAxisRaw("Horizontal");
        vInput = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(hInput, vInput).normalized;

        transform.Translate(movement * movespeed * Time.deltaTime);    
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Asteroid")
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Debug.Log("Game Over! Ship hit by an asteroid.");

            Destroy(gameObject);

            // Restart the game after the player is destroyed
            GameManager.instance.RestartGame(restartDelay);
        }
    }

}
        


