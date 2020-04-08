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

        string predictionAM = string.Empty;
        public string PredictionAM {
            get => predictionAM;
            set => SetProperty(ref predictionAM, value);
        }

        string predictionPM = string.Empty;
        public string PredictionPM {
            get => predictionPM;
            set => SetProperty(ref predictionPM, value);
        }

        public bool IsSunday { get; set; }
        public bool IsNotSunday => !IsSunday;
    }
}
