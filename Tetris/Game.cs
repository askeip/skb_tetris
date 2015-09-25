using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Tetris
{
    class Game
    {
        readonly ImmutableArray<ImmutableArray<Cell>> gameField;
        public ImmutableArray<ImmutableArray<Cell>> GameField {get {return gameField;}}
        readonly Pieces pieces;
        private readonly CommandNumerator commandNumerator;
        readonly Piece movingPiece;
        public  Piece MovingPiece {get { return movingPiece; }}
        private readonly string commands;
        public string Commands { get { return commands; } }

        [JsonConstructor]
        public Game(int width,int height,ImmutableArray<Piece> pieces,
            string commands)//ImmutableArray<ImmutableArray<Cell>> gameField
        {
            gameField = CreateGameField(width, height);
            this.pieces = CentralizedPieces(pieces, width);
            this.commands = commands;
            commandNumerator = new CommandNumerator();
            //this.gameField = gameField;
            //this.movingPiece = movingPiece;
        }

        private ImmutableArray<ImmutableArray<Cell>> CreateGameField(int width, int height)
        {
            ImmutableArray<Cell>[] gameField = new ImmutableArray<Cell>[height];
            for (int i = 0; i < gameField.Length;i++)
            {
                Cell[] row = new Cell[width];
                for (int j = 0; j < row.Length; j++)
                {
                    row[j] = new Cell(j,i);
                }
                gameField[i] = ImmutableArray.Create<Cell>(row);
            }
            return ImmutableArray.Create<ImmutableArray<Cell>>(gameField);

        }

        private Pieces CentralizedPieces(ImmutableArray<Piece> piecesArray,int width)
        {
            Piece[] centralizedPieces = new Piece[piecesArray.Length];
            for (int i = 0; i < centralizedPieces.Length; i++)
            {
                int shift = width / 2 - width % 2 - 1 - piecesArray[i].MinX; 
                Cell[] centralizedCells = new Cell[piecesArray[i].Cells.Length];
                for (int j = 0; j < centralizedCells.Length; j++)
                {
                    centralizedCells[j] = new Cell(piecesArray[i].Cells[j].X + shift,piecesArray[i].Cells[j].Y);
                }
                centralizedPieces[i] = new Piece(centralizedCells);
            }
            return new Pieces(ImmutableArray.Create<Piece>(centralizedPieces));
        }

        public Game(ImmutableArray<ImmutableArray<Cell>> gameField, Pieces pieces,
            Piece movingPiece,CommandNumerator commandNumerator)
        {
            this.commandNumerator = commandNumerator;
            this.pieces = pieces;
            this.gameField = gameField;
            this.movingPiece = movingPiece;
        }

        /*public Game PlaceNextFigure()
        {
            Piece piece = pieces.GetCurrentPiece();
            Pieces piecesNext = new Pieces(pieces.PiecesArray,pieces.CurrentPiece + 1);
        }*/
        /*public override string ToString()
        {
            StringBuilder fieldInfo = new StringBuilder();
            fieldInfo.Append(commandNumerator);
        }*/
    }
}
