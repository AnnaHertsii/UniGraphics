using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UniGraphics.Transformation
{
    public partial class TransformationView : Page
    {
        private static readonly string defaultHelperMessage = 
            "Щоб почати роботу з рухом шестикутника, введіть необхідні параметри зверху :)";

        public TransformationView()
        {
            InitializeComponent();
            UnicornHelper.Text = defaultHelperMessage;
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
            ToggleTransformUI(false);
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
            ToggleTransformUI(true);
        }

        public void HandleAnimationEnd()
        {
            Dispatcher.Invoke(() => {
                StartButtonText.Text = "Розпочати";
                StartButton.Visibility = Visibility.Visible;
                PauseButton.Visibility = Visibility.Collapsed;
                StopButton.IsEnabled = false;
                ToggleTransformUI(true);
            });
        }

        private void ToggleTransformUI(bool state)
        {
            InputX.IsEnabled = state;
            InputY.IsEnabled = state;
            InputSideLength.IsEnabled = state;
            InputMaxRotation.IsEnabled = state;
            VertexTransformRadio.IsEnabled = state;
            CenterTransformRadio.IsEnabled = state;
            VertexPivotLabel.IsEnabled = state;
            CenterPivotLabel.IsEnabled = state;
        }

        private void CenterPivotClicked(object sender, RoutedEventArgs e)
        {
            CenterTransformRadio.IsChecked = true;
        }

        private void VertexPivotClicked(object sender, RoutedEventArgs e)
        {
            VertexTransformRadio.IsChecked = true;
        }

        private void KeyDownHandler(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if(e.Key == Key.Return)
                textBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        private void HelpableHover(object sender, MouseEventArgs e)
        {
            var helpable = sender as FrameworkElement;
            UnicornHelper.Text = helpable.Tag.ToString();
        }

        private void HelpableMouseLeave(object sender, MouseEventArgs e)
        {
            UnicornHelper.Text = defaultHelperMessage;
        }
    }
}
