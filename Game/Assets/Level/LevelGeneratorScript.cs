using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Point
{
    public int x, y;
    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static bool operator ==(Point p1, Point p2)
    {
        return p1.x == p2.x && p1.y == p2.y;
    }
    public static bool operator !=(Point p1, Point p2)
    {
        return p1.x != p2.x || p1.y != p2.y;
    }

    public Point RandomPoint(int x, int y)
    {
        return new Point(Random.Range(0, x), Random.Range(0, y));
    }
}
public struct RoomTemplate
{
    public bool isRoom;         //False means the room is empty
    public int[] directions;   //Is always 4 elements. Element 0 = Up, 1 = Right, 2 = Down, 3 = Left

    public RoomTemplate(bool isRoom)
    {
        this.isRoom = isRoom;
        directions = new int[4];
    }
}

public class LevelGeneratorScript : MonoBehaviour {

    public FieldScript[,] roomArray;

    public int maxX, maxY;
    RoomTemplate[,] field;

    public int entranceChance;
    int entranceAmt = 4;

    static float gravity = 0;
    public static float LevelGravity
    {
        get { return gravity; }
        set { gravity = value; }
    }

    //Prefabs
    public GameObject RoomPreset;
    public GameObject Camera;
    public GameObject Player;

	// Use this for initialization
	void Start () {
        field = new RoomTemplate[maxX, maxY];

        //Generate a random field with a snakepath algorithm
        Point[] passages = SnakePath();
        //Find the quickest path int the snakepath field
        BestPath(passages[0], passages[1]);

        //Generate and fill in the rest of the field
        FillIn();
        //Filter out any rooms that aren't accessible, start from the entrance
        FloodFill(passages[0].x, passages[0].y);

        //Make a list to save the rooms in
        roomArray = new FieldScript[maxX, maxY];
        //Draw the level and save the rooms
        DrawLevel();


        //Instantiate the player and camera
        GameObject p = Instantiate(Player, new Vector3(passages[0].x * 16, -passages[0].y * 12, -1), Quaternion.Euler(0, 0, 0));
        p.transform.parent = this.transform;

        FollowPlayer fP = Instantiate(Camera).GetComponent<FollowPlayer>();
        fP.player = p.transform;
        fP.level = this;
    }
    
    //Pathfinding algorithms
    Point[] SnakePath()
    {
        //Entrance is always on the left, exit always on the left
        Point entrance = new Point(0, Random.Range(0, maxY));
        Point exit = new Point(maxX - 1, Random.Range(0, maxY));

        //Any direction except left
        int dir = 2;        //1 = Up, 2 = Right, 3 = Down, 4 = Left

        //Setup coordinates. (0,0) is the TOP LEFT corner.
        int a = entrance.x;
        int b = entrance.y;

        field[a, b].isRoom = true;

        //Make a connecting path with a snakepath algorithm
        while (a != exit.x || b != exit.y)
        {
            //Check if the next step is possible
            while (a < 0 || a >= maxX ||
                b < 0 || b >= maxY ||
                field[a, b].isRoom)
            {
                //Pick the next direction
                switch (Random.Range(0, 5))
                {
                    case (0):
                    case (1):
                    case (2):
                        break;
                    case (3):
                        dir++;
                        break;
                    case (4):
                        dir--;
                        break;
                }

                //Cycle dir around if needed
                if (dir > 4)
                    dir = 1;
                if (dir < 1)
                    dir = 4;

                //Move to the next spot
                switch (dir)
                {
                    case (1):
                        b--;
                        break;
                    case (2):
                        a++;
                        break;
                    case (3):
                        b++;
                        break;
                    case (4):
                        a--;
                        break;
                }

                //Clamp the position
                a = Mathf.Clamp(a, 0, maxX - 1);
                b = Mathf.Clamp(b, 0, maxY - 1);
            }

            field[a, b].isRoom = true;
        }

        //Set the exit to true
        field[exit.x, exit.y].isRoom = true;
        
        return new Point[] { entrance, exit };
    }

    void BestPath(Point start, Point goal)
    {
        //Based on the A* pathfinding algorithm
        
        //Create an open set for nodes to be checked, and a closed set for checked nodes
        List<Point> closedSet = new List<Point>();
        List<Point> openSet = new List<Point>();

        //Start checking from the Start node
        openSet.Add(start);

        //To save the quickest path to a specific node
        Point[,] cameFrom = new Point[maxX, maxY];

        //An integer indicating the distance between this node and the start.
        int[,] gScore = new int[maxX, maxY];
        for (int i = 0; i < maxX; i++)
            for (int j = 0; j < maxY; j++)
                gScore[i, j] = int.MaxValue;

        //Distance between start and itself is 0
        gScore[start.x, start.y] = 0;
            
        //As long as there are nodes to check
        while(openSet.Count > 0)
        {
            //If we reached the goal, reconstruct the path from the goal backwards to start
            Point current = openSet[0];
            if (current == goal)
            {
                ReconstructPath(cameFrom, current, start);
                return;
            }

            //Move the current node to not check it again
            openSet.Remove(current);
            closedSet.Add(current);
                        
            foreach (Point n in Neighbours(current))
            {
                //If we checked this node already
                if (closedSet.Contains(n))
                    continue;

                //So that this node may be checked next
                if (!openSet.Contains(n))
                    openSet.Add(n);

                //Calculate the distance according to this path
                int tentative_gScore = gScore[current.x, current.y] + 1;
                //If there is already an established quicker route, ignore
                if (tentative_gScore >= gScore[n.x, n.y])
                    continue;

                //Save the current path in CameFrom and assign the distance to the node
                cameFrom[n.x, n.y] = current;
                gScore[n.x, n.y] = tentative_gScore;
            }
        }

        //In case there is no suitable path found
        Debug.Log("No path found!");
    }
    List<Point> Neighbours(Point p)
    {
        List<Point> neighbours = new List<Point>();

        //Look in 4 directions around the point
        neighbours.Add(new Point(p.x + 1, p.y));
        neighbours.Add(new Point(p.x, p.y + 1));
        neighbours.Add(new Point(p.x - 1, p.y));
        neighbours.Add(new Point(p.x, p.y - 1));

        //If the possible neighbour is outside the array, or was not thought of by the snakepath algorithm, remove it
        for (int i = 0; i < neighbours.Count; i++)
        {
            Point n = neighbours[i];
            if (n.x < 0 || n.x >= maxX || n.y < 0 || n.y >= maxY ||
                !field[n.x, n.y].isRoom)
            {
                neighbours.RemoveAt(i);
                i--;
            }
        }

        return neighbours;
    }
    void ReconstructPath(Point[,] cameFrom, Point current, Point start)
    {
        //Reset the entire field to a clean slate
        for (int i = 0; i < maxX; i++)
            for (int j = 0; j < maxY; j++)
                field[i, j] = new RoomTemplate(false);

        //Backtrack from goal to start through the CameFrom list
        while (current != start)
        {
            //Check the direction between the nodes, to save where to put the room's entrance(s)
            CheckDirections(current, cameFrom[current.x, current.y]);

            //Complete the walkthrough algorithm
            current = cameFrom[current.x, current.y];
        }
    }
    void CheckDirections(Point a, Point b)
    {
        //A represents the current room, B the CameFrom room
        int dirA = -1;
        int dirB = -1;      //0 = Up, 1 = Right, 2 = Down, 3 = Left

        //Assumes that (0,0) is the Top Left, and that the nodes are directly connected on horizontal and vertical axes.
        if (a.x > b.x)
        {
            dirA = 3;
            dirB = 1;
        }
        else if (a.x < b.x)
        {
            dirA = 1;
            dirB = 3;
        }
        else if (a.y > b.y)
        {
            dirA = 0;
            dirB = 2;
        }
        else if (a.y < b.y)
        {
            dirA = 2;
            dirB = 0;
        }

        //Assign the directions
        if (dirA != -1 && dirB != -1)
        {
            //Decide a random opening
            int o = Random.Range(1, entranceAmt);
            
            field[a.x, a.y].directions[dirA] = o;
            field[b.x, b.y].directions[dirB] = o;
        }
    }

    //Filling algorithms
    void FillIn()
    {
        for (int i = 0; i < maxX; i++)
            for (int j = 0; j < maxY; j++)
            {
                if (field[i, j].isRoom)
                    continue;

                //Make random entrances for each of the four walls
                for (int e = 0; e < 4; e++)
                {
                    //Roll a dice to determine wether this side gets an entrance or not
                    if (Random.Range(0, entranceChance) != 0)
                        continue;

                    //Continue if the direction would move outside of the array
                    if (e == 0 && j == 0 || e == 2 && j == maxY - 1 ||
                        e == 3 && i == 0 || e == 1 && i == maxX - 1)
                        continue;

                    //Decide a random opening
                    int o = Random.Range(1, entranceAmt);
                    
                    //Open the wall
                    field[i, j].directions[e] = o;

                    //Open the opposing wall
                    switch (e)
                    { 
                        case (0):
                            field[i, j - 1].directions[2] = o;
                            break;
                        case (1):
                            field[i + 1, j].directions[3] = o;
                            break;
                        case (2):
                            field[i, j + 1].directions[0] = o;
                            break;
                        case (3):
                            field[i - 1, j].directions[1] = o;
                            break;
                    }
                }
            }
    }
    void FloodFill(int x, int y)
    {
        if (field[x, y].isRoom)
            return;

        field[x, y].isRoom = true;

        if (field[x, y].directions[0] != 0)
            FloodFill(x, y - 1);
        if (field[x, y].directions[1] != 0)
            FloodFill(x + 1, y);
        if (field[x, y].directions[2] != 0)
            FloodFill(x, y + 1);
        if (field[x, y].directions[3] != 0)
            FloodFill(x - 1, y);
    }

    //Drawing the level from the presets
    void DrawLevel()
    {
        //Draws the field with (0,0) as the Top Left room
        for (int i = 0; i < maxX; i++)
            for (int j = 0; j < maxY; j++)
                if (field[i, j].isRoom)
                    roomArray[i, j] = NewRoom(i, j);
    }
    FieldScript NewRoom(int x, int y)
    {
        //Instantiates a new room according to the preset
        GameObject room = RoomPreset;
        RoomPresetScript roomPreset = room.GetComponent<RoomPresetScript>();

        //Create openings
        roomPreset.Up(field[x, y].directions[0]);
        roomPreset.Right(field[x, y].directions[1]);
        roomPreset.Down(field[x, y].directions[2]);
        roomPreset.Left(field[x, y].directions[3]);

        //Create the contents
        int i = Random.Range(0, 3);
        roomPreset.Contents(i);

        //Instantiate the room as a child of the Level Generator
        GameObject o = Instantiate(room, new Vector3(x * 16, -y * 12), Quaternion.Euler(0, 0, 0), this.transform);

        //Assign an ID and return the FieldScript
        FieldScript fS = o.GetComponent<FieldScript>();
        fS.fieldID = new Point(x, y);

        return fS;
    }
}
