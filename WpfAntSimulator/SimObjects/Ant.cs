using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Point = System.Drawing.Point;
using Color = System.Drawing.Color;
using Bitmap = System.Drawing.Bitmap;

namespace WpfAntSimulator.SimObjects
{
    public class Ant : ISimObject
    {
        public Point PrevPosition { get; set; } // Maybe this won't be neccessary

        private const int width = 1127;
        private const int height = 814;


        public Point Position { get; set; }
        public Color MyColor { get; set; }

        private Direction dir;

        public Ant(Direction d, Point start)
        {
            dir = d;
            Position = start;
            MyColor = Color.White;
        }



        public void Update()
        {
            switch (dir)
            {
                case Direction.Left:
                    updatePos(-1, 0);
                    break;
                case Direction.Up:
                    updatePos(0, 1);
                    break;
                case Direction.Right:
                    updatePos(1, 0);
                    break;
                case Direction.Down:
                    updatePos(0, -1);
                    break;
            }
        }

        private void updatePos(int x, int y)
        {
            if(Position.X + x >= 0 && Position.X + x < width)
            {
                Position = new Point(Position.X + x, Position.Y);
            }
            if (Position.Y + y >= 0 && Position.Y + y < height)
            {
                Position = new Point(Position.X, Position.Y+y);
            }
        }

        public void Render(Bitmap bm)
        {
            bm.SetPixel(Position.X, Position.Y, MyColor);
        }
        public void Enlarge(Bitmap bm)
        {
            bm.SetPixel(Position.X - 1, Position.Y, MyColor);
            bm.SetPixel(Position.X + 1, Position.Y, MyColor);
            bm.SetPixel(Position.X, Position.Y + 1, MyColor);
            bm.SetPixel(Position.X, Position.Y - 1, MyColor);
        }
    }
}
