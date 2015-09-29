using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Immutable; 
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Tetris
{
    class Program
    {
        static void Main(string[] args)
        {
            var jsonFile = File.ReadAllText("random-w100000-h5-c100000.json");
            Game game= JsonConvert.DeserializeObject<Game>(jsonFile);
            //Game game = new Game();
            //var jsonFile = Directory.GetFiles( AppDomain.CurrentDomain.BaseDirectory,"smallest.json").
            //    FirstOrDefault();
            Console.WriteLine(game);
            for (int i = 0; i < game.Commands.Length; i++)
            {
                char command = game.GetCurrentCommand();
                if (char.ToLower(command) != 'p')
                    game = game.MovePiece();
                else
                {
                    Console.WriteLine(game.ToString());
                    game = new Game(game.GameField,game.PubPieces,game.MovingPiece,game.CommandNum + 1,game.Commands,game.Score);
                }
                if (game.PieceFixed)
                {
                    Console.WriteLine((game.CommandNum - 1) + " " + game.Score);
                    //Console.WriteLine(game);
                    //Console.ReadKey();
                }
            }
            //Console.WriteLine(game.ToString());
        }

        static void ShowGameCondition(GameInfo gameInfo)
        {
           //foreach (var line in gameInfo)
        }
    }
}
