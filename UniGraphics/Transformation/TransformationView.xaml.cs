using System.Windows;
using System.Windows.Controls;

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
            StartButtonText.Text = "Продовжити";
            StartButton.Visibility = Visibility.Collapsed;
            PauseButton.Visibility = Visibility.Visible;
            StopButton.IsEnabled = true;
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            StartButton.Visibility = Visibility.Visible;
            PauseButton.Visibility = Visibility.Collapsed;
            StopButton.IsEnabled = true;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            StartButtonText.Text = "Розпочати";
            StartButton.Visibility = Visibility.Visible;
            PauseButton.Visibility = Visibility.Collapsed;
            StopButton.IsEnabled = false;
        }

        public void HandleAnimationEnd()
        {
            Dispatcher.Invoke(() => {
                StartButtonText.Text = "Розпочати";
                StartButton.Visibility = Visibility.Visible;
                PauseButton.Visibility = Visibility.Collapsed;
                StopButton.IsEnabled = false;
            });
        }
    }
}
