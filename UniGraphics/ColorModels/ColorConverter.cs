using System;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;

namespace UniGraphics.ColorModels
{
    public class ColorConverter
    {
        private HSLColor[,] originalHSLPixels = null;
        public WriteableBitmap Image { get; private set; } = null; //вихідне зображення
        private HSLColor[,] HSLPixels = null; //масив з пікселями в моделі HSL
        private Color[,] RGBPixels = null; //масив з пікселями в моделі RGB
        private int width;
        private int height;
        private readonly object lockHSL = new object();
        private readonly object lockRGB = new object();
        public HSLColor GetHSL(int x, int y)
        {
            if (HSLPixels == null)
                return null;
            lock (lockHSL)
            {
                return HSLPixels[x, y];
            }
        }
        public Color? GetRGB(int x, int y)
        {
            if (RGBPixels == null)
                return null;
            lock (lockRGB)
            {
                return RGBPixels[x, y];
            }
        }

        //робить певні перетворення для кожної з компонент майбутнього RGB-кольору
        private byte ComponentToRGB(float p, float q, float t)
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

        //перетворює HSL-колір в RGB
        private Color HSLtoRGB(HSLColor hslColor)
        {
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
            return Color.FromArgb(ComponentToRGB(p, q, tR), ComponentToRGB(p, q, tG), ComponentToRGB(p, q, tB));
        }

        public bool Convert(Bitmap InputImage, CancellationToken? token)
        {
            width = InputImage.Width;
            height = InputImage.Height;
            HSLColor[,] tempHSLPixels = new HSLColor[width, height];
            lock(lockRGB)
            {
                RGBPixels = new Color[width, height];
                for (int x = 0; x < width; ++x)
                    for (int y = 0; y < height; ++y)
                        RGBPixels[x, y] = InputImage.GetPixel(x, y);
            }
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
                    tempHSLPixels[x, y] = hslColor; //записуємо колір в масив
                    //конвертація назад в RGB
                    int pixelOffset = (y * width + x) * 4;
                    Color newRgbColor = HSLtoRGB(hslColor);
                    pixels[pixelOffset] = newRgbColor.B; //записуємо результат конвертації в масив
                    pixels[pixelOffset + 1] = newRgbColor.G;
                    pixels[pixelOffset + 2] = newRgbColor.R;
                    pixels[pixelOffset + 3] = 255;
                }
            if (token != null && token.Value.IsCancellationRequested)
                return false;
            lock(lockHSL)
            {
                HSLPixels = tempHSLPixels;
            }
            tempImage.WritePixels(new Int32Rect(0, 0, width, height), pixels, bytesPerRow, 0);
            if (token != null && token.Value.IsCancellationRequested)
                return false;
            Image = tempImage;
            //робимо копію кольорів в масив original (пригодиться під час редагування яскравості)
            originalHSLPixels = new HSLColor[width, height];
            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                    originalHSLPixels[x, y] = HSLPixels[x, y].DeepCopy();
            return true;
        }

        //перевіряє чи даний Hue є достатньо приближений (визначається за 
        //допомогою tolerance) до  бажаного (target)
        private bool IsHueAdjacent(short hue, int target, int tolerance)
        {
            //пошук відстані на градусному колі між двома Hue
            int diff = (target - hue + 180) % 360 - 180;
            if (diff < -180)
                diff += 360;
            //вважатимемо Hue наближеним, якщо відстань від бажаного <= tolerance
            return Math.Abs(diff) <= tolerance;
        }

        //функція для редагування яскравості по певному тону
        public bool AdjustLightness(int hue, int lightness, CancellationToken? token)
        {
            lock(lockHSL)
            {
                for (int x = 0; x < width; ++x)
                    for (int y = 0; y < height; ++y)
                    {
                        //беремо значення яскравості з незміненого зображення
                        HSLColor pixelColor = originalHSLPixels[x, y];
                        if (IsHueAdjacent(pixelColor.H, hue, 10))
                        {
                            //збільшення яскравості
                            int newLightness = lightness + (int)pixelColor.L;
                            //перевірки для того, щоб значення L завжди було в межах [0;100]
                            if (newLightness < 0)
                                newLightness = 0;
                            else if (newLightness > 100)
                                newLightness = 100;
                            //записуємо змінене зображення в масив HSL-пікселів
                            HSLPixels[x, y].L = (byte)newLightness;
                        }
                    }
            }
            if (token != null && token.Value.IsCancellationRequested)
                return false;
            lock (lockRGB)
            {
                //робимо зміни також в масиві RGB-пікселів
                for (int x = 0; x < width; ++x)
                    for (int y = 0; y < height; ++y)
                        RGBPixels[x, y] = HSLtoRGB(HSLPixels[x, y]);
            }
            if (token != null && token.Value.IsCancellationRequested)
                return false;
            //і застосовуємо масив пікселів власне до зображення
            int bytesPerPixel = Image.Format.BitsPerPixel / 8;
            int bytesAmount = width * height * bytesPerPixel;
            int bytesPerRow = width * bytesPerPixel;
            byte[] pixels = new byte[bytesAmount];
            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                {
                    Color rgbColor = RGBPixels[x, y];
                    int pixelOffset = (y * width + x) * 4;
                    pixels[pixelOffset] = rgbColor.B; //записуємо результат конвертації в масив
                    pixels[pixelOffset + 1] = rgbColor.G;
                    pixels[pixelOffset + 2] = rgbColor.R;
                    pixels[pixelOffset + 3] = 255;
                }
            if (token != null && token.Value.IsCancellationRequested)
                return false;
            //і записуємо масив RGB-пікселів в зображення
            Image = new WriteableBitmap(width, height, 
                Image.DpiX, Image.DpiY, System.Windows.Media.PixelFormats.Bgra32, null);
            Image.WritePixels(new Int32Rect(0, 0, width, height), pixels, bytesPerRow, 0);
            return true;
        }
    }
}