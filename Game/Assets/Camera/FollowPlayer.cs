using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

    public Transform player;

    public LevelGeneratorScript level;
    Point currentRoom;
    
    new Camera camera;

    private Vector3 smoothV;
    private float smoothF;
    public float regSmoothTime = 0.3f;
    public float fallSmoothTime = 0.12f;
    float smoothTime = 0;
    bool transitioning = false;

    // Use this for initialization
    void Start () {
        transform.position = new Vector3(player.position.x, player.position.y, -10);

        camera = GetComponent<Camera>();

        //Calculate the initial position of the camera according to where the player starts
        Vector3 p = player.position;
        foreach(FieldScript f in level.roomArray)
        {
            if (f != null && f.BottomLeft.x < p.x && f.BottomLeft.y < p.y && f.TopRight.x > p.x && f.TopRight.y > p.y)
            {
                transform.position = f.Center;
                currentRoom = f.fieldID;
            }
        }
    }

    void CheckRoom()
    {
        Vector3 min = level.roomArray[currentRoom.x, currentRoom.y].BottomLeft;
        Vector3 max = level.roomArray[currentRoom.x, currentRoom.y].TopRight;
        Vector3 p = player.position;
        Point nextRoom = currentRoom;

        if (min.x > p.x)
            nextRoom.x--;
        if (min.y > p.y)
            nextRoom.y++;
        if (max.x < p.x)
            nextRoom.x++;
        if (max.y < p.y)
            nextRoom.y--;

        if (nextRoom != currentRoom)
        {
            transitioning = true;

            smoothTime = (nextRoom.y == 1 + currentRoom.y) ? fallSmoothTime : regSmoothTime;

            currentRoom = nextRoom;
        }
    }

    void Transition()
    {
        Vector3 target = level.roomArray[currentRoom.x, currentRoom.y].Center;

        transform.position = Vector3.SmoothDamp(transform.position, target, ref smoothV, smoothTime);

        if (transform.position == target)
            transitioning = false;

        camera.orthographicSize = Mathf.SmoothDamp(camera.orthographicSize, level.roomArray[currentRoom.x, currentRoom.y].CameraSize, ref smoothF, smoothTime);
    }

	void Update ()
    {
        CheckRoom();

        if (transitioning)
            Transition();
    }
}
