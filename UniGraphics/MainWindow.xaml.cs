using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Numerics;

namespace UniGraphics
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Rect area = new Rect(new Point(-5, 2.5), new Point(0.8, 0.8));

        public MainWindow()
        {
            InitializeComponent();
        }

        private void createFractalBtn_Click(object sender, RoutedEventArgs e)
        {
            fractalImg.Source = drawSet(area);
        }

        Int32 startMandelbrot(Complex c)
        {
            Int32 k = 0;
            Complex z = Complex.Zero;
            while(k < 1000 && z.Magnitude < 4)
            //while(k < 256 * 256 * 256 && z.Magnitude < 4)
            {
                z = z * z + c;
                ++k;
            }
            return k;
        }

        Color colorMap(Int32 k)
        {
            Color color = new Color();
            color.B = (byte)(k / 100 * 25);
            k = k % 100;
            color.G = (byte)(k / 10 * 25);
            color.R = (byte)(k % 10 * 25);
            //color.B = (byte)(k / 256);
            //k = k % 256;
            //color.G = (byte)(k / 256);
            //color.R = (byte)(k % 256);
            color.A = 255;
            return color;
        }

        WriteableBitmap drawSet(Rect area)
        {
            int imgWidth = (int)fractalImg.Width;
            int imgHeight = (int)fractalImg.Height;
            WriteableBitmap btmp = new WriteableBitmap(imgWidth, imgHeight, 96, 96, PixelFormats.Bgra32, null);
            int bytesPerPixel = btmp.Format.BitsPerPixel / 8;
            byte[] pixels = new byte[imgHeight * imgWidth * bytesPerPixel];
            int bytesPerRow = imgWidth * bytesPerPixel;
            int pixelX, pixelY;
            double scaleX = (area.Right - area.Left) / imgWidth;
            double scaleY = (area.Top - area.Bottom) / imgHeight;
            double x, y;
            for (int i = 0; i < pixels.Length; i += bytesPerPixel)
            {
                pixelY = i / bytesPerRow;
                pixelX = i % bytesPerRow / bytesPerPixel;
                x = area.Left + pixelX * scaleX;
                y = area.Top + pixelY * scaleY;
                Complex c = new Complex(x, y);
                Int32 k = startMandelbrot(c);
                Color pixelColor = colorMap(k);
                pixels[i] = pixelColor.B;
                pixels[i + 1] = pixelColor.G;
                pixels[i + 2] = pixelColor.R;
                pixels[i + 3] = pixelColor.A;
            }
            btmp.WritePixels(new Int32Rect(0, 0, imgWidth, imgHeight), pixels, bytesPerRow, 0);
            return btmp;
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            area = new Rect(new Point(sliderx1.Value, slidery1.Value), new Point(sliderx2.Value, slidery2.Value));
            labelx1.Content = sliderx1.Value;
            labelx2.Content = sliderx2.Value;
            labely1.Content = slidery1.Value;
            labely2.Content = slidery2.Value;
            //fractalImg.Source = drawSet(area);
        }
    }
}
