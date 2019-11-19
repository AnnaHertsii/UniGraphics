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
using UniGraphics.StudyMaterials;

namespace UniGraphics
{
    /// <summary>
    /// Логика взаимодействия для StudyWindow.xaml
    /// </summary>
    public partial class StudyWindow : Window
    {
        public StudyWindow()
        {
            InitializeComponent();
        }

        private void FractalStudyClicked(object sender, MouseButtonEventArgs e)
        {
            StudyFrame.Content = new FractalsStudy();
        }

        private void ColorModelsStudyClicked(object sender, MouseButtonEventArgs e)
        {
            StudyFrame.Content = new ColorModelsStudy();
        }
    }
}
