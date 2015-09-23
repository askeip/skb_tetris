using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    class Figure// : IFigure
    {
        readonly ImmutableArray<Point> cells;
        public ImmutableArray<Point> Cells { get { return cells; } }
        readonly int minY;
        readonly int maxY;
        readonly int minX;
        //readonly int maxX;
        public int MinY { get { return minY; } }
        public int MaxY { get { return maxY; } }
        public int MinX { get { return minX; } }
        //public int MaxX { get { return maxX; } }

        [JsonConstructor]
        public Figure(ImmutableArray<Point> cells)
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
        public Figure(Point[] cells)
        {
            this.cells = ImmutableArray.Create<Point>(cells);
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

        public Figure TurnRight()
        {
            return TurnFigure(true);
            /*Point[] turnedPoints = new Point[figurePoints.Length];
            turnedPoints[0] = figurePoints[0];
            for (int i = 1; i < figurePoints.Length; i++)
            {
                turnedPoints[i] = new Point(figurePoints[i].Y, -figurePoints[i].X);
            }
            return new Figure(turnedPoints);*/
        }

        public Figure TurnLeft()
        {
            return TurnFigure(false);
            /*Point[] turnedPoints = new Point[figurePoints.Length];
            turnedPoints[0] = figurePoints[0];
            for (int i = 1; i < figurePoints.Length; i++)
            {
                turnedPoints[i] = new Point(-figurePoints[i].Y, figurePoints[i].X);
            }
            return new Figure(turnedPoints);*/
        }

        public Figure TurnFigure(bool turnRight)
        {
            Point[] turnedPoints = new Point[cells.Length];
            turnedPoints[0] = cells[0];
            for (int i = 1; i < cells.Length; i++)
            {
                if (turnRight)
                    turnedPoints[i] = new Point(cells[i].Y, -cells[i].X);
                else
                    turnedPoints[i] = new Point(-cells[i].Y, cells[i].X);
            }
            return new Figure(turnedPoints);
        }

        public Figure MoveTowards(int deltaX) //IFigure 
        {
            /*sforeach (var point in figurePoints)
            {
                if (!point.CanMoveTowards(deltaX))
                    return this;
            }*/
            Point[] movedPoints = new Point[cells.Length];
            for (int i = 0; i < cells.Length; i++)
                movedPoints[i] = cells[i].MoveTowards(deltaX);
            return new Figure(movedPoints);
        }

        public Figure MoveDown(int deltaY) //IFigure 
        {
            Point[] movedPoints = new Point[cells.Length];
            for (int i = 0; i < cells.Length; i++)
                movedPoints[i] = cells[i].MoveDown(deltaY);
            return new Figure(movedPoints);
        }

        //public IFigure TurnRight()
        //{
            
        //}
    }
}
