using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UniGraphics.Help;

namespace UniGraphics
{
    /// <summary>
    /// Логика взаимодействия для HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow : Window
    {
        public HelpWindow()
        {
            InitializeComponent();
        }

        private void FractalHelpClicked(object sender, MouseButtonEventArgs e)
        {
            HelpFrame.Content = new FractalsHelp();
        }

        private void ColorModelHelpClicked(object sender, MouseButtonEventArgs e)
        {
            HelpFrame.Content = new ColorModelsHelp();
        }
    }
}
