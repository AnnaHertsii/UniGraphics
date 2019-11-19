using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UniGraphics.ViewModels;

namespace UniGraphics.ColorModels
{
    public partial class ColorModelsView : Page
    {
        public ColorModelsView()
        {
            InitializeComponent();
        }

        private void ImageMouseMove(object sender, MouseEventArgs e)
        {
            var point = e.GetPosition((IInputElement)sender);
            ((ColorModelsViewModel)DataContext).HandleLeftImageMousePosition((int)point.X, (int)point.Y);
        }

        private void ImageMouseLeave(object sender, MouseEventArgs e)
        {
            ((ColorModelsViewModel)DataContext).RGBText = "";
            ((ColorModelsViewModel)DataContext).HSLText = "";
        }

        private void HelpClicked(object sender, RoutedEventArgs e)
        {
            HelpWindow helpWindow = new HelpWindow();
            helpWindow.ShowDialog();
        }

        private void CreditsClicked(object sender, RoutedEventArgs e)
        {
            CreditsWindow creditWindow = new CreditsWindow();
            creditWindow.ShowDialog();
        }

        private void StudyMaterialsClicked(object sender, RoutedEventArgs e)
        {
            StudyWindow studyWindow = new StudyWindow();
            studyWindow.ShowDialog();
        }

        private void BackClicked(object sender, RoutedEventArgs e)
        {
            /*MainWindow main = new MainWindow();
            main.MainFrame.Content = new MainPage();
            main.ShowDialog();*/
        }
    }
}
