using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AppCenter.Analytics;
using MvvmHelpers.Commands;
using TurnipTracker.Services;
using Xamarin.Essentials;

namespace TurnipTracker.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        public AsyncCommand TransferCommand { get; }
        public AsyncCommand DeleteAccountCommand { get; }

        public SettingsViewModel()
        {
            TransferCommand = new AsyncCommand(Transfer);
            DeleteAccountCommand = new AsyncCommand(DeleteAccount);

        }

        async Task Transfer()
        {

            var choice = await App.Current.MainPage.DisplayActionSheet("Transfer profile?", "Cancel", null, "Transfer to another device", "Transfer to this device");


            if (choice.Contains("another"))
            {

                if (await DisplayAlert("Transfer profile out", "This will export your credentials that you can re-import on another device. Your credentials will remain on this device. Do you want to proceed?", "Yes, transfer", "Cancel"))
                {
                    var info = await SettingsService.TransferOut();
                    await Share.RequestAsync(info);

                    Analytics.TrackEvent("Transfer", new Dictionary<string, string>
                    {
                        ["type"] = "out"
                    });
                }
            }
            else if (choice.Contains("this device"))
            {
                if (await DisplayAlert("Transfer in profile?", "Warning! This will start a transfer process that will override your existing profile. Ensure that you have exported your existing profile first as you can not go back. Do you still want to proceed?", "Yes, transfer in", "Cancel"))
                {
                    var info = await App.Current.MainPage.DisplayPromptAsync("Entry transfer code", "Enter your transfer code that you exported to continue.", "OK", "Cancel");

                    if (string.IsNullOrWhiteSpace(info) || info == "Cancel")
                        return;

                    Analytics.TrackEvent("Transfer", new Dictionary<string, string>
                    {
                        ["type"] = "in"
                    });

                    if (await SettingsService.TransferIn(info))
                    {
                        await DisplayAlert("Success", "Your profile has been updated. Ensure you update information in the app and sync with the cloud.");
                    }
                    else
                    {
                        await DisplayAlert("Error", "Please contact support with your transfer code for help.");
                    }
                }
            }
        }

        async Task DeleteAccount()
        {
            try
            {
                Analytics.TrackEvent("DeleteAccount");
                var info = await SettingsService.TransferOut();
                var message = new EmailMessage
                {
                    Subject = $"Delete Island Tracker Account",
                    Body = info,
                    To = new List<string> { "acislandtracker@gmail.com" }
                };

                await Email.ComposeAsync(message);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Unable to send email", "Email acislandtracker@gmail.com directly.");
            }
        }

        public bool HideFirstTimeBuying
        {
            get => SettingsService.HideFirstTimeBuying;
            set => SettingsService.HideFirstTimeBuying = value;
        }

        public bool AutoRefreshFriends
        {
            get => SettingsService.AutoRefreshFriends;
            set
            {
                SettingsService.AutoRefreshFriends = value;
                OnPropertyChanged();
            }
        }

        public int RefreshAfterHours
        {
            get => SettingsService.RefreshAfterHours switch
            {
                1 => 0,
                2 => 1,
                4 => 2,
                6 => 3,
                12 => 4,
                24 => 5,
                _ => 0
            };
            set
            {
                var val = value switch
                {
                    0 => 1,
                    1 => 2,
                    2 => 4,
                    3 => 6,
                    4 => 12,
                    5 => 24,
                    _ => 1
                };
                SettingsService.RefreshAfterHours = val;
            }
        }
    }
}
