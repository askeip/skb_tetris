using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    class GameLine
    {
        private readonly ImmutableArray<Cell> lineCells;
        public ImmutableArray<Cell> LineCells { get { return lineCells; } }
        private readonly int cellsCount;
        public int CellsCouns { get { return cellsCount; } }
        private readonly int width;

        public GameLine(int width)
        {
            lineCells = ImmutableArray.Create<Cell>();
            cellsCount = 0;
            this.width = width;
        }

        public GameLine(Cell[] mutableLineCells, int cellsCount, int width)
        {
            lineCells = ImmutableArray.Create(mutableLineCells);
            this.cellsCount = cellsCount;
            this.width = width;
        }

        /*public GameLine AddCell(Cell cell)
        {
            Cell[] mutableLineCells = lineCells.Length == 0 ? new Cell[width] : lineCells.Select(z => z).ToArray();
            if (mutableLineCells[cell.X] != null)
                return null;
            mutableLineCells[cell.X] = cell;
            return new GameLine(mutableLineCells,cellsCount + 1,width);
        }

        public GameLine*/
    }
}
