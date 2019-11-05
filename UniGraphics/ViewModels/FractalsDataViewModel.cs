using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using UniGraphics.Fractals;

namespace UniGraphics.ViewModels
{
    public class FractalsDataViewModel : ViewModelBase
    {
        private Task currentRunningTask = null;
        private CancellationTokenSource tokenSource;
        private Action<double> setProgressAction;
        private void startGeneration()
        {
            if(currentRunningTask != null && !currentRunningTask.IsCanceled)
            {
                tokenSource.Cancel();
                GeneratingProgress = 1.0;
            }
            tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            currentRunningTask = new Task(() =>
            {
                if(Generator.generate(token, setProgressAction))
                {
                    Generator.Image.Freeze();
                    Dispatcher.CurrentDispatcher.Invoke(() => Image = Generator.Image);
                    currentRunningTask = null;
                }
            }, token);
            currentRunningTask.Start();
        }

        private double _fractalScale;
        public double FractalScale
        {
            get { return _fractalScale; }
            set
            {
                _fractalScale = value;
                OnPropertyChanged("FractalScale");
                Generator.FractalScale = value;
                startGeneration();
            }
        }

        private double _fractalPower;
        public double FractalPower
        {
            get { return _fractalPower; }
            set
            {
                _fractalPower = value;
                OnPropertyChanged("FractalPower");
                Generator.Power = value;
                startGeneration();
            }
        }

        private double _constantReal;
        public double ConstantReal
        {
            get { return _constantReal; }
            set
            {
                _constantReal = value;
                OnPropertyChanged("ConstantReal");
                Generator.Constant = new Complex(value, _constantImaginary);
                startGeneration();
            }
        }

        private double _constantImaginary;
        public double ConstantImaginary
        {
            get { return _constantImaginary; }
            set
            {
                _constantImaginary = value;
                OnPropertyChanged("ConstantImaginary");
                Generator.Constant = new Complex(_constantReal, value);
                startGeneration();
            }
        }

        private double _generatingProgress;
        public double GeneratingProgress
        {
            get { return _generatingProgress; }
            set
            {
                _generatingProgress = value;
                OnPropertyChanged("GeneratingProgress");
                ProgressVisibility = (value == 1.0 ? Visibility.Collapsed : Visibility.Visible);
            }
        }

        private Visibility _progressVisibility;
        public Visibility ProgressVisibility
        {
            get { return _progressVisibility; }
            set
            {
                _progressVisibility = value;
                OnPropertyChanged("ProgressVisibility");
            }
        }

        private int _currentColorModel;
        public int CurrentColorModel
        {
            get { return _currentColorModel; }
            set
            {
                _currentColorModel = value;
                OnPropertyChanged("CurrentColorModel");
                Generator.currentColorModel = NewtonFractalGenerator.colorModels[value];
                startGeneration();
            }
        }

        private WriteableBitmap _image;
        public WriteableBitmap Image
        {
            get { return _image; }
            set
            {
                _image = value;
                OnPropertyChanged("Image");
            }
        }
        private NewtonFractalGenerator Generator { get; set; }

        public void respondToWindowSizeChange(int width, int height)
        {
            if (width == 0 || height == 0)
                return;
            Generator.Width = width;
            Generator.Height = height;
            startGeneration();
        }

        public FractalsDataViewModel(int width, int height)
        {
            _generatingProgress = 0.0;
            _progressVisibility = Visibility.Collapsed;
            _constantReal = -1.0;
            _constantImaginary = 0.0;
            _currentColorModel = 0;
            _fractalScale = 1.0;
            _fractalPower = 3.0;
            setProgressAction = (p => GeneratingProgress = p);
            Generator = new NewtonFractalGenerator(_fractalPower,
                                                   new Complex(_constantReal, _constantImaginary),
                                                   NewtonFractalGenerator.colorModels[_currentColorModel])
            {
                Width = width,
                Height = height,
                FractalScale = _fractalScale
            };
            Generator.generate(null, setProgressAction);
            _image = Generator.Image;
        }
    }
}
