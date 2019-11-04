using System.Windows;
using UniGraphics.Fractals;
using System.Windows.Navigation;
using UniGraphics.ViewModels;

namespace UniGraphics
{
    public partial class MainWindow : Window
    {
        FractalsView fractalsView;
        FractalsDataViewModel fractalsViewModel;

        public MainWindow()
        {
            fractalsViewModel = new FractalsDataViewModel();
            InitializeComponent();
            MainFrame.DataContext = fractalsViewModel;
            fractalsView = new FractalsView();
            fractalsView.FractalPower3Radio.Checked += FractalPowerChanged;
            fractalsView.FractalPower4Radio.Checked += FractalPowerChanged;
            MainFrame.Content = fractalsView;
        }

        private void MainFrame_LoadCompleted(object sender, NavigationEventArgs e)
        {
            UpdateFrameDataContext(sender, null);
        }

        private void MainFrame_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateFrameDataContext(sender, null);
        }

        private void UpdateFrameDataContext(object sender, NavigationEventArgs e)
        {
            var content = MainFrame.Content as FrameworkElement;
            if (content == null)
                return;
            content.DataContext = MainFrame.DataContext;
        }

        private void FractalPowerChanged(object sender, RoutedEventArgs e)
        {
            if (fractalsView.FractalPower3Radio.IsChecked == true)
                fractalsViewModel.FractalPower = 3;
            else
                fractalsViewModel.FractalPower = 4;
        }
    }
}
