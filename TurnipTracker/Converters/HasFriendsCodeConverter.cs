using System;
using System.Globalization;
using Xamarin.Forms;

namespace TurnipTracker.Converters
{
    public class HasFriendsCodeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;
            if (value is string diff)
                return !string.IsNullOrWhiteSpace(diff) && diff.Length == 17;

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
