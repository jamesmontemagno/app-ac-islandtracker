using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using MvvmHelpers;
using MvvmHelpers.Commands;
using TurnipTracker.Helpers;
using TurnipTracker.Model;
using TurnipTracker.Services;
using Xamarin.Essentials;

namespace TurnipTracker.ViewModel
{
    public class ProfileViewModel : ViewModelBase
    {

        public AsyncCommand<Xamarin.Forms.View> UpsertProfileCommand { get; }
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

            UpsertProfileCommand = new AsyncCommand<Xamarin.Forms.View>(UpsertProfile);
        }

        void SaveProfile()
        {
            DataService.SaveProfile(Profile);
            NeedsSync = true;
            SettingsService.UpdateProfile = true;
        }

        async Task UpsertProfile(Xamarin.Forms.View element)
        {
            if (IsBusy)
                return;

            var firstCreate = NeedsProfile;

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

                if (firstCreate)
                    await Transfer(element);
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

        async Task Transfer(Xamarin.Forms.View element)
        {
           
            var result = await DisplayAlert("Backup account", "It is recommended that you backup your account codes. This will export your credentials that you can re-import on another device in the settings page.", "OK", "Not Now");
            if (!result)
                return;

            var info = await SettingsService.TransferOut();

            var bounds = element.GetAbsoluteBounds();

            await Share.RequestAsync(new ShareTextRequest
            {
                PresentationSourceBounds = bounds.ToSystemRectangle(),
                Title = "Island Tracker for ACNH Backup Codes",
                Text = info
            });

            Analytics.TrackEvent("Transfer", new Dictionary<string, string>
            {
                ["type"] = "out"
            });
        }
    }
}
