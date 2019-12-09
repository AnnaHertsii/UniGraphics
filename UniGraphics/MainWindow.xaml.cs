using System.Windows;
using UniGraphics.Fractals;
using System.Windows.Navigation;
using UniGraphics.ViewModels;
using UniGraphics.ColorModels;
using UniGraphics.Transformation;
using System.Windows.Controls;

namespace UniGraphics
{
    public partial class MainWindow : Window
    {
        FractalsView fractalsView;
        ColorModelsView colorModelsView;
        TransformationView transformationsView;
        MainPage mainPageView;
        LoadingPage loadingPage;

        FractalsDataViewModel fractalsViewModel;
        ColorModelsViewModel colorModelsViewModel;
        TransformationsViewModel transformationsViewModel;

        int pageIndex = -2;

        public MainWindow()
        {
            InitializeComponent();

            loadingPage = new LoadingPage(LoadFinished);
            MainFrame.Content = loadingPage;
        }

        private void LoadFinished()
        {
            mainPageView = new MainPage(PageChanged); 
            MainFrame.Content = mainPageView;
            loadingPage = null;
        }

        private void PageChanged(int pageIndex)
        {
            this.pageIndex = pageIndex;
            switch(pageIndex)
            {
                case 0:
                    colorModelsView = new ColorModelsView(CloseFrame);
                    colorModelsViewModel = new ColorModelsViewModel();
                    MainFrame.DataContext = colorModelsViewModel;
                    MainFrame.Content = colorModelsView;
                    break;
                case 1:
                    fractalsView = new FractalsView(CloseFrame);
                    fractalsViewModel = new FractalsDataViewModel(400, 400);
                    MainFrame.DataContext = fractalsViewModel;
                    fractalsView.FractalPower3Radio.Checked += FractalPowerChanged;
                    fractalsView.FractalPower4Radio.Checked += FractalPowerChanged;
                    MainFrame.Content = fractalsView;
                    break;
                case 2:
                    transformationsView = new TransformationView(CloseFrame);
                    transformationsViewModel = new TransformationsViewModel(transformationsView.HandleAnimationEnd);
                    MainFrame.DataContext = transformationsViewModel;
                    transformationsView.CenterTransformRadio.Checked += TransformRadioChanged;
                    transformationsView.VertexTransformRadio.Checked += TransformRadioChanged;
                    MainFrame.Content = transformationsView;
                    break;
            }
        }

        private void CloseFrame()
        {
            pageIndex = -1;
            MainFrame.Content = mainPageView;
            MainFrame.DataContext = null;
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

        private void TransformRadioChanged(object sender, RoutedEventArgs e)
        {
            bool rotateAroundCenter = 
                transformationsView.CenterTransformRadio.IsChecked.Value;
            if (!rotateAroundCenter)
            {
                PivotVertexWindow pivotVertexWindow = new PivotVertexWindow();
                pivotVertexWindow.ShowDialog();
                if (pivotVertexWindow.PivotSelected)
                {
                    transformationsViewModel.PivotVertex = pivotVertexWindow.PivotVertex;
                    transformationsViewModel.RotateAroundCenter = false;
                }
                else
                {
                    transformationsViewModel.RotateAroundCenter = true;
                    transformationsView.CenterTransformRadio.IsChecked = true;
                }
            }
            else
                transformationsViewModel.RotateAroundCenter = true;
        }

        private void WindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            switch(pageIndex)
            {
                case 1:
                    fractalsViewModel.respondToWindowSizeChange((int)fractalsView.fractalImg.ActualWidth,
                                                                (int)fractalsView.fractalImg.ActualHeight);
                    break;
                case 2:
                    transformationsViewModel.FullUpdate(
                        (int)transformationsView.TransformCanvas.ActualWidth,
                        (int)transformationsView.TransformCanvas.ActualHeight);
                    break;
            }
        }

        private void MainFrame_ContentRendered(object sender, System.EventArgs e)
        {
            if(pageIndex == 2)
            {
                transformationsViewModel.FullUpdate(
                        (int)transformationsView.TransformCanvas.ActualWidth,
                        (int)transformationsView.TransformCanvas.ActualHeight);
            }
        }
    }
}
