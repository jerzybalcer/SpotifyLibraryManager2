using SpotifyLibraryManager.Models;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace SpotifyLibraryManager.Helpers.Converters
{
    public class ComboBoxItemSortByConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var selectedItem = (ComboBoxItem)value;

            if (Enum.TryParse(selectedItem.Content.ToString(), out SortBy sortBy))
            {
                return sortBy;
            }
            else
            {
                return SortBy.LikeDate;
            }
        }
    }
}
