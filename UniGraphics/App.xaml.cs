using System.Windows;
using UniGraphics.ViewModels;

namespace UniGraphics
{
    public partial class App : Application
    {
        DataViewModel viewmodel;

        public App()
        {
            viewmodel = new DataViewModel();
            var mainWindow = new MainWindow() { DataContext = viewmodel };
            mainWindow.Show();
        }
    }
}
