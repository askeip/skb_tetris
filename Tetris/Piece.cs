using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    class Piece// : IFigure
    {
        readonly ImmutableArray<Cell> cells;
        public ImmutableArray<Cell> Cells { get { return cells; } }
        readonly int minY;
        readonly int maxY;
        readonly int minX;
        //readonly int maxX;
        public int MinY { get { return minY; } }
        public int MaxY { get { return maxY; } }
        public int MinX { get { return minX; } }
        //public int MaxX { get { return maxX; } }

        [JsonConstructor]
        public Piece(ImmutableArray<Cell> cells)
        {
            this.cells = cells;
            foreach (var point in cells)
            {
                if (minX > point.X)
                    minX = point.X;
                if (minY > point.Y)
                    minY = point.Y;
                if (maxY < point.Y)
                    maxY = point.Y;
            }
        }
        public Piece(Cell[] cells)
        {
            this.cells = ImmutableArray.Create<Cell>(cells);
            foreach (var point in cells)
            {
                if (minX > point.X)
                    minX = point.X;
                if (minY > point.Y)
                    minY = point.Y;
                if (maxY < point.Y)
                    maxY = point.Y;
            }
        }

        public Piece TurnRight()
        {
            return TurnFigure(true);
        }

        public Piece TurnLeft()
        {
            return TurnFigure(false);
        }

        public Piece TurnFigure(bool turnRight)
        {
            Cell[] turnedPoints = new Cell[cells.Length];
            turnedPoints[0] = cells[0];
            for (int i = 1; i < cells.Length; i++)
            {
                if (turnRight)
                    turnedPoints[i] = new Cell(cells[i].Y, -cells[i].X);
                else
                    turnedPoints[i] = new Cell(-cells[i].Y, cells[i].X);
            }
            return new Piece(turnedPoints);
        }

        public Piece MoveTowards(int deltaX,int width) 
        {
            Cell[] movedPoints = new Cell[cells.Length];
            for (int i = 0; i < cells.Length; i++)
            {
                movedPoints[i] = cells[i].MoveTowards(deltaX);
                if (movedPoints[i].X >= width || movedPoints[i].X < 0)
                    return null;
            }
            return new Piece(movedPoints);
        }

        public Piece MoveDown(int deltaY,int height) 
        {
            Cell[] movedPoints = new Cell[cells.Length];
            for (int i = 0; i < cells.Length; i++)
            {
                movedPoints[i] = cells[i].MoveDown(deltaY);
                if (movedPoints[i].Y >= height || movedPoints[i].Y < 0)
                    return null;
            }
            return new Piece(movedPoints);
        }
    }
}
