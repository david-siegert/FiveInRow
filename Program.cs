using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FiveInRow
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Game gameInstance = new Game())
            {
                // start game
                if (!gameInstance.StartGameSafe())
                    return;

                // chcek status for the first time
                if (!gameInstance.GetStatusSafe())
                    return;

                // first status isnt expected to be any other 
                // because the game can not end in one move
                if (gameInstance.StatusCode != "200")
                {
                    Console.WriteLine("Unexpected status");
                    return;
                }

                // we start the game
                if (gameInstance.MyTurn && gameInstance.Coordinates.Count == 0)
                {
                    if (!gameInstance.PlaySafe(new XY { X = 13, Y = 0 }))
                        return;
                }
                // oponent starts the game and he already played
                else if (gameInstance.MyTurn)
                {
                    XY xy = MoveProcessor.FindMove(gameInstance.Coordinates);

                    if (!gameInstance.PlaySafe(xy))
                        return;
                }

                while (true) // game = infinite loop 
                {
                    if (!gameInstance.GetStatusSafe())
                        return;

                    // game ends
                    if (gameInstance.StatusCode == "226")
                    {
                        if (gameInstance.Victory)
                            Console.WriteLine("Victory!");
                        else
                            Console.WriteLine("Defeat!");

                        Console.ReadKey();
                        return;
                    }

                    if (!gameInstance.MyTurn)
                    {
                        // we to wait a bit before checking status again
                        Console.WriteLine("Waiting for other player");
                        Thread.Sleep(2000);
                        continue;
                    }

                    // My Turn
                    XY xy = MoveProcessor.FindMove(gameInstance.Coordinates);

                    Console.WriteLine($"My move: x = {xy.X}, y = {xy.Y}");
                    if (!gameInstance.PlaySafe(xy))
                        return;
                }

            }
        }
    }


}
