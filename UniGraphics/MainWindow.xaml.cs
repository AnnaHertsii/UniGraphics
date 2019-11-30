using System.Windows;
using UniGraphics.Fractals;
using System.Windows.Navigation;
using UniGraphics.ViewModels;
using UniGraphics.ColorModels;
using UniGraphics.Transformation;

namespace UniGraphics
{
    public partial class MainWindow : Window
    {
        FractalsView fractalsView;
        ColorModelsView colorModelsView;
        TransformationView transformationsView;

        FractalsDataViewModel fractalsViewModel;
        ColorModelsViewModel colorModelsViewModel;
        TransformationsViewModel transformationsViewModel;

        public MainWindow()
        {
            InitializeComponent();
            transformationsView = new TransformationView();
            transformationsViewModel = new TransformationsViewModel();
            MainFrame.DataContext = transformationsViewModel;
            MainFrame.Content = transformationsView;

            //colorModelsView = new ColorModelsView();
            //colorModelsViewModel = new ColorModelsViewModel();
            //MainFrame.DataContext = colorModelsViewModel;
            //MainFrame.Content = colorModelsView;

            //fractalsView = new FractalsView();
            //fractalsViewModel = new FractalsDataViewModel(400, 400);
            //MainFrame.DataContext = fractalsViewModel;
            //fractalsView.FractalPower3Radio.Checked += FractalPowerChanged;
            //fractalsView.FractalPower4Radio.Checked += FractalPowerChanged;
            //MainFrame.Content = fractalsView;
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

        private void WindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            transformationsViewModel.respondToWindowSizeChange(
                    (int)transformationsView.TransformCanvas.ActualWidth,
                    (int)transformationsView.TransformCanvas.ActualHeight);
            return;//TODO remove this later
            fractalsViewModel.respondToWindowSizeChange((int)fractalsView.fractalImg.ActualWidth,
                                                        (int)fractalsView.fractalImg.ActualHeight);
        }

        private void MainFrame_ContentRendered(object sender, System.EventArgs e)
        {
            transformationsViewModel.initialSettings(
                    (int)transformationsView.TransformCanvas.ActualWidth,
                    (int)transformationsView.TransformCanvas.ActualHeight);
        }
    }
}
