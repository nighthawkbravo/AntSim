using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Point = System.Drawing.Point;
using Color = System.Drawing.Color;
using Bitmap = System.Drawing.Bitmap;
using Direction = WpfAntSimulator.Globals.Direction;
using System.Windows;

namespace WpfAntSimulator.SimObjects
{
    public class Ant : ISimObject
    {
        public Point PrevPosition { get; set; } // Maybe this won't be neccessary

        private const int width = 1127;
        private const int height = 814; // 814
        private Random rnd;
        private int multiplyer = 1;
        private int lifeSpan;


        public Point Position { get; set; }
        public Color MyColor { get; set; }

        private readonly Color obstacleColor = Color.Brown;

        private Direction prevDir;
        private Direction dir;

        public Ant(Direction d, Point start, Random r)
        {
            dir = d;
            Position = start;
            MyColor = Color.White;
            rnd = r;
            lifeSpan = r.Next(1,201)*20;
        }

        public bool ShouldBeRendered()
        {
            if (lifeSpan <= 0)
            {
                return false;
            }
            return true;
        }

        public void Update(Bitmap bm)
        {
            lifeSpan--;
            prevDir = dir;
            switch (dir)
            {
                case Direction.west:
                    UpdatePos(-1, 0, bm);                    
                    break;
                case Direction.northwest:
                    UpdatePos(-1, 1, bm);
                    break;
                case Direction.north:
                    UpdatePos(0, 1, bm);
                    break;
                case Direction.northeast:
                    UpdatePos(1, 1, bm);
                    break;
                case Direction.east:
                    UpdatePos(1, 0, bm);
                    break;
                case Direction.southeast:
                    UpdatePos(1, -1, bm);
                    break;
                case Direction.south:
                    UpdatePos(0, -1, bm);
                    break;
                case Direction.southwest:
                    UpdatePos(-1, -1, bm);
                    break;
                case Direction.center:
                    break;
            }
            dir = WillIChange(dir);
        }
        private Direction WillIChange(Direction d)
        {
            if (rnd.Next(7) < 3) // 0 - 6
            {
                //return Globals.directions[rnd.Next(Globals.directions.Count)];

                switch (d)
                {
                    case Direction.west:
                        return Globals.directions[rnd.Next(6,9)]; // 6 - 8
                    case Direction.northwest:
                        return Globals.directions[rnd.Next(7, 10)]; // 7-9
                    case Direction.north:
                        return Globals.directions[rnd.Next(0, 3)]; // 0-2
                    case Direction.northeast:
                        return Globals.directions[rnd.Next(1, 4)]; // 1-3
                    case Direction.east:
                        return Globals.directions[rnd.Next(2, 5)]; // 2-4
                    case Direction.southeast:
                        return Globals.directions[rnd.Next(3, 6)]; // 3-5
                    case Direction.south:
                        return Globals.directions[rnd.Next(4, 7)]; // 4-6
                    case Direction.southwest:
                        return Globals.directions[rnd.Next(5, 8)]; // 5-7
                    case Direction.center:
                        return Globals.directions[rnd.Next(Globals.directions.Count)];
                }
            }
            return d;
        }

        private void UpdatePos(int x, int y, Bitmap bm)
        {
            if(Position.X + x*multiplyer >= 0 && Position.X + x * multiplyer < width && !IsObstacle(Position.X + x * multiplyer, Position.Y, bm))
            {
                Position = new Point(Position.X + x * multiplyer, Position.Y);
            }
            if (Position.Y + y * multiplyer >= 0 && Position.Y + y * multiplyer < height && !IsObstacle(Position.X, Position.Y + y * multiplyer, bm))
            {
                Position = new Point(Position.X , Position.Y+y * multiplyer);
            }
        }

        public void Render(Bitmap bm)
        {
            if (lifeSpan == 0)
            {
                return;
            }
            bm.SetPixel(Position.X, Position.Y, MyColor);
            //Enlarge(bm);
        }
        public void Enlarge(Bitmap bm)
        {
            bm.SetPixel(Position.X - 1, Position.Y, MyColor);
            bm.SetPixel(Position.X + 1, Position.Y, MyColor);
            bm.SetPixel(Position.X, Position.Y + 1, MyColor);
            bm.SetPixel(Position.X, Position.Y - 1, MyColor);
        }

        public bool IsObstacle(int i, int j, Bitmap bm)
        {
            var c = bm.GetPixel(i, j);
            if (c.A == obstacleColor.A &&
               c.R == obstacleColor.R &&
               c.G == obstacleColor.G &&
               c.B == obstacleColor.B) return true;
            return false;
        }
    }
}
