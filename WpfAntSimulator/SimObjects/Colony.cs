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
class Colony : ISimObject
    {
        public Point Position { get; set; }
        public Color MyColor { get; set; }

        public Colony(Point p)
        {
            Position = p;
            MyColor = Color.Violet;
        }






        public void Update()
        {

        }

        public void Render(Bitmap bm)
        {
            // This should render a circle and the Position should be its center. For now we can just have it be a 1 pixel point.
            // There also should be a function here that can check whether an ant is on the circle or within it.



            // This is temporary
            bm.SetPixel(Position.X, Position.Y, MyColor);
            Enlarge(bm);
        }


        // This function is to make 1 pixel objects look a bit bigger then they actually are.
        public void Enlarge(Bitmap bm)
        {
            bm.SetPixel(Position.X - 1, Position.Y, MyColor);
            bm.SetPixel(Position.X + 1, Position.Y, MyColor);
            bm.SetPixel(Position.X, Position.Y + 1, MyColor);
            bm.SetPixel(Position.X, Position.Y - 1, MyColor);
        }
    }
}
