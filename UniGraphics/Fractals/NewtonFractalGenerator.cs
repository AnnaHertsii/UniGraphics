﻿using System.Numerics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UniGraphics.Fractals
{
    public class NewtonFractalGenerator
    {
        public double FractalScale { get; set; } = 1; //масштабування фрактала
        public double Power { get; set; } = 4; //степінь n в формулі
        public Complex Constant { get; set; } = -1; //константа c в формулі
        private Rect area; //межі комплексних чисел
        private double tolerance = 0.001; //точність пошуку кореня
        private int maxIterations = 512; //максимальна кількість ітерацій
        public delegate Color ColorModel(int k);
        public static readonly ColorModel[] colorModels; //масив з кольоровими моделями
        public ColorModel currentColorModel; //активна кольорова модель
        public WriteableBitmap Image { get; set; } = null;

        static NewtonFractalGenerator()
        {
            colorModels = new ColorModel[]
            {
                Standard,
                Gold,
                Candy,
                Zebra,
                BlackRed
            };
        }

        public NewtonFractalGenerator()
        {
            area = new Rect(new Point(-FractalScale, FractalScale),
                            new Point(FractalScale, FractalScale * 3));
            currentColorModel = colorModels[0];
        }

        public NewtonFractalGenerator(double power, Complex constant, ColorModel model)
        {
            Power = power;
            Constant = constant;
            currentColorModel = model;
            area = new Rect(new Point(-FractalScale, FractalScale),
                            new Point(FractalScale, FractalScale * 3));
        }

        //функція f(z)
        private Complex f(Complex z)
        {
            return Complex.Pow(z, Power) + Constant;
        }

        //похідна функції f(z)
        private Complex df(Complex z)
        {
            return Power * Complex.Pow(z, Power - 1);
        }

        //функція для знаходження кольору конкретної точки
        private Color startNewtonGeneration(Complex point)
        {
            int k = 0; //ітерація
            Complex z = point, tempZ;
            while (k < maxIterations)
            {
                tempZ = z - f(z) / df(z); //власне основний код для фрактала Ньютона
                if (Complex.Abs(tempZ - z) < tolerance) //умова, при якій зупиняємо ітерації і надаємо точці колір 
                    return currentColorModel(k);
                ++k; //збільшення лічильника ітерацій
                z = tempZ;
            }
            return Colors.Black;
        }

        //функція, що повертає зображення, яке містить фрактал
        public void generate(int imgWidth, int imgHeight)
        {
            Image = new WriteableBitmap(imgWidth, imgHeight, 96, 96, PixelFormats.Bgra32, null);
            int bytesPerPixel = Image.Format.BitsPerPixel / 8;
            byte[] pixels = new byte[imgHeight * imgWidth * bytesPerPixel]; //масив пікселів
            int bytesPerRow = imgWidth * bytesPerPixel;
            int pixelX, pixelY; //координати пікселя
            double x, y; //координати в комплексній площині
            //числа для переходу з растрових координат в комплексну площину
            double scaleX = (area.Right - area.Left) / imgWidth;
            double scaleY = (area.Top - area.Bottom) / imgHeight;
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
            Image.WritePixels(new Int32Rect(0, 0, imgWidth, imgHeight), pixels, bytesPerRow, 0);
        }

        public static Color Standard(int k)
        {
            Color color = new Color();
            color.R = (byte)(k % 8 * 32);
            k /= 8;
            color.G = (byte)(k % 8 * 32);
            color.B = (byte)(k / 8 * 32);
            color.A = 255;
            return color;
        }

        public static Color Candy(int k)
        {
            Color color = new Color();
            color.B = (byte)((k & 7) << 5);
            color.G = (byte)((k & 3) << 6);
            color.R = (byte)((k & 1) << 7);
            color.A = 255;
            return color;
        }

        public static Color Gold(int k)
        {
            Color color = new Color();
            color.B = (byte)(k & 255);
            color.G = (byte)((k & 63) << 2);
            color.R = (byte)((k & 31) << 3);
            color.A = 255;
            return color;
        }

        public static Color Zebra(int k)
        {
            Color color = new Color();
            k = (k & 1) * 255;
            color.B = (byte)(k);
            color.G = (byte)(k);
            color.R = (byte)(k);
            color.A = 255;
            return color;
        }

        public static Color BlackRed(int k)
        {
            Color color = new Color();
            color.B = (byte)(k & 255);
            color.G = (byte)((k & 80) << 10);
            color.R = (byte)((k & 1) << 7);
            color.A = 255;
            return color;
        }
    }
}
