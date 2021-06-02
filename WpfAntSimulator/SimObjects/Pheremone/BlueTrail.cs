using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAntSimulator.SimObjects.Pheremone
{
    public class BlueTrail : ISimObject
    {
        public Point Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Color MyColor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Enlarge(Bitmap bm)
        {
            throw new NotImplementedException();
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
