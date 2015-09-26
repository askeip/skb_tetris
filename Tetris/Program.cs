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
            var jsonFile = File.ReadAllText("smallest.json");
            Game game= JsonConvert.DeserializeObject<Game>(jsonFile);
            
            //Game game = new Game();
            //var jsonFile = Directory.GetFiles( AppDomain.CurrentDomain.BaseDirectory,"smallest.json").
            //    FirstOrDefault();
            Console.WriteLine(game.ToString());
        }

        static void ShowGameCondition(GameInfo gameInfo)
        {
           //foreach (var line in gameInfo)
        }
    }
}
