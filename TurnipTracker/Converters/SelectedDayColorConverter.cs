using System;
using System.Globalization;
using Xamarin.Forms;

namespace TurnipTracker.Converters
{
    public class SelectedDayColorConverter : IValueConverter
    {
        static Color acrylicColor;
        static SelectedDayColorConverter()
        {
            acrylicColor = (Color)App.Current.Resources["AcrylicFrameBackgroundColor"];
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool val)
                return val ? Color.LightPink : acrylicColor;

            return acrylicColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
