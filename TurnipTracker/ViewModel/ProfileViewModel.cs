using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using MvvmHelpers;
using MvvmHelpers.Commands;
using NodaTime.TimeZones;
using TurnipTracker.Model;
using TurnipTracker.Services;

namespace TurnipTracker.ViewModel
{
    public class ProfileViewModel : ViewModelBase
    {
        public AsyncCommand UpsertProfileCommand { get; }
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

            UpsertProfileCommand = new AsyncCommand(UpsertProfile);
        }

        void SaveProfile()
        {
            DataService.SaveProfile(Profile);
        }

        async Task UpsertProfile()
        {
            if (IsBusy)
                return;

            if (string.IsNullOrWhiteSpace(Profile.Name))
            {
                await DisplayAlert("Update Profile", "Please enter your name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Profile.IslandName))
            {
                await DisplayAlert("Update Profile", "Please enter your island name.");
                return;
            }

            if (!(await CheckConnectivity("Check connectivity", "Unable to update profile, please check internet and try again")))
                return;

            var sync = await DisplayAlert("Sync profile?", "Are you sure you want to sync your profile to the cloud?", "Yes, sync", "Cancel");
            if (!sync)
                return;

            try
            {
                IsBusy = true;
                await DataService.UpsertUserProfile(Profile);
                await DisplayAlert("Profile synced", "You are all set!");
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
