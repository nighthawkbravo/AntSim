
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using Bitmap = System.Drawing.Bitmap;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using System.IO;
using System.Drawing.Imaging;
using WpfAntSimulator.SimObjects;
using System.Text.RegularExpressions;

namespace WpfAntSimulator
{
    public static class Globals
    {
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

        private string mylightRed = "#FF5555";
        private string mylightGreen = "#42f548";

        public delegate void nextSimulationTick();
        private bool continueCalculating;
        private bool simInit = false;

        private Random rnd;

        private List<ISimObject> toBeRemoved = new List<ISimObject>();
        private List<ISimObject> simObjects;
        private Point center = new Point(1126 / 2, 814 / 2);

        private static readonly Regex _regex = new Regex("[^0-9]+"); //regex that matches disallowed text

        private ISimObject selectedObject;


        public MainWindow()
        {
            InitializeComponent();
            simObjects = new List<ISimObject>();
            rnd = new Random(Guid.NewGuid().GetHashCode());

            numOfAnts = Int32.Parse(AntAmount.Text);
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
        private void RenderAll()
        {
            bm = new Bitmap(width, height);
            foreach (var simObj in simObjects)
            {
                simObj.Render(bm);
            }
            DisplayImage(bm);
        }
        private void InitSim()
        {
            simObjects.Add(new Colony(center));
            for (int i=0; i<numOfAnts; ++i)
            {
                // Random rand = new Random(Guid.NewGuid().GetHashCode()); // Very useful for generating random objects with random seeds!
                simObjects.Add(new Ant(Globals.NumToDir(rnd.Next(Globals.directions.Count)), center, new Random(Guid.NewGuid().GetHashCode())));
                
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
            if(Int32.TryParse(AntAmount.Text, out i))
            {
                numOfAnts = i;
            }
        }
        private void ResetSimButton(object sender, RoutedEventArgs e)
        {
            simObjects.Clear();
            simInit = false;
            RenderAll();
        }


        // This function is the engine.
        public void RunNextTick()
        {
            if (!simInit)
            {
                InitSim();
                simInit = true;
            }

            UpdateAll();

            RenderAll();
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
            selectedObject.Position = new Point((int)e.GetPosition(myImage).X, (int)e.GetPosition(myImage).Y);
            ISimObject nextSimObj;
            switch (selectedObject)
            {
                case Obstacle obstacle:
                    (selectedObject as Obstacle).Width = Int32.Parse(ObstacleWidth.Text);
                    (selectedObject as Obstacle).Height = Int32.Parse(ObstacleHeight.Text);
                    nextSimObj = new Obstacle();
                    break;
                case Colony colony:
                    (selectedObject as Colony).Radius = Int32.Parse(NestRadius.Text);
                    nextSimObj = new Colony();
                    break;
                default:
                    return;
            }

            simObjects.Add(selectedObject);
            selectedObject = nextSimObj;
            RenderAll();
        }


    }
}
