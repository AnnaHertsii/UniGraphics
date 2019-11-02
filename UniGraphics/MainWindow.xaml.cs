using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Numerics;

namespace UniGraphics
{
    public partial class MainWindow : Window
    {
        private static double fractalScale = 2; //масштабування фрактала
        private Rect area = new Rect(new Point(-fractalScale, fractalScale), 
                            new Point(fractalScale, fractalScale * 3)); //межі комплексних чисел
        private double tolerance = 0.001; //точність пошуку кореня
        private double power = 3; //степінь n в формулі
        private Complex constant = -1; //константа c в формулі

        public MainWindow()
        {
            InitializeComponent();
        }

        private void createFractalBtn_Click(object sender, RoutedEventArgs e)
        {
            fractalImg.Source = drawSet(area); //виклик побудови фрактала
        }

        //функція f(z)
        private Complex f(Complex z)
        {
            return Complex.Pow(z, power) + constant;
        }

        //похідна функції f(z)
        private Complex df(Complex z)
        {
            return power * Complex.Pow(z, power - 1);
        }

        //функція для знаходження кольору конкретної точки
        Color startNewtonGeneration(Complex point)
        {
            int k = 0; //ітерація
            Complex z = point, tempZ;
            while(k < 256)
            {
                tempZ = z - f(z) / df(z); //власне основний код для фрактала Ньютона
                if(Complex.Abs(tempZ - z) < tolerance) //умова, при якій зупиняємо ітерації і надаємо точці колір 
                {
                    Color color = new Color();
                    color.B = (byte)(k); //розділяємо k на компоненти RGB
                    color.G = (byte)(k % 64 * 4);
                    color.R = (byte)(k % 32 * 8);
                    color.A = 255;
                    return color;
                }
                ++k; //збільшення лічильника ітерацій
                z = tempZ;
            }
            return Colors.Black;
        }

        //функція, що повертає зображення, яке містить фрактал
        WriteableBitmap drawSet(Rect area)
        {
            int imgWidth = (int)fractalImg.Width;
            int imgHeight = (int)fractalImg.Height;
            WriteableBitmap btmp = new WriteableBitmap(imgWidth, imgHeight, 96, 96, PixelFormats.Bgra32, null);
            int bytesPerPixel = btmp.Format.BitsPerPixel / 8;
            byte[] pixels = new byte[imgHeight * imgWidth * bytesPerPixel]; //масив пікселів
            int bytesPerRow = imgWidth * bytesPerPixel;
            int pixelX, pixelY;
            //числа для переходу з растрових координат в комплексну площину
            double scaleX = (area.Right - area.Left) / imgWidth;
            double scaleY = (area.Top - area.Bottom) / imgHeight;
            double x, y;
            for (int i = 0; i < pixels.Length; i += bytesPerPixel)
            {
                //шукаємо координати піксела
                pixelY = i / bytesPerRow;
                pixelX = i % bytesPerRow / bytesPerPixel;
                //шукаємо координати в комплексній площині
                x = area.Left + pixelX * scaleX;
                y = area.Top + pixelY * scaleY;
                Complex z = new Complex(x, y);
                //запускаємо ітерації метода Ньютона
                Color pixelColor = startNewtonGeneration(z);
                pixels[i] = pixelColor.B;
                pixels[i + 1] = pixelColor.G;
                pixels[i + 2] = pixelColor.R;
                pixels[i + 3] = pixelColor.A;
            }
            btmp.WritePixels(new Int32Rect(0, 0, imgWidth, imgHeight), pixels, bytesPerRow, 0);
            return btmp;
        }
    }
}
