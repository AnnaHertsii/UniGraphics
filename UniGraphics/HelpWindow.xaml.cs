using System.Windows;
using System.Windows.Input;
using UniGraphics.Help;

namespace UniGraphics
{
    public partial class HelpWindow : Window
    {
        public HelpWindow()
        {
            InitializeComponent();
        }

        private void FractalHelpClicked(object sender, MouseButtonEventArgs e)
        {
            HelpFrame.Content = new FractalsHelp();
            FractalsSection.FontWeight = FontWeights.Bold;
            ColorModelsSection.FontWeight = FontWeights.Normal;
            AnimationsSection.FontWeight = FontWeights.Normal;
        }

        private void ColorModelHelpClicked(object sender, MouseButtonEventArgs e)
        {
            HelpFrame.Content = new ColorModelsHelp();
            FractalsSection.FontWeight = FontWeights.Normal;
            ColorModelsSection.FontWeight = FontWeights.Bold;
            AnimationsSection.FontWeight = FontWeights.Normal;
        }

        private void TransformationHelpClicked(object sender, MouseButtonEventArgs e)
        {
            HelpFrame.Content = new AnimationsHelp();
            FractalsSection.FontWeight = FontWeights.Normal;
            ColorModelsSection.FontWeight = FontWeights.Normal;
            AnimationsSection.FontWeight = FontWeights.Bold;
        }
    }
}
