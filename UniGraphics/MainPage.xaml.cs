using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UniGraphics
{
    public partial class MainPage : Page
    {
        private static readonly string defaultHelperMessage =
               "Вивчайте основи комп'ютерної графіки разом з UniGraphics!";
        private Action<int> pageChanged;

        public MainPage(Action<int> pageChanged)
        {
            this.pageChanged = pageChanged;
            InitializeComponent();
            UnicornHelper.Text = defaultHelperMessage;
        }

        private void ExitClicked(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
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

        private void ColorModelsButton_Click(object sender, RoutedEventArgs e)
        {
            pageChanged(0);
        }

        private void FractalsButton_Click(object sender, RoutedEventArgs e)
        {
            pageChanged(1);
        }

        private void AnimationButton_Click(object sender, RoutedEventArgs e)
        {
            pageChanged(2);
        }
    }
}
