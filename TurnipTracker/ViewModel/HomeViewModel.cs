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
        public Profile Profile { get; set; }

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
                await DisplayAlert("Welcome!", "Welcome to Island Tracker, your social turnip tracking companion. Let's get started by creating your profile.");
                await GoToAsync("profile");
            }
            else if(SettingsService.UpdateProfile)
            {
                SettingsService.UpdateProfile = false;
                Profile = DataService.GetProfile();
                Profile.SaveProfileAction = SaveProfile;
                OnPropertyChanged(nameof(Profile));
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
                await DisplayAlert("Create Profile", "Please create your profile before updating your status.");
                await GoToAsync("profile");
                return;
            }

            if (string.IsNullOrWhiteSpace(Profile.Name) ||  string.IsNullOrWhiteSpace(Profile.IslandName))
            {
                await DisplayAlert("Profile needs updates", "Please ensure your profile has a nickname and island name.");
                await GoToAsync("profile");
                return;
            }

            if(Profile.GateStatus == 3 && Profile.DodoCode?.Length != 5)
            {
                await DisplayAlert("Invalid Dodo Code", "Please enter a valid Dodo code, it should be 5 characters in length.");
                
                return;
            }

            if (!(await CheckConnectivity("Check connectivity", "Unable to update profile, please check internet and try again")))
                return;

            Analytics.TrackEvent("SyncProfile");

            try
            {
                IsBusy = true;
                OnPropertyChanged(nameof(IsProAndNotBusy));
                await DataService.UpsertUserProfile(Profile);
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
                OnPropertyChanged(nameof(IsProAndNotBusy));
            }
        }
    }
}
