using System;
using System.Globalization;
using TurnipTracker.Shared;
using Xamarin.Forms;

namespace TurnipTracker.Converters
{
    public class ExpiresAtConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;
            if (value is DateTime time && time != null && time > DateTime.UtcNow)
            {
                var local = time.ToLocalTime();
                if (local.Day == DateTime.Now.Day)
                    return $"Closes @{local.ToShortTimeString()}";
                else
                    return $"Closes @{local.ToShortTimeString()} tmw";

            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
