using System;
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
        readonly Piece movingPiece;
        public  Piece MovingPiece {get { return movingPiece; }}
        private readonly string commands;
        private readonly int commandNum;
        public int CommandNum { get { return commandNum; } }
        public string Commands { get { return commands; } }
        private readonly int score;

        [JsonConstructor]
        public Game(int width,int height,ImmutableArray<Piece> pieces,
            string commands)//ImmutableArray<ImmutableArray<Cell>> gameField
        {
            this.pieces = CentralizedPieces(pieces, width);
            movingPiece = this.pieces.GetCurrentPiece();
            this.pieces = new Pieces(this.pieces.PiecesArray,this.pieces.CurrentPiece + 1);
            gameField = CreateGameField(width, height);
            this.commands = commands;
            commandNum = 0;
            score = 0;
            //commandNumerator = new CommandNumerator();
            //this.gameField = gameField;
            //this.movingPiece = movingPiece;
        }

        private ImmutableArray<ImmutableArray<Cell>> CreateGameField(int width, int height)
        {
            ImmutableArray<Cell>[] gameField = new ImmutableArray<Cell>[height];
            Cell[][] arrayField = new Cell[gameField.Length][];
            for (int i = 0; i < gameField.Length;i++)
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
                arrayField[point.X][point.Y] = point;
            }
            for (int i = 0; i < gameField.Length; i++)
            {
                gameField[i] = ImmutableArray.Create<Cell>(arrayField[i]);
            }
            return ImmutableArray.Create<ImmutableArray<Cell>>(gameField);
        }

        private Pieces CentralizedPieces(ImmutableArray<Piece> piecesArray,int width)
        {
            Piece[] centralizedPieces = new Piece[piecesArray.Length];
            for (int i = 0; i < centralizedPieces.Length; i++)
            {
                int xShift = width / 2 - width % 2 - piecesArray[i].MinX;
                int yShift = piecesArray[i].MinY;
                Cell[] centralizedCells = new Cell[piecesArray[i].Cells.Length];
                for (int j = 0; j < centralizedCells.Length; j++)
                {
                    centralizedCells[j] = new Cell(piecesArray[i].Cells[j].X + xShift,piecesArray[i].Cells[j].Y - yShift,true);
                }
                centralizedPieces[i] = new Piece(centralizedCells);
            }
            return new Pieces(ImmutableArray.Create<Piece>(centralizedPieces));
        }

        public Game(ImmutableArray<ImmutableArray<Cell>> gameField, Pieces pieces,
            Piece movingPiece,int commandNum, int score)
        {
            this.commandNum= commandNum;
            this.pieces = pieces;
            this.gameField = gameField;
            this.movingPiece = movingPiece;
            this.score = score;
        }

        /*public Game PlaceNextFigure()
        {
            Piece piece = pieces.GetCurrentPiece();
            Pieces piecesNext = new Pieces(pieces.PiecesArray, pieces.CurrentPiece + 1);
            Cell[][] placed = new Cell[piece.MaxY - piece.MinY][];
            for (int i = 0; i < placed.Length; i++)
            {
                placed[i] = new Cell[gameField[0].Length];
                for (int j = 0; j < placed[i].Length; j++)
                {
                    placed[i][j] = gameField[piece.MinY + i][j];
                }
            }
            bool wrongPlacement = false;
            foreach (var point in piece.Cells)
            {
                if (placed[point.X][point.Y - piece.MinY] != null)
                    wrongPlacement = true;
                placed[point.X][point.Y - piece.MinY] = point;
            }
            if (wrongPlacement)
            for (int y = piece.MinY; y < piece.MaxY; y++)
            {

            }
        }*/

        /*public Game MovePiece()
        {
            Piece piece = pieces.GetCurrentPiece();
            int minY = piece.MinY < movingPiece.MinY ? piece.MinY : movingPiece.MinY;
            //Cell[][] oldGameField = new Cell[][];
            for (int i = 0; i < piece.MinY; i++)
            {

            }
            Pieces piecesNext = new Pieces(pieces.PiecesArray,pieces.CurrentPiece + 1);
            for (int y = piece.MinY; y < piece.MaxY; y++)
            {

            }
        }*/

        public override string ToString()
        {
            StringBuilder fieldInfo = new StringBuilder();
            fieldInfo.Append(commandNum + " " + score);
            for (int i = 0; i < gameField.Length; i++)
            {
                fieldInfo.Append("\n");
                for (int j = 0; j < gameField[i].Length; j++)
                {
                    fieldInfo.Append((gameField[i][j] == null ? "." : gameField[i][j].ToString()) + " ");
                }
            }
            return fieldInfo.ToString();
        }
    }
}
