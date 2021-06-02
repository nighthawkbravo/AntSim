using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAntSimulator.SimObjects.Pheremone
{
    public class RedTrail : ISimObject
    {
        public Point Position { get; set; }
        public Color MyColor { get; set; }

        public void Enlarge(Bitmap bm)
        {
            bm.SetPixel(Position.X - 1, Position.Y, MyColor);
            bm.SetPixel(Position.X + 1, Position.Y, MyColor);
            bm.SetPixel(Position.X, Position.Y + 1, MyColor);
            bm.SetPixel(Position.X, Position.Y - 1, MyColor);
        }

        public void Render(Bitmap bm)
        {
            throw new NotImplementedException();
        }

        public void Update(Bitmap bm)
        {
            throw new NotImplementedException();
        }
        public bool ShouldBeRendered()
        {
            return true;
        }
    }
}
