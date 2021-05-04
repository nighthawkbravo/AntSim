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
    class Obstacle : ISimObject
    {
        public Point Position { get; set; }
        public Color MyColor { get; set; }

        public void Update()
        {
            
        }
        public void Render(Bitmap bm)
        {

        }

        public void Enlarge(Bitmap bm)
        {

        }
    }
}
