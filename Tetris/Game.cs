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
        private readonly int width;
        public int Width { get { return width; } }
        public ImmutableArray<ImmutableArray<Cell>> GameField {get {return gameField;}}
        readonly Pieces pieces;
        public Pieces PubPieces { get { return pieces; } } 
        readonly Piece movingPiece;
        public Piece MovingPiece { get { return movingPiece; } }
        /*private readonly string commands;
        private readonly int commandNum;
        public int CommandNum { get { return commandNum; } }
        public string Commands { get { return commands; } }*/
        private readonly int score;
        public int Score { get { return score; } }
        private readonly bool pieceFixed;
        public bool PieceFixed { get { return pieceFixed; } }
        private readonly ImmutableArray<int> linesLength;

        //[JsonConstructor]
        public Game(int width, int height, ImmutableArray<Piece> pieces, bool pieceFixed = false)
            //string commands,bool pieceFixed = false)
        {
            this.width = width;
            this.pieces = CentralizedPieces(pieces, width);
            movingPiece = this.pieces.GetCurrentPiece();
            this.pieces = new Pieces(this.pieces.PiecesArray,this.pieces.CurrentPiece + 1);
            int[] mutablelinesLength;
            gameField = CreateGameField(width, height,out mutablelinesLength);
            linesLength = ImmutableArray.Create(mutablelinesLength);
            //this.commands = commands;
            //commandNum = 0;
            score = 0;
            this.pieceFixed = pieceFixed;
        }

        public Game(ImmutableArray<ImmutableArray<Cell>> gameField, Pieces pieces,
            Piece movingPiece, int[] mutableLinesLength, int score, int width, bool pieceFixed = false) // int commandNum,string commands, 
        {
            this.gameField = gameField;
            this.pieces = pieces;
            linesLength = ImmutableArray.Create(mutableLinesLength);
            //this.commandNum = commandNum;
            //this.commands = commands;
            this.score = score;
            this.movingPiece = movingPiece;
            this.width = width;
            this.pieceFixed = pieceFixed;
        }

        private ImmutableArray<ImmutableArray<Cell>> CreateGameField(int width, int height,
            out int[] mutablelinesLength, Piece movingPiece = null)
        {
            mutablelinesLength = new int[height];//linesLength == null ? new int[height] : linesLength.Select(z=>z).ToArray();
            if (movingPiece == null)
                movingPiece = this.movingPiece != null ? this.movingPiece : pieces.GetCurrentPiece();
            ImmutableArray<Cell>[] mutableGameField = new ImmutableArray<Cell>[height];
            Cell[][] arrayField = new Cell[mutableGameField.Length][];
            foreach (var point in movingPiece.Cells)
            {
                if (arrayField[point.Y] == null)
                    arrayField[point.Y] = new Cell[width];
                arrayField[point.Y][point.X] = point;
                mutablelinesLength[point.Y]++;
            }
            mutableGameField = arrayField.Select(z => ImmutableArray.Create(z)).ToArray();
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
            List<int> fullLines = FullLines();
            int[] mutableLinesLength = linesLength.Select(z => z).ToArray();
            mutableGameField = DeleteFullLines(mutableGameField, fullLines,mutableLinesLength);
            foreach (var cell in movedPiece.Cells)
            {
                if (mutableGameField[cell.Y] == null)
                    mutableGameField[cell.Y] = new Cell[width];
                if (mutableGameField[cell.Y][cell.X] == null)
                {
                    mutableGameField[cell.Y][cell.X] = cell.StartMoving();
                    mutableLinesLength[cell.Y]++;
                }
                else
                {
                    piecesNext = pieces.ChangeCurrentPiece(); //ne fact
                    var changedGameField = CreateGameField(width, gameField.Length, out mutableLinesLength,
                        movedPiece);
                    return new Game(changedGameField,
                        piecesNext, movedPiece, mutableLinesLength, score + fullLines.Count - 10, width, true);
                }

            }
            ImmutableArray<ImmutableArray<Cell>> reworkedGameField =
                ImmutableArray.Create(mutableGameField.Select(z => ImmutableArray.Create(z))
                    .ToArray());
            return new Game(reworkedGameField,piecesNext,movedPiece,mutableLinesLength,score + fullLines.Count,width,true);
        }

        public List<int> FullLines()
        {
            List<int> fullLines = new List<int>();
            for (int i = movingPiece.MinY; i <= movingPiece.MaxY; i++)
            {
                //!!!!!!!!!!!!!!!!!
                if (linesLength[i] != 0 && linesLength[i] == gameField[i].Length)
                    fullLines.Add(i);
            }
            return fullLines;
        }

        public Cell[][] DeleteFullLines(Cell[][] mutableGameField, List<int> fullLines,int[] mutableLinesLength)
        {
            for (int i = 0; i < fullLines.Count; i++)
            {
                int lineToStart = fullLines[fullLines.Count - 1 - i];
                int lineToStop = i == fullLines.Count - 1 ? 0 : fullLines[fullLines.Count - 2 - i];
                for (int j = lineToStart; j > lineToStop; j--)
                {
                    mutableGameField[j + i] = mutableGameField[j - 1];//ne fact
                    mutableLinesLength[j + i] = mutableLinesLength[j - 1];
                }
            }
            for (int i = 0; i < fullLines.Count; i++)
            {
                mutableGameField[i] = null;//new Cell[mutableGameField[i].Length];
                mutableLinesLength[i] = 0;
            }
            return mutableGameField;
        }

        public Game MovePiece(Piece movedPiece)
        {
            //var command = Commands[commandNum]; //commandNum++
            //Piece movedPiece = GetCurrentCommand(command);
            if (movedPiece == null)
                return PlaceNextPiece();
            int minY = movedPiece.MinY > movingPiece.MinY ? movingPiece.MinY : movedPiece.MinY;
            int maxY = movedPiece.MaxY > movingPiece.MaxY ? movedPiece.MaxY : movingPiece.MaxY;
            Cell[][] changedArrays = GameFieldMutablePart(minY, maxY); //movedPiece,
            int[] mutableLinesLength = linesLength.Select(z => z).ToArray();
            int movingYDiff = movingPiece.MinY - minY;
            foreach (var cell in movingPiece.Cells)
            {//?DSLKFJDSNLFSKDJLSEFDKL///
                int y = cell.Y - movingPiece.MinY + movingYDiff;
                mutableLinesLength[cell.Y]--;
                if (mutableLinesLength[cell.Y] == 0)
                    changedArrays[y] = null;
                else
                    changedArrays[y][cell.X] = null;
            }
            bool wrongPlacement = false;
            int movedYDiff = movedPiece.MinY - minY;
            foreach (var cell in movedPiece.Cells)
            {
                int y = cell.Y - movedPiece.MinY + movedYDiff;
                if (mutableLinesLength[cell.Y] == 0)
                    changedArrays[y] = new Cell[width];
                if (changedArrays[y][cell.X] != null)
                {
                    wrongPlacement = true;
                    break;
                }
                changedArrays[y][cell.X] = cell;
                mutableLinesLength[cell.Y]++;
            }
            if (wrongPlacement)
            {
                return PlaceNextPiece();
            }
            return new Game(ReworkedGameField(changedArrays,minY,maxY),pieces,
                movedPiece,mutableLinesLength,score,width);
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
                if (gameField[minY + i].Length == 0)
                {
                    changedArrays[i] = null;
                    continue;
                }
                changedArrays[i] = new Cell[gameField[minY + i].Length];
                for (int j = 0; j < changedArrays[i].Length; j++)
                {
                    changedArrays[i][j] = gameField[minY + i][j];
                }
            }
            return changedArrays;
        }


        public override string ToString()
        {
            StringBuilder fieldInfo = new StringBuilder();
            //fieldInfo.Append((commandNum - 1) + " " + score);
            for (int i = 0; i < gameField.Length; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    fieldInfo.Append((gameField[i].Length == 0 || gameField[i][j] == null ? "." : gameField[i][j].ToString()) + " ");
                }
                fieldInfo.Append("\n");
            }
            return fieldInfo.ToString();
        }
    }

    /*internal class SmallGameFieldException : Exception
    {
        public override string Message
        {
            get { return "Cant place piece on this gamefield"; }
        }
    }*/
}
