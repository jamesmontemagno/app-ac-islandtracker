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
    public class HomeViewModel : ViewModelBase
    {

        public AsyncCommand FirstRunCommand { get; }
        public AsyncCommand UpsertProfileCommand { get; }
        public Profile Profile { get; }

        bool needsSync;
        public bool NeedsSync
        {
            get => needsSync;
            set => SetProperty(ref needsSync, value);
        }

        public HomeViewModel()
        {
            if (Xamarin.Forms.DesignMode.IsDesignModeEnabled)
                return;

            Profile = DataService.GetProfile();
            Profile.SaveProfileAction = SaveProfile;

            UpsertProfileCommand = new AsyncCommand(UpsertProfile);

            FirstRunCommand = new AsyncCommand(FirstRun);
        }


        async Task FirstRun()
        {

            if (SettingsService.FirstRun)
            {
                SettingsService.FirstRun = false;
                await DisplayAlert("Welcome!", "Get started with Island Tracker by filling in your profile. Then head over to the tracking section to track turnip prices and get predictions. Finally, sync everything to the cloud and share with your friends.");
                await GoToAsync("profile");
            }
        }

        void SaveProfile()
        {
            DataService.SaveProfile(Profile);
            NeedsSync = true;
        }

        async Task UpsertProfile()
        {
            if (IsBusy)
                return;

            if (!SettingsService.HasRegistered)
            {
                await DisplayAlert("Create Profile", "Please create your profile first.");
                await GoToAsync("profile");
                return;
            }

            if (string.IsNullOrWhiteSpace(Profile.Name) ||  string.IsNullOrWhiteSpace(Profile.IslandName))
            {
                await DisplayAlert("Profile needs updates", "Please ensure your profile has a nickname and island name.");
                await GoToAsync("profile");
                return;
            }

            if (!(await CheckConnectivity("Check connectivity", "Unable to update profile, please check internet and try again")))
                return;

            //doesn't need sync
            if (!NeedsSync)
                return;

            Analytics.TrackEvent("SyncProfile");

            try
            {
                IsBusy = true;
                await DataService.UpsertUserProfile(Profile);
                await DisplayAlert("Profile synced", "You are all set!");
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
    }
}
