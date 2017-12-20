using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPresetScript : MonoBehaviour {

    private char[,] roomTemplate;

    //All prefabs
    public GameObject wall;
    public GameObject LDSlope;
    public GameObject LUSlope;
    public GameObject RDSlope;
    public GameObject RUSlope;
    public GameObject topLadder;
    public GameObject bottomLadder;

    //Enemy prefabs
    public GameObject ratBird;

    public void Initialize(char[,] roomTemplate)
    {
        //Save the string for convience and/or debugging
        this.roomTemplate = roomTemplate;

        /*
         * The way this works is that each room prefabs contains every possible block at every possible location,
         * but all of the gameObjects are disabled. These are all neatly organized inside the prefab, sorted on rows first and then on columns.
         * The switch statement then goes through the char array, and enables the blocks at the correct positions.
         * If you use Instantiate() for this method, it results in many bugs and lots of lag, so I came up with this method.
         * 
         * Walls and slopes are done in this manner, every other block (which is less common) is done using Instantiate
         * This is really stupid, as it means updating the wall-prefab will result in no change in the levels, so I'll look into this.
        */

        for (int x = 0; x < roomTemplate.GetLength(0); x++)
            for (int y = 0; y < roomTemplate.GetLength(1); y++)
            {
                Vector3 pos = new Vector3(x, y, 0) + transform.GetChild(0).position;

                switch (roomTemplate[x, y])
                {
                    //An empty block is the default
                    default:
                    case 'X':   //Empty space
                        break;

                    case ' ':   //Block
                        Instantiate(wall, pos, Quaternion.identity, transform.GetChild(0));
                        break;
                    case 'l':   //Slope LD
                        Instantiate(LDSlope, pos, Quaternion.identity, transform.GetChild(0));
                        break;
                    case 'L':   //Slope LU
                        Instantiate(LUSlope, pos, Quaternion.identity, transform.GetChild(0));
                        break;
                    case 'r':   //Slope RD
                        Instantiate(RDSlope, pos, Quaternion.identity, transform.GetChild(0));
                        break;
                    case 'R':   //Slope RU
                        Instantiate(RUSlope, pos, Quaternion.identity, transform.GetChild(0));
                        break;
                    case 't':   //Bottom Ladder
                        Instantiate(bottomLadder, pos, Quaternion.identity, transform.GetChild(0));
                        break;
                    case 'T':   //Top Ladder
                        Instantiate(topLadder, pos, Quaternion.identity, transform.GetChild(0));
                        break;
                }
            }
    }
}
