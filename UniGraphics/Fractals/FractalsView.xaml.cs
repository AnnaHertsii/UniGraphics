using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace UniGraphics.Fractals
{
    public partial class FractalsView : Page
    {
        private static readonly string defaultHelperMessage =
            "Зміни цей текст в файлі FractalsView.xaml.cs";
        public FractalsView()
        {
            InitializeComponent();
            UnicornHelper.Text = defaultHelperMessage;
        }

        private void SavePictureAs(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "JPEG Image (.jpeg)|*.jpeg |JPG Image (.jpg)|*.jpg |Png Image (.png)|*.png |Tiff Image (.tiff)|*.tiff |Bitmap Image (.bmp)|*.bmp|Wmf Image (.wmf)|*.wmf";
            if (dialog.ShowDialog() == true)
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)fractalImg.Source));
                using (FileStream stream = new FileStream(dialog.FileName, FileMode.Create))
                    encoder.Save(stream);
            } 
        }

        private void ExitClicked(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void HelpableHover(object sender, MouseEventArgs e)
        {
            var helpable = sender as FrameworkElement;
            UnicornHelper.Text = helpable.Tag.ToString();
        }

        private void HelpableMouseLeave(object sender, MouseEventArgs e)
        {
            UnicornHelper.Text = defaultHelperMessage;
        }
    }
}
