using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPresetScript : MonoBehaviour {

    //Call these to turn on a wall
    public void Up(int index)
    {
        for (int i = 0; i < transform.GetChild(1).childCount; i++)
        {
            transform.GetChild(1).GetChild(i).gameObject.SetActive(i == index);
        }
    }
    public void Right(int index)
    {
        for (int i = 0; i < transform.GetChild(2).childCount; i++)
        {
            transform.GetChild(2).GetChild(i).gameObject.SetActive(i == index);
        }
    }
    public void Down(int index)
    {
        for (int i = 0; i < transform.GetChild(3).childCount; i++)
        {
            transform.GetChild(3).GetChild(i).gameObject.SetActive(i == index);
        }
    }
    public void Left(int index)
    {
        for (int i = 0; i < transform.GetChild(4).childCount; i++)
        {
            transform.GetChild(4).GetChild(i).gameObject.SetActive(i == index);
        }
    }

    //For the contents of the room
    public void Contents(int index)
    {
        for (int i = 0; i < transform.GetChild(5).childCount; i++)
        {
            transform.GetChild(5).GetChild(i).gameObject.SetActive(i == index);
        }
    }
}
