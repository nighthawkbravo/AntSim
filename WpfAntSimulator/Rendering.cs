using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WpfAntSimulator.SimObjects;
using Bitmap = System.Drawing.Bitmap;
using Point = System.Drawing.Point;

namespace WpfAntSimulator
{
    public partial class MainWindow
    {

        private void UpdateAll()
        {
            foreach (var simObj in simObjects)
            {
                if (!simObj.ShouldBeRendered())
                {
                    toBeRemoved.Add(simObj);
                    continue;
                }
                simObj.Update();
            }
            if (toBeRemoved.Count > 0)
            {
                foreach (var o in toBeRemoved)
                {
                    simObjects.Remove(o);
                }
                toBeRemoved.Clear();
            }
        }
        private void RenderStatics()
        {
            bmStatic = new Bitmap(width, height);

            foreach (var simObj in simStaticObjects)
            {
                simObj.Render(bmStatic);
            }
        }

        private void RenderAll()
        {
            bm = new Bitmap(width, height);
            //bm = bmStatic;
            foreach (var simObj in simObjects)
            {
                simObj.Render(bm);
            }
            var res = MergedBitmaps(bmStatic, bm);
            DisplayImage(res);
        }

        // Displays image from bitmap.
        private void DisplayImage(Bitmap b)
        {
            myImage.Source = Bitmap2BitmapImage(b); ;
        }
        // Converts Bitmaps to BitmapImages.
        private BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }
    }
}
