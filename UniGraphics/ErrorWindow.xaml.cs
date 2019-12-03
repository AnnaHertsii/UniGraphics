using System.Windows;

namespace UniGraphics
{
    public partial class ErrorWindow : Window
    {
        public ErrorWindow(string errorText)
        {
            InitializeComponent();
            ErrorText.Text = errorText;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
