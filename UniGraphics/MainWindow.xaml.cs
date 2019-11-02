using System.Windows;
using System;
using UniGraphics.Fractals;

namespace UniGraphics
{
    public partial class MainWindow : Window
    {
        NewtonFractalGenerator generator = new NewtonFractalGenerator();
        Random rand = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
