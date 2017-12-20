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
        center = transform.position;

        if (largeRoom)
        {
            bottomLeft = center - new Vector3(8, 6);
            topRight = center + new Vector3(8, 6);
            cameraSize = 9;
        }
        else
        {
            bottomLeft = center - new Vector3(8, 6);
            topRight = center + new Vector3(8, 6);
            cameraSize = 6;
        }

        //DEBUG
        //Debug.Log("Center: " + center + " Bottom Left: " + bottomLeft + " Top Right: " + topRight);
	}
}
