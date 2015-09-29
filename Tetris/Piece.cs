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
        private readonly int centralCellIndex;
        public int CentralCellIndex { get { return centralCellIndex; } }
        readonly int minY;
        readonly int maxY;
        readonly int minX;
        readonly int maxX;
        public int MinY { get { return minY; } }
        public int MaxY { get { return maxY; } }
        public int MinX { get { return minX; } }
        public int MaxX { get { return maxX; } }

        [JsonConstructor]
        public Piece(ImmutableArray<Cell> cells,int centralCellIndex = -1)
        {
            this.cells = cells;
            this.centralCellIndex = centralCellIndex == -1 ? FindCentralCellIndex() : centralCellIndex;
            minX = int.MaxValue;
            minY = int.MaxValue;
            maxX = int.MinValue;
            maxY = int.MinValue;
            foreach (var point in cells)
            {
                var movingPoint = point.StartMoving();
                if (minX > movingPoint.X)
                    minX = movingPoint.X;
                if (maxX < movingPoint.X)
                    maxX = movingPoint.X;
                if (minY > movingPoint.Y)
                    minY = movingPoint.Y;
                if (maxY < movingPoint.Y)
                    maxY = movingPoint.Y;
            }
        }

        public int FindCentralCellIndex()
        {
            for (int i = 0; i < cells.Length;i++)
                if (cells[i].GetNotMovingX() == 0 && cells[i].GetNotMovingY() == 0)
                    return i;
            throw new NoCentralPointException();
        }

        public Piece(Cell[] cells,int centralCellIndex = -1)
        {
            this.cells = ImmutableArray.Create(cells);
            this.centralCellIndex = centralCellIndex == -1 ? FindCentralCellIndex() : centralCellIndex;
            minX = int.MaxValue;
            minY = int.MaxValue;
            maxX = int.MinValue;
            maxY = int.MinValue;
            foreach (var point in cells)
            {
                //!!!!!!!!!!!!!!!
                if (minX > point.X)
                    minX = point.X;
                if (maxX < point.X)
                    maxX = point.X;
                if (minY > point.Y)
                    minY = point.Y;
                if (maxY < point.Y)
                    maxY = point.Y;
            }
        }

        public Piece TurnRight(int width,int height)
        {
            return TurnFigure(width,height,true);
        }

        public Piece TurnLeft(int width,int height)
        {
            return TurnFigure(width,height,false);
        }

        public Piece TurnFigure(int width,int height,bool turnRight)
        {
            Cell[] turnedPoints = new Cell[cells.Length];
            turnedPoints[centralCellIndex] = cells[centralCellIndex];
            for (int i = 0; i < cells.Length; i++)
            {
                if (i == centralCellIndex)
                    continue;
                turnedPoints[i] = ReflectedCell(cells[i], turnRight);
                if (turnedPoints[i].X >= width || turnedPoints[i].X < 0 ||
                    turnedPoints[i].Y >= height || turnedPoints[i].Y < 0)
                    return null;
            }
            return new Piece(turnedPoints,centralCellIndex);
        }

        private Cell ReflectedCell(Cell cellToReflect,bool turnRight)
        {
            int cci = centralCellIndex;
            int reverse = turnRight ? 1 : -1;
            return new Cell(cells[cci].X - (cellToReflect.Y - cells[cci].Y) * reverse,
                cells[cci].Y + (cellToReflect.X - cells[cci].X) * reverse,true);
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
            return new Piece(movedPoints,centralCellIndex);
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
            return new Piece(movedPoints,centralCellIndex);
        }
    }

    class NoCentralPointException : Exception
    {
        public override string Message
        {
            get { return "This piece have no central point"; }
        }
    }
}
