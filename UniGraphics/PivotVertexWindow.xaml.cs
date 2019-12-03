using System.Windows;
using System.Windows.Controls;

namespace UniGraphics
{
    /// <summary>
    /// Логика взаимодействия для PivotVertexWindow.xaml
    /// </summary>
    public partial class PivotVertexWindow : Window
    {
        public bool PivotSelected = false;
        public int PivotVertex = 1;
        public PivotVertexWindow()
        {
            InitializeComponent();
        }

        private void ChooseClicked(object sender, RoutedEventArgs e)
        {
            PivotSelected = true;
            Close();
        }

        private void RadioClicked(object sender, RoutedEventArgs e)
        {
            PivotVertex = int.Parse((sender as RadioButton).Content.ToString());
            ChooseButton.IsEnabled = true;
        }
    }
}
