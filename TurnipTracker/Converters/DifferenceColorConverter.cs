using System;
using System.Globalization;
using Xamarin.Forms;

namespace TurnipTracker.Converters
{
    public class DifferenceColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string diff)
                return diff.Contains("+") ? Color.Green : (diff.Contains("-") ? Color.Red : Color.Black);

            return Color.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
