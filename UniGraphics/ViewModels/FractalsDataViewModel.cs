using System.Numerics;
using System.Windows.Media.Imaging;
using UniGraphics.Fractals;

namespace UniGraphics.ViewModels
{
    public class FractalsDataViewModel : ViewModelBase
    {
        private double _fractalScale;
        public double FractalScale
        {
            get { return _fractalScale; }
            set
            {
                _fractalScale = value;
                OnPropertyChanged("FractalScale");
                Generator.FractalScale = value;
                Generator.generate();
                Image = Generator.Image;
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
                Generator.generate();
                Image = Generator.Image;
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
                Generator.generate();
                Image = Generator.Image;
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
                Generator.generate();
                Image = Generator.Image;
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
                Generator.generate();
                Image = Generator.Image;
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

        public FractalsDataViewModel()
        {
            _image = null;
            _constantReal = -1.0;
            _constantImaginary = 0.0;
            _currentColorModel = 0;
            _fractalScale = 1.0;
            _fractalPower = 3.0;
            Generator = new NewtonFractalGenerator(_fractalPower,
                                                   new Complex(_constantReal, _constantImaginary),
                                                   NewtonFractalGenerator.colorModels[_currentColorModel])
            {
                Width = 400,
                Height = 400,
                FractalScale = _fractalScale
            };
        }
    }
}
