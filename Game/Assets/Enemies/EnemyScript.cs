using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2DScript))]
public class EnemyScript : ControllableObject {

    protected Transform[] waypoints;
    public Transform[] Waypoints
    {
        set { waypoints = value; }
    }

    protected void Start()
    {
        controller = GetComponent<Controller2DScript>();

        gravity = LevelGeneratorScript.LevelGravity;

        //Calculate timeToJumpApex and jumpVelocity according to the gravity of the player
        float timeToJumpApex = Mathf.Sqrt(jumpHeight / (0.5f * -gravity));
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
    }

    protected void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag != "Player" && coll.gameObject.tag != "StairEnemy")
            return;

        coll.gameObject.GetComponent<PlayerScript>().Flinch(transform.position);
    }
}
