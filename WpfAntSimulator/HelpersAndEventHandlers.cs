using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        // ################## Helper functions ##################
        private void ResetSimButton(object sender, RoutedEventArgs e)
        {
            simObjects.Clear();
            simStaticObjects.Clear();
            simInit = false;
            RenderStatics();
            RenderAll();
        }
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private Bitmap MergedBitmaps(Bitmap bmp1, Bitmap bmp2)
        {
            Bitmap result = new Bitmap(Math.Max(bmp1.Width, bmp2.Width),
                                       Math.Max(bmp1.Height, bmp2.Height));
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(bmp2, Point.Empty);
                g.DrawImage(bmp1, Point.Empty);
            }
            return result;
        }

        // ################## Event handlers ##################

        private void SelectObstacle_Click(object sender, RoutedEventArgs e)
        {
            selectedObject = new Obstacle();
        }
        private void SelectNest_Click(object sender, RoutedEventArgs e)
        {
            selectedObject = new Colony();
        }

        private void myImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (selectedObject == null) return;

            bool changeWasMade = false;
            var prevSelectedObject = selectedObject;
            Point prevPos = selectedObject.Position;

            selectedObject.Position = new Point((int)e.GetPosition(myImage).X, (int)e.GetPosition(myImage).Y);
            switch (selectedObject)
            {
                case Obstacle obstacle:
                    int prevW = (selectedObject as Obstacle).Width;
                    int prevH = (selectedObject as Obstacle).Height;

                    int newW = Int32.Parse(ObstacleWidth.Text);
                    int newH = Int32.Parse(ObstacleHeight.Text);

                    (selectedObject as Obstacle).Width = newW;
                    (selectedObject as Obstacle).Height = newH;

                    if (prevW != newW || prevH != newH || AreDifferent(prevPos, selectedObject.Position))
                    {
                        changeWasMade = true;
                    }

                    break;
                case Colony colony:
                    int prevR = (selectedObject as Colony).Radius;
                    int newR = Int32.Parse(NestRadius.Text);
                    (selectedObject as Colony).Radius = newR;

                    simStaticObjects.Remove(OriginalColony);
                    simStaticObjects.Add(OriginalColony = new Colony(selectedObject.Position, newR));

                    if (prevR != newR || AreDifferent(prevPos, selectedObject.Position))
                    {
                        changeWasMade = true;
                    }

                    break;
                default:
                    return;
            }
            if (!simStaticObjects.Contains(selectedObject) || changeWasMade)
            {
                if (changeWasMade)
                {
                    simStaticObjects.Remove(prevSelectedObject);
                    changeWasMade = false;
                }
                simStaticObjects.Add(selectedObject);
                RenderStatics();
            }
            RenderAll();
        }


        private bool AreDifferent(Point a, Point b)
        {
            if (a.X != b.X) return true;
            if (a.Y != b.Y) return true;
            return false;
        }


    }
}
