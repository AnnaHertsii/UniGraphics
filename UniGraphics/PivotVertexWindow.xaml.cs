using System.Windows;
using System.Windows.Controls;

namespace UniGraphics
{
    /// <summary>
    /// Логика взаимодействия для PivotVertexWindow.xaml
    /// </summary>
    public partial class PivotVertexWindow : Window
    {
        private bool canClose = false;
        public int PivotVertex = 1;
        public PivotVertexWindow()
        {
            InitializeComponent();
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(!canClose)
                e.Cancel = true;
        }

        private void ChooseClicked(object sender, RoutedEventArgs e)
        {
            canClose = true;
            Close();
        }

        private void RadioClicked(object sender, RoutedEventArgs e)
        {
            PivotVertex = int.Parse((sender as RadioButton).Content.ToString());
        }
    }
}
