
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
    public static class Globals
    {
        public static readonly Color obstacleColor = Color.Brown;
        public static readonly Color foodColor = Color.Green;

        public enum Direction
        {
            north,
            northeast,
            east,
            southeast,
            south,
            southwest,
            west,
            northwest,
            center
        }

        public static List<Direction> directions = new List<Direction>()
        { Direction.northwest, Direction.north, Direction.northeast, Direction.east,    // 0, 1, 2, 3
          Direction.southeast, Direction.south, Direction.southwest,                    // 4, 5, 6
          Direction.west, Direction.northwest, Direction.north,                         // 7, 8, 9
          Direction.center, Direction.center                                            // 10, 11
        };

        public static Direction NumToDir(int i)
        {
            return directions[i];
        }
    }

    public partial class MainWindow : Window
    {

        private const int width = 1127;
        private const int height = 814;

        private int numOfAnts;

        private Bitmap bm;
        private Bitmap bmStatic;
        private Bitmap mergedBMs;

        private string mylightRed = "#FF5555";
        private string mylightGreen = "#42f548";

        public delegate void nextSimulationTick();
        private bool continueCalculating;
        private bool simInit = false;

        private Random rnd;

        private Colony OriginalColony;
        private List<ISimObject> toBeRemoved = new List<ISimObject>();
        private List<ISimObject> simObjects;
        private List<ISimObject> simStaticObjects = new List<ISimObject>();
        private Point center = new Point(1126 / 2, 814 / 2);

        private static readonly Regex _regex = new Regex("[^0-9]+"); //regex that matches disallowed text

        private ISimObject selectedObject;


        public MainWindow()
        {
            InitializeComponent();
            simObjects = new List<ISimObject>();
            rnd = new Random(Guid.NewGuid().GetHashCode());

            numOfAnts = Int32.Parse(AntAmount.Text);
            bmStatic = new Bitmap(width, height);

            RenderAll();
            simStaticObjects.Add(OriginalColony = new Colony(center, 3));
            RenderStatics();
            RenderAll();
        }

        private void UpdateAll()
        {
            foreach (var simObj in simObjects)
            {
                if (!simObj.ShouldBeRendered())
                {
                    toBeRemoved.Add(simObj);
                    continue;
                }
                simObj.Update(mergedBMs);
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
            mergedBMs = MergedBitmaps(bmStatic, bm);


            DisplayImage(mergedBMs);
        }
        private void InitSim()
        {
            for (int i = 0; i < numOfAnts; ++i)
            {
                // Random rand = new Random(Guid.NewGuid().GetHashCode()); // Very useful for generating random objects with random seeds!
                simObjects.Add(new Ant(Globals.NumToDir(rnd.Next(Globals.directions.Count)), OriginalColony.Position, new Random(Guid.NewGuid().GetHashCode())));

            }
        }
        private void StartOrStopSimButton(object sender, RoutedEventArgs e)
        {
            if (continueCalculating)
            {
                continueCalculating = false;
                StartOrStopText.Text = "Resume";
            }
            else
            {
                selectedObject = null;
                continueCalculating = true;
                StartOrStopText.Text = "Stop";
                StartOrStopButton.Dispatcher.BeginInvoke(
                    DispatcherPriority.Normal,
                    new nextSimulationTick(RunNextTick));
            }
        }
        private void AntAmountChange(object sender, TextChangedEventArgs e)
        {
            int i;
            if (Int32.TryParse(AntAmount.Text, out i))
            {
                numOfAnts = i;
            }
        }



        // This function is the engine.
        public void RunNextTick()
        {
            if (!simInit)
            {
                InitSim();
                simInit = true;
                
            }           

            RenderAll();

            UpdateAll();

            System.Threading.Thread.Sleep(8);
            if (continueCalculating)
            {
                StartOrStopButton.Dispatcher.BeginInvoke(
                    DispatcherPriority.SystemIdle,
                    new nextSimulationTick(RunNextTick));
            }
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
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

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
                case Food food:
                    CreateFood(selectedObject.Position, Int32.Parse(FoodWidth.Text), Int32.Parse(FoodHeight.Text));
                    RenderStatics();
                    RenderAll();
                    return;
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

        private void ResetSimButton(object sender, RoutedEventArgs e)
        {
            simObjects.Clear();
            simStaticObjects.Clear();

            simStaticObjects.Add(OriginalColony = new Colony(new Point(563, 407), 3));

            if (StartOrStopText.Text == "Stop")
                StartOrStopSimButton(null, null);
            simInit = false;
            RenderStatics();
            RenderAll();
        }
        public bool IsInBound(int x, int y, Bitmap bm)
        {
            if (x > 0 && x < bm.Width && y > 0 && y < bm.Height)
            {
                return true;
            }
            return false;
        }

        private void SelectFood_Click(object sender, RoutedEventArgs e)
        {
            selectedObject = new Food();
        }

        private void CreateFood(Point point, int width, int height)
        {

            for (int x = point.X - (width / 2); x < point.X + (width / 2); x++)
                for (int y = point.Y - (height / 2); y < point.Y + (height / 2); y++)
                    if (IsInBound(x, y, bm))
                        simStaticObjects.Add(new Food(new Point(x, y)));
        }
    }
}