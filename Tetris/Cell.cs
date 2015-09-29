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

        public Cell(int x, int y,bool moving)
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
    }

    class NotMovingPointException : Exception
    {
        public override string Message
        {
            get { return "This cell's x and y can be wrong"; }
        }
    }
}
