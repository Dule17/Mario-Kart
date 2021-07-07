using System;
using System.Threading;
namespace GameServer
{
    class Program
    {

        public static bool isRunning;
        public static double Ticks_per_Second = 30;
        public static double Frames_per_Tick = 1000/ Ticks_per_Second;
        static void Main(string[] args)
        {
            Console.Title = "GameServer";
            Console.WriteLine("Enter server IP adress:");
            string ip = Console.ReadLine();
            Console.WriteLine("Enter servers max players between 2-4");
            int maxPlayers;
            while(true)
            {
                maxPlayers =int.Parse(Console.ReadLine());
                if (maxPlayers < 2 || maxPlayers > 4) Console.WriteLine("You entered number higher or lower. Try again");
                else break;
            }
            isRunning = true;

            Thread mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();
            
            Server.Start(maxPlayers,ip, 26564);
             
        }

        public static void MainThread()
        {
            Console.WriteLine("MainThread started running at " + Ticks_per_Second);
            DateTime nextLoop = DateTime.Now;
            while(isRunning)
            {
                while(nextLoop<DateTime.Now)
                {
                    GameLogic.Update();
                    nextLoop = nextLoop.AddMilliseconds(Frames_per_Tick);

                    if (nextLoop > DateTime.Now) Thread.Sleep(nextLoop - DateTime.Now);
                }
            }
        }
    }
}
