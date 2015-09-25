using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    class Cell
    {
        readonly int x;
        public int X { get { return x; } }
        readonly int y;
        public int Y { get { return y; } }

        public Cell(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Cell MoveTowards(int deltaX)
        {
            return new Cell(x + deltaX, y);
        }

        public Cell MoveDown(int deltaY)
        {
            return new Cell(x, y + deltaY);
        }
    }
}
