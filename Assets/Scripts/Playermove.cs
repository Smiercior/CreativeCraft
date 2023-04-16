using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class Playermove : MonoBehaviour
{
    public CharacterController2D control;
    public Rigidbody2D playerPosition;

    // Player move variables // Circlecolider2d is necessary, otherwise character stop in random places
    Vector2 newPosition;
    public float runspeed = 40f;
    float horizontalmove = 0f;
    bool jump = false;

    private void Start()
    {
        // Set player spawn position
        newPosition.x = 1;
        newPosition.y = 81;
        playerPosition.position = newPosition;
    }

    // Update is called once per frame
    void Update()
    {
        horizontalmove = Input.GetAxisRaw("Horizontal")*runspeed; // Get -1 - left arrow, Get 1 - right arrow
        if (Input.GetButtonDown("Jump")) jump = true;
        PlayerOutOfMap();
    }

    void FixedUpdate()
    {
        control.Move(horizontalmove * Time.fixedDeltaTime, false, jump); // fixeDdeltaTime how much time has passed since that function call we are moving about the same independently how many time that functions was called
        jump = false;                                                  
    }                                                                    

    // Check if player go outside the map and stop him to go further
    void PlayerOutOfMap()
    {
        if(playerPosition.position.x <= 0)
        {
            newPosition.x = 0;
            newPosition.y = playerPosition.position.y;
            playerPosition.position = newPosition;
        }
        else if( playerPosition.position.x > WorldManager.maxWidth + 1)
        {
            newPosition.x = WorldManager.maxWidth + 1;
            newPosition.y = playerPosition.position.y;
            playerPosition.position = newPosition;
        }
    }
}
