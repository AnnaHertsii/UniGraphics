using System;
using System.Windows;
using System.Windows.Controls;

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
    }
}
