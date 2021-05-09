
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
        { Direction.north, Direction.northeast, Direction.east, Direction.southeast, Direction.south, Direction.southwest, Direction.west, Direction.northwest, Direction.center, Direction.center};

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

        private List<ISimObject> simObjects;

        private Bitmap bm;

        private string mylightRed = "#FF5555";
        private string mylightGreen = "#42f548";

        public delegate void nextSimulationTick();
        private bool continueCalculating;
        private bool simInit = false;

        private Random rnd;
        

        private Point center = new Point(1126 / 2, 814 / 2);




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
                simObj.Update();
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
            System.Threading.Thread.Sleep(10);
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
    }
}
