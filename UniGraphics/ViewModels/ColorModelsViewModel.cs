using Microsoft.Win32;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using UniGraphics.ColorModels;

namespace UniGraphics.ViewModels
{
    public class ColorModelsViewModel : ViewModelBase
    {
        private void openFile(object args)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            dialog.DefaultExt = "png";
            dialog.Filter = "Зображення (.jpeg, .jpg, .png, .tiff, .bmp, .wmf) | *.jpeg; *.jpg; *.png; *.tiff; *.bmp; *.wmf";
            dialog.RestoreDirectory = true;
            if (dialog.ShowDialog() == true)
                ImageLeft = new WriteableBitmap(new BitmapImage(new Uri(dialog.FileName, UriKind.Absolute)));
        }

        private void performExit(object args)
        {
            Application.Current.Shutdown();
        }

        private void saveFile(object args)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "JPEG Image (.jpeg)|*.jpeg |JPG Image (.jpg)|*.jpg |Png Image (.png)|*.png |Tiff Image (.tiff)|*.tiff |Bitmap Image (.bmp)|*.bmp|Wmf Image (.wmf)|*.wmf";
            dialog.DefaultExt = "png";
            if (dialog.ShowDialog() == true)
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(ImageRight));
                using (FileStream stream = new FileStream(dialog.FileName, FileMode.Create))
                    encoder.Save(stream);
            }
        }

        public ICommand OpenFile { get; set; }
        public ICommand PerformExit { get; set; }
        public ICommand SaveFile { get; set; }


        private Task currentRunningTask = null;
        private CancellationTokenSource tokenSource;

        private void startConverting()
        {
            if (currentRunningTask != null && !currentRunningTask.IsCanceled)
                tokenSource.Cancel();
            tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            currentRunningTask = new Task(() =>
            {
                if (CConverter.Convert(BitmapFromWriteableBitmap(ImageLeft), token))
                {
                    CConverter.Image.Freeze();
                    Dispatcher.CurrentDispatcher.Invoke(() => ImageRight = CConverter.Image);
                    currentRunningTask = null;
                }
            }, token);
            currentRunningTask.Start();
        }

        static private System.Drawing.Bitmap BitmapFromWriteableBitmap(WriteableBitmap wrtBmp)
        {
            System.Drawing.Bitmap bmp = null;
            using(MemoryStream stream = new MemoryStream())
            {
                wrtBmp.Dispatcher.Invoke(() =>
                {
                    BitmapEncoder encoder = new BmpBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create((BitmapSource)wrtBmp));
                    encoder.Save(stream);
                    bmp = new System.Drawing.Bitmap(stream);
                });
            }
            return bmp;
        }

        private int _HueNumber;
        public int HueNumber
        {
            get { return _HueNumber; }
            set
            {
                HueNumber = value;
                OnPropertyChanged("HueNumber");
            }
        }

        private double _Lightness;
        public double Lightness
        {
            get { return _Lightness; }
            set
            {
                _Lightness = value;
                OnPropertyChanged("Lightness");
            }
        }

        private string _RGBText;
        public string RGBText
        {
            get { return _RGBText; }
            set
            {
                _RGBText = value;
                OnPropertyChanged("RGBText");
            }
        }

        private string _HSLText;
        public string HSLText
        {
            get { return _HSLText; }
            set
            {
                _HSLText = value;
                OnPropertyChanged("HSLText");
            }
        }

        private WriteableBitmap _ImageLeft;
        public WriteableBitmap ImageLeft
        {
            get { return _ImageLeft; }
            set
            {
                _ImageLeft = value;
                OnPropertyChanged("ImageLeft");
                startConverting();
            }
        }

        private WriteableBitmap _ImageRight;
        public WriteableBitmap ImageRight
        {
            get { return _ImageRight; }
            set
            {
                _ImageRight = value;
                OnPropertyChanged("ImageRight");
            }
        }

        private ColorConverter CConverter { get; set; }

        public void respondToWindowSizeChange(int width, int height)
        {
            if (width == 0 || height == 0)
                return;
            //Generator.Width = width;
            //Generator.Height = height;
            //TODO
        }

        public ColorModelsViewModel()
        {
            _RGBText = "hey";
            _HSLText = "";
            _Lightness = 0.0;
            _HueNumber = 60;
            _ImageLeft = null;
            _ImageRight = null;
            CConverter = new ColorConverter();
            OpenFile = new Command(openFile);
            PerformExit = new Command(performExit);
            SaveFile = new Command(saveFile);
        }
    }
}
