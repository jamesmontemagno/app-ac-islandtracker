using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AppCenter.Analytics;
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
        public AsyncCommand ComputeCommand { get; }
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
            ComputeCommand = new AsyncCommand(Compute);
        }

        async Task Compute()
        {
            if (IsBusy)
                return;

            var result = await App.Current.MainPage.DisplayActionSheet("Turnip Calculators", "Cancel", null, "How Many Bells Do I Need?", "How Many Turnips Can I Buy?");

            var page = result switch
            {
                "How Many Bells Do I Need?" => "calc-howmanybells",
                "How Many Turnips Can I Buy?" => "calc-howmanyturnips",
                _ => string.Empty
            };

            if (string.IsNullOrWhiteSpace(page))
                return;

            var price = SelectedDay.ActualPurchasePrice.HasValue ? 
                SelectedDay.ActualPurchasePrice.Value : 
                (SelectedDay.BuyPrice.HasValue ? SelectedDay.BuyPrice.Value : 0);


            await GoToAsync($"{page}?Price={price}", page);


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

        bool needsSync;
        public bool NeedsSync
        {
            get => needsSync;
            set => SetProperty(ref needsSync, value);
        }
        public bool IsToday => Days.IndexOf(SelectedDay) == (int)DateTime.Now.DayOfWeek;

        int min = 0;
        public int Min
        {
            get => min;
            set
            {
                if (SetProperty(ref min, value))
                    OnPropertyChanged(nameof(MinString));
            }
        }

        int max = 0;
        public int Max
        {
            get => max;
            set
            {
                if (SetProperty(ref max, value))
                    OnPropertyChanged(nameof(MaxString));
            }
        }

        public string MinString => Min == 0 ? "Guaranteed Min: ???" : $"Guarenteed Min: {Min}";
        public string MaxString => Max == 999 ? "Potential Max: ???" : $"Potential Max: {Max}";


        void OnDaySelected(Day day) => MainThread.BeginInvokeOnMainThread(() =>
                                     {
                                         SelectedDay = day;
                                     });

        public void SaveCurrentWeek()
        {
            if(IsToday && SettingsService.HasRegistered)
            {
                NeedsSync = true;
            }    
            DataService.SaveCurrentWeek(Days);
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
            for (var i = (int)DateTime.Now.DayOfWeek; i < Days.Count; i++)
            {
                var day = Days[i];
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
                await App.Current.MainPage.DisplayAlert("Register First", "Please create a profile before syncing turnip prices.", "OK");
                return;
            }

            if (!(await CheckConnectivity("Check connectivity", "Unable to update prices, please check internet and try again")))
                return;

            //doesn't need sync
            if (!NeedsSync)
                return;

            Analytics.TrackEvent("SyncTurnipPrices");

            try
            {
                IsBusy = true;
                await DataService.UpdateTurnipPrices(SelectedDay, Min, Max);
                NeedsSync = false;
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


        public bool ShowFirstTimeBuying => !SettingsService.HideFirstTimeBuying;

        public void OnAppearing()
        {
            OnPropertyChanged(nameof(ShowFirstTimeBuying));
        }
    }
}
