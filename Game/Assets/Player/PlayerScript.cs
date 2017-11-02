using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2DScript))]
public class PlayerScript : ControllableObject {

    //Height in units
    public float maxJumpHeight = 4.5f;
    //Time in seconds
    public float timeToJumpApex = 0.4f;

    float jumpEmphasis;
    bool isJumping = false;
    float jumpingTimePassed = 0f;

    new void Start()
    {
        base.Start();
        
        //Calculate gravity and jump velocity based on jump height and jump time, according to physics
        gravity = -(2 * (jumpHeight) / Mathf.Pow(timeToJumpApex, 2));
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        //Calculate how long the jump can be held in order to reach the max height
        jumpEmphasis = (maxJumpHeight - jumpHeight) / jumpVelocity;

        jumpingTimePassed = jumpEmphasis;

        //Save the gravity in the LevelGenerator
        LevelGeneratorScript.LevelGravity = gravity;

        //DEBUG
        //print("Gravity: " + gravity + " Jump Velocity: " + jumpVelocity + " Emphasis: " + jumpEmphasis);
    }

    protected override void Update ()
    {
        horizontalAxis = Input.GetAxisRaw("Horizontal");
        verticalAxis = Input.GetAxisRaw("Vertical");
        jump = Input.GetKey(KeyCode.Space);
        run = Input.GetKey(KeyCode.LeftShift);

        base.Update();
    }

    protected override void Jump()
    {
        //Jump when on the ground. When button is held, velocity is held constant for *jumpEmphasis* seconds so player jumps higher.
        if (!flinched && Input.GetKey(KeyCode.Space) && (controller.collisions.below || isJumping))
            if (jumpingTimePassed > 0)
            {
                velocity.y = jumpVelocity;
                jumpingTimePassed -= Time.deltaTime;
                isJumping = true;
            }
        //Reset jumping ability when button is released
        if (Input.GetKeyUp(KeyCode.Space))
        {
            jumpingTimePassed = jumpEmphasis;
            isJumping = false;
        }
    }
}