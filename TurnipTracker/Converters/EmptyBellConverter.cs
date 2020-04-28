using System;
using System.Globalization;
using Xamarin.Forms;

namespace TurnipTracker.Converters
{
    public class EmptyBellConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;
            if (value is int diff)
                return diff > 0;

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
