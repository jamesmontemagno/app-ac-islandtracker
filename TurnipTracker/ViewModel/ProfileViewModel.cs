using System;
using System.Collections.Generic;
using System.Linq;
using MvvmHelpers;
using NodaTime.TimeZones;
using TurnipTracker.Model;
using TurnipTracker.Services;

namespace TurnipTracker.ViewModel
{
    public class ProfileViewModel : BaseViewModel
    {
        public Profile Profile { get; }
        public ObservableRangeCollection<FruitItem> Fruits { get; }
        public ProfileViewModel()
        {
            if (Xamarin.Forms.DesignMode.IsDesignModeEnabled)
                return;

            Profile = DataService.GetProfile();
            Profile.SaveProfileAction = SaveProfile;
            Fruits = new ObservableRangeCollection<FruitItem>
            {
                new FruitItem { Icon = "apple.png", Name="Apple"},
                new FruitItem { Icon = "cherry.png", Name="Cherry"},
                new FruitItem { Icon = "orange.png", Name="Orange"},
                new FruitItem { Icon = "peach.png", Name="Peach"},
                new FruitItem { Icon = "pear.png", Name="Pear"},
            };
        }

        void SaveProfile()
        {
            DataService.SaveProfile(Profile);
        }
    }
}
