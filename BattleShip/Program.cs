using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShip
{
    public enum PointStatus { Empty, Ship, Hit, Miss }
    public enum PlayerType { Player, AI }
    public enum ShipType { Carrier, Battleship, Cruiser, Submarine, MineSweeper }
    public enum PlaceShipDirection {Horizontal, Vertical}

    public class Player
    {
        public string Name;
        public PlayerType PlayerType;

        public Player(string name, PlayerType pType)
        {
            this.Name = name;
            this.PlayerType = pType;
        }
    }
    public class Grid
    {
        public Point[,] Ocean;
        public bool AllShipsDestroyed { get { return !ListOfShips.Where(x => !x.isDestroyed).Any(); } }
        public List<Ship> ListOfShips;
        public int CombatRound = 0;
        public Random rnd = new Random(DateTime.Now.Millisecond);
     
        public Grid()
        {
            //Initialize Ocean
            this.Ocean = new Point[10, 10];

            // Fill with Empty
            for (int i = 0; i <= 9; i++)
            {
                for (int j = 0; j <= 9; j++)
                {
                    Ocean[i, j] = new Point(i, j, PointStatus.Empty);
                }
            }
            ListOfShips = new List<Ship>();

            // Fill Ocean(ListOfShips) with the Ships
            for (int i = 0; i < 5; i++)
            {
                Ship newSHip = new Ship((ShipType)i);
                ListOfShips.Add(newSHip);
            }
        }

        //Debug function for print ships
        public void TestPrint()
        {
          for (int i = 0; i < 10; i++)
			{
              string st = "";
			 for (int j = 0; j < 10; j++)
			{

                if (Ocean[i, j].PointStatus == PointStatus.Ship) st += "0";
                else st += "-";
			}
             Console.WriteLine(st);
			}
     
        }

     

        //Display Ocean main function
        public void DisplayOcean()
        {
            Console.ResetColor();
            Console.WriteLine("   0  1  2  3  4  5  6  7  8  9  X");
            for (int i = 0; i < 10; i++)
            {
                Console.Write(i + "|");
                for (int j = 0; j < 10; j++)
                {
                    if (Ocean[j, i].PointStatus == PointStatus.Empty || Ocean[j, i].PointStatus == PointStatus.Ship)
                    {
                        Console.Write("[ ]");
                    }
                    else if (Ocean[j, i].PointStatus == PointStatus.Hit)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("[X]");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("[O]");
                        Console.ResetColor();
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine("Y");

        }

        // Placing ships randomly
        public void PlaceShipsRandomly()
        {       
            int x=0;
            int y=0;
           
            for (int i = 0; i < ListOfShips.Count; i++)
            {
                bool placeThatStuff = false;
                while (!placeThatStuff)
                {

                    bool rightPoint = false;
                    while (!rightPoint)
                    {
                        x = rnd.Next(1, 11)-1;
                        y = rnd.Next(1, 11)-1;
                        if (Ocean[x, y].PointStatus == PointStatus.Empty)
                            rightPoint = true;
                    }

                    int incrementer = 0;
                    PlaceShipDirection pdir = (PlaceShipDirection)rnd.Next(0,2);
                    if (rnd.Next(0, 2) == 0) incrementer = -1; else incrementer = 1;
                    if (canPlaceShip(ListOfShips[i], pdir, incrementer, x, y))
                    {
                        if (pdir == PlaceShipDirection.Horizontal)
                        {
                            if (incrementer < 0)
                                x -= ListOfShips[i].Length;
                        }
                        else if
                             (pdir == PlaceShipDirection.Vertical)
                        {
                            if (incrementer < 0)
                                y -= ListOfShips[i].Length;
                        } 
                         PlaceShip(ListOfShips[i], pdir, x, y);
                         placeThatStuff = true;
                    }
                }
            }
        }

        // return true if there is enough space for ship
        public bool canPlaceShip(Ship shipToPlace, PlaceShipDirection direction, int incrementer, int start_x, int start_y)
        {
            bool value = true;
            int x = start_x;
            int y = start_y;
            
            for (int i = 0; i < shipToPlace.Length; i++)
            {
                if (x < 0 || x > 9) value = false;
                else if (y < 0 || y > 9) value = false;
                else if (Ocean[x, y].PointStatus == PointStatus.Ship) value = false;
               

                if (direction == PlaceShipDirection.Horizontal) x+=incrementer;
                else if (direction == PlaceShipDirection.Vertical) y+=incrementer;

                if (x < 0 || x > 9) value = false;
                else if (y < 0 || y > 9) value = false;
            }
          
            return value;
        }

        // Place one ship
        public void PlaceShip(Ship shipToPlace, PlaceShipDirection direction, int start_x, int start_y)
        {
            int x = start_x;
            int y = start_y;
            for (int i = 0; i < shipToPlace.Length; i++)
            {
                Ocean[x,y].PointStatus = PointStatus.Ship;
                shipToPlace.OccupiedPoints.Add(Ocean[x,y]);

                if (direction == PlaceShipDirection.Horizontal) x++;
                else if (direction == PlaceShipDirection.Vertical) y++;
            }
        }

        /// <summary>
        /// Targets a point and performs game logic on it
        /// </summary>
        /// <param name="x">X Position of Target Point</param>
        /// <param name="y">Y Position of Target Point</param>
        public bool Target(int x, int y)
        {
            bool value = false;
            //Store point in ocean at x, y
            Point target = this.Ocean[x, y];

            //If target status is Ship
            if (target.PointStatus == PointStatus.Ship)
            {
                //Change to a hit
                target.PointStatus = PointStatus.Hit;
                value = true;
            }
            //If status is empty
            else if (target.PointStatus == PointStatus.Empty)
            {
                //Change to a miss
                target.PointStatus = PointStatus.Miss;
            }

            return value;
        }
    }

    public class Point
    {
     public int x;
     public int y;
     public PointStatus PointStatus;
     public Point(int x, int y, PointStatus pointStatus)
     {
         this.x = x;
         this.y = y;
         this.PointStatus = pointStatus;
     }
     

    }
    public class Ship
    {  
       
        public ShipType ShipType;
        public List<Point> OccupiedPoints;
        public int Length;
        public bool isDestroyed
          {
            get {
                return !OccupiedPoints.Where(x => x.PointStatus == PointStatus.Ship).Any();
                }
          }
        public Ship(ShipType shipType)
            { 
                int[] shipLen = new int[] {5,4,3,3,2};
                OccupiedPoints = new List<Point>();
                this.ShipType = shipType;
                this.Length = shipLen[(int)shipType];

            }
        
        
    
    }

    public class Bot
    {
        public List<Point> startOcean = new List<Point>();
        private Point lastHitPoint = new Point(0,0, PointStatus.Empty);
        bool GotIt = false;
        int AttemtsLeft = 0;

        public Point Hit()
        {
            Random rnd = new Random();
            Point pnt = new Point(0, 0, PointStatus.Empty);
            if (!GotIt )
            {
               
                List<Point> notHitYet = startOcean.Where(x => x.PointStatus == PointStatus.Empty).ToList();
                if (notHitYet.Count !=0)
                {
                    int index = rnd.Next(0, notHitYet.Count);
                    pnt = new Point(notHitYet[index].x, notHitYet[index].y, PointStatus.Empty);    
                }
                else
                    pnt = new Point(startOcean[0].x, startOcean[0].y, PointStatus.Empty);   
    
            }
            else
            { 
               List<Point> toHit = new List<Point>();
               if (lastHitPoint.x > 0) toHit.Add( new Point(lastHitPoint.x-1, lastHitPoint.y, PointStatus.Empty));
               if (lastHitPoint.x < 9) toHit.Add(new Point(lastHitPoint.x + 1, lastHitPoint.y, PointStatus.Empty));
               if (lastHitPoint.y > 0) toHit.Add(new Point(lastHitPoint.x, lastHitPoint.y-1, PointStatus.Empty));
               if (lastHitPoint.y < 9) toHit.Add(new Point(lastHitPoint.x, lastHitPoint.y+1, PointStatus.Empty));

               foreach (var item in toHit)
               {
                   item.PointStatus = startOcean.Where(point => point.x == item.x && point.y == item.y).First().PointStatus;
               }

               toHit = toHit.Where(x => x.PointStatus == PointStatus.Empty).ToList();

               AttemtsLeft = toHit.Count;
               if (AttemtsLeft != 0)
               {
                   int index = rnd.Next(0, AttemtsLeft);
                   pnt = new Point(toHit[index].x, toHit[index].y, PointStatus.Empty);
                   AttemtsLeft--;
               
               }
               else
               {
               GotIt = false;
               this.Hit();
               }
            }
            if (AttemtsLeft<=0) 
                GotIt = false;
            return pnt;

          
        }

        public void BotGotIt(Point pnt)
        {
            startOcean.Where(point => point.x == pnt.x && point.y == pnt.y).First().PointStatus = PointStatus.Hit;
            GotIt = true;
            lastHitPoint = pnt;
            AttemtsLeft = 4;
        }

        public void BotNotGotIt(Point pnt)
        {
            startOcean.Where(point => point.x == pnt.x && point.y == pnt.y).First().PointStatus = PointStatus.Miss;
        }



        public Bot()
        {
            // Fill with Empty
            for (int i = 0; i <= 9; i++)
            {
                for (int j = 0; j <= 9; j++)
                {
                    startOcean.Add(new Point(i, j, PointStatus.Empty));
                }
            }
        }
    }

    class Program
    {
        static Grid Ocean; 
        static Grid myOcean; 

        static void Main(string[] args)
        {
            Ocean = new Grid();
            System.Threading.Thread.Sleep(55);
            myOcean = new Grid();
            
            Ocean.PlaceShipsRandomly();
            myOcean.PlaceShipsRandomly();

            Console.ReadLine();
            DisplayOceans(Ocean, myOcean);
            PlayGame();
            Console.ReadKey();
        }

        static void AddHighScore(int playerScore)
        {
            Console.WriteLine("Your name: ");
            string playerName = Console.ReadLine();

            //Create a gateway to the database
            KevinEntities db = new KevinEntities();

            //Create new high score object
            HighScore newHighScore = new HighScore();
            newHighScore.DateCreated = DateTime.Now;
            newHighScore.Game = "Battleship";
            newHighScore.Name = playerName;
            newHighScore.Score = playerScore;

            //Add it to the database
            db.HighScores.Add(newHighScore);

            //Save changes to db
            db.SaveChanges();
        }

        static void DisplayHighScores()
        {
            Console.Clear();
            Console.WriteLine("Battleship High Scores");
            Console.WriteLine("-----------------------------");

            KevinEntities db = new KevinEntities();
            List<HighScore> highScoreList = db.HighScores.Where(x => x.Game == "Battleship").OrderBy(x => x.Score).Take(10).ToList();
            foreach (HighScore i in highScoreList)
            {
                Console.WriteLine("{0}. {1} - {2} - {3}", highScoreList.IndexOf(i) + 1, i.Name, i.Score, i.DateCreated.Value.ToShortDateString());
            }
        }

        /// <summary>
        /// Main Game Loop
        /// </summary>
        static void PlayGame()
        {
            int attacks = 0;
            Bot liitleBot = new Bot();
            //While there are still ships to destroy
            while (!Ocean.AllShipsDestroyed && !myOcean.AllShipsDestroyed)
            {
                int[] TargCoord = new int[2];
                do
                {
                    
                    // Debug Section
                    //for (int i = 0; i < 10; i++)
                    //{
                    //    for (int j = 0; j < 10; j++)
                    //    {

                    //        TargCoord = getTarget(i + "," + j);
                    //        myOcean.Target(TargCoord[0], TargCoord[1]);
                    //        Ocean.Target(TargCoord[0], TargCoord[1]);
                    //    }
                    //}



                    Console.WriteLine("Select your Coordinates (x,y): ");
                    TargCoord = getTarget(Console.ReadLine());
                    attacks++;


 

                }
                while ((TargCoord[0] > 9 || TargCoord[0] < 0) && (TargCoord[1] > 9 || TargCoord[1] < 0));

                Ocean.Target(TargCoord[0], TargCoord[1]);


                if (!Ocean.AllShipsDestroyed)
                {
                    Point pnt = liitleBot.Hit();
                    if (myOcean.Target(pnt.x, pnt.y))
                        liitleBot.BotGotIt(pnt);
                    else liitleBot.BotNotGotIt(pnt);
                }


                Console.Clear();

                DisplayOceans(Ocean, myOcean);

            }

            Console.WriteLine();
            if (Ocean.AllShipsDestroyed)
            {
                Console.WriteLine("Winner, winner!");
                AddHighScore(attacks);
                DisplayHighScores();
                SayString("Congratulations, you've won.");
            }
            else
            {
                Console.WriteLine("You lose");
                SayString("You are a big loser.");
            }
        }

        static void DisplayOceans(Grid ocean1, Grid ocean2)
        {
            Console.ResetColor();
            Console.WriteLine("             Enemy ships                          Player's ships       ");
            Console.WriteLine("    0  1  2  3  4  5  6  7  8  9  X      0  1  2  3  4  5  6  7  8  9  ");
            for (int i = 0; i < 10; i++)
            {
                Console.Write(i + "||");
                for (int j = 0; j < 10; j++)
                {
                    if (ocean1.Ocean[j, i].PointStatus == PointStatus.Empty || ocean1.Ocean[j, i].PointStatus == PointStatus.Ship)
                    {
                        Console.Write("[ ]");
                    }
                    else if (ocean1.Ocean[j, i].PointStatus == PointStatus.Hit)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("[X]");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("[O]");
                        Console.ResetColor();
                    }

                }

                Console.Write("    ");
                Console.Write(i + "||");
                for (int j = 0; j < 10; j++)
                {
                    if (ocean2.Ocean[j, i].PointStatus == PointStatus.Empty)
                    {
                        Console.Write("[ ]");
                    }
                    else if (ocean2.Ocean[j, i].PointStatus == PointStatus.Hit)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("[X]");
                        Console.ResetColor();
                    }
                    else if (ocean2.Ocean[j, i].PointStatus == PointStatus.Ship)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("[O]");
                        Console.ResetColor();
                    }
                    else if (ocean2.Ocean[j, i].PointStatus == PointStatus.Miss)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("[x]");
                        Console.ResetColor();
                    }
                   
                }
                Console.WriteLine();

            }

        }

        static int[] getTarget(string s){
            string[] split = s.Split(',');
            int X, Y;
            int[] coords  = new int[2];
            if(int.TryParse(split[0], out X) && int.TryParse(split[1], out Y)){
                coords = split.Select(x => int.Parse(x)).ToArray();
            }else{
                return new int[2] {-1, -1};
            }
            return coords;
        }

        static void SayString(string s){
            System.Speech.Synthesis.SpeechSynthesizer synth = new System.Speech.Synthesis.SpeechSynthesizer();

            synth.SetOutputToDefaultAudioDevice();
            synth.SelectVoiceByHints(System.Speech.Synthesis.VoiceGender.Female, System.Speech.Synthesis.VoiceAge.Senior);
            synth.Volume = 100;
            synth.Rate = 0;
            synth.Speak(s);
        }
    }


}
