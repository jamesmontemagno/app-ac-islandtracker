using System;
using System.Linq;
using MvvmHelpers;

namespace TurnipTracker.Model
{
    public class Day : ObservableObject
    {
        public Action UpdatePredictionAction { get; set; }
        public string DayLong { get; set; }
        public string DayShort => DayLong.FirstOrDefault().ToString();

        bool firstPurchase;
        public bool FirstPurchase
        {
            get => firstPurchase;
            set => SetProperty(ref firstPurchase, value, onChanged: UpdatePredictionAction);
        }

        int lastWeekPattern = (int)PredictionPattern.IDontKnow;
        public int LastWeekPattern
        {
            get => lastWeekPattern;
            set => SetProperty(ref lastWeekPattern, value, onChanged: UpdatePredictionAction);
        }

        int? buyPrice;

        public int? BuyPrice
        {
            get => buyPrice;
            set => SetProperty(ref buyPrice, value, onChanged: UpdatePredictionAction);
        }

        int? priceAM;

        public int? PriceAM
        {
            get => priceAM;
            set => SetProperty(ref priceAM, value, onChanged: UpdatePredictionAction);
        }

        int? pricePM;
        public int? PricePM
        {
            get => pricePM;
            set => SetProperty(ref pricePM, value, onChanged: UpdatePredictionAction);
        }

        string predictionAM = string.Empty;
        public string PredictionAM
        {
            get => predictionAM;
            set => SetProperty(ref predictionAM, value);
        }

        string predictionPM = string.Empty;
        public string PredictionPM
        {
            get => predictionPM;
            set => SetProperty(ref predictionPM, value);
        }

        public bool IsSunday { get; set; }
        public bool IsNotSunday => !IsSunday;
    }
}
