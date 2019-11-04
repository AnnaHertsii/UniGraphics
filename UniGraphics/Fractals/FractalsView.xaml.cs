using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace UniGraphics.Fractals
{
    public partial class FractalsView : Page
    {
        NewtonFractalGenerator generator = new NewtonFractalGenerator();
        Random rand = new Random();

        public FractalsView()
        {
            InitializeComponent();
        }

        void generateFractal(object sender, RoutedEventArgs e)
        {
            //тут можна замість рандому вибрати собі номер кольорової моделі
            NewtonFractalGenerator.ColorModel randomModel =
                NewtonFractalGenerator.colorModels[rand.Next(NewtonFractalGenerator.colorModels.Length)];
            generator.currentColorModel = randomModel;
            generator.generate((int)fractalImg.Width, (int)fractalImg.Height);//виклик побудови фрактала
            fractalImg.Source = generator.Image; //встановлюємо зображення
        }

        private void SavePicture(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "JPG Image (.jpg)|*.jpg";
            if (dialog.ShowDialog() == true)
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)fractalImg.Source));
                using (FileStream stream = new FileStream(dialog.FileName, FileMode.Create))
                    encoder.Save(stream);
            }
        }

        private void SavePictureAs(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "JPEG Image (.jpeg)|*.jpeg |JPG Image (.jpg)|*.jpg |Png Image (.png)|*.png |Tiff Image (.tiff)|*.tiff |Bitmap Image (.bmp)|*.bmp|Wmf Image (.wmf)|*.wmf";
            if (dialog.ShowDialog() == true)
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)fractalImg.Source));
                using (FileStream stream = new FileStream(dialog.FileName, FileMode.Create))
                    encoder.Save(stream);
            } 
        }
            private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
