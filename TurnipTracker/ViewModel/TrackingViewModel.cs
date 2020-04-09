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
                new Day { DayLong = "Sunday", IsSunday = true, UpdatePredictionAction = UpdatePredications },
                new Day { DayLong = "Monday", UpdatePredictionAction = UpdatePredications },
                new Day { DayLong = "Tuesday", UpdatePredictionAction = UpdatePredications },
                new Day { DayLong = "Wednesday", UpdatePredictionAction = UpdatePredications },
                new Day { DayLong = "Thursday", UpdatePredictionAction = UpdatePredications },
                new Day { DayLong = "Friday", UpdatePredictionAction = UpdatePredications },
                new Day { DayLong = "Saturday", UpdatePredictionAction = UpdatePredications }
            };

            SelectedDay = Days[(int)DateTime.Now.DayOfWeek];
        }

        Day selectedDay;

        public Day SelectedDay
        {
            get => selectedDay;
            set => SetProperty(ref selectedDay, value, onChanged: UpdatePredications);
        }

        public void UpdatePredications()
        {
            PredictionUpdater.Update(this.Days);
        }
    }
}
