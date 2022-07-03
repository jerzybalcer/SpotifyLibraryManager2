using System;
using System.Globalization;
using System.Windows.Data;

namespace SpotifyLibraryManager.Helpers.Converters
{
    public class CenterPopupConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double popupWidth = (double)values[0];
            double toggleButtonWidth = (double)values[1];

            return -(popupWidth - toggleButtonWidth) / 2d;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
