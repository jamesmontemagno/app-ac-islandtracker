using System;
using System.Globalization;
using TurnipTracker.Shared;
using Xamarin.Forms;

namespace TurnipTracker.Converters
{
    public class FriendPredictionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            if (value is FriendStatus status)
            {
                if (status.MinPrediction == 0 || status.MinPrediction == 999 || status.MaxPrediction == 0 || status.MaxPrediction == 999)
                    return $"🔮 ???";

                if (status.MinPrediction == status.MaxPrediction)
                    return $"🔮 {status.MinPrediction}";

                return $"🔮 {status.MinPrediction}-{status.MaxPrediction}";
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
