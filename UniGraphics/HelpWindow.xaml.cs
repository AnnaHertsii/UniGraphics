using System.Windows;
using System.Windows.Controls;
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

        private void HelpTreeItemSelected(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            TreeViewItem parent = item.Parent as TreeViewItem;
            if (parent == null)
                return;
            Page content;
            if(parent.Name == "FractalsSection")
                content = new FractalsHelp();
            else if(parent.Name == "ColorModelsSection")
                content = new ColorModelsHelp();
            else
                content = new AnimationsHelp();
            HelpFrame.Content = content;
            TextBlock block = (TextBlock)content.FindName((string)item.Tag);
            block.Visibility = Visibility.Visible;
        }
    }
}
