using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using UniGraphics.StudyMaterials;

namespace UniGraphics
{
    public partial class StudyWindow : Window
    {
        public StudyWindow()
        {
            InitializeComponent();
        }

        private void FractalStudyClicked(object sender, MouseButtonEventArgs e)
        {
            StudyFrame.Content = new FractalsStudy();
            FractalsSection.FontWeight = FontWeights.Bold;
            ColorModelsSection.FontWeight = FontWeights.Normal;
            AnimationsSection.FontWeight = FontWeights.Normal;
        }

        private void ColorModelsStudyClicked(object sender, MouseButtonEventArgs e)
        {
            StudyFrame.Content = new ColorModelsStudy();
            FractalsSection.FontWeight = FontWeights.Normal;
            ColorModelsSection.FontWeight = FontWeights.Bold;
            AnimationsSection.FontWeight = FontWeights.Normal;
        }

        private void TransformationStudyClicked(object sender, MouseButtonEventArgs e)
        {
            StudyFrame.Content = new AnimationStudy();
            FractalsSection.FontWeight = FontWeights.Normal;
            ColorModelsSection.FontWeight = FontWeights.Normal;
            AnimationsSection.FontWeight = FontWeights.Bold;
        }
        
    }
}
