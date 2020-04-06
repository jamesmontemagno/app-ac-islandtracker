using System;
using System.Collections.Generic;
using MvvmHelpers;
using TurnipTracker.Model;

namespace TurnipTracker.ViewModel
{
    public class TrackingViewModel : BaseViewModel
    {

        public List<Day> Days { get; }


        public TrackingViewModel()
        {
            if (Xamarin.Forms.DesignMode.IsDesignModeEnabled)
                return;
            Days = new List<Day>
            {
                new Day { DayLong = "Sunday", IsSunday = true },
                new Day { DayLong = "Monday" },
                new Day { DayLong = "Tuesday" },
                new Day { DayLong = "Wednesday" },
                new Day { DayLong = "Thursday" },
                new Day { DayLong = "Friday" },
                new Day { DayLong = "Saturday" }
            };

            SelectedDay = Days[(int)DateTime.Now.DayOfWeek];
        }

        Day selectedDay;

        public Day SelectedDay
        {
            get => selectedDay;
            set => SetProperty(ref selectedDay, value);
        }
    }
}
