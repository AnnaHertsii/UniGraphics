using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using UniGraphics.Transformation;

namespace UniGraphics.ViewModels
{
    public class TransformationsViewModel : ViewModelBase
    {
        private TransformationManager transformer;

        Action animationFinished;

        private void PerformExit(object args)
        {
            Application.Current.Shutdown();
        }

        private bool IsAnimationFinished = false;

        private void PerformAnimation(object args)
        {
            if (currentRunningTask != null && !currentRunningTask.IsCanceled)
                tokenSource.Cancel();
            tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            if (IsAnimationFinished)
                transformer.Rollback();
            currentRunningTask = new Task(() =>
            {
                IsAnimationFinished = false;
                Stopwatch timer = new Stopwatch();
                timer.Start();
                while (true)
                {
                    if (token.IsCancellationRequested)
                        return;
                    IsAnimationFinished = transformer.HandleTimeFlow(timer.Elapsed.TotalSeconds);
                    timer.Restart();
                    if (transformer.UpdateTransform(token))
                    {
                        transformer.Image.Freeze();
                        Dispatcher.CurrentDispatcher.Invoke(() => TransformationImage = transformer.Image);
                    }
                    else
                        return;
                    if (IsAnimationFinished)
                    {
                        animationFinished();
                        return;
                    }
                }
            }, token);
            currentRunningTask.Start();
        }

        private void PerformPause(object args)
        {
            if (currentRunningTask != null && !currentRunningTask.IsCanceled)
                tokenSource.Cancel();
        }

        public ICommand Exit { get; set; }
        public ICommand Transform { get; set; }
        public ICommand StopTransform { get; set; }
        public ICommand PauseTransform { get; set; }

        private void PerformStop(object args)
        {
            if (currentRunningTask != null && !currentRunningTask.IsCanceled)
                tokenSource.Cancel();
            tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            currentRunningTask = new Task(() =>
            {
                transformer.Rollback();
                transformer.GenerateWithSameResolution(token);
                transformer.Image.Freeze();
                currentRunningTask = null;
                Dispatcher.CurrentDispatcher.Invoke(() => TransformationImage = transformer.Image);
            }, token);
            currentRunningTask.Start();
        }

        private Task currentRunningTask = null;
        private CancellationTokenSource tokenSource;

        public void FullUpdate(int width, int height)
        {
            if (width == 0 || height == 0)
                return;
            if (currentRunningTask != null && !currentRunningTask.IsCanceled)
                tokenSource.Cancel();
            tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            currentRunningTask = new Task(() =>
            {
                if (transformer.Generate(width, height, token))
                {
                    transformer.Image.Freeze();
                    currentRunningTask = null;
                    Dispatcher.CurrentDispatcher.Invoke(() => TransformationImage = transformer.Image);
                }
            }, token);
            currentRunningTask.Start();
        }

        public void PartUpdate()
        {
            if (currentRunningTask != null && !currentRunningTask.IsCanceled)
                tokenSource.Cancel();
            tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            currentRunningTask = new Task(() =>
            {
                if (transformer.GenerateWithSameResolution(token))
                {
                    transformer.Image.Freeze();
                    currentRunningTask = null;
                    Dispatcher.CurrentDispatcher.Invoke(() => TransformationImage = transformer.Image);
                }
            }, token);
            currentRunningTask.Start();
        }

        private WriteableBitmap _TransformationImage;
        public WriteableBitmap TransformationImage
        {
            get { return _TransformationImage; }
            set
            {
                _TransformationImage = value;
                OnPropertyChanged("TransformationImage");
            }
        }

        private static readonly string badCoordRangeError =
            "Введенні вами дані є некоректними! :(\n" + 
            "Заборонено вводити значення, що не належать проміжку [-200000; 200000]. " + 
            "Ознайомтесь детальніше з розділом 'Вхідні дані' у вкладці " + 
            "'Афінні перетворення' довідки.";

        private double _CoordX;
        public double CoordX
        {
            get { return _CoordX; }
            set
            {
                if (Math.Abs(value) > 200000)
                {
                    ErrorWindow error = new ErrorWindow(badCoordRangeError);
                    error.ShowDialog();
                    return;
                }
                _CoordX = value;
                transformer.RootX = value;
                OnPropertyChanged("CoordX");
                PartUpdate();
            }
        }

        private double _CoordY;
        public double CoordY
        {
            get { return _CoordY; }
            set
            {
                if (Math.Abs(value) > 200000)
                {
                    ErrorWindow error = new ErrorWindow(badCoordRangeError);
                    error.ShowDialog();
                    return;
                }
                _CoordY = value;
                transformer.RootY = value;
                OnPropertyChanged("CoordY");
                PartUpdate();
            }
        }

        private static readonly string badSideLengthError =
            "Введенні вами дані є некоректними! :(\n" +
            "Заборонено вводити значення, що не належать проміжку (0; 50000]. " +
            "Ознайомтесь детальніше з розділом 'Вхідні дані' у вкладці " +
            "'Афінні перетворення' довідки.";

        private double _SideLength;
        public double SideLength
        {
            get { return _SideLength; }
            set
            {
                if (value <= 0.0 || value > 50000)
                {
                    ErrorWindow error = new ErrorWindow(badSideLengthError);
                    error.ShowDialog();
                    return;
                }
                _SideLength = value;
                transformer.SideLength = value;
                OnPropertyChanged("SideLength");
                PartUpdate();
            }
        }

        private static readonly string badRotationError =
            "Введенні вами дані є некоректними! :(\n" +
            "Заборонено вводити значення, що не належать проміжку [0.01; 360]. " +
            "Ознайомтесь детальніше з розділом 'Вхідні дані' у вкладці " +
            "'Афінні перетворення' довідки.";

        private double _MaxRotation;
        public double MaxRotation
        {
            get { return _MaxRotation; }
            set
            {
                if (value < 0.01 || value > 360.0)
                {
                    ErrorWindow error = new ErrorWindow(badRotationError);
                    error.ShowDialog();
                    return;
                }
                _MaxRotation = value;
                transformer.MaxRotation = value;
                OnPropertyChanged("MaxRotation");
            }
        }

        private bool _RotateAroundCenter;
        public bool RotateAroundCenter
        {
            get { return _RotateAroundCenter; }
            set
            {
                _RotateAroundCenter = value;
                transformer.RotateAroundCenter = value;
                OnPropertyChanged("RotateAroundCenter");
                PartUpdate();
            }
        }

        private int _PivotVertex;
        public int PivotVertex
        {
            get { return _PivotVertex; }
            set
            {
                _PivotVertex = value;
                transformer.VertexIndex = value;
                OnPropertyChanged("PivotVertex");
                PartUpdate();
            }
        }

        public TransformationsViewModel(Action animFinished)
        {
            animationFinished = animFinished;
            Exit = new Command(PerformExit);
            Transform = new Command(PerformAnimation);
            StopTransform = new Command(PerformStop);
            PauseTransform = new Command(PerformPause);
            _CoordX = 0;
            _CoordY = 0;
            _SideLength = 1;
            _MaxRotation = 90;
            _RotateAroundCenter = true;
            _PivotVertex = 1;
            transformer = new TransformationManager(_CoordX, _CoordY, _SideLength)
            {
                RotateAroundCenter = _RotateAroundCenter,
                MaxRotation = _MaxRotation, 
                VertexIndex = _PivotVertex
            };
        }
    }
}
