using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using MvvmHelpers;
using MvvmHelpers.Commands;
using TurnipTracker.Model;
using TurnipTracker.Services;

namespace TurnipTracker.ViewModel
{
    public class ProfileViewModel : ViewModelBase
    {
        public AsyncCommand UpsertProfileCommand { get; }
        public Profile Profile { get; }
        public ObservableRangeCollection<FruitItem> Fruits { get; }

        public bool NeedsProfile => !SettingsService.HasRegistered;
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

            if(SettingsService.HasRegistered)
            {
                var sync = await DisplayAlert($"Sync profile?", "Are you sure you want to sync your profile to the cloud?", $"Yes, Sync", "Cancel");
                if (!sync)
                    return;
            }
            else
            {
                var sync = await DisplayAlert($"Create profile?", "Are you sure you want to create your profile and sync it to the cloud? By doing so you agree to the Terms and Conditions found on the About page.", $"Yes, Create", "Cancel");
                if (!sync)
                    return;
            }            

            Analytics.TrackEvent("SyncProfile");

            try
            {
                IsBusy = true;
                await DataService.UpsertUserProfile(Profile);
                await DisplayAlert("Profile synced", "You are all set!");

                OnPropertyChanged(nameof(NeedsProfile));
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
