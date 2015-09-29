using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    class Pieces
    {
        readonly int currentPiece;
        public int CurrentPiece { get { return currentPiece; } }
        readonly ImmutableArray<Piece> piecesArray;
        public ImmutableArray<Piece> PiecesArray { get { return piecesArray; } }

        public Pieces(ImmutableArray<Piece> piecesArray,int currentPiece = 0)
        {
            this.piecesArray = piecesArray;
            this.currentPiece = (currentPiece % piecesArray.Length);
        }

        public Piece GetCurrentPiece()
        {
            return piecesArray[currentPiece];
        }

        public Pieces ChangeCurrentPiece()
        {
            return new Pieces(piecesArray, currentPiece + 1);
        }
    }
}
