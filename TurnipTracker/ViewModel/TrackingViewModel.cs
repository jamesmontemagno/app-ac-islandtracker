using System;
using System.Collections.Generic;
using MvvmHelpers;
using TurnipTracker.Model;
using TurnipTracker.Services;
using Xamarin.Forms;

namespace TurnipTracker.ViewModel
{
    public class TrackingViewModel : BaseViewModel
    {

        public List<Day> Days { get; }


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
                if (selectedDay != null)
                    selectedDay.IsSelectedDay = false;

                SetProperty(ref selectedDay, value, onChanged: UpdatePredications);
                selectedDay.IsSelectedDay = true;
            }
        }

        public Command<Day> DaySelectedCommand { get; }

        void OnDaySelected(Day day) =>
            SelectedDay = day;

        public void SaveCurrentWeek()
        {
            DataService.SaveCurrentWeek(Days);
            UpdatePredications();
        }

        public void UpdatePredications()
        {
            PredictionUpdater.Update(this.Days);
        }
    }
}
