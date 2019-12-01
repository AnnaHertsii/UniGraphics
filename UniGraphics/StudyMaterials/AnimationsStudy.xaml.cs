using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Diagnostics;

namespace UniGraphics.StudyMaterials
{
    /// <summary>
    /// Логика взаимодействия для AnimationStudy.xaml
    /// </summary>
    public partial class AnimationStudy : Page
    {
        public AnimationStudy()
        {
            InitializeComponent();
        }

        private void LinkRequested(object sender, RequestNavigateEventArgs e)
        {
            Hyperlink hl = (Hyperlink)sender;
            string navigateUri = hl.NavigateUri.ToString();
            Process.Start(new ProcessStartInfo(navigateUri));
            e.Handled = true;
        }
    }
}
