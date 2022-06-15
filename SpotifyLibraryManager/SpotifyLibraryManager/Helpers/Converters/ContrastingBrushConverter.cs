using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SpotifyLibraryManager.Helpers.Converters
{
    public class ContrastingBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush? brush = value as SolidColorBrush;

            if (brush is null)
            {
                return Brushes.White;
            }
            else
            {
                return ContrastCalculator.GetContrastingBrush(brush.Color);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
