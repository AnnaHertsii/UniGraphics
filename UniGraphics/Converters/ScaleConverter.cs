using System;
using System.Globalization;
using System.Windows.Data;

namespace UniGraphics.Converters
{
    public class ScaleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val = Math.Round(remapScale((double)value) * 100, 2);
            return $"{val}%";
        }

        public static double remapScale(double scale)
        {
            if (scale >= 0.0)
                return 1.0 + scale * 4;
            else
                return (1.0 + scale) * 0.9 + 0.1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
