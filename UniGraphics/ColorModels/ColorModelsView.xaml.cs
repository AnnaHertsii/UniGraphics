using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UniGraphics.ViewModels;

namespace UniGraphics.ColorModels
{
    public partial class ColorModelsView : Page
    {
        public ColorModelsView()
        {
            InitializeComponent();
        }

        private void ImageMouseMove(object sender, MouseEventArgs e)
        {
            var point = e.GetPosition((IInputElement)sender);
            ((ColorModelsViewModel)DataContext).HandleLeftImageMousePosition((int)point.X, (int)point.Y);
        }

        private void ImageMouseLeave(object sender, MouseEventArgs e)
        {
            ((ColorModelsViewModel)DataContext).RGBText = "";
            ((ColorModelsViewModel)DataContext).HSLText = "";
        }
    }
}
