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

        public string SyncCreateText => SettingsService.HasRegistered ? "Sync" : "Create";
        public bool NeedsProfile => !SettingsService.HasRegistered;
        bool needsSync;
        public bool NeedsSync
        {
            get => needsSync;
            set => SetProperty(ref needsSync, value);
        }
        public ProfileViewModel()
        {
            if (Xamarin.Forms.DesignMode.IsDesignModeEnabled)
                return;

            Profile = DataService.GetProfile();
            Profile.SaveProfileAction = SaveProfile;

            UpsertProfileCommand = new AsyncCommand(UpsertProfile);
        }

        void SaveProfile()
        {
            DataService.SaveProfile(Profile);
            NeedsSync = true;
            SettingsService.UpdateProfile = true;
        }

        async Task UpsertProfile()
        {
            if (IsBusy)
                return;

            if (string.IsNullOrWhiteSpace(Profile.Name))
            {
                await DisplayAlert("Update Profile", "Please enter your nickname.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Profile.IslandName))
            {
                await DisplayAlert("Update Profile", "Please enter your island name.");
                return;
            }

            if (!(await CheckConnectivity("Check connectivity", "Unable to update profile, please check internet and try again")))
                return;

            Analytics.TrackEvent("SyncStatus");

            try
            {
                IsBusy = true;
                await DataService.UpsertUserProfile(Profile);

                OnPropertyChanged(nameof(NeedsProfile));
                OnPropertyChanged(nameof(SyncCreateText));
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
