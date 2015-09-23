using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;

namespace Tetris
{
    class GameInfo
    {
        readonly int width;
        public int Width { get { return width; } }
        readonly int height;
        public int Height { get { return height; } }
        readonly ImmutableArray<Figure> pieces;
        public ImmutableArray<Figure> Pieces { get { return pieces; } }
        readonly string commands;
        public string Commands { get { return commands; } }

        public GameInfo(int width, int height, ImmutableArray<Figure> pieces, string commands)
        {
            this.width = width;
            this.height = height;
            this.pieces = pieces;// ImmutableArray.Create<ImmutableArray<Point>>(pieces);
            this.commands = commands;
        }

        public override string ToString()
        {
            return "Width: " + Width + ",Height: " + Height + ",Pieces " + pieces[0].Cells[1].X + " " + pieces[0].Cells[1].Y;//,Commands:{3}";
        }
    }

    class CellsClass
    {
        readonly ImmutableArray<Point> cells;
        public ImmutableArray<Point> Cells { get { return cells; } }
        public CellsClass(ImmutableArray<Point> cells)
        {
            this.cells = cells;
        }
    }
}
