using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AppCenter.Crashes;
using MvvmHelpers;
using MvvmHelpers.Commands;
using TurnipTracker.Model;
using TurnipTracker.Services;
using Xamarin.Essentials;

namespace TurnipTracker.ViewModel
{
    public class TrackingViewModel : ViewModelBase
    {

        public AsyncCommand UpdateTurnipPricesCommand { get; }
        public List<Day> Days { get; }

        public ObservableRangeCollection<ChartDataModel> ChartData { get; }

        public Command<Day> DaySelectedCommand { get; }

        public TrackingViewModel()
        {
            if (Xamarin.Forms.DesignMode.IsDesignModeEnabled)
                return;


            ChartData = new ObservableRangeCollection<ChartDataModel>();


            Days = DataService.GetCurrentWeek();
            foreach (var day in Days)
                day.SaveCurrentWeekAction = SaveCurrentWeek;



            SelectedDay = Days[(int)DateTime.Now.DayOfWeek];

            DaySelectedCommand = new Command<Day>(OnDaySelected);
            UpdateTurnipPricesCommand = new AsyncCommand(UpdateTurnipPrices);
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
                OnPropertyChanged(nameof(IsToday));
            }
        }

        public bool IsToday => Days.IndexOf(SelectedDay) == (int)DateTime.Now.DayOfWeek;

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


        void OnDaySelected(Day day) => MainThread.BeginInvokeOnMainThread(() =>
                                     {
                                         SelectedDay = day;
                                     });

        public void SaveCurrentWeek()
        {
            DataService.SaveCurrentWeek(Days);
            // skip if still entring AM
            if (SelectedDay.PriceAM.HasValue && SelectedDay.PriceAM.Value < 10)
                return;
            if (SelectedDay.PricePM.HasValue && SelectedDay.PricePM.Value < 10)
                return;

            UpdatePredications();
        }

        bool isGraphExpanded;
        public bool IsGraphExpanded
        {
            get => isGraphExpanded;
            set
            {
                isGraphExpanded = value;
                if (isGraphExpanded)
                    UpdateGraph();
            }
        }

        public void UpdateGraph()
        {
            var chartData = new List<ChartDataModel>();
            var sunday = Days[0];
            foreach (var day in Days)
            {

                if (day == sunday)
                {

                    var val = day.BuyPrice ?? 0;
                    chartData.Add(new ChartDataModel("S", day.DayLong, val));
                    continue;
                }

                if (!day.PriceAM.HasValue)
                {
                    chartData.Add(new ChartDataModel($"{day.DayShort} A", $"{day.DayLong} AM", day.PredictionAMMax, day.PredictionAMMin));
                }
                else
                {
                    chartData.Add(new ChartDataModel($"{day.DayShort} A", $"{day.DayLong} AM", day.PriceAM.Value));
                }

                if (!day.PricePM.HasValue)
                {
                    chartData.Add(new ChartDataModel($"{day.DayShort} P", $"{day.DayLong} PM", day.PredictionPMMax, day.PredictionPMMin));
                }
                else
                {
                    chartData.Add(new ChartDataModel($"{day.DayShort} P", $"{day.DayLong} PM", day.PricePM.Value));
                }
            }

            ChartData.ReplaceRange(chartData);
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
                        SelectedDay.DifferenceAM = "↔️ 0";
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
                        SelectedDay.DifferencePM = "↔️ 0";
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
            foreach (var day in Days)
            {

                if (day == sunday)
                {

                    var val = day.BuyPrice ?? 0;
                    continue;
                }


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


            if (IsGraphExpanded)
                UpdateGraph();
        }

        async Task UpdateTurnipPrices()
        {
            if (IsBusy)
                return;

            if (!IsToday)
                return;

            if (!SettingsService.HasRegistered)
            {
                await App.Current.MainPage.DisplayAlert("Register First", "Please register your account on the profile tab.", "OK");
                return;
            }

            if (!(await CheckConnectivity("Check connectivity", "Unable to update prices, please check internet and try again")))
                return;

            var sync = await DisplayAlert("Sync prices?", "Are you sure you want to sync your prices to the cloud?", "Yes, sync", "Cancel");
            if (!sync)
                return;

            try
            {
                IsBusy = true;
                await DataService.UpdateTurnipPrices(SelectedDay);
                await DisplayAlert("Turnip prices synced", "You are all set!");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Sync Error", ex.Message);
                Crashes.TrackError(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
