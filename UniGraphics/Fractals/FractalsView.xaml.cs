using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using UniGraphics.ViewModels;

namespace UniGraphics.Fractals
{
    public partial class FractalsView : Page
    {
        private static readonly string defaultHelperMessage =
            "Давай почнемо вивчати фрактали! Введи початкові дані в полях вище.";
        private Action goBack;

        public FractalsView(Action goBack)
        {
            this.goBack = goBack;
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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            goBack();
        }

        private void StudyMaterialsClicked(object sender, RoutedEventArgs e)
        {
            StudyWindow studyWindow = new StudyWindow();
            studyWindow.ShowDialog();
        }

        private void HelpClicked(object sender, RoutedEventArgs e)
        {
            HelpWindow helpWindow = new HelpWindow();
            helpWindow.ShowDialog();
        }

        private void CreditsClicked(object sender, RoutedEventArgs e)
        {
            CreditsWindow creditWindow = new CreditsWindow();
            creditWindow.ShowDialog();
        }

        private static readonly string badSymbolsError =
            "Введенні вами дані є некоректними! :(\n" +
            "Заборонено вводити символи. " +
            "Ознайомтесь детальніше з розділом 'Вхідні дані' у вкладці " +
            "'Афінні перетворення' довідки.";

        private void VisualizeButton_Click(object sender, RoutedEventArgs e)
        {
            FractalsDataViewModel viewModel = (FractalsDataViewModel)DataContext;
            if (Validation.GetHasError(ConstantRealText))
            {
                ConstantRealText.Text = viewModel.ConstantReal.ToString();
                ErrorWindow error = new ErrorWindow(badSymbolsError);
                error.ShowDialog();
            }
            if (Validation.GetHasError(ConstantImaginaryText))
            {
                ConstantImaginaryText.Text = viewModel.ConstantImaginary.ToString();
                ErrorWindow error = new ErrorWindow(badSymbolsError);
                error.ShowDialog();
            }
        }
    }
}
