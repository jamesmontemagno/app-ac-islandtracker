using System;
using System.Collections.Generic;
using MvvmHelpers;
using TurnipTracker.Model;
using TurnipTracker.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TurnipTracker.ViewModel
{
    public class TrackingViewModel : BaseViewModel
    {

        public List<Day> Days { get; }

        public Command<Day> DaySelectedCommand { get; }

        public TrackingViewModel()
        {
            if (Xamarin.Forms.DesignMode.IsDesignModeEnabled)
                return;


            
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
            if (sunday.BuyPrice.HasValue && SelectedDay != sunday)
            {
                if (SelectedDay.PriceAM.HasValue)
                {
                    var diffAM = SelectedDay.PriceAM.Value - sunday.BuyPrice.Value;
                    SelectedDay.DifferenceAM = diffAM < 0 ? $"↘️ {diffAM}" : $"↗️ +{diffAM}";
                }
                else
                {
                    SelectedDay.DifferenceAM = string.Empty;
                }

                if (SelectedDay.PricePM.HasValue)
                {
                    var diffPM = SelectedDay.PricePM.Value - sunday.BuyPrice.Value;
                    SelectedDay.DifferencePM = diffPM < 0 ? $"↘️ {diffPM}" : $"↗️ +{diffPM}";
                }
                else
                {
                    SelectedDay.DifferencePM = string.Empty;
                }
            }

            var low = 0;
            var high = 0;
            foreach(var day in Days)
            {
                if (day == sunday)
                    continue;

                if (!day.PriceAM.HasValue)
                {
                    if (day.PredictionAMMin > low)
                        low = day.PredictionAMMin;

                    if (day.PredictionAMMax > high)
                        high = day.PredictionAMMax;
                }

                if (!day.PricePM.HasValue)
                {
                    if (day.PredictionPMMin > low)
                        low = day.PredictionPMMin;

                    if (day.PredictionPMMax > high)
                        high = day.PredictionPMMax;
                }
            }

            Min = low;
            Max = high;
        }
    }
}
