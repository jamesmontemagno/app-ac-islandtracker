using System;
using System.Linq;
using MvvmHelpers;

namespace TurnipTracker.Model
{
    public class Day : ObservableObject
    {
        public string DayLong { get; set; }
        public string DayShort => DayLong.FirstOrDefault().ToString();

        string priceAM = string.Empty;

        public string PriceAM
        {
            get => priceAM;
            set => SetProperty(ref priceAM, value);
        }

        string pricePM = string.Empty;
        public string PricePM
        {
            get => pricePM;
            set => SetProperty(ref pricePM, value);
        }

        public bool IsSunday { get; set; }
        public bool IsNotSunday => !IsSunday;
    }
}
