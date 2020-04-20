using System;
using System.Linq;
using MvvmHelpers;
using Newtonsoft.Json;

namespace TurnipTracker.Model
{
    public class Day : ObservableObject
    {
        [JsonIgnore]
        public Action SaveCurrentWeekAction { get; set; }

        public string DayLong { get; set; }

        [JsonIgnore]
        public string DayShort => DayLong.FirstOrDefault().ToString();

        bool firstPurchase;
        public bool FirstPurchase
        {
            get => firstPurchase;
            set => SetProperty(ref firstPurchase, value, onChanged: SaveCurrentWeekAction);
        }

        int lastWeekPattern = (int)PredictionPattern.IDontKnow;
        public int LastWeekPattern
        {
            get => lastWeekPattern;
            set => SetProperty(ref lastWeekPattern, value, onChanged: SaveCurrentWeekAction);
        }

        int? buyPrice;

        public int? BuyPrice
        {
            get => buyPrice;
            set => SetProperty(ref buyPrice, value, onChanged: SaveCurrentWeekAction);
        }

        int? actualPurchasePrice;

        public int? ActualPurchasePrice
        {
            get => actualPurchasePrice;
            set => SetProperty(ref actualPurchasePrice, value, onChanged: SaveCurrentWeekAction);
        }

        int? priceAM;

        public int? PriceAM
        {
            get => priceAM;
            set => SetProperty(ref priceAM, value, onChanged: SaveCurrentWeekAction);
        }

        int? pricePM;
        public int? PricePM
        {
            get => pricePM;
            set => SetProperty(ref pricePM, value, onChanged: SaveCurrentWeekAction);
        }

        public bool IsSunday { get; set; }

        string differenceAM = string.Empty;
        [JsonIgnore]
        public string DifferenceAM
        {
            get => differenceAM;
            set => SetProperty(ref differenceAM, value);
        }

        string differencePM = string.Empty;
        [JsonIgnore]
        public string DifferencePM
        {
            get => differencePM;
            set => SetProperty(ref differencePM, value);
        }

        [JsonIgnore]
        public int PredictionAMMin { get; set; }
        [JsonIgnore]
        public int PredictionAMMax { get; set; }
        [JsonIgnore]
        public int PredictionPMMin { get; set; }
        [JsonIgnore]
        public int PredictionPMMax { get; set; }

        string predictionAM = string.Empty;
        [JsonIgnore]
        public string PredictionAM
        {
            get => predictionAM;
            set => SetProperty(ref predictionAM, value);
        }

        string predictionPM = string.Empty;
        [JsonIgnore]
        public string PredictionPM
        {
            get => predictionPM;
            set => SetProperty(ref predictionPM, value);
        }

        bool isSelectedDay;
        [JsonIgnore]
        public bool IsSelectedDay
        {
            get => isSelectedDay;
            set => SetProperty(ref isSelectedDay, value);
        }

        

        [JsonIgnore]
        public bool IsNotSunday => !IsSunday;
    }
}
