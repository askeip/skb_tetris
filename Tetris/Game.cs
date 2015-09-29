using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Tetris
{
    class Game
    {
        readonly ImmutableArray<ImmutableArray<Cell>> gameField;
        public ImmutableArray<ImmutableArray<Cell>> GameField {get {return gameField;}}
        readonly Pieces pieces;
        public Pieces PubPieces { get { return pieces; } } 
        readonly Piece movingPiece;
        public Piece MovingPiece { get { return movingPiece; } }
        private readonly string commands;
        private readonly int commandNum;
        public int CommandNum { get { return commandNum; } }
        public string Commands { get { return commands; } }
        private readonly int score;
        public int Score { get { return score; } }
        private readonly bool pieceFixed;
        public bool PieceFixed { get { return pieceFixed; } }

        [JsonConstructor]
        public Game(int width,int height,ImmutableArray<Piece> pieces,
            string commands,bool pieceFixed = false)
        {
            this.pieces = CentralizedPieces(pieces, width);
            movingPiece = this.pieces.GetCurrentPiece();
            this.pieces = new Pieces(this.pieces.PiecesArray,this.pieces.CurrentPiece + 1);
            gameField = CreateGameField(width, height);
            this.commands = commands;
            commandNum = 0;
            score = 0;
            this.pieceFixed = pieceFixed;
        }

        public Game(ImmutableArray<ImmutableArray<Cell>> gameField, Pieces pieces,
            Piece movingPiece, int commandNum,string commands, int score,bool pieceFixed = false)
        {
            this.gameField = gameField;
            this.pieces = pieces;
            this.commandNum = commandNum;
            this.commands = commands;
            this.score = score;
            this.movingPiece = movingPiece;
            this.pieceFixed = pieceFixed;
        }

        private ImmutableArray<ImmutableArray<Cell>> CreateGameField(int width, int height,Piece movingPiece = null)
        {
            if (movingPiece == null)
                movingPiece = this.movingPiece;
            ImmutableArray<Cell>[] mutableGameField = new ImmutableArray<Cell>[height];
            Cell[][] arrayField = new Cell[mutableGameField.Length][];
            for (int i = 0; i < mutableGameField.Length;i++)
            {
                Cell[] row = new Cell[width];
                for (int j = 0; j < row.Length; j++)
                {
                    row[j] = null;
                }
                arrayField[i] = row;
            }
            foreach (var point in movingPiece.Cells)
            {
                if (arrayField[point.Y][point.X] == null)
                    arrayField[point.Y][point.X] = point;
                else
                    throw new SmallGameFieldException();
            }
            for (int i = 0; i < mutableGameField.Length; i++)
            {
                mutableGameField[i] = ImmutableArray.Create(arrayField[i]);
            }
            return ImmutableArray.Create(mutableGameField);
        }

        private Pieces CentralizedPieces(ImmutableArray<Piece> piecesArray,int width)
        {
            Piece[] centralizedPieces = new Piece[piecesArray.Length];
            for (int i = 0; i < centralizedPieces.Length; i++)
            {
                //!!!!!!!!!!!!!!!!!!!!!!!!это 1!!!!!!!!!!!!!!
                int xShift = (width - (piecesArray[i].MaxX - piecesArray[i].MinX + 1)) / 2 - piecesArray[i].MinX; //width / 2 - width % 2 - piecesArray[i].MinX;
                int yShift = piecesArray[i].MinY;
                Cell[] centralizedCells = new Cell[piecesArray[i].Cells.Length];
                for (int j = 0; j < centralizedCells.Length; j++)
                { 
                    centralizedCells[j] = piecesArray[i].Cells[j].MoveTowards(xShift).MoveDown(-yShift);
                        //new Cell(piecesArray[i].Cells[j].X + xShift,piecesArray[i].Cells[j].Y - yShift,true);
                }
                centralizedPieces[i] = new Piece(centralizedCells,piecesArray[i].FindCentralCellIndex());
            }
            return new Pieces(ImmutableArray.Create(centralizedPieces));
        }

        public Game PlaceNextPiece()
        {//!!!!!!!!!!!!!!!!!!
            Piece movedPiece = pieces.GetCurrentPiece();
            Pieces piecesNext = pieces.ChangeCurrentPiece();
            Cell[][] mutableGameField = GameFieldMutablePart(0, gameField.Length - 1);
            foreach (var cell in movingPiece.Cells)
            {
                mutableGameField[cell.Y][cell.X] = cell.StopMoving();
            }
            List<int> fullLines = FullLines(mutableGameField);
            mutableGameField = DeleteFullLines(mutableGameField, fullLines);
            foreach (var cell in movedPiece.Cells)
            {
                if (mutableGameField[cell.Y][cell.X] == null)
                    mutableGameField[cell.Y][cell.X] = cell.StartMoving();
                else
                {
                    piecesNext = pieces.ChangeCurrentPiece(); //ne fact
                    return new Game(CreateGameField(gameField[0].Length, gameField.Length,movedPiece),
                        piecesNext, movedPiece, commandNum + 1, commands, score + fullLines.Count - 10,true);
                }

            }
            ImmutableArray<ImmutableArray<Cell>> reworkedGameField =
                ImmutableArray.Create(mutableGameField.Select(z => ImmutableArray.Create(z))
                    .ToArray());
            return new Game(reworkedGameField,piecesNext,movedPiece,commandNum + 1,commands,score + fullLines.Count,true);
        }

        public List<int> FullLines(Cell[][] mutableGameField)
        {
            List<int> fullLines = new List<int>();
            for (int i = movingPiece.MinY; i <= movingPiece.MaxY; i++)
            {
                //!!!!!!!!!!!!!!!!!
                if (mutableGameField[i].Where(z => z != null)
                    .Count() == mutableGameField[i].Length)
                    fullLines.Add(i);
            }
            return fullLines;
        }

        public Cell[][] DeleteFullLines(Cell[][] mutableGameField, List<int> fullLines)
        {
            for (int i = 0; i < fullLines.Count; i++)
            {
                int lineToStart = fullLines[fullLines.Count - 1 - i];
                int lineToStop = i == fullLines.Count - 1 ? 0 : fullLines[fullLines.Count - 2 - i];
                for (int j = lineToStart; j > lineToStop; j--)
                {
                    mutableGameField[j + i] = mutableGameField[j - 1];//ne fact
                }
            }
            for (int i = 0; i < fullLines.Count; i++)
            {
                mutableGameField[i] = new Cell[mutableGameField[i].Length];
            }
            return mutableGameField;
        }

        public Game MovePiece()
        {
            var command = Commands[commandNum]; //commandNum++
            Piece movedPiece = ExecuteCommand(command);
            if (movedPiece == null)
                return PlaceNextPiece();
            int minY = movedPiece.MinY > movingPiece.MinY ? movingPiece.MinY : movedPiece.MinY;
            int maxY = movedPiece.MaxY > movingPiece.MaxY ? movedPiece.MaxY : movingPiece.MaxY;
            Cell[][] changedArrays = GameFieldMutablePart(minY, maxY); //movedPiece,
            int movingYDiff = movingPiece.MinY - minY;
            foreach (var cell in movingPiece.Cells)
            {
                changedArrays[cell.Y - movingPiece.MinY + movingYDiff][cell.X] = null;
            }
            bool wrongPlacement = false;
            int movedYDiff = movedPiece.MinY - minY;
            foreach (var point in movedPiece.Cells)
            {
                if (changedArrays[point.Y - movedPiece.MinY + movedYDiff][point.X] != null)
                {
                    wrongPlacement = true;
                }
                changedArrays[point.Y - movedPiece.MinY + movedYDiff][point.X] = point;
            }
            if (wrongPlacement)
            {//!!!!!!!!!!!!!!!!!
                return PlaceNextPiece();
                //piecesNext = pieces.ChangeCurrentPiece();
                //return new Game(CreateGameField(gameField[0].Length,gameField.Length),
                //    piecesNext, piecesNext.GetCurrentPiece(),commandNum + 1, commands, score);
            }
            return new Game(ReworkedGameField(changedArrays,minY,maxY),pieces,
                movedPiece,commandNum + 1,commands,score);
        }

        public ImmutableArray<ImmutableArray<Cell>> ReworkedGameField(Cell[][] changedArrays, int minY, int maxY)
        {
            ImmutableArray<Cell>[] mutableGameField = new ImmutableArray<Cell>[gameField.Length];
            for (int i = 0; i < minY; i++)
            {
                mutableGameField[i] = gameField[i];
            }
            for (int i = minY; i < maxY + 1; i++)
            {
                mutableGameField[i] = ImmutableArray.Create(changedArrays[i - minY]);
            }
            for (int i = maxY + 1; i < gameField.Length; i++)
            {
                mutableGameField[i] = gameField[i];
            }
            return ImmutableArray.Create(mutableGameField);
        }

        private Cell[][] GameFieldMutablePart(int minY,int maxY) //Piece movedPiece
        {
            int changedArraysAmount = maxY - minY + 1;
            Cell[][] changedArrays = new Cell[changedArraysAmount][];
            for (int i = 0; i < changedArrays.Length; i++)
            {
                changedArrays[i] = new Cell[gameField[0].Length];
                for (int j = 0; j < changedArrays[i].Length; j++)
                {
                    changedArrays[i][j] = gameField[minY + i][j];
                }
            }
            return changedArrays;
        }

        public char GetCurrentCommand()
        {
            return commands[commandNum];
        }

        public Piece ExecuteCommand(char command)
        {
            command = char.ToLower(command);
            Piece result;
            switch (command)
            {
                case 'a':
                    result = MovingPiece.MoveTowards(-1, gameField[0].Length);
                    break;
                case 'd':
                    result = MovingPiece.MoveTowards(1, gameField[0].Length);
                    break;
                case 's':
                    result = MovingPiece.MoveDown(1,gameField.Length);
                    break;
                case 'q':
                    result = MovingPiece.TurnLeft(gameField[0].Length, gameField.Length);
                    break;
                default://case 'e':
                    result = MovingPiece.TurnRight(gameField[0].Length, gameField.Length);
                    break;
            }
            return result;
        }

        public override string ToString()
        {
            StringBuilder fieldInfo = new StringBuilder();
            //fieldInfo.Append((commandNum - 1) + " " + score);
            for (int i = 0; i < gameField.Length; i++)
            {
                for (int j = 0; j < gameField[i].Length; j++)
                {
                    fieldInfo.Append((gameField[i][j] == null ? "." : gameField[i][j].ToString()) + " ");
                }
                fieldInfo.Append("\n");
            }
            return fieldInfo.ToString();
        }
    }

    internal class SmallGameFieldException : Exception
    {
        public override string Message
        {
            get { return "Cant place piece on this gamefield"; }
        }
    }
}
