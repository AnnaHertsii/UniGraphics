using System;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;

namespace UniGraphics.ColorModels
{
    public class ColorConverter
    {
        public WriteableBitmap Image { get; private set; } = null; //вихідне зображення
        public HSLColor[,] HSLPixels { get; private set; } = null; //масив з пікселями в моделі HSL

        private byte toRGB(float p, float q, float t)
        {
            if (t < 0.0f)
                t += 1.0f;
            else if (t > 1.0f)
                t -= 1.0f;
            if (t < 1.0f / 6)
                return (byte) (255 * (p + ((q - p) * 6 * t)));
            if (t < 0.5f)
                return (byte) (255 * q);
            if (t < 2.0f / 3)
                return (byte) (255 *  (p + ((q - p) * (2.0f / 3 - t) * 6)));
            return (byte) (255 * p);
        }

        public bool Convert(Bitmap InputImage, CancellationToken? token)
        {
            int width = InputImage.Width;
            int height = InputImage.Height;
            HSLPixels = new HSLColor[width, height];
            var tempImage = new WriteableBitmap(width, height, 
                InputImage.HorizontalResolution, InputImage.VerticalResolution, 
                System.Windows.Media.PixelFormats.Bgra32, null);
            int bytesPerPixel = tempImage.Format.BitsPerPixel / 8;
            int bytesAmount = width * height * bytesPerPixel;
            byte[] pixels = new byte[bytesAmount]; //масив пікселів для вихідного зображення
            int bytesPerRow = width * bytesPerPixel;
            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                {
                    if (token != null && token.Value.IsCancellationRequested)
                        return false;
                    var rgbColor = InputImage.GetPixel(x, y); //отримуємо колір піксела
                    var hslColor = new HSLColor();
                    //перетворюємо значення кольорів так, щоб вони були в межах [0;1]
                    float r = rgbColor.R / 255.0f;
                    float g = rgbColor.G / 255.0f;
                    float b = rgbColor.B / 255.0f;
                    //шукаємо максимальне та мінімальне значення
                    float cMax = Math.Max(Math.Max(r, g), b);
                    float cMin = Math.Min(Math.Min(r, g), b);
                    //шукаємо розмах
                    float delta = cMax - cMin;
                    //рахуємо Hue в градусах
                    if (delta == 0.0f) //якщо всі компоненти RGB мають рівне значення
                        hslColor.H = 0; //встановимо тон 0
                    else
                    {
                        if (cMax == r) //якщо найбільшим є значення компоненти R
                        {
                            if (g >= b)
                                hslColor.H = (short)(60 * (g - b) / delta);
                            else
                                hslColor.H = (short)(60 * (g - b) / delta + 360);
                        }
                        else if (cMax == g) //якщо найбільшим є значення компоненти G
                            hslColor.H = (short)(60 * (b - r) / delta + 120);
                        else //якщо найбільшим є значення компоненти B
                            hslColor.H = (short)(60 * (r - g) / delta + 240);
                    }
                    //рахуємо Saturation у відсотках
                    hslColor.S = (byte) (delta / (1 - Math.Abs(1 - (cMax + cMin))) * 100);
                    //рахуємо Lightness у відсотках
                    hslColor.L = (byte)((cMax + cMin) * 50); //те саме, що ((cMax + cMin) / 2 * 100)
                    HSLPixels[x, y] = hslColor; //записуємо колір в масив
                    //конвертація назад в RGB
                    float h = hslColor.H / 360.0f;
                    float s = hslColor.S / 100.0f;
                    float l = hslColor.L / 100.0f;
                    float q;
                    if (l < 0.5f)
                        q = l * (1.0f + s);
                    else
                        q = l + s - l * s;
                    float p = 2 * l - q;
                    float tR = h + 1.0f / 3;
                    float tG = h;
                    float tB = h - 1.0f / 3;
                    int pixelOffset = (y * width + x) * 4;
                    pixels[pixelOffset] = toRGB(p, q, tB);
                    pixels[pixelOffset + 1] = toRGB(p, q, tG);
                    pixels[pixelOffset + 2] = toRGB(p, q, tR);
                    pixels[pixelOffset + 3] = 255;
                }
            if (token != null && token.Value.IsCancellationRequested)
                return false;
            tempImage.WritePixels(new Int32Rect(0, 0, width, height), pixels, bytesPerRow, 0);
            if (token != null && token.Value.IsCancellationRequested)
                return false;
            Image = tempImage;
            return true;
        }
    }
}