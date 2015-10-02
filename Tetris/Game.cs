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
        private readonly int width;
        public int Width { get { return width; } }
        readonly ImmutableArray<ImmutableSortedSet<Cell>> gameField;
        public ImmutableArray<ImmutableSortedSet<Cell>> GameField { get { return gameField; } }
        readonly Pieces pieces;
        public Pieces PubPieces { get { return pieces; } } 
        readonly Piece movingPiece;
        public Piece MovingPiece { get { return movingPiece; } }
        private readonly int score;
        public int Score { get { return score; } }
        private readonly bool pieceFixed;
        public bool PieceFixed { get { return pieceFixed; } }

        public Game(int width, int height, ImmutableArray<Piece> pieces, bool pieceFixed = false)
        {
            this.width = width;
            this.pieces = CentralizedPieces(pieces, width);
            movingPiece = this.pieces.GetCurrentPiece();
            this.pieces = new Pieces(this.pieces.PiecesArray,this.pieces.CurrentPiece + 1);
            gameField = CreateGameField(height);
            score = 0;
            this.pieceFixed = pieceFixed;
        }

        public Game(ImmutableArray<ImmutableSortedSet<Cell>> gameField, Pieces pieces,
            Piece movingPiece, int score, int width, bool pieceFixed = false) 
        {
            this.gameField = gameField;
            this.pieces = pieces;
            this.score = score;
            this.movingPiece = movingPiece;
            this.width = width;
            this.pieceFixed = pieceFixed;
        }

        private ImmutableArray<ImmutableSortedSet<Cell>> CreateGameField(int height, Piece movingPiece = null)
        {
            if (movingPiece == null)
                movingPiece = this.movingPiece != null ? this.movingPiece : pieces.GetCurrentPiece();
            ImmutableSortedSet<Cell>[] mutableGameField = new ImmutableSortedSet<Cell>[height];
            for (int i = 0; i < mutableGameField.Length;i++)
                mutableGameField[i] = ImmutableSortedSet<Cell>.Empty;
            foreach (var point in movingPiece.Cells)
            {
                mutableGameField[point.Y] = mutableGameField[point.Y].Add(point);
            }
            return ImmutableArray.Create(mutableGameField);
        }

        private Pieces CentralizedPieces(ImmutableArray<Piece> piecesArray,int width)
        {
            Piece[] centralizedPieces = new Piece[piecesArray.Length];
            for (int i = 0; i < centralizedPieces.Length; i++)
            {
                int xShift = (width - (piecesArray[i].MaxX - piecesArray[i].MinX + 1)) / 2 - piecesArray[i].MinX;
                int yShift = piecesArray[i].MinY;
                Cell[] centralizedCells = new Cell[piecesArray[i].Cells.Length];
                for (int j = 0; j < centralizedCells.Length; j++)
                { 
                    centralizedCells[j] = piecesArray[i].Cells[j].MoveTowards(xShift).MoveDown(-yShift);
                }
                centralizedPieces[i] = new Piece(centralizedCells,piecesArray[i].FindCentralCellIndex());
            }
            return new Pieces(ImmutableArray.Create(centralizedPieces));
        }

        public Game PlaceNextPiece()
        {//!!!!!!!!!!!!!!!!!!
            Piece movedPiece = pieces.GetCurrentPiece();
            Pieces piecesNext = pieces.ChangeCurrentPiece();
            ImmutableSortedSet<Cell>[] mutableGameField = GameFieldMutablePart(0, gameField.Length - 1);
                /*GameFieldMutablePart(movedPiece.MinY, movedPiece.MaxY)
                    .Concat(GameFieldMutablePart(movingPiece.MinY, movingPiece.MaxY))
                    .ToArray();*/
            //int notIncludedLines = movingPiece.MinY - movedPiece.MaxY - 1;
            //if (notIncludedLines < 0)
            //    notIncludedLines = 0;
            foreach (var cell in movingPiece.Cells)
            {
                mutableGameField[cell.Y] = mutableGameField[cell.Y].Remove(cell); // - notIncludedLines
                mutableGameField[cell.Y] = mutableGameField[cell.Y].Add(cell.StopMoving());
            }
            List<int> fullLines = FullLines();
            mutableGameField = DeleteFullLines(mutableGameField, fullLines);
            foreach (var cell in movedPiece.Cells)
            {
                //if (mutableGameField[cell.Y] == null)
                //    mutableGameField[cell.Y] = new Cell[width];
                if (!mutableGameField[cell.Y].Contains(cell))//[cell.X] == null)
                {
                    mutableGameField[cell.Y] = mutableGameField[cell.Y].Add(cell.StartMoving());
                    //mutableLinesLength[cell.Y]++;
                }
                else
                {
                    piecesNext = pieces.ChangeCurrentPiece(); //ne fact
                    var changedGameField = CreateGameField(gameField.Length, movedPiece);
                    return new Game(changedGameField,
                        piecesNext, movedPiece, score + fullLines.Count - 10, width, true);
                }

            }
            ImmutableArray<ImmutableSortedSet<Cell>> reworkedGameField =
                ImmutableArray.Create(mutableGameField.Select(z => z)
                    .ToArray());
            return new Game(reworkedGameField,piecesNext,movedPiece,score + fullLines.Count,width,true);
        }

        public List<int> FullLines()
        {
            List<int> fullLines = new List<int>();
            for (int i = movingPiece.MinY; i <= movingPiece.MaxY; i++)
            {
                //!!!!!!!!!!!!!!!!!
                if (gameField[i].Count == width) //??-1 not sure
                    fullLines.Add(i);
            }
            return fullLines;
        }

        public ImmutableSortedSet<Cell>[] DeleteFullLines(ImmutableSortedSet<Cell>[] mutableGameField, List<int> fullLines)
        {
            for (int i = 0; i < fullLines.Count; i++)
            {
                int lineToStart = fullLines[fullLines.Count - 1 - i];
                int lineToStop = i == fullLines.Count - 1 ? 0 : fullLines[fullLines.Count - 2 - i];
                for (int j = lineToStart; j > lineToStop; j--)
                {
                    mutableGameField[j + i] = mutableGameField[j - 1];//ne fact
                    //mutableLinesLength[j + i] = mutableLinesLength[j - 1];
                }
            }
            for (int i = 0; i < fullLines.Count; i++)
            {
                mutableGameField[i] = ImmutableSortedSet<Cell>.Empty;// mutableGameField[i].Clear();//null;//new Cell[mutableGameField[i].Length];
                //mutableLinesLength[i] = 0;
            }
            return mutableGameField;
        }

        public Game MovePiece(Piece movedPiece)
        {
            if (movedPiece == null)
                return PlaceNextPiece();
            int minY = movedPiece.MinY > movingPiece.MinY ? movingPiece.MinY : movedPiece.MinY;
            int maxY = movedPiece.MaxY > movingPiece.MaxY ? movedPiece.MaxY : movingPiece.MaxY;
            ImmutableSortedSet<Cell>[] changedArrays = GameFieldMutablePart(minY, maxY); //movedPiece,
            //int[] mutableLinesLength = linesLength.Select(z => z).ToArray();
            int movingYDiff = movingPiece.MinY - minY;
            foreach (var cell in movingPiece.Cells)
            {//?DSLKFJDSNLFSKDJLSEFDKL///
                int y = cell.Y - movingPiece.MinY + movingYDiff;
                //mutableLinesLength[cell.Y]--;
                /*if (mutableLinesLength[cell.Y] == 0)
                    changedArrays[y] = null;
                else*/
                changedArrays[y] = changedArrays[y].Remove(cell);// = null;
            }
            bool wrongPlacement = false;
            int movedYDiff = movedPiece.MinY - minY;
            foreach (var cell in movedPiece.Cells)
            {
                int y = cell.Y - movedPiece.MinY + movedYDiff;
                //if (mutableLinesLength[cell.Y] == 0)
                //    changedArrays[y] = new Cell[width];
                if (changedArrays[y].Contains(cell))// != null)
                {
                    wrongPlacement = true;
                    break;
                }
                changedArrays[y] = changedArrays[y].Add(cell);//[cell.X] = cell;
                //mutableLinesLength[cell.Y]++;
            }
            if (wrongPlacement)
            {
                return PlaceNextPiece();
            }
            return new Game(ReworkedGameField(changedArrays,minY,maxY),pieces,
                movedPiece,score,width);
        }

        public ImmutableArray<ImmutableSortedSet<Cell>> ReworkedGameField(ImmutableSortedSet<Cell>[] changedArrays, int minY, int maxY)
        {
            ImmutableSortedSet<Cell>[] mutableGameField = new ImmutableSortedSet<Cell>[gameField.Length];
            for (int i = 0; i < minY; i++)
            {
                mutableGameField[i] = gameField[i];
            }
            for (int i = minY; i < maxY + 1; i++)
            {
                mutableGameField[i] = changedArrays[i - minY];
            }
            for (int i = maxY + 1; i < gameField.Length; i++)
            {
                mutableGameField[i] = gameField[i];
            }
            return ImmutableArray.Create(mutableGameField);
        }

        private ImmutableSortedSet<Cell>[] GameFieldMutablePart(int minY,int maxY) //Piece movedPiece
        {
            int changedArraysAmount = maxY - minY + 1;
            ImmutableSortedSet<Cell>[] changedArrays = new ImmutableSortedSet<Cell>[changedArraysAmount];
            for (int i = 0; i < changedArrays.Length; i++)
            {
                changedArrays[i] = gameField[minY + i];
                /*if (gameField[minY + i].Length == 0)
                {
                    changedArrays[i] = null;
                    continue;
                }
                changedArrays[i] = new Cell[gameField[minY + i].Length];
                for (int j = 0; j < changedArrays[i].Length; j++)
                {
                    changedArrays[i][j] = gameField[minY + i][j];
                }*/
            }
            return changedArrays;
        }


        public override string ToString()
        {
            StringBuilder fieldInfo = new StringBuilder();
            for (int i = 0; i < gameField.Length; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Cell cell = new Cell(j, i);
                    fieldInfo.Append((!gameField[i].Contains(cell) ? "." : gameField[i][gameField[i].IndexOf(cell)].ToString()) + " ");// gameField[i][j].ToString()
                }
                fieldInfo.Append("\n");
            }
            return fieldInfo.ToString();
        }
    }
}
