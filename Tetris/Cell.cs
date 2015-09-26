using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    class Cell
    {
        private readonly bool moving;
        public bool Mobing { get { return moving; } }
        readonly int x;
        public int X { get { return x; } }
        readonly int y;
        public int Y { get { return y; } }

        public Cell(int x, int y,bool moving = false)
        {
            this.moving = moving;
            this.x = x;
            this.y = y;
        }

        public Cell MoveTowards(int deltaX)
        {
            return new Cell(x + deltaX, y,true);
        }

        public Cell MoveDown(int deltaY)
        {
            return new Cell(x, y + deltaY,true);
        }

        public override string ToString()
        {
            return moving ? "*" : "#";
        }
    }
}
