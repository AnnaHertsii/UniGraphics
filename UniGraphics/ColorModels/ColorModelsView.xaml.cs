using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UniGraphics.ViewModels;

namespace UniGraphics.ColorModels
{
    /// <summary>
    /// Interaction logic for ColorModelsView.xaml
    /// </summary>
    public partial class ColorModelsView : Page
    {
        public ColorModelsView()
        {
            InitializeComponent();
        }

        private void LeftImageMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var point = e.GetPosition((IInputElement)sender);
            ((ColorModelsViewModel)DataContext).handleLeftImageMousePosition((int)point.X, (int)point.Y);
        }
    }
}
