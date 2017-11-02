using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldScript : MonoBehaviour {

    public Point fieldID;

    Vector3 center;
    Vector3 bottomLeft, topRight;
    public Vector3 Center
    {
        get { return new Vector3(center.x, center.y, -10); }
    }
    public Vector3 BottomLeft
    {
        get { return new Vector3(bottomLeft.x, bottomLeft.y, -10); }
    }
    public Vector3 TopRight
    {
        get { return new Vector3(topRight.x, topRight.y, -10); }
    }

    public bool largeRoom = false;

    float cameraSize;
    public float CameraSize
    {
        get { return cameraSize; }
    }

	// Use this for initialization
	void Start () {
        Transform[] children = GetComponentsInChildren<Transform>();
        bottomLeft = topRight = children[0].position;

        foreach (Transform t in children)
        {
            if (t.position.x < bottomLeft.x || t.position.y < bottomLeft.y)
                bottomLeft = t.position;
            if (t.position.x > topRight.x || t.position.y > topRight.y)
                topRight = t.position;
        }
        
        bottomLeft -= new Vector3(0.5f, 0.5f);
        topRight += new Vector3(0.5f, 0.5f);

        center = (bottomLeft + topRight) / 2;

        cameraSize = largeRoom ? 9 : 6;

        //DEBUG
        //Debug.Log("Center: " + center + " Bottom Left: " + bottomLeft + " Top Right: " + topRight);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
