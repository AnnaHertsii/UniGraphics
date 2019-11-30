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

        private void PerformExit(object args)
        {
            Application.Current.Shutdown();
        }

        private void PerformTransform(object args)
        {
            if (currentRunningTask != null && !currentRunningTask.IsCanceled)
                tokenSource.Cancel();
            tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            currentRunningTask = new Task(() =>
            {
                bool finished;
                Stopwatch timer = new Stopwatch();
                timer.Start();
                while (true)
                {
                    finished = transformer.HandleTimeFlow(timer.Elapsed.TotalSeconds);
                    timer.Restart();
                    if (transformer.UpdateTransform(token))
                    {
                        var clone = transformer.Image.Clone();
                        clone.Freeze();
                        currentRunningTask = null;
                        Dispatcher.CurrentDispatcher.Invoke(() => TransformationImage = clone);
                    }
                    if (finished)
                        return;
                }
            }, token);
            currentRunningTask.Start();
        }

        public ICommand Exit { get; set; }
        public ICommand Transform { get; set; }


        private Task currentRunningTask = null;
        private CancellationTokenSource tokenSource;

        public void respondToWindowSizeChange(int width, int height)
        {
            if (width == 0 || height == 0)
                return;
            if (currentRunningTask != null && !currentRunningTask.IsCanceled)
                tokenSource.Cancel();
            tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            currentRunningTask = new Task(() =>
            {
                if (transformer.FullGenerate(width, height, token))
                {
                    var clone = transformer.Image.Clone();
                    clone.Freeze();
                    currentRunningTask = null;
                    Dispatcher.CurrentDispatcher.Invoke(() => TransformationImage = clone);
                }
            }, token);
            currentRunningTask.Start();
        }

        public void initialSettings(int width, int height)
        {
            tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            currentRunningTask = new Task(() =>
            {
                if (transformer.FullGenerate(width, height, token))
                {
                    var clone = transformer.Image.Clone();
                    clone.Freeze();
                    currentRunningTask = null;
                    Dispatcher.CurrentDispatcher.Invoke(() => TransformationImage = clone);
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

        public TransformationsViewModel()
        {
            Exit = new Command(PerformExit);
            Transform = new Command(PerformTransform);
            transformer = new TransformationManager(50, 100, 20)
            {
                RotateAroundCenter = true,
                MaxRotation = 60, 
                VertexIndex = 0
            };
        }
    }
}
