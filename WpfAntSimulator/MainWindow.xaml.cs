﻿
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
    public enum Direction
    {
        Left,   // 0
        Up,     // 1
        Right,  // 2
        Down    // 3
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
            rnd = new Random();
            simObjects = new List<ISimObject>();

            numOfAnts = Int32.Parse(AntAmount.Text);
            simObjects.Add(new Colony(center));


            RenderAll();
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
            for (int i=0; i<numOfAnts; ++i)
            {
                simObjects.Add(new Ant(NumToDir(rnd.Next(4)), center));
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



        public void RunNextTick()
        {
            if (!simInit)
            {
                InitSim();
                //simInit = true;
            }

            UpdateAll();

            RenderAll();

            if (continueCalculating)
            {
                StartOrStopButton.Dispatcher.BeginInvoke(
                    DispatcherPriority.SystemIdle,
                    new nextSimulationTick(RunNextTick));
            }
        }







        private Direction NumToDir(int i)
        {
            switch (i)
            {
                case 0:
                    return Direction.Left;
                case 1:
                    return Direction.Up;
                case 2:
                    return Direction.Right;
            }
            return Direction.Down;
        }

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
