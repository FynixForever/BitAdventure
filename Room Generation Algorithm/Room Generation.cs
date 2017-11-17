using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Room_Path_v1
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] entrance = { 0, 0 };
            int[] exit = { 0, 0 };

            bool cont = true;

            while (cont)
            {

            

            Console.WriteLine("Hey Piemel hier is je random kamer");

            //Console.WriteLine("Entrance Orientation Please");
            //string entranceOr = Console.ReadLine();

            //Console.WriteLine("Entrance location Please");
            //string entranceLoc = Console.ReadLine();

            //Console.WriteLine("Exit Orientation Please");
            //string exitOr = Console.ReadLine();

            //Console.WriteLine("Exit location Please");
            //string exitLoc = Console.ReadLine();

            //entrance = OrToCor(entranceOr);

            //if(entrance[0] == -1)
            //{
            //    entrance[0] = int.Parse(entranceLoc);
            //}
            //else
            //{
            //    entrance[1] = int.Parse(entranceLoc);
            //}

            //exit = OrToCor(exitOr);

            //if (exit[0] == -1)
            //{
            //    exit[0] = int.Parse(exitLoc);
            //}
            //else
            //{
            //    exit[1] = int.Parse(exitLoc);
            //}

            //set random entrance and exit
            Random rand = new Random();

            //decide entrance=
            switch (rand.Next(0, 3)) // up, down, left, right
            {
                case 0:
                    entrance[0] = rand.Next(0, 13);
                    entrance[1] = 8;
                    break;

                case 1:
                    entrance[0] = rand.Next(0, 13);
                    entrance[1] = 0;
                    break;

                case 2:
                    entrance[0] = 0;
                    entrance[1] = rand.Next(0, 8);
                    break;

                case 3:
                    entrance[0] = 13;
                    entrance[0] = rand.Next(0, 8);

                    break;
            }

            //decide exit
            switch (rand.Next(0, 3)) // up, down, left, right
            {
                case 0:
                    exit[0] = rand.Next(0, 13);
                    exit[1] = 8;
                    break;

                case 1:
                    exit[0] = rand.Next(0, 13);
                    exit[1] = 0;
                    break;

                case 2:
                    exit[0] = 0;
                    exit[1] = rand.Next(0, 8);
                    break;

                case 3:
                    exit[0] = 13;
                    exit[0] = rand.Next(0, 8);

                    break;
            }

                //Hardcoded entrance (bottom left) and exit (top right)
                entrance[0] = 0;
                entrance[1] = 0;

                exit[0] = 13;
                exit[1] = 8;

            Console.WriteLine("entrance:" + entrance[0] + ',' + entrance[1]);
            Console.WriteLine("exit:" + exit[0] + ',' + exit[1]);

            string[,] room = GenerateRoom(entrance, exit);

            //Visualize the room
            Console.WriteLine();
            for (int y = 9; y >= 0; y--)
            {
                Console.Write("|");
                for (int x = 0; x < 14; x++)
                {
                    Console.Write(room[x, y]);
                }

                Console.WriteLine("|");
            }

                Console.WriteLine();
                Console.WriteLine("Continue? (Y/N)");

                string yn = Console.ReadLine();

                if (yn.Equals("N"))
                {
                    break;
                }
                
            }
        }
        
        static int[] OrToCor(string or)
        {
            if (or.Equals("E"))
            {
                return new int[] {13,-1};
            }
            else if (or.Equals("S"))
            {
                return new int[] { -1,0};
            }
            else if (or.Equals("W"))
            {
                return new int[]  {0,-1};
            }
            else
            {
                return new int[] {-1,8 };
            }
            
        }

        static string[,] GenerateRoom(int[] ent, int[] exit)
        {
            string[,] room = new string[14, 10];
            


            //fill room
            for(int i = 0; i< 14; i++)
            {
                for(int j = 0; j<10; j++)
                {
                    room[i, j] = " ";
                }
            }

            //mark the entrance and exit
            
            room[exit[0], exit[1]] = "O";
            room[exit[0], exit[1] + 1] = "O";

            room[ent[0], ent[1]] = "I";
            room[ent[0], ent[1] + 1] = "I";

            bool foundI = false;
            Random rnd = new Random();

            //set current location to the exit
            int[] cur = new int[2];
            cur[0] = exit[0];
            cur[1] = exit[1];

            //set left, right, up and down coordinates
            int[] left = { -1, 0 };
            int[] right = { 1, 0 };
            int[] up = { 0, 1 };
            int[] down = { 0, -1 };

            

            while (!foundI)
            {
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
                int c = 15;
                c *= 2;
                
             
                xAbs = c + k;
                yAbs = c + k;
                    
                int mod = xAbs + yAbs + 2*k;

                //set left-right balance
                if (dirVector[0] < 0)
                {
                    l = xAbs - 1;
                    r = l + k;
                }
                else if (dirVector[0] == 0)
                {
                    l = ((xAbs + k)/2) - 1;
                    r = xAbs + k -1;
                }
                else
                {
                    l = k-1;
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



                int dir = rnd.Next(0, 100*mod) % mod;

                //now we set the next location
                if (dir <=l )
                {
                    next[0] += left[0];

                }
                else if(dir <= r)
                {
                    next[0] += right[0];
                }
                else if(dir <= u)
                {
                    next[1] += up[1];
                }
                else if(dir <= d)
                {
                    next[1] += down[1];
                }

                //check if next location is still in the room
                if(!(next[0] < 0) && !(next[0] > 13) && !(next[1] < 0) && !(next[1] > 8))
                {
                    cur[0] = next[0];
                    cur[1] = next[1];
                    //check if next location is the entrance or exit
                    if (!room[cur[0], cur[1]].Equals("I") && !room[cur[0], cur[1]].Equals("O"))
                    {
                        room[cur[0], cur[1]] = "X";
                        if (!room[cur[0], cur[1] + 1].Equals("I") && !room[cur[0], cur[1] + 1].Equals("O"))
                        { 
                        room[cur[0], cur[1] + 1] = "X";
                        }
                    }
                    else if (room[cur[0], cur[1]].Equals("I"))
                    {
                        foundI = true;
                    }
                }
                


            }

            return room;
        }

        
    }
}
