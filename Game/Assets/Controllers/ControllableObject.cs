using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2DScript))]
public class ControllableObject : MonoBehaviour
{
    protected float velocitySmoothX, velocitySmoothY;
    protected Vector3 velocity;

    protected Controller2DScript controller;
    protected SpriteRenderer renderer;

    //Height in units
    public float jumpHeight = 2.5f;
    
    protected float gravity;
    protected float jumpVelocity;

    //Speed in units per second
    public float walkSpeed = 6f;
    //Speed in units per second
    public float runSpeed = 10f;
    //Time in seconds
    public float smoothTimeAirborne = 0.2f;
    //Time in seconds
    public float smoothTimeGrounded = 0.05f;

    //Controls
    protected bool jump;
    protected bool run;
    protected float horizontalAxis;
    protected float verticalAxis;

    protected bool onStairs;
    public bool OnStairs
    {
        get { return onStairs; }
        set { onStairs = value; }
    }

    protected bool flinched;
    protected float flinchTimer;
    //Speed in units per second
    public float flinchVelocityX = 8;
    public float flinchVelocityY = 16;
    //Time in seconds
    public float flinchTime = 1;

    //Animator variables
    private bool facing = true;    //true = right, false = left
    protected bool Facing
    {
        get { return facing; }
        set
        {
            facing = value;
            renderer.flipX = facing;
        }
    }

    protected void Start()
    {
        controller = GetComponent<Controller2DScript>();
        renderer = GetComponent<SpriteRenderer>();
    }

    virtual protected void Update()
    {
        //Stop moving when hitting a roof or standing on the ground
        if (controller.collisions.above || controller.collisions.below && !flinched)
            velocity.y = 0;
        //Lose velocity when hitting a wall mid-air
        if (!controller.collisions.below && (controller.collisions.right || controller.collisions.left))
            velocity.x = 0;

        DoFlinch();

        Jump();

        //Smooth horizontal movement
        float targetVelocity = horizontalAxis * (run ? runSpeed : walkSpeed);
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocity, ref velocitySmoothX, (controller.collisions.below || onStairs) && !flinched ? smoothTimeGrounded : smoothTimeAirborne);
        if (velocity.x != 0)
            Facing = velocity.x > 0;

        targetVelocity = verticalAxis * walkSpeed;
        bool moveThroughFloor = controller.collisions.semiFloor && targetVelocity < 0;

        //For making the player float on stairs
        if (onStairs || moveThroughFloor)
        {
            //So that you can also move with the spacebar on stairs
            if (jump && targetVelocity <= 0)
                targetVelocity += 1 * walkSpeed;

            velocity.y = Mathf.SmoothDamp(velocity.y, targetVelocity, ref velocitySmoothY, smoothTimeGrounded);
        }
        else
            velocity.y += gravity * Time.deltaTime;

        controller.MovePlayer(velocity * Time.deltaTime, moveThroughFloor);
    }
    virtual protected void Jump()
    {
        //Jump when on the ground. When button is held, velocity is held constant for *jumpEmphasis* seconds so player jumps higher.
        if (jump && controller.collisions.below)
            velocity.y = jumpVelocity;
    }
    
    protected void DoFlinch()
    {
        if (!flinched)
            return;

        horizontalAxis = 0;
        verticalAxis = 0;

        flinchTimer -= Time.deltaTime;

        //Unflinch when you hit the ground again
        if (flinchTimer < flinchTime - 0.1f && controller.collisions.below)
            flinched = false;
    }
    public void Flinch(Vector3 colliderPos)
    {
        velocity.y = flinchVelocityY;

        int sign = transform.position.x > colliderPos.x ? 1 : -1;
        velocity.x = sign * flinchVelocityX;

        flinched = true;
        flinchTimer = flinchTime;
    }
}
