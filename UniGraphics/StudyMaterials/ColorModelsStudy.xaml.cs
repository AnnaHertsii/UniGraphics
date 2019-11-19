using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace UniGraphics.StudyMaterials
{
    public partial class ColorModelsStudy : Page
    {
        public ColorModelsStudy()
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
