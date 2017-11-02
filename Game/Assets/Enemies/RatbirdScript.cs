using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatbirdScript : EnemyScript {

    private Vector3 pos1, pos2;

	// Use this for initialization
	new void Start () {
        base.Start();

        pos1 = this.transform.position;
        pos2 = waypoints[0].position;

        Facing = pos1.x < pos2.x;
        if (!Facing)
        {
            Vector3 p = pos1;
            pos1 = pos2;
            pos2 = p;
        }
	}

    // Update is called once per frame
    protected override void Update()
    {
        if (Facing)
            Facing = !(transform.position.x >= pos2.x);
        else
            Facing = transform.position.x <= pos1.x;

        horizontalAxis = Facing ? 1 : -1;

        base.Update();
    }
}
