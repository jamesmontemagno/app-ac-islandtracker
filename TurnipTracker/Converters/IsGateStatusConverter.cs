using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TurnipTracker.Shared;
using Xamarin.Forms;

namespace TurnipTracker.Converters
{
    public class IsGateStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int status && parameter is string test && int.TryParse(test, out var check))
                return status == check;

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IsGateOpenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FriendStatus status && status.GateStatus != 0 && status.GateClosesAtUTC != null && status.GateClosesAtUTC > DateTime.UtcNow)
                return true;

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
