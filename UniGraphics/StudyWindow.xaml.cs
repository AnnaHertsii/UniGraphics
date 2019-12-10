using System.Windows;
using System.Windows.Controls;
using UniGraphics.StudyMaterials;

namespace UniGraphics
{
    public partial class StudyWindow : Window
    {
        public StudyWindow()
        {
            InitializeComponent();
        }

        private void HelpTreeItemSelected(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            TreeViewItem parent = item.Parent as TreeViewItem;
            if (parent == null)
                return;
            Page content;
            if (parent.Name == "FractalsSection")
                content = new FractalsStudy();
            else if (parent.Name == "ColorModelsSection")
                content = new ColorModelsStudy();
            else
                content = new AnimationStudy();
            StudyFrame.Content = content;
            FrameworkElement block = content.FindName((string)item.Tag) as FrameworkElement;
            block.Visibility = Visibility.Visible;
        }
    }
}
