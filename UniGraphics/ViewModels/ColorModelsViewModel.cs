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
using MColor = System.Windows.Media.Color;
using DColor = System.Drawing.Color;
using MBrush = System.Windows.Media.SolidColorBrush;

namespace UniGraphics.ViewModels
{
    public class ColorModelsViewModel : ViewModelBase
    {
        private void PerformOpenFile(object args)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "png",
                Filter = "Зображення (.jpeg, .jpg, .png, .tiff, .bmp, .wmf) | *.jpeg; *.jpg; *.png; *.tiff; *.bmp; *.wmf",
                RestoreDirectory = true
            };
            if (dialog.ShowDialog() == true)
            {
                ImageLeft = new WriteableBitmap(new BitmapImage(new Uri(dialog.FileName, UriKind.Absolute)));
                CConverter = new ColorConverter()
                {
                    InputImage = CConverter.InputImage = BitmapFromWriteableBitmap(ImageLeft)
                };
                ImageRight = null;
                ConvertButtonVisibility = Visibility.Visible;
            }
        }

        private void PerformExit(object args)
        {
            Application.Current.Shutdown();
        }

        private void PerformSaveFile(object args)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "JPEG Image (.jpeg)|*.jpeg |JPG Image (.jpg)|*.jpg |Png Image (.png)|*.png |Tiff Image (.tiff)|*.tiff |Bitmap Image (.bmp)|*.bmp|Wmf Image (.wmf)|*.wmf",
                DefaultExt = "png"
            };
            if (dialog.ShowDialog() == true)
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(ImageRight));
                using (FileStream stream = new FileStream(dialog.FileName, FileMode.Create))
                    encoder.Save(stream);
            }
        }

        private void PerformConverting(object args)
        {
            StartConverting();
        }

        public ICommand OpenFile { get; set; }
        public ICommand Exit { get; set; }
        public ICommand SaveFile { get; set; }
        public ICommand Convert { get; set; }


        private Task currentRunningTask = null;
        private CancellationTokenSource tokenSource;
        private bool isLoadingImage = false;

        private void StartConverting()
        {
            isLoadingImage = true;
            if (currentRunningTask != null && !currentRunningTask.IsCanceled)
                tokenSource.Cancel();
            tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            currentRunningTask = new Task(() =>
            {
                if (CConverter.Convert(token))
                {
                    CConverter.Image.Freeze();
                    currentRunningTask = null;
                    isLoadingImage = false;
                    ConvertButtonVisibility = Visibility.Collapsed;
                    if (_Lightness == 0) //якщо немає змін в яскравості
                        Dispatcher.CurrentDispatcher.Invoke(() => ImageRight = CConverter.Image); //просто встановити зображення
                    else //якщо ж є
                        Lightness = Lightness; //то викликаємо зміни яскравості ще раз, щоб виконалась функція StartAdjusting
                }
            }, token);
            currentRunningTask.Start();
        }

        private void StartAdjusting()
        {
            if (currentRunningTask != null && !currentRunningTask.IsCanceled)
                tokenSource.Cancel();
            tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            currentRunningTask = new Task(() =>
            {
                if (CConverter.AdjustLightness(HueNumber, Lightness, token))
                {
                    CConverter.Image.Freeze();
                    Dispatcher.CurrentDispatcher.Invoke(() => ImageRight = CConverter.Image);
                    currentRunningTask = null;
                }
            }, token);
            currentRunningTask.Start();
        }

        public void HandleMouseMove(int x, int y)
        {
            var rgb = CConverter.GetRGB(x, y);
            if (rgb == null)
                RGBText = "";
            else
                RGBText = $"({rgb.Value.R}, {rgb.Value.G}, {rgb.Value.B})";

            var hsl = CConverter.GetHSL(x, y);
            if (hsl == null)
                HSLText = "";
            else
                HSLText = $"({hsl.H}°, {hsl.S}%, {hsl.L}%)";
        }

        public void HandleHuePicked(int x, int width)
        {
            HueNumber = (int) ((x / (double)width) * 360);
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

        private static MColor ToMediaColor(DColor color)
        {
            return MColor.FromArgb(color.A, color.R, color.G, color.B);
        }

        private int _HueNumber;
        public int HueNumber
        {
            get { return _HueNumber; }
            set
            {
                _HueNumber = value;
                OnPropertyChanged("HueNumber");
                HueColor = new MBrush(ToMediaColor(ColorConverter.HSLtoRGB(new HSLColor((short)value, 100, 50))));
                if (!isLoadingImage)
                    StartAdjusting();
            }
        }

        private int _Lightness;
        public int Lightness
        {
            get { return _Lightness; }
            set
            {
                _Lightness = value;
                OnPropertyChanged("Lightness");
                if(!isLoadingImage)
                    StartAdjusting();
            }
        }

        private string _RGBText;
        public string RGBText
        {
            get { return _RGBText; }
            set
            {
                _RGBText = value;
                if (value.Length == 0)
                    RGBValuesVisibility = Visibility.Collapsed;
                else
                    RGBValuesVisibility = Visibility.Visible;
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
                if (value.Length == 0)
                    HSLValuesVisibility = Visibility.Collapsed;
                else
                    HSLValuesVisibility = Visibility.Visible;
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

        private Visibility _RGBValuesVisibility;
        public Visibility RGBValuesVisibility
        {
            get { return _RGBValuesVisibility; }
            set
            {
                _RGBValuesVisibility = value;
                OnPropertyChanged("RGBValuesVisibility");
            }
        }

        private Visibility _HSLValuesVisibility;
        public Visibility HSLValuesVisibility
        {
            get { return _HSLValuesVisibility; }
            set
            {
                _HSLValuesVisibility = value;
                OnPropertyChanged("HSLValuesVisibility");
            }
        }

        private Visibility _ConvertButtonVisibility;
        public Visibility ConvertButtonVisibility
        {
            get { return _ConvertButtonVisibility; }
            set
            {
                _ConvertButtonVisibility = value;
                OnPropertyChanged("ConvertButtonVisibility");
            }
        }

        private MBrush _HueColor;
        public MBrush HueColor
        {
            get { return _HueColor; }
            set
            {
                _HueColor = value;
                OnPropertyChanged("HueColor");
            }
        }

        public ColorModelsViewModel()
        {
            _HueColor = new MBrush(MColor.FromRgb(255, 0, 0));
            _RGBValuesVisibility = Visibility.Collapsed;
            _HSLValuesVisibility = Visibility.Collapsed;
            _ConvertButtonVisibility = Visibility.Collapsed;
            _RGBText = "";
            _HSLText = "";
            _Lightness = 0;
            _HueNumber = 0;
            _ImageLeft = null;
            _ImageRight = null;
            CConverter = new ColorConverter();
            OpenFile = new Command(PerformOpenFile);
            Exit = new Command(PerformExit);
            SaveFile = new Command(PerformSaveFile);
            Convert = new Command(PerformConverting);
        }
    }
}
