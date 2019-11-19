using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Documents;

namespace UniGraphics.StudyMaterials
{
    public partial class FractalsStudy : Page
    {
        public FractalsStudy()
        {
            InitializeComponent();
        }

        private void LinkRequested(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Hyperlink hl = (Hyperlink)sender;
            string navigateUri = hl.NavigateUri.ToString();
            Process.Start(new ProcessStartInfo(navigateUri));
            e.Handled = true;
        }
    }
}
