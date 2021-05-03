
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

namespace WpfAntSimulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private const int width = 1127;
        private const int height = 814;

        //private List<ISimObject> simObjects;
        private Bitmap bm;

        private string mylightRed = "#FF5555";
        private string mylightGreen = "#42f548";

        public delegate void nextSimulationTick();
        private bool continueCalculating;
        

        public MainWindow()
        {
            InitializeComponent();
            //simObjects = new List<ISimObject>();
            bm = new Bitmap(width, height);

            //bm.SetPixel((int)1127 / 2, (int) 814 / 2, Color.White);
            //blah();
            
            DisplayImage(bm);
        }

        private void blah()
        {
            for(int i=100; i < 800; ++i)
            {
                for (int j = 100; j < 800; ++j)
                {
                    bm.SetPixel(i, j, Color.White);
                }
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

        public void RunNextTick()
        {
            // Here the engine will render

            if (continueCalculating)
            {
                StartOrStopButton.Dispatcher.BeginInvoke(
                    DispatcherPriority.SystemIdle,
                    new nextSimulationTick(RunNextTick));
            }
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
