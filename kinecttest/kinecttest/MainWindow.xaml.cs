using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Coding4Fun.Kinect.Wpf;
using Coding4Fun.Kinect.WinForm;
namespace kinecttest

{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly KinectSensorChooser _sensorchooser = new KinectSensorChooser();
        KinectSensor _sensor;

        public MainWindow()
        { 
            InitializeComponent();
            _sensorchooser.Start();
            _sensor = KinectSensor.KinectSensors[0];
            if(_sensor.Status== KinectStatus.Connected)
            {
                _sensor.DepthStream.Enable();
                _sensor.DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(_sensor_DepthFrameReady);
            }
        }

        void _sensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthframe = e.OpenDepthImageFrame())
            {
                if (depthframe == null)
                {
                    return;
                
                }
                byte[] pixels = Generatecoloredbytes(depthframe);
                int Stride = depthframe.Width * 4;
                image1.Source = BitmapSource.Create(depthframe.Width, depthframe.Height, 96, 96, PixelFormats.Bgr32, null, pixels, Stride);
            }
            //throw new NotImplementedException();
        }

        private byte[] Generatecoloredbytes(DepthImageFrame depthframe)
        {
            short[] rawdepthdata = new short[depthframe.PixelDataLength];
            depthframe.CopyPixelDataTo(rawdepthdata);
            Byte[] pixels = new byte[depthframe.Height * depthframe.Width * 4];
            const int blueindex = 0;
            const int greenindex = 1;
            const int redindex = 2;

            for (int depthindex = 0, colorindex = 0; depthindex < rawdepthdata.Length && colorindex < pixels.Length; depthindex++, colorindex += 4)
            {
                int player = rawdepthdata[depthindex] & DepthImageFrame.PlayerIndexBitmask;
                int depth = rawdepthdata[depthindex] >> DepthImageFrame.PlayerIndexBitmaskWidth;
                if (depth <= 500)
                {
                    pixels[colorindex + blueindex] = 255;
                    pixels[colorindex + greenindex] = 0;
                    pixels[colorindex + redindex] = 0;
                
                }
                else if (depth >= 500 && depth<= 1300)
                {
                    pixels[colorindex + blueindex] = 0;
                    pixels[colorindex + greenindex] = 255;
                    pixels[colorindex + redindex] = 0;

                }
                else if (depth >1300 && depth<2000)
                {
                    pixels[colorindex + blueindex] = 0;
                    pixels[colorindex + greenindex] = 0;
                    pixels[colorindex + redindex] = 255;

                }
                else if (depth >= 2000)
                {
                    pixels[colorindex + blueindex] = 255;
                    pixels[colorindex + greenindex] = 255;
                    pixels[colorindex + redindex] = 0;

                }
                
      
            }
            return pixels;
            //throw new NotImplementedException();
        }

       
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

           
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}
