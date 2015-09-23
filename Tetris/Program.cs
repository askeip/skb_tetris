﻿using System;
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
            GameInfo gameInfo = JsonConvert.DeserializeObject<GameInfo>(jsonFile);
            //var jsonFile = Directory.GetFiles( AppDomain.CurrentDomain.BaseDirectory,"smallest.json").
            //    FirstOrDefault();
            Console.WriteLine(gameInfo.ToString());
        }

        static void ShowGameCondition(GameInfo gameInfo)
        {
           //foreach (var line in gameInfo)
        }
    }
}