using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Room_Path_v1
{
    class Program
    {
        public static Random rnd = new Random();


        static void Main(string[] args)
        {
            int[] entrance = { 0, 0 };
            int[] exit = { 0, 0 };

            bool cont = true;

            while (cont)
            {


                //set random entrance and exit


                //decide entrance=
                switch (rnd.Next(0, 4)) // up, down, left, right
                {
                    case 0:
                        entrance[0] = rnd.Next(0, 14);
                        entrance[1] = 8;
                        break;

                    case 1:
                        entrance[0] = rnd.Next(0, 14);
                        entrance[1] = 0;
                        break;

                    case 2:
                        entrance[0] = 0;
                        entrance[1] = rnd.Next(0, 9);
                        break;

                    case 3:
                        entrance[0] = 13;
                        entrance[0] = rnd.Next(0, 9);

                        break;
                }

                //decide exit
                switch (rnd.Next(0, 4)) // up, down, left, right
                {
                    case 0:
                        exit[0] = rnd.Next(0, 14);
                        exit[1] = 8;
                        break;

                    case 1:
                        exit[0] = rnd.Next(0, 14);
                        exit[1] = 0;
                        break;

                    case 2:
                        exit[0] = 0;
                        exit[1] = rnd.Next(0, 9);
                        break;

                    case 3:
                        exit[0] = 13;
                        exit[0] = rnd.Next(0, 9);

                        break;
                }

                //Hardcoded entrance (bottom left) and exit (top right)
                entrance[0] = 0;
                entrance[1] = 0;

                exit[0] = 13;
                exit[1] = 8;

                //extra exit
                int[] exit2 = { 13, 0 };
                //extra entrance
                int[] entrance2 = { 0, 8 };


                int[][] entrances = { entrance, entrance2 };
                int[][] objectives = { exit, exit2 };


                char[,] room = GenerateRoom(entrances, objectives);

                //Visualize the room
                Console.WriteLine();

                VisualizeRoom(room);

                Console.WriteLine();
                Console.WriteLine("Continue? (Y/N)");

                string yn = Console.ReadLine();

                if (yn.Equals("N"))
                {
                    break;
                }

            }
        }

        static void VisualizeRoom(char[,] room)
        {
            for (int y = 9; y >= 0; y--)
            {
                Console.Write('|');
                for (int x = 0; x < 14; x++)
                {
                    Console.Write(room[x, y]);
                }

                Console.WriteLine('|');
            }
        }

        static void VisualizeRoom(int[,] room)
        {
            for (int y = 9; y >= 0; y--)
            {
                Console.Write('|');
                for (int x = 0; x < 14; x++)
                {
                    Console.Write(room[x, y]);
                }

                Console.WriteLine('|');
            }
        }

        static char[,] GenerateRoom(int[][] entrances, int[][] objectives)
        {
            //entrances/exits/objectives are defined as follows: the first index gives you the entrance/exit/objective number, the second the x location (column 1) and y location (column 2)

            //create 14 by 10 room
            char[,] room = new char[14, 10];

            //fill room with blanks
            for (int i = 0; i < 14; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    room[i, j] = ' ';
                }
            }

            //First we generate the 'main path' from a random objective to a random entrance, which cannot be equal

            int numObjectives = objectives.Length;
            int numEntrances = entrances.Length;

            //chose random objective
            int rand = rnd.Next(0, numObjectives);
            int[] rndObjective = objectives[rand];

            //chose random entrance, not equal to the random objective
            rand = rnd.Next(0, numEntrances);
            int[] rndEntrance = entrances[rand];
            while (rndEntrance.Equals(rndObjective))
            {
                rand = rnd.Next(0, numEntrances);
                int[] rndTmpEntrance = entrances[rand];
                //Array.Copy(rndTmpEntrance, rndEntrance, rndEntrance.Length);
                rndEntrance = rndTmpEntrance;
            }

            //Generate the 'main path'
            room = GenerateMainPath(room, rndEntrance, rndObjective);

            //now we generate a path for every entrance and exit that is not visited

            for (int i = 0; i < numEntrances; i++)
            {
                int[] currEntrance = entrances[i];
                if (!room[currEntrance[0], currEntrance[1]].Equals('X'))
                {
                    room = GeneratePath(room, currEntrance);
                }
            }

            for (int i = 0; i < numObjectives; i++)
            {
                int[] currObjective = objectives[i];
                if (!room[currObjective[0], currObjective[1]].Equals('X'))
                {
                    room = GeneratePath(room, currObjective);
                }
            }

            //now we make the generated room playable
            MakeRoomPlayable(room, entrances, objectives);

            return room;
        }

        static char[,] GeneratePath(char[,] room, int[] origin)
        {
            //this method just finds a spot in the original path to use as target for a "main" path

            //create a Target
            //first we create a list of arrays that represent locations

            List<int[]> locations = new List<int[]>();

            //fill the list with locations
            for (int i = 0; i < 14; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    int[] tmpLocation = { i, j };
                    locations.Add(tmpLocation);
                }
            }

            //now we find a random target
            bool visited = false;
            int[] target = { 0, 0 };
            int numLocations = locations.Count;

            while (!visited)
            {
                //generate random number to pick a location
                int locationIndex = rnd.Next(0, numLocations);

                //check if it is visited
                int[] currLocation = locations[locationIndex];
                visited = room[currLocation[0], currLocation[1]].Equals('X');
                if (visited)
                {
                    Array.Copy(currLocation, target, 2);
                }
                else
                {
                    locations.RemoveAt(locationIndex);
                    numLocations--;
                }
            }

            //now we can generate a "main" path with origin as exit and target as entrance!
            room = GenerateMainPath(room, target, origin);

            return room;

        }

        static char[,] GenerateMainPath(char[,] room, int[] ent, int[] objective)
        {
            //create a working copy for our room called tempRoom. This is the variable we modify, room is used to do checks.

            char[,] tempRoom = new char[14, 10];
            Array.Copy(room, 0, tempRoom, 0, room.Length);

            //mark the entrance in BOTH rooms
            room[ent[0], ent[1]] = 'X';
            tempRoom[ent[0], ent[1]] = 'X';
            tempRoom[ent[0], ent[1] + 1] = 'X';

            //mark the objective in tempRoom
            tempRoom[objective[0], objective[1]] = 'X';
            tempRoom[objective[0], objective[1] + 1] = 'X';


            bool found = false;


            //set current location to the objective
            int[] cur = new int[2];
            cur[0] = objective[0];
            cur[1] = objective[1];



            //set left, right, up and down coordinates
            int[] left = { -1, 0 };
            int[] right = { 1, 0 };
            int[] up = { 0, 1 };
            int[] down = { 0, -1 };



            while (!found)
            {
                //first of all we check if the current location is already visitable (so check all nearest neighbours)

                //set next location to current location
                int[] next = new int[2];
                next[0] = cur[0];
                next[1] = cur[1];

                int[] dirVector = new int[] { ent[0] - cur[0], ent[1] - cur[1] };

                int xAbs = Math.Abs(dirVector[0]);
                int yAbs = Math.Abs(dirVector[1]);



                int l; int r; int u; int d;

                //Create the denominator, this will be variable
                int k = 200;

                //constant factor, 'force' will be (c+k)/k
                //TODO: make this factor dependant of INITIAL distance to target
                int c = 15;
                c *= 2;


                xAbs = c + k;
                yAbs = c + k;

                int mod = xAbs + yAbs + 2 * k;

                //set left-right balance
                if (dirVector[0] < 0)
                {
                    l = xAbs - 1;
                    r = l + k;
                }
                else if (dirVector[0] == 0)
                {
                    l = ((xAbs + k) / 2) - 1;
                    r = xAbs + k - 1;
                }
                else
                {
                    l = k - 1;
                    r = l + xAbs;
                }


                //set up-down balance
                if (dirVector[1] < 0)
                {
                    u = r + k;
                    d = u + yAbs;
                }
                else if (dirVector[1] == 0)
                {
                    u = r + ((yAbs + k) / 2);
                    d = u + ((yAbs + k) / 2);
                }
                else
                {
                    u = r + yAbs;
                    d = u + k;
                }



                int dir = rnd.Next(0, 100 * mod + 1) % mod;

                //now we set the next location
                if (dir <= l)
                {
                    next[0] += left[0];

                }
                else if (dir <= r)
                {
                    next[0] += right[0];
                }
                else if (dir <= u)
                {
                    next[1] += up[1];
                }
                else if (dir <= d)
                {
                    next[1] += down[1];
                }

                //check if next location is still in the room
                if (!(next[0] < 0) && !(next[0] > 13) && !(next[1] < 0) && !(next[1] > 8))
                {
                    cur[0] = next[0];
                    cur[1] = next[1];
                    //check if next location is an already filled spot from the main path
                    if (!room[cur[0], cur[1]].Equals('X'))
                    {
                        tempRoom[cur[0], cur[1]] = 'X';

                        //check the spot above the next location
                        if (!room[cur[0], cur[1] + 1].Equals('X'))
                        {
                            tempRoom[cur[0], cur[1] + 1] = 'X';
                        }
                        else if (room[cur[0], cur[1] + 1].Equals('X'))
                        {
                            found = true;

                            //mark the spot
                            tempRoom[cur[0], cur[1] + 1] = 'X';

                            //room becomes tempRoom
                            room = tempRoom;
                        }
                    }
                    else if (room[cur[0], cur[1]].Equals('X'))
                    {
                        found = true;

                        //mark the spot
                        tempRoom[cur[0], cur[1]] = 'X';

                        //room becomes tempRoom
                        room = tempRoom;
                    }
                }



            }

            return room;
        }

        static char[,] MakeRoomPlayable(char[,] room, int[][] entrances, int[][] objectives)
        {
            //First we convert our room so we can create playable paths from entrances to exits and objectives
            int[,] convertedRoom = ConvertRoom(room, entrances, objectives);

            //visualize the current converted roomn
            char[,] visRoom = new char[14, 10];
            for (int i = 0; i < 14; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (convertedRoom[i, j].Equals(0))
                    {
                        visRoom[i, j] = 'X';
                    }
                    else
                    {
                        visRoom[i, j] = ' ';
                    }
                }
            }

            Console.WriteLine();
            VisualizeRoom(visRoom);

            //Now we run a DFS to find a path from each entrance to each objective and make sure that path is playable
            int[] entrance = entrances[0];
            int[] objective = objectives[0];
            int[,] path = DFS(convertedRoom, entrance, objective);

            //finally we smooth our path

            int[,] smoothPath = SmoothPath(entrance, path);

            //visualize path
            for (int i = 0; i < 14; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (path[i, j] > 0)
                    {
                        visRoom[i, j] = 'X';
                    }
                    else
                    {
                        visRoom[i, j] = ' ';
                    }
                }
            }

            Console.WriteLine();
            VisualizeRoom(visRoom);

            //visualize smoothPath
            for (int i = 0; i < 14; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (smoothPath[i, j] > 0)
                    {
                        visRoom[i, j] = 'X';
                    }
                    else
                    {
                        visRoom[i, j] = ' ';
                    }
                }
            }

            Console.WriteLine();
            VisualizeRoom(visRoom);


            //visualize path with numbers
            for (int j = 9; j >= 0; j--)
            {
                for (int i = 0; i < 14; i++)
                {
                    if (path[i, j] >= 0)
                    {
                        if (path[i, j] > 9)
                        {
                            Console.Write("|" + path[i, j] + "|");
                        }
                        else
                        {
                            Console.Write("|" + "0" + path[i, j] + "|");
                        }
                    }
                    else
                    {
                        Console.Write("    ");
                    }
                }
                Console.WriteLine();
            }

            //Now we have to take our smoothPath and make it playable by editing the room
            char[,] playRoom = MakePathPlayable(room, smoothPath, entrance);



            return room;
        }

        static int[,] ConvertRoom(char[,] room, int[][] entrances, int[][] objectives)
        {
            //create the new room type and fill it with 1's
            int[,] convertedRoom = new int[14, 10];

            for (int i = 0; i < 14; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    convertedRoom[i, j] = 1;
                }
            }

            //now we set the location where there is an ' ' to -1 and locations directly above an ' ' (that aren't ' ' themselves) to 0
            //also all '0-areas' are vertically connected where possible
            for (int j = 0; j < 10; j++)
            {
                for (int i = 0; i < 14; i++)
                {
                    if (room[i, j].Equals(' '))
                    {
                        //we're on a block
                        convertedRoom[i, j] = -1;

                    }
                    else if (j > 0 && room[i, j - 1].Equals(' '))
                    {
                        //[i,j] is now directly above a block and on a free space
                        convertedRoom[i, j] = 0;

                        //we check if we can vertically connect left
                        if (i > 0 && room[i - 1, j].Equals('X'))
                        {
                            int temp = j - 1;

                            convertedRoom[i - 1, j] = 0;


                            while (temp >= 0 && convertedRoom[i - 1, temp].Equals(1))
                            {
                                convertedRoom[i - 1, temp] = 0;

                                temp--;
                            }

                        }

                        //and we check if we can vertically connect right
                        if (i < 13 && room[i + 1, j].Equals('X'))
                        {
                            int temp = j - 1;

                            convertedRoom[i + 1, j] = 0;



                            while (temp >= 0 && convertedRoom[i + 1, temp].Equals(1))
                            {
                                convertedRoom[i + 1, temp] = 0;

                                temp--;
                            }

                        }

                    }
                    else if (j == 0)
                    {
                        //on the ground, this is always a 0
                        convertedRoom[i, j] = 0;
                    }


                }
            }

            //go through all entrances and vertically connect them
            for (int i = 0; i < entrances.Length; i++)
            {
                int xTemp = entrances[i][0];
                int yTemp = entrances[i][1];

                // first we set the location to 0
                convertedRoom[xTemp, yTemp] = 0;

                yTemp--;

                //we set all locations derictly below to 0 until we don't have 1 anymore
                while (yTemp >= 0 && convertedRoom[xTemp, yTemp].Equals(1))
                {
                    convertedRoom[xTemp, yTemp] = 0;

                    yTemp--;
                }
            }

            //go through all objectives and vertically connect them
            for (int i = 0; i < objectives.Length; i++)
            {
                int xTemp = objectives[i][0];
                int yTemp = objectives[i][1];

                // first we set the location to 0
                convertedRoom[xTemp, yTemp] = 0;

                yTemp--;

                //we set all locations derictly below to 0 until we don't have 1 anymore
                while (yTemp >= 0 && convertedRoom[xTemp, yTemp].Equals(1))
                {
                    convertedRoom[xTemp, yTemp] = 0;

                    yTemp--;
                }

            }


            //finally we can set all 1's to -1
            for (int i = 0; i < 14; i++)
            {
                for (int j = 1; j < 10; j++)
                {
                    if (convertedRoom[i, j].Equals(1))
                    {
                        convertedRoom[i, j] = -1;
                    }
                }
            }

            return convertedRoom;
        }

        static int[,] DFS(int[,] convertedRoom, int[] start, int[] goal)
        {
            //we'll use: 0 is unvisited, -1 is not visitable/visited, n is in progress at step n. when recursion is finished just follow 1 and up for the path
            //create roomPath and fill it with convertRoom's values
            int[,] roomPath = new int[14, 10];
            for (int i = 0; i < 14; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    roomPath[i, j] = convertedRoom[i, j];
                }
            }



            //start the recursion
            roomPath = recursiveDFS(roomPath, 1, start, goal);

            //return the path
            return roomPath;
        }

        static int[,] recursiveDFS(int[,] roomPath, int count, int[] location, int[] goal)
        {
            //set location variables
            int xLoc = location[0];
            int yLoc = location[1];

            //set location to in progress
            roomPath[location[0], location[1]] = count;

            //check if we are at goal
            if (location.Equals(goal))
            {
                //end recursion

                return roomPath;
            }

            //Do DFS on all visitable neighbors (in random order)
            //first we check which neighbors we can visit and store that in a list
            List<int[]> neighbors = new List<int[]>();

            //if we can go down, never go left or right!

            bool downPos = false;

            //up
            if (yLoc < 9 && roomPath[xLoc, yLoc + 1] == 0)
            {
                //we can go up
                int[] up = { xLoc, yLoc + 1 };
                neighbors.Add(up);
            }
            //down
            if (yLoc > 0 && roomPath[xLoc, yLoc - 1] == 0)
            {
                //we can go down
                downPos = true;
                int[] down = { xLoc, yLoc - 1 };
                neighbors.Add(down);
            }


            //left
            if (xLoc > 0 && roomPath[xLoc - 1, yLoc] == 0)
            {
                //we can go left
                int[] left = { xLoc - 1, yLoc };
                neighbors.Add(left);
            }
            //right
            if (xLoc < 13 && roomPath[xLoc + 1, yLoc] == 0)
            {
                //we can go right
                int[] right = { xLoc + 1, yLoc };
                neighbors.Add(right);
            }



            //now we can enter recursion we recurse in each possible direction while the goal is not found yet
            while (roomPath[goal[0], goal[1]] < 1)
            {
                //if our list is empty, we have to backtrack our recursion
                if (neighbors.Count == 0)
                {
                    //set current location to -1 and return
                    roomPath[xLoc, yLoc] = -1;

                    return roomPath;
                }

                //chose a random location from our list
                //if down is possible, we can't chose left or right


                int randNum = 0;
                int[] randLoc = { 0, 0 };

                bool done = false;

                while (!done)
                {
                    randNum = rnd.Next(0, neighbors.Count);
                    randLoc = neighbors[randNum];

                    if (!downPos)
                    {
                        done = true;
                    }
                    else if (randLoc[0] != xLoc - 1 && randLoc[0] != xLoc + 1)
                    {
                        //up or down is chosen
                        done = true;

                        //if that location is down, down is not possible anymore
                        if (yLoc > 0 && randLoc[0] == xLoc && randLoc[1] == yLoc - 1)
                        {
                            downPos = false;
                        }
                    }
                }


                //remove that location from the list
                neighbors.RemoveAt(randNum);

                //recurse with count 1 higher
                roomPath = recursiveDFS(roomPath, count + 1, randLoc, goal);
            }
            //goal is found so we add current location to the path


            return roomPath;
        }

        static int[,] SmoothPath(int[] start, int[,] roomPath)
        {
            //create our smoothPath
            int[,] smoothPath = new int[14, 10];

            //fill it with -1
            for (int i = 0; i < 14; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    smoothPath[i, j] = -1;
                }
            }

            //go through our path, visiting the neighbor with highest value, however when down is possible ALWAYS GO DOWN
            bool found = false;
            int count = 0;
            int currNum = 1;
            int nextNum = 0;
            int[] next = { start[0], start[1] };
            int[] curr = { start[0], start[1] };

            while (!found)
            {
                int xLoc = curr[0];
                int yLoc = curr[1];

                //increment count and put it in smoothPath at current location
                count++;
                smoothPath[xLoc, yLoc] = count;


                //up
                if (yLoc < 9 && roomPath[xLoc, yLoc + 1] > 0)
                {
                    //we can go up, so we set currNum
                    currNum = roomPath[xLoc, yLoc + 1];
                    //first check so set nextNum to currNum
                    nextNum = currNum;
                    next[0] = xLoc;
                    next[1] = yLoc + 1;

                }

                //left
                if (xLoc > 0 && roomPath[xLoc - 1, yLoc] > 0)
                {
                    //we can go left
                    currNum = roomPath[xLoc - 1, yLoc];
                    if (currNum > nextNum)
                    {
                        nextNum = currNum;
                        next[0] = xLoc - 1;
                        next[1] = yLoc;
                    }
                }
                //right
                if (xLoc < 13 && roomPath[xLoc + 1, yLoc] > 0)
                {
                    //we can go right
                    currNum = roomPath[xLoc + 1, yLoc];
                    if (currNum > nextNum)
                    {
                        nextNum = currNum;
                        next[0] = xLoc + 1;
                        next[1] = yLoc;
                    }
                }
                //down
                if (yLoc > 0 && roomPath[xLoc, yLoc - 1] > 0)
                {
                    //we can go down
                    currNum = roomPath[xLoc, yLoc - 1];
                    //down should always be done if it's value is higher than our current position, so we go down

                    if (currNum > roomPath[xLoc, yLoc])
                    {
                        nextNum = currNum;
                        next[0] = xLoc;
                        next[1] = yLoc - 1;
                    }

                }

                //if there is no next we are done
                if (next[0] == xLoc && next[1] == yLoc)
                {
                    found = true;
                }



                //we set curr to next
                curr = next;

            }

            return smoothPath;
        }

        static char[,] MakePathPlayable(char[,] room, int[,] smoothPath, int[] start)
        {
            char[,] playableRoom = new char[14, 10];
            playableRoom = room;

            //we traverse our path till we find a vertical that is too high (5 blocks or more)
            int vertical = 0;
            int currDrop = 0;
            int maxDrop = 0;
            int[] maxDropLoc = new int[2];
            maxDropLoc = start;
            int bottomDirection = 0; //-1 is left, +1 is right
            int topDirection = 0;
            int[] curr = new int[2];
            curr = start;
            int[] next = new int[2];
            int currNum = 1;
            int nextNum = 1;
            bool done = false;
            while (!done)
            {
                int xLoc = curr[0];
                int yLoc = curr[1];

                //Now we check all directions, if we find currNum + 1 we go there

                //up
                if (yLoc < 9 && smoothPath[xLoc, yLoc + 1] == currNum + 1)
                {
                    //we go up, so we set nextNum
                    nextNum++;

                    
                        //we go up so we set next and add to vertical
                        next[0] = xLoc;
                        next[1] = yLoc + 1;
                        vertical++;
                    


                }

                //left
                else if (xLoc > 0 && smoothPath[xLoc - 1, yLoc] == currNum + 1)
                {
                    //we go left
                    nextNum++;

                        //we go left so we set next, if we were going vertical we have to check if this is jumpable   
                        next[0] = xLoc - 1;
                        next[1] = yLoc;

                        //if we were going down, we have to check if currDrop is bigger than maxDrop
                        if (currDrop > maxDrop)
                        {
                            maxDrop = currDrop;
                            maxDropLoc = curr;
                        }

                        if (vertical < 4)
                        {
                            bottomDirection = -1;
                        }
                        else
                        {
                            topDirection = -1;

                            //BUGFIXING
                            //we draw the area that is relevant (so from the bottom of the maxDrop to the top of the climb

                            Console.WriteLine("obstacle area:");
                            for (int j = 9; j >= 0; j--)
                            {
                                //start a new line
                                Console.WriteLine();
                                for (int i = 0; i < 14; i++)
                                {
                                    if (bottomDirection == 1)
                                    { //we're going right
                                        if (maxDropLoc[0] <= i && i <= curr[0])
                                        {
                                            //now we are in the relevant area
                                            Console.Write(playableRoom[i, j]);

                                        }
                                        else
                                        {
                                            //we are outside the relevant area
                                            Console.Write(' ');
                                        }
                                    }
                                    else
                                    {
                                        //we're going left
                                        if (maxDropLoc[0] >= i && i >= curr[0])
                                        {
                                            //now we are in the relevant area
                                            Console.Write(playableRoom[i, j]);

                                        }
                                        else
                                        {
                                            //we are outside the relevant area
                                            Console.Write(' ');
                                        }
                                    }
                                }

                            }

                            //not jumpable so we need to edit the room!
                            // playableRoom = FixObstacle(playableRoom, maxDropLoc, curr, vertical, maxDrop, bottomDirection, topDirection);

                            //now that the obstacle is fixed, we can reset our maxDrop
                            maxDrop = 0;

                        }

                        //now we reset vertical and currDrop
                        currDrop = 0;
                        vertical = 0;
                    
                }
                //right
                else if (xLoc < 13 && smoothPath[xLoc + 1, yLoc] == currNum +1)
                {
                    //we go right
                    nextNum++;
                    
                        //we go right so we set next, if we were going vertical we have to check if this is jumpable
                        next[0] = xLoc + 1;
                        next[1] = yLoc;

                        //first we check if currDrop is bigger than maxDrop
                        if (currDrop > maxDrop)
                        {
                            maxDrop = currDrop;
                            maxDropLoc = curr;
                        }

                        if (vertical < 4)
                        {
                            bottomDirection = 1;
                        }
                        else
                        {
                            topDirection = 1;

                            //BUGFIXING
                            //we draw the area that is relevant (so from the bottom of the maxDrop to the top of the climb

                            Console.WriteLine("obstacle area:");

                            for (int j = 9; j >= 0; j--)
                            {
                                //start a new line
                                Console.WriteLine();
                                for (int i = 0; i < 14; i++)
                                {
                                    if (bottomDirection == 1)
                                    { //we're going right
                                        if (maxDropLoc[0] <= i && i <= curr[0])
                                        {
                                            //now we are in the relevant area
                                            Console.Write(playableRoom[i, j]);

                                        }
                                        else
                                        {
                                            //we are outside the relevant area
                                            Console.Write(' ');
                                        }
                                    }
                                    else
                                    {
                                        //we're going left
                                        if (maxDropLoc[0] >= i && i >= curr[0])
                                        {
                                            //now we are in the relevant area
                                            Console.Write(playableRoom[i, j]);

                                        }
                                        else
                                        {
                                            //we are outside the relevant area
                                            Console.Write(' ');
                                        }
                                    }
                                }

                            }


                            //not jumpable so we need to edit the room!
                            //playableRoom = FixObstacle(playableRoom, maxDropLoc, curr, vertical, maxDrop, bottomDirection, topDirection);

                            //the obstacle is fixed so we can reset our maxDrop
                            maxDrop = 0;

                        }

                        //now we reset vertical and currDrop
                        currDrop = 0;
                        vertical = 0;

                    
                    
                }
                //down
                else if (yLoc > 0 && smoothPath[xLoc, yLoc - 1] == currNum +1)
                {
                    //we go down

                    nextNum++;


                    
                        
                    currDrop++;
                    next[0] = xLoc;
                    next[1] = yLoc - 1;
                }

                //if there is no next we are done
                if (nextNum == currNum)
                {
                    done = true;
                }



                //we set curr to next
                currNum++;
                curr = next;
            }



            return playableRoom;
        }

            
        static char[,] FixObstacle(char[,] room, int[] from, int[] to, int climb, int drop, int bottomDirection, int topDirection)
        {
            // from is the location at the bottom of the maximum drop
            // to is the location at the top of our climb

            if (bottomDirection != topDirection)
            {
                //Looparound

                if (drop < 4)
                {
                    //wall-looparound
                }
                else
                {
                    //pit-looparound
                }
            }
            else
            {
                if (drop < 4)
                {
                    //Wall
                    room = FixWall(room, to, climb, topDirection);
                }
                else
                {
                    //Pit
                }
            }
            return room;
        }

        static char[,] FixWall(char[,] room, int[] wallTop, int climb, int direction)
        {
            //we need to decide wether our next action is to jump or to use a ladder. Then to which position this action is,
            //then the next action, and so forth.


            return room;
        }
    }
}
