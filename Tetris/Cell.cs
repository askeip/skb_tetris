using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    class Cell : IComparable
    {
        private readonly bool moving;
        public bool Moving { get { return moving; } }
        readonly int x;

        public int X
        {
            get
            {
                if (moving)
                    return x;
                throw new NotMovingPointException();
            }
        }

        readonly int y;
        public int Y
        {
            get
            {
                if (moving)
                    return y;
                throw new NotMovingPointException();
            }
        }

        public Cell(int x, int y,bool moving = true)
        {
            this.moving = moving;
            this.x = x;
            this.y = y;
        }

        public int GetNotMovingX()
        {
            return x;
        }

        public int GetNotMovingY()
        {
            return y;
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

        public Cell StartMoving()
        {
            return new Cell(x,y,true);
        }

        public Cell StopMoving()
        {
            return new Cell(x,y,false);
        }

        public override bool Equals(object obj)
        {
            if (obj is Cell)
            {
                var otherCell = obj as Cell;
                return x == otherCell.x;// && y == otherCell.y;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Y*10000 + x;
        }

        public static bool operator >(Cell firstCell, Cell otheCell)
        {
            return firstCell.x > otheCell.x;
        }
        public static bool operator <(Cell firstCell, Cell otheCell)
        {
            return firstCell.x < otheCell.x;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is Cell))
                throw new ArgumentException();
            var cell = obj as Cell;
            if (this > cell)
                return 1;
            if (this.Equals(cell))
                return 0;
            return -1;
        }
    }

    class NotMovingPointException : Exception
    {
        public override string Message
        {
            get { return "This cell's x and y can be wrong"; }
        }
    }
}
