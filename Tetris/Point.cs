using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    class Point
    {
        readonly int x;
        public int X { get { return x; } }
        readonly int y;
        public int Y { get { return y; } }

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        /*public bool CanMoveTowards(int deltaX)//, int maxPossibleX)
        {
            var changedX = x + deltaX;
            return !(changedX < 0);// || changedX > maxPossibleX);
        }*/

        public Point MoveTowards(int deltaX)
        {
            return new Point(x + deltaX, y);
        }

        public Point MoveDown(int deltaY)
        {
            return new Point(x, y + deltaY);
        }
    }
}
