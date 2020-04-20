using System;
using System.Collections.Generic;
using MvvmHelpers;
using Syncfusion.SfChart.XForms;
using TurnipTracker.Model;
using TurnipTracker.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TurnipTracker.ViewModel
{
    public class TrackingViewModel : BaseViewModel
    {

        public List<Day> Days { get; }

        public List<ChartDataPoint> ChartDataMin { get; }
        public List<ChartDataPoint> ChartDataMax { get; }
        public List<ChartDataPoint> ChartDataPrice { get; }

        public Command<Day> DaySelectedCommand { get; }

        public TrackingViewModel()
        {
            if (Xamarin.Forms.DesignMode.IsDesignModeEnabled)
                return;


            ChartDataMin = new List<ChartDataPoint>();
            ChartDataMax = new List<ChartDataPoint>();
            ChartDataPrice = new List<ChartDataPoint>();


            Days = DataService.GetCurrentWeek();
            foreach (var day in Days)
                day.SaveCurrentWeekAction = SaveCurrentWeek;



            SelectedDay = Days[(int)DateTime.Now.DayOfWeek];

            DaySelectedCommand = new Command<Day>(OnDaySelected);
        }

        Day selectedDay;

        public Day SelectedDay
        {
            get => selectedDay;
            set
            {
                if (selectedDay == value)
                    return;

                if (selectedDay != null)
                    selectedDay.IsSelectedDay = false;

                SetProperty(ref selectedDay, value, onChanged: UpdatePredications);
                selectedDay.IsSelectedDay = true;
            }
        }

        int min = 0;
        public int Min
        {
            get => min;
            set => SetProperty(ref min, value);
        }

        int max = 0;
        public int Max
        {
            get => max;
            set => SetProperty(ref max, value);
        }


        void OnDaySelected(Day day)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                SelectedDay = day;
            });
        }

        public void SaveCurrentWeek()
        {
            DataService.SaveCurrentWeek(Days);
            UpdatePredications();
        }



        public void UpdatePredications()
        {
            PredictionUpdater.Update(Days);

            var sunday = Days[0];
            if ((sunday.BuyPrice.HasValue || sunday.ActualPurchasePrice.HasValue) && SelectedDay != sunday)
            {
                var actualBuy = sunday.ActualPurchasePrice.HasValue ? sunday.ActualPurchasePrice.Value : sunday.BuyPrice.Value;
                if (SelectedDay.PriceAM.HasValue)
                {
                    var diffAM = SelectedDay.PriceAM.Value - actualBuy;
                    if (diffAM == 0)
                        SelectedDay.DifferenceAM = "↔ 0";
                    else
                        SelectedDay.DifferenceAM = diffAM < 0 ? $"↘️ {diffAM}" : $"↗️ +{diffAM}";
                }
                else
                {
                    SelectedDay.DifferenceAM = string.Empty;
                }

                if (SelectedDay.PricePM.HasValue)
                {
                    var diffPM = SelectedDay.PricePM.Value - actualBuy;
                    if (diffPM == 0)
                        SelectedDay.DifferencePM = "↔ 0";
                    else
                        SelectedDay.DifferencePM = diffPM < 0 ? $"↘️ {diffPM}" : $"↗️ +{diffPM}";
                }
                else
                {
                    SelectedDay.DifferencePM = string.Empty;
                }
            }

            var low = 0;
            var high = 0;
            ChartDataPrice.Clear();
            ChartDataMin.Clear();
            ChartDataMax.Clear();
            foreach (var day in Days)
            {

                if (day == sunday)
                {

                    var val = day.BuyPrice.HasValue ? day.BuyPrice.Value : 0;
                    ChartDataPrice.Add(new ChartDataPoint("S", val));
                    ChartDataMin.Add(new ChartDataPoint("S", val));
                    ChartDataMax.Add(new ChartDataPoint("S", val));
                    continue;
                }
                

                if (!day.PriceAM.HasValue)
                {
                    if (day.PredictionAMMin > low)
                        low = day.PredictionAMMin;

                    if (day.PredictionAMMax > high)
                        high = day.PredictionAMMax;

                    ChartDataMin.Add(new ChartDataPoint($"{day.DayShort} A", day.PredictionAMMin));
                    ChartDataMax.Add(new ChartDataPoint($"{day.DayShort} A", day.PredictionAMMax));
                }
                else
                {
                    ChartDataPrice.Add(new ChartDataPoint($"{day.DayShort} A", day.PriceAM.Value));
                    ChartDataMin.Add(new ChartDataPoint($"{day.DayShort} A", day.PriceAM.Value));
                    ChartDataMax.Add(new ChartDataPoint($"{day.DayShort} A", day.PriceAM.Value));
                }

                if (!day.PricePM.HasValue)
                {
                    if (day.PredictionPMMin > low)
                        low = day.PredictionPMMin;

                    if (day.PredictionPMMax > high)
                        high = day.PredictionPMMax;

                    ChartDataMin.Add(new ChartDataPoint($"{day.DayShort} P", day.PredictionPMMin));
                    ChartDataMax.Add(new ChartDataPoint($"{day.DayShort} P", day.PredictionPMMax));
                }
                else
                {
                    ChartDataPrice.Add(new ChartDataPoint($"{day.DayShort} P", day.PricePM.Value));
                    ChartDataMin.Add(new ChartDataPoint($"{day.DayShort} P", day.PricePM.Value));
                    ChartDataMax.Add(new ChartDataPoint($"{day.DayShort} P", day.PricePM.Value));
                }
            }

            Min = low;
            Max = high;

            OnPropertyChanged(nameof(ChartDataPrice));
            OnPropertyChanged(nameof(ChartDataMin));
            OnPropertyChanged(nameof(ChartDataMax));
        }
    }
}
