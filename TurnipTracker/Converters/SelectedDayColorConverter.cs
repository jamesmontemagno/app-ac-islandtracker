using System;
using System.Globalization;
using Xamarin.Forms;

namespace TurnipTracker.Converters
{
    public class SelectedDayColorConverter : IValueConverter
    {
        static Color textPrimary;
        static Color primary;
        static SelectedDayColorConverter()
        {
            textPrimary = (Color)App.Current.Resources["TextPrimaryColor"];
            primary = (Color)App.Current.Resources["NavigationPrimary"];
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool val)
                return val ? primary : textPrimary;

            return textPrimary;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
