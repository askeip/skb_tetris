using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Design;
using System.Diagnostics;
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
            GameInfo gameInfo = JsonConvert.DeserializeObject<GameInfo>(jsonFile);
            Game game = new Game(gameInfo.Width,gameInfo.Height,gameInfo.Pieces);
            Commands commands = new Commands(gameInfo.Commands);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (!commands.IsFinished())
            {
                char command = commands.GetCurrentCommand();
                commands = commands.CommandExecuted();
                if (char.ToLower(command) != 'p')
                {
                    game = game.MovePiece(ExecuteCommand(game,command));
                    if (game.PieceFixed)
                    {
                        Console.WriteLine((commands.CommandNum - 1) + " " + game.Score);
                    }
                }
                else
                {
                    Console.WriteLine(game.ToString());
                }
                /*if (commands.CommandNum > 425)
                {
                    Console.WriteLine(commands.GetCurrentCommand());
                    Console.WriteLine(game.ToString());
                    Console.ReadKey();
                }*/
            }
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        public static Piece ExecuteCommand(Game game,char command)
        {
            command = char.ToLower(command);
            Piece result;
            switch (command)
            {
                case 'a':
                    result = game.MovingPiece.MoveTowards(-1, game.Width);
                    break;
                case 'd':
                    result = game.MovingPiece.MoveTowards(1, game.Width);
                    break;
                case 's':
                    result = game.MovingPiece.MoveDown(1, game.GameField.Length);
                    break;
                case 'q':
                    result = game.MovingPiece.TurnLeft(game.Width, game.GameField.Length);
                    break;
                default://case 'e':
                    result = game.MovingPiece.TurnRight(game.Width, game.GameField.Length);
                    break;
            }
            return result;
        }

        static void ShowGameCondition(GameInfo gameInfo)
        {
           //foreach (var line in gameInfo)
        }
    }

    internal class Commands
    {
        private readonly string commands;
        private readonly int commandNum;
        public int CommandNum { get { return commandNum; } }
        public Commands(string commands,int commandNum = 0)
        {
            this.commands = commands;
            this.commandNum = commandNum;
        }

        public bool IsFinished()
        {
            return commandNum == commands.Length;
        }

        public char GetCurrentCommand()
        {
            return commands[commandNum];
        }

        public Commands CommandExecuted()
        {
            return new Commands(commands,commandNum + 1);
        }
    }
}
