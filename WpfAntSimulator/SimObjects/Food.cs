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
    class Food : ISimObject
    {
        public int FoodAmount { get; set; }


        public Point Position { get; set; }
        public Color MyColor { get; set; }



        public void Update()
        {

        }
        public void Render(Bitmap bm)
        {
            bm.SetPixel(Position.X, Position.Y, MyColor);
        }

        public void Enlarge(Bitmap bm)
        {

        }
    }
}
