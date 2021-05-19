
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



        /// This file was getting quite lengthy
        /// I've moved methods connected to rendering the bitmap to the seperate file
        /// Same with helper functions and event handlers
    }
}
