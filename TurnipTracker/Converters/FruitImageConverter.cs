using System;
using System.Globalization;
using Xamarin.Forms;

namespace TurnipTracker.Converters
{
    public class FruitImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;
            if (value is int fruit)
            {
                return fruit switch
                {
                    0 => "apple.png",
                    1 => "cherry.png",
                    2 => "orange.png",
                    3 => "peach.png",
                    4 => "pear.png",
                    _ => "bells.png"
                };
            }

            /*
             *  <x:String>🍎 Apple</x:String>
                <x:String>🍒 Cherry</x:String>
                <x:String>🍊 Orange</x:String>
                <x:String>🍑 Peach</x:String>
                <x:String>🍐 Pear</x:String>
             */
            return "bells.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
