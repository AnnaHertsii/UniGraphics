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

namespace UniGraphics.Transformation
{
    /// <summary>
    /// Логика взаимодействия для TransformationView.xaml
    /// </summary>
    public partial class TransformationView : Page
    {
        public TransformationView()
        {
            InitializeComponent();
        }

        private void StudyMaterialsClicked(object sender, RoutedEventArgs e)
        {
            StudyWindow studyWindow = new StudyWindow();
            studyWindow.ShowDialog();
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

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartButton.Visibility = Visibility.Hidden;
            PauseButton.Visibility = Visibility.Visible;
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
